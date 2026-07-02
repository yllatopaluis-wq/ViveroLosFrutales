using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class TransferenciaService(
    ITransferenciaFinancieraRepository repository,
    ICuentaFinancieraRepository cuentaRepository,
    IEmpresaContext empresaContext)
{
    public Task<PagedResult<TransferenciaListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<TransferenciaEditDto> NuevoAsync(CancellationToken cancellationToken)
    {
        await cuentaRepository.EnsureCuentaPrincipalAsync(empresaContext.EmpresaId, cancellationToken);
        return await PrepararFormularioAsync(new TransferenciaEditDto { Fecha = PeruDateTime.Today }, cancellationToken);
    }

    public async Task<TransferenciaEditDto> PrepararFormularioAsync(TransferenciaEditDto dto, CancellationToken cancellationToken)
    {
        dto.Cuentas = await repository.ListarCuentasActivasAsync(empresaContext.EmpresaId, cancellationToken);
        return dto;
    }

    public async Task RegistrarAsync(TransferenciaEditDto dto, CancellationToken cancellationToken)
    {
        if (dto.CuentaOrigenId <= 0) throw new InvalidOperationException("Seleccione la cuenta origen.");
        if (dto.CuentaDestinoId <= 0) throw new InvalidOperationException("Seleccione la cuenta destino.");
        if (dto.CuentaOrigenId == dto.CuentaDestinoId) throw new InvalidOperationException("La cuenta origen debe ser distinta de la cuenta destino.");
        if (dto.Monto <= 0) throw new InvalidOperationException("El monto debe ser mayor a cero.");

        var cuentaOrigen = await repository.ObtenerCuentaActivaAsync(empresaContext.EmpresaId, dto.CuentaOrigenId, cancellationToken)
            ?? throw new InvalidOperationException("La cuenta origen no existe o no esta activa.");
        var cuentaDestino = await repository.ObtenerCuentaActivaAsync(empresaContext.EmpresaId, dto.CuentaDestinoId, cancellationToken)
            ?? throw new InvalidOperationException("La cuenta destino no existe o no esta activa.");

        var monto = decimal.Round(dto.Monto, 2);
        var fecha = dto.Fecha.Date;
        var observacion = (dto.Observacion ?? string.Empty).Trim();

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var transferencia = new TransferenciaFinanciera
            {
                EmpresaId = empresaContext.EmpresaId,
                Fecha = fecha,
                CuentaOrigenId = cuentaOrigen.CuentaFinancieraId,
                CuentaDestinoId = cuentaDestino.CuentaFinancieraId,
                Monto = monto,
                Observacion = observacion,
                UsuarioRegistro = empresaContext.UsuarioNombre,
                Estado = EstadoRegistro.Activo
            };

            await repository.GuardarAsync(transferencia, cancellationToken);

            var movimientoEgreso = new MovimientoCaja
            {
                EmpresaId = empresaContext.EmpresaId,
                CuentaFinancieraId = cuentaOrigen.CuentaFinancieraId,
                TipoMovimiento = TipoMovimientoCaja.EGRESO,
                Origen = OrigenMovimientoCaja.TRANSFERENCIA,
                OrigenId = transferencia.TransferenciaFinancieraId,
                Fecha = fecha,
                Monto = monto,
                MedioPago = "TRANSFERENCIA",
                Descripcion = $"Transferencia a {cuentaDestino.Nombre}",
                UsuarioRegistro = empresaContext.UsuarioNombre,
                Estado = EstadoRegistro.Activo
            };

            var movimientoIngreso = new MovimientoCaja
            {
                EmpresaId = empresaContext.EmpresaId,
                CuentaFinancieraId = cuentaDestino.CuentaFinancieraId,
                TipoMovimiento = TipoMovimientoCaja.INGRESO,
                Origen = OrigenMovimientoCaja.TRANSFERENCIA,
                OrigenId = transferencia.TransferenciaFinancieraId,
                Fecha = fecha,
                Monto = monto,
                MedioPago = "TRANSFERENCIA",
                Descripcion = $"Transferencia desde {cuentaOrigen.Nombre}",
                UsuarioRegistro = empresaContext.UsuarioNombre,
                Estado = EstadoRegistro.Activo
            };

            await repository.GuardarMovimientoAsync(movimientoEgreso, cancellationToken);
            await repository.GuardarMovimientoAsync(movimientoIngreso, cancellationToken);

            transferencia.MovimientoEgresoId = movimientoEgreso.MovimientoCajaId;
            transferencia.MovimientoIngresoId = movimientoIngreso.MovimientoCajaId;
            await repository.GuardarAsync(transferencia, cancellationToken);
        }, cancellationToken);
    }

    public async Task AnularAsync(int transferenciaFinancieraId, string motivo, CancellationToken cancellationToken)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo de anulacion de la transferencia.");

        var transferencia = await repository.ObtenerAsync(empresaContext.EmpresaId, transferenciaFinancieraId, cancellationToken)
            ?? throw new InvalidOperationException("Transferencia no encontrada.");
        if (transferencia.Estado == EstadoRegistro.Anulado) return;

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            transferencia.Estado = EstadoRegistro.Anulado;
            transferencia.FechaAnulacion = DateTime.UtcNow;
            transferencia.MotivoAnulacion = motivo;
            transferencia.UsuarioAnulacion = empresaContext.UsuarioNombre;

            if (transferencia.MovimientoEgreso is not null)
            {
                transferencia.MovimientoEgreso.Estado = EstadoRegistro.Anulado;
            }

            if (transferencia.MovimientoIngreso is not null)
            {
                transferencia.MovimientoIngreso.Estado = EstadoRegistro.Anulado;
            }

            await repository.GuardarAsync(transferencia, cancellationToken);
        }, cancellationToken);
    }
}
