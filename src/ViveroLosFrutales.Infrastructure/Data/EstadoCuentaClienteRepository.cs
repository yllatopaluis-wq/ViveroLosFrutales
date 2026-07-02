using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class EstadoCuentaClienteRepository(ApplicationDbContext db) : IEstadoCuentaClienteRepository
{
    public async Task<EstadoCuentaClienteDto?> ObtenerAsync(int empresaId, int clienteId, CancellationToken cancellationToken)
    {
        var cliente = await db.Clientes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.ClienteId == clienteId)
            .Select(x => new { x.ClienteId, x.NombreCompleto })
            .FirstOrDefaultAsync(cancellationToken);
        if (cliente is null) return null;

        var pedidos = await db.NotasPedido.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.ClienteId == clienteId)
            .OrderByDescending(x => x.Fecha)
            .Select(x => new
            {
                NotaPedido = x,
                TotalCobrado = x.Cobros
                    .Where(c => c.Estado != CobroClienteEstado.ANULADO)
                    .Sum(c => (decimal?)c.Monto) ?? 0
            })
            .Select(x => new NotaPedidoListDto(
                x.NotaPedido.NotaPedidoId,
                x.NotaPedido.Serie + "-" + x.NotaPedido.Correlativo,
                x.NotaPedido.Fecha,
                x.NotaPedido.Cliente!.NombreCompleto,
                x.NotaPedido.Total,
                x.TotalCobrado,
                x.NotaPedido.Total - x.TotalCobrado < 0 ? 0 : x.NotaPedido.Total - x.TotalCobrado,
                x.NotaPedido.EstadoDocumento,
                x.TotalCobrado <= 0
                    ? EstadoPagoNotaPedido.PENDIENTE
                    : x.NotaPedido.Total - x.TotalCobrado <= 0
                        ? EstadoPagoNotaPedido.PAGADO
                        : EstadoPagoNotaPedido.PAGO_PARCIAL,
                "No aplica",
                x.NotaPedido.ComprobanteId))
            .ToListAsync(cancellationToken);

        var cobros = await db.CobrosCliente.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.ClienteId == clienteId)
            .OrderByDescending(x => x.FechaCobro)
            .Select(x => new CobroClienteListDto(
                x.CobroClienteId,
                x.FechaCobro,
                x.Cliente!.NombreCompleto,
                x.NotaPedidoId != null ? x.NotaPedido!.Serie + "-" + x.NotaPedido.Correlativo : x.Comprobante!.Serie + "-" + x.Comprobante.Correlativo,
                x.Monto,
                x.MedioPago,
                x.CuentaFinanciera == null ? string.Empty : x.CuentaFinanciera.Nombre,
                x.Estado,
                x.Observacion))
            .ToListAsync(cancellationToken);

        var comprobantes = await db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.ClienteId == clienteId && x.TipoComprobante != TipoComprobante.COT)
            .OrderByDescending(x => x.FechaEmision)
            .Select(x => new
            {
                Comprobante = x,
                TotalPagado = x.Cobros
                    .Where(c => c.Estado == CobroClienteEstado.ACTIVO
                        && !x.CobrosAplicados.Any(a => a.CobroClienteId == c.CobroClienteId))
                    .Sum(c => (decimal?)c.Monto) ?? 0,
                TotalAplicado = x.CobrosAplicados
                    .Where(c => c.CobroCliente != null && c.CobroCliente.Estado == CobroClienteEstado.ACTIVO)
                    .Sum(c => (decimal?)c.MontoAplicado) ?? 0
            })
            .Select(x => new ComprobanteListDto(
                x.Comprobante.ComprobanteId,
                x.Comprobante.TipoComprobante,
                x.Comprobante.Serie,
                x.Comprobante.Correlativo,
                x.Comprobante.FechaEmision,
                x.Comprobante.Cliente!.NombreCompleto,
                x.Comprobante.Cliente.NumeroDocumento,
                x.Comprobante.Direccion,
                x.Comprobante.Total,
                x.TotalPagado + x.TotalAplicado,
                x.Comprobante.Total - (x.TotalPagado + x.TotalAplicado) < 0 ? 0 : x.Comprobante.Total - (x.TotalPagado + x.TotalAplicado),
                x.TotalPagado + x.TotalAplicado <= 0
                    ? EstadoPagoComprobante.PENDIENTE
                    : x.TotalPagado + x.TotalAplicado >= x.Comprobante.Total
                        ? EstadoPagoComprobante.PAGADO
                        : EstadoPagoComprobante.PAGO_PARCIAL,
                x.Comprobante.EstadoSunat,
                x.Comprobante.DocumentoImpreso,
                x.Comprobante.EstadoSunat == EstadoSunat.Aceptado,
                x.Comprobante.Estado,
                false,
                false,
                false))
            .ToListAsync(cancellationToken);

        return new EstadoCuentaClienteDto
        {
            ClienteId = cliente.ClienteId,
            Cliente = cliente.NombreCompleto,
            TotalPedidos = pedidos.Sum(x => x.Total),
            TotalCobrado = cobros.Where(x => x.Estado != CobroClienteEstado.ANULADO).Sum(x => x.Monto),
            SaldoPendiente = pedidos.Sum(x => x.SaldoPendiente),
            Pedidos = pedidos,
            Cobros = cobros,
            Comprobantes = comprobantes
        };
    }
}




