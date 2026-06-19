using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class CobroClienteService(
    ICobroClienteRepository cobroRepository,
    IMovimientoCajaRepository movimientoCajaRepository,
    INotaPedidoRepository notaPedidoRepository,
    IComprobanteRepository comprobanteRepository,
    IEmpresaContext empresaContext)
{
    public Task<PagedResult<CobroClienteListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        cobroRepository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<IReadOnlyList<CobroClienteListDto>> ListarPorNotaPedidoAsync(int notaPedidoId, CancellationToken cancellationToken) =>
        cobroRepository.ListarPorNotaPedidoAsync(empresaContext.EmpresaId, notaPedidoId, cancellationToken);

    public Task<IReadOnlyList<CobroClienteListDto>> ListarPorComprobanteAsync(int comprobanteId, CancellationToken cancellationToken) =>
        cobroRepository.ListarPorComprobanteAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken);

    public Task<IReadOnlyList<CobroClienteListDto>> ListarPorClienteAsync(int clienteId, CancellationToken cancellationToken) =>
        cobroRepository.ListarPorClienteAsync(empresaContext.EmpresaId, clienteId, cancellationToken);

    public async Task<CobroCliente> RegistrarAsync(RegistrarCobroDto dto, CancellationToken cancellationToken)
    {
        if (dto.Monto <= 0) throw new InvalidOperationException("El monto del cobro debe ser mayor a cero.");
        if (dto.NotaPedidoId is null && dto.ComprobanteId is null) throw new InvalidOperationException("El cobro debe estar asociado a una nota de pedido o comprobante.");
        if (dto.NotaPedidoId is not null && dto.ComprobanteId is not null) throw new InvalidOperationException("Seleccione solo una referencia para el cobro.");

        CobroCliente? cobro = null;
        await cobroRepository.EjecutarEnTransaccionAsync(async () =>
        {
            if (dto.NotaPedidoId is int notaPedidoId)
            {
                var notaPedido = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, notaPedidoId, cancellationToken)
                    ?? throw new InvalidOperationException("Nota de pedido no encontrada.");

                if (notaPedido.EstadoDocumento != NotaPedidoEstado.ACTIVO)
                {
                    throw new InvalidOperationException("No se puede cobrar una nota de pedido anulada.");
                }
                if (notaPedido.ComprobanteId is not null || notaPedido.Comprobantes.Count > 0)
                {
                    throw new InvalidOperationException("No se puede cobrar una nota de pedido con comprobante relacionado.");
                }

                var totalCobradoPrevio = TotalCobradoNota(notaPedido);
                var saldoPendiente = SaldoPendienteNota(notaPedido.Total, totalCobradoPrevio);
                if (dto.Monto > saldoPendiente)
                {
                    throw new InvalidOperationException("El monto no puede superar el saldo pendiente.");
                }

                cobro = CrearCobro(dto, notaPedido.ClienteId);
                cobro.NotaPedidoId = notaPedido.NotaPedidoId;
                await cobroRepository.GuardarAsync(cobro, cancellationToken);
                await CrearMovimientoCajaAsync(cobro, $"Cobro nota de pedido {notaPedido.Serie}-{notaPedido.Correlativo:000000}", cancellationToken);

                ActualizarNota(notaPedido, totalCobradoPrevio + cobro.Monto);
                notaPedido.FechaModificacion = DateTime.UtcNow;
                notaPedido.UsuarioModificacion = empresaContext.UsuarioNombre;
                await notaPedidoRepository.GuardarAsync(notaPedido, cancellationToken);
            }
            else
            {
                var comprobanteId = dto.ComprobanteId!.Value;
                var comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
                    ?? throw new InvalidOperationException("Comprobante no encontrado.");

                if (comprobante.Estado == EstadoRegistro.Anulado)
                {
                    throw new InvalidOperationException("No se puede cobrar un comprobante anulado.");
                }

                if (comprobante.TipoComprobante == TipoComprobante.NCR)
                {
                    throw new InvalidOperationException("No se registran cobros sobre notas de credito.");
                }

                var cobrado = TotalPagadoComprobante(comprobante);
                var totalNotasCredito = await comprobanteRepository.TotalNotasCreditoActivasAsync(empresaContext.EmpresaId, comprobante.ComprobanteId, cancellationToken);
                var saldo = SaldoPendienteComprobante(comprobante, totalNotasCredito);
                if (saldo <= 0 || comprobante.EstadoPago == EstadoPagoComprobante.PAGADO)
                {
                    throw new InvalidOperationException("El comprobante ya se encuentra pagado.");
                }

                if (dto.Monto > saldo) throw new InvalidOperationException("El monto no puede superar el saldo pendiente del comprobante.");

                cobro = CrearCobro(dto, comprobante.ClienteId);
                cobro.ComprobanteId = comprobante.ComprobanteId;
                await cobroRepository.GuardarAsync(cobro, cancellationToken);
                await CrearMovimientoCajaAsync(cobro, $"Cobro comprobante {comprobante.Serie}-{comprobante.Correlativo:000000}", cancellationToken);

                ActualizarResumenPago(comprobante, cobrado + cobro.Monto, totalNotasCredito);
                await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
            }
        }, cancellationToken);

        return cobro!;
    }

    public async Task AnularAsync(int cobroClienteId, string motivo, CancellationToken cancellationToken)
    {
        var cobro = await cobroRepository.ObtenerAsync(empresaContext.EmpresaId, cobroClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cobro no encontrado.");

        if (cobro.Estado == CobroClienteEstado.ANULADO) return;
        if (cobro.Aplicaciones.Count > 0 && cobro.Comprobante?.EstadoSunat == EstadoSunat.Aceptado)
        {
            throw new InvalidOperationException("No se puede anular un cobro aplicado a un comprobante aceptado sin reversión controlada.");
        }

        await cobroRepository.EjecutarEnTransaccionAsync(async () =>
        {
            cobro.Estado = CobroClienteEstado.ANULADO;
            cobro.FechaAnulacion = DateTime.UtcNow;
            cobro.UsuarioAnulacion = empresaContext.UsuarioNombre;
            cobro.MotivoAnulacion = motivo.Trim();
            await cobroRepository.GuardarAsync(cobro, cancellationToken);

            var movimiento = await movimientoCajaRepository.ObtenerPorOrigenAsync(empresaContext.EmpresaId, OrigenMovimientoCaja.COBRO_CLIENTE, cobro.CobroClienteId, cancellationToken);
            if (movimiento is not null)
            {
                movimiento.Estado = EstadoRegistro.Anulado;
                await movimientoCajaRepository.GuardarAsync(movimiento, cancellationToken);
            }

            if (cobro.NotaPedidoId is int notaPedidoId)
            {
                var nota = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, notaPedidoId, cancellationToken)
                    ?? throw new InvalidOperationException("Nota de pedido no encontrada.");
                RecalcularNota(nota);
                await notaPedidoRepository.GuardarAsync(nota, cancellationToken);
            }

            if (cobro.ComprobanteId is int comprobanteId)
            {
                var comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
                    ?? throw new InvalidOperationException("Comprobante no encontrado.");
                var totalNotasCredito = await comprobanteRepository.TotalNotasCreditoActivasAsync(empresaContext.EmpresaId, comprobante.ComprobanteId, cancellationToken);
                ActualizarResumenPago(comprobante, TotalPagadoComprobante(comprobante), totalNotasCredito);
                await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
            }
        }, cancellationToken);
    }

    private CobroCliente CrearCobro(RegistrarCobroDto dto, int clienteId) => new()
    {
        EmpresaId = empresaContext.EmpresaId,
        ClienteId = clienteId,
        FechaCobro = dto.FechaCobro.Date,
        Monto = decimal.Round(dto.Monto, 2),
        MedioPago = dto.MedioPago.Trim(),
        Observacion = dto.Observacion.Trim(),
        UsuarioRegistro = empresaContext.UsuarioNombre
    };

    private async Task CrearMovimientoCajaAsync(CobroCliente cobro, string descripcion, CancellationToken cancellationToken)
    {
        await movimientoCajaRepository.GuardarAsync(new MovimientoCaja
        {
            EmpresaId = empresaContext.EmpresaId,
            ClienteId = cobro.ClienteId,
            TipoMovimiento = TipoMovimientoCaja.INGRESO,
            Origen = OrigenMovimientoCaja.COBRO_CLIENTE,
            OrigenId = cobro.CobroClienteId,
            Fecha = cobro.FechaCobro,
            Monto = cobro.Monto,
            MedioPago = cobro.MedioPago,
            Descripcion = descripcion,
            UsuarioRegistro = empresaContext.UsuarioNombre
        }, cancellationToken);
    }

    private static decimal TotalPagadoComprobante(Comprobante comprobante) =>
        comprobante.Cobros
            .Where(x => x.Estado == CobroClienteEstado.ACTIVO
                && !comprobante.CobrosAplicados.Any(a => a.CobroClienteId == x.CobroClienteId))
            .Sum(x => x.Monto)
        + comprobante.CobrosAplicados
            .Where(x => x.CobroCliente?.Estado == CobroClienteEstado.ACTIVO)
            .Sum(x => x.MontoAplicado);

    private static decimal SaldoPendienteComprobante(Comprobante comprobante, decimal totalNotasCredito) =>
        SaldoPendiente(comprobante.Total - totalNotasCredito, TotalPagadoComprobante(comprobante));

    private static void ActualizarResumenPago(Comprobante comprobante, decimal totalPagado, decimal totalNotasCredito = 0)
    {
        comprobante.TotalPagado = decimal.Round(totalPagado, 2);
        comprobante.SaldoPendiente = SaldoPendiente(comprobante.Total - totalNotasCredito, comprobante.TotalPagado);
        comprobante.EstadoPago = comprobante.TotalPagado <= 0
            ? EstadoPagoComprobante.PENDIENTE
            : comprobante.SaldoPendiente <= 0
                ? EstadoPagoComprobante.PAGADO
                : EstadoPagoComprobante.PAGO_PARCIAL;
    }

    private static decimal CalcularSaldoPendiente(NotaPedido nota)
    {
        return SaldoPendienteNota(nota.Total, TotalCobradoNota(nota));
    }

    private static decimal TotalCobradoNota(NotaPedido nota) =>
        nota.Cobros.Where(x => x.Estado != CobroClienteEstado.ANULADO).Sum(x => x.Monto);

    private static void ActualizarNota(NotaPedido nota, decimal totalCobrado)
    {
        nota.TotalCobrado = totalCobrado;
        nota.SaldoPendiente = SaldoPendienteNota(nota.Total, nota.TotalCobrado);
        nota.EstadoPago = nota.TotalCobrado <= 0
            ? EstadoPagoNotaPedido.PENDIENTE
            : nota.SaldoPendiente <= 0
                ? EstadoPagoNotaPedido.PAGADO
                : EstadoPagoNotaPedido.PAGO_PARCIAL;
    }

    private static void RecalcularNota(NotaPedido nota)
    {
        nota.TotalCobrado = nota.Cobros.Where(x => x.Estado != CobroClienteEstado.ANULADO).Sum(x => x.Monto);
        nota.SaldoPendiente = SaldoPendienteNota(nota.Total, nota.TotalCobrado);
        nota.EstadoPago = nota.TotalCobrado <= 0
            ? EstadoPagoNotaPedido.PENDIENTE
            : nota.SaldoPendiente <= 0
                ? EstadoPagoNotaPedido.PAGADO
                : EstadoPagoNotaPedido.PAGO_PARCIAL;
    }

    private static decimal SaldoPendienteNota(decimal total, decimal totalCobrado) =>
        SaldoPendiente(total, totalCobrado);

    private static decimal SaldoPendiente(decimal total, decimal pagado)
    {
        var saldo = total - pagado;
        return decimal.Round(saldo < 0 ? 0 : saldo, 2);
    }
}
