using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class IngresoService(IIngresoRepository repository, CuentaFinancieraService cuentaFinancieraService, IProveedorRepository proveedorRepository, IFormularioConfiguracionService formularioConfiguracionService, IEmpresaContext empresaContext)
{
    public Task<PagedResult<IngresoListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<IReadOnlyList<CategoriaIngresoOptionDto>> ListarCategoriasAsync(CancellationToken cancellationToken) =>
        repository.ListarCategoriasAsync(empresaContext.EmpresaId, cancellationToken);

    public Task<IReadOnlyList<CuentaFinancieraOptionDto>> ListarCuentasFinancierasAsync(CancellationToken cancellationToken) =>
        cuentaFinancieraService.ListarActivasAsync(cancellationToken);

    public Task<IReadOnlyList<ProveedorListDto>> ListarProveedoresAsync(CancellationToken cancellationToken) =>
        proveedorRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken);

    public Task<FormularioConfiguracionDto> ObtenerFormularioConfiguracionAsync(CancellationToken cancellationToken) =>
        formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoIngreso, empresaContext.EmpresaId, null, cancellationToken);

    public async Task<IngresoEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var ingreso = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);
        return ingreso is null ? null : ToDto(ingreso);
    }

    public async Task GuardarAsync(IngresoEditDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Descripcion)) throw new InvalidOperationException("La descripcion es obligatoria.");
        if (string.IsNullOrWhiteSpace(dto.MedioPago)) throw new InvalidOperationException("El medio de pago es obligatorio.");
        if (dto.Importe <= 0) throw new InvalidOperationException("El importe debe ser mayor a cero.");

        var categoria = dto.CategoriaIngresoId is int categoriaId
            ? await repository.ObtenerCategoriaAsync(empresaContext.EmpresaId, categoriaId, cancellationToken)
            : null;
        var tipoIngreso = categoria?.Nombre ?? dto.TipoIngreso.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(tipoIngreso)) throw new InvalidOperationException("El tipo de ingreso es obligatorio.");

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var ingreso = dto.IngresoId == 0
                ? new Ingreso { EmpresaId = empresaContext.EmpresaId, Estado = EstadoRegistro.Activo, UsuarioRegistro = empresaContext.UsuarioNombre }
                : await repository.ObtenerAsync(empresaContext.EmpresaId, dto.IngresoId, cancellationToken);
            if (ingreso is null) throw new InvalidOperationException("Ingreso no encontrado.");
            if (ingreso.Estado == EstadoRegistro.Anulado) throw new InvalidOperationException("No se puede editar un ingreso anulado.");

            ingreso.Fecha = dto.Fecha.Date;
            ingreso.CategoriaIngresoId = categoria?.CategoriaIngresoId;
            ingreso.TipoIngreso = tipoIngreso;
            ingreso.Descripcion = dto.Descripcion.Trim();
            ingreso.DocumentoReferencia = dto.DocumentoReferencia?.Trim() ?? string.Empty;
            ingreso.Importe = decimal.Round(dto.Importe, 2);
            ingreso.MedioPago = dto.MedioPago.Trim().ToUpperInvariant();
            ingreso.ProveedorId = dto.ProveedorId;
            ingreso.CuentaFinancieraId = await cuentaFinancieraService.ResolverCuentaIdAsync(dto.CuentaFinancieraId, cancellationToken);
            ingreso.Observacion = dto.Observacion?.Trim() ?? string.Empty;
            ingreso.Estado = EstadoRegistro.Activo;

            await repository.GuardarAsync(ingreso, cancellationToken);

            var movimiento = ingreso.MovimientoCaja ?? new MovimientoCaja
            {
                EmpresaId = empresaContext.EmpresaId,
                TipoMovimiento = TipoMovimientoCaja.INGRESO,
                Origen = OrigenMovimientoCaja.INGRESO_MANUAL,
                OrigenId = ingreso.IngresoId,
                UsuarioRegistro = empresaContext.UsuarioNombre
            };

            movimiento.Fecha = ingreso.Fecha;
            movimiento.CuentaFinancieraId = ingreso.CuentaFinancieraId;
            movimiento.Monto = ingreso.Importe;
            movimiento.MedioPago = ingreso.MedioPago;
            movimiento.Descripcion = ingreso.Descripcion;
            movimiento.Estado = EstadoRegistro.Activo;
            ingreso.MovimientoCaja = movimiento;
            await repository.GuardarAsync(ingreso, cancellationToken);
        }, cancellationToken);
    }

    public async Task AnularAsync(int id, string motivo, CancellationToken cancellationToken)
    {
        var ingreso = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken) ?? throw new InvalidOperationException("Ingreso no encontrado.");
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo de anulacion del ingreso.");
        if (ingreso.Estado == EstadoRegistro.Anulado) return;

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            ingreso.Estado = EstadoRegistro.Anulado;
            ingreso.MotivoAnulacion = motivo;
            ingreso.FechaAnulacion = DateTime.UtcNow;
            if (ingreso.MovimientoCaja is not null)
            {
                ingreso.MovimientoCaja.Estado = EstadoRegistro.Anulado;
            }

            await repository.GuardarAsync(ingreso, cancellationToken);
        }, cancellationToken);
    }

    private static IngresoEditDto ToDto(Ingreso ingreso) => new()
    {
        IngresoId = ingreso.IngresoId,
        Fecha = ingreso.Fecha,
        CategoriaIngresoId = ingreso.CategoriaIngresoId,
        TipoIngreso = ingreso.TipoIngreso,
        Descripcion = ingreso.Descripcion,
        DocumentoReferencia = ingreso.DocumentoReferencia,
        Importe = ingreso.Importe,
        MedioPago = ingreso.MedioPago,
        ProveedorId = ingreso.ProveedorId,
        CuentaFinancieraId = ingreso.CuentaFinancieraId,
        Observacion = ingreso.Observacion,
        Estado = ingreso.Estado
    };
}







