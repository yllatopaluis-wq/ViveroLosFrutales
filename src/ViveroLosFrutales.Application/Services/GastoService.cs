using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class GastoService(IGastoRepository repository, IEmpresaContext empresaContext)
{
    public Task<PagedResult<GastoListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<IReadOnlyList<CategoriaGastoOptionDto>> ListarCategoriasAsync(CancellationToken cancellationToken) =>
        repository.ListarCategoriasAsync(empresaContext.EmpresaId, cancellationToken);

    public async Task<GastoEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var gasto = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);
        return gasto is null ? null : ToDto(gasto);
    }

    public async Task GuardarAsync(GastoEditDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Descripcion)) throw new InvalidOperationException("La descripcion es obligatoria.");
        if (string.IsNullOrWhiteSpace(dto.MedioPago)) throw new InvalidOperationException("El medio de pago es obligatorio.");
        if (dto.Importe <= 0) throw new InvalidOperationException("El importe debe ser mayor a cero.");

        var categoria = dto.CategoriaGastoId is int categoriaId
            ? await repository.ObtenerCategoriaAsync(empresaContext.EmpresaId, categoriaId, cancellationToken)
            : null;
        var nombreCategoria = categoria?.Nombre ?? dto.Categoria.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(nombreCategoria)) throw new InvalidOperationException("La categoria es obligatoria.");

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var gasto = dto.GastoId == 0
                ? new Gasto { EmpresaId = empresaContext.EmpresaId, Estado = EstadoRegistro.Activo, UsuarioRegistro = empresaContext.UsuarioNombre }
                : await repository.ObtenerAsync(empresaContext.EmpresaId, dto.GastoId, cancellationToken);
            if (gasto is null) throw new InvalidOperationException("Gasto no encontrado.");
            if (gasto.Estado == EstadoRegistro.Anulado) throw new InvalidOperationException("No se puede editar un gasto anulado.");

            gasto.Fecha = dto.Fecha.Date;
            gasto.CategoriaGastoId = categoria?.CategoriaGastoId;
            gasto.Categoria = nombreCategoria;
            gasto.Descripcion = dto.Descripcion.Trim();
            gasto.Importe = decimal.Round(dto.Importe, 2);
            gasto.MedioPago = dto.MedioPago.Trim().ToUpperInvariant();
            gasto.Observacion = dto.Observacion.Trim();
            gasto.Estado = EstadoRegistro.Activo;

            await repository.GuardarAsync(gasto, cancellationToken);

            var movimiento = gasto.MovimientoCaja ?? new MovimientoCaja
            {
                EmpresaId = empresaContext.EmpresaId,
                TipoMovimiento = TipoMovimientoCaja.EGRESO,
                Origen = OrigenMovimientoCaja.GASTO,
                OrigenId = gasto.GastoId,
                UsuarioRegistro = empresaContext.UsuarioNombre
            };

            movimiento.Fecha = gasto.Fecha;
            movimiento.Monto = gasto.Importe;
            movimiento.MedioPago = gasto.MedioPago;
            movimiento.Descripcion = gasto.Descripcion;
            movimiento.Estado = EstadoRegistro.Activo;
            gasto.MovimientoCaja = movimiento;
            await repository.GuardarAsync(gasto, cancellationToken);
        }, cancellationToken);
    }

    public async Task AnularAsync(int id, string motivo, CancellationToken cancellationToken)
    {
        var gasto = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken) ?? throw new InvalidOperationException("Gasto no encontrado.");
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo de anulacion del gasto.");
        if (gasto.Estado == EstadoRegistro.Anulado) return;

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            gasto.Estado = EstadoRegistro.Anulado;
            gasto.MotivoAnulacion = motivo;
            gasto.FechaAnulacion = DateTime.UtcNow;
            if (gasto.MovimientoCaja is not null)
            {
                gasto.MovimientoCaja.Estado = EstadoRegistro.Anulado;
            }

            await repository.GuardarAsync(gasto, cancellationToken);
        }, cancellationToken);
    }

    private static GastoEditDto ToDto(Gasto gasto) => new()
    {
        GastoId = gasto.GastoId,
        Fecha = gasto.Fecha,
        CategoriaGastoId = gasto.CategoriaGastoId,
        Categoria = gasto.Categoria,
        Descripcion = gasto.Descripcion,
        Importe = gasto.Importe,
        MedioPago = gasto.MedioPago,
        Observacion = gasto.Observacion,
        Estado = gasto.Estado
    };
}
