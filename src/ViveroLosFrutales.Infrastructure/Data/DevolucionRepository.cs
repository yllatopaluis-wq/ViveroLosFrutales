using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class DevolucionRepository(ApplicationDbContext db) : IDevolucionRepository
{
    public Task<PagedResult<DevolucionListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Devoluciones.AsNoTracking().Where(x => x.EmpresaId == empresaId);

        if (request.FechaDesde is not null)
        {
            var desde = request.FechaDesde.Value.Date;
            query = query.Where(x => x.FechaGeneracion >= desde);
        }

        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaGeneracion < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                (x.Cliente != null && (x.Cliente.NombreCompleto.Contains(term) || x.Cliente.NumeroDocumento.Contains(term)))
                || (x.Proveedor != null && (x.Proveedor.RazonSocial.Contains(term) || x.Proveedor.NumeroDocumento.Contains(term)))
                || (x.NotaPedido != null && (x.NotaPedido.Serie + "-" + x.NotaPedido.Correlativo).Contains(term))
                || (x.Comprobante != null && (x.Comprobante.Serie + "-" + x.Comprobante.Correlativo).Contains(term))
                || (x.NotaCredito != null && (x.NotaCredito.Serie + "-" + x.NotaCredito.Correlativo).Contains(term))
                || (x.Compra != null && x.Compra.Documento.Contains(term))
                || x.Observacion.Contains(term)
                || x.MotivoGeneracion.Contains(term));
        }

        return query.OrderByDescending(x => x.FechaGeneracion)
            .ThenByDescending(x => x.DevolucionId)
            .Select(x => new DevolucionListDto(
                x.DevolucionId,
                x.FechaGeneracion,
                x.TipoTercero,
                x.TipoTercero == TipoTerceroDevolucion.CLIENTE
                    ? x.Cliente!.NombreCompleto
                    : x.Proveedor!.RazonSocial,
                x.Origen,
                x.Origen == OrigenDevolucion.ANULACION_NOTA_PEDIDO
                    ? "Anulacion Nota Pedido"
                    : x.Origen == OrigenDevolucion.NOTA_CREDITO
                        ? "Nota de Credito"
                        : x.Origen == OrigenDevolucion.ANULACION_COMPRA
                            ? "Anulacion Compra"
                            : "Anulacion Comprobante",
                x.NotaPedido != null
                    ? x.NotaPedido.Serie + "-" + x.NotaPedido.Correlativo.ToString()
                    : x.NotaCredito != null
                        ? x.NotaCredito.Serie + "-" + x.NotaCredito.Correlativo.ToString()
                        : x.Comprobante != null
                            ? x.Comprobante.Serie + "-" + x.Comprobante.Correlativo.ToString()
                            : x.Compra != null
                                ? x.Compra.Documento
                                : "-",
                x.MontoOriginal,
                x.MontoDevuelto,
                x.MontoPendiente,
                x.EstadoDevolucion,
                x.EstadoDevolucion == EstadoDevolucion.PENDIENTE || x.EstadoDevolucion == EstadoDevolucion.PARCIAL,
                false))
            .ToPagedAsync(request, cancellationToken);
    }

    public async Task<DevolucionAlertasDto> ObtenerAlertasAsync(int empresaId, int cantidad, CancellationToken cancellationToken)
    {
        var pendientes = db.Devoluciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && (x.EstadoDevolucion == EstadoDevolucion.PENDIENTE
                    || x.EstadoDevolucion == EstadoDevolucion.PARCIAL));

        var total = await pendientes.CountAsync(cancellationToken);
        var recientes = await pendientes
            .OrderByDescending(x => x.FechaGeneracion)
            .ThenByDescending(x => x.DevolucionId)
            .Take(Math.Clamp(cantidad, 1, 10))
            .Select(x => new DevolucionAlertaDto(
                x.DevolucionId,
                x.FechaGeneracion,
                x.TipoTercero == TipoTerceroDevolucion.CLIENTE
                    ? x.Cliente!.NombreCompleto
                    : x.Proveedor!.RazonSocial,
                x.NotaPedido != null
                    ? x.NotaPedido.Serie + "-" + x.NotaPedido.Correlativo.ToString()
                    : x.NotaCredito != null
                        ? x.NotaCredito.Serie + "-" + x.NotaCredito.Correlativo.ToString()
                        : x.Comprobante != null
                            ? x.Comprobante.Serie + "-" + x.Comprobante.Correlativo.ToString()
                            : x.Compra != null
                                ? x.Compra.Documento
                                : "-",
                x.MontoPendiente,
                x.Origen == OrigenDevolucion.ANULACION_NOTA_PEDIDO
                    ? "Anulacion de nota de pedido"
                    : x.Origen == OrigenDevolucion.NOTA_CREDITO
                        ? "Nota de credito"
                        : x.Origen == OrigenDevolucion.ANULACION_COMPRA
                            ? "Anulacion de compra"
                            : "Anulacion de comprobante"))
            .ToListAsync(cancellationToken);

        return new DevolucionAlertasDto
        {
            TotalPendientes = total,
            Recientes = recientes
        };
    }

    public Task<Devolucion?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Devoluciones
            .Include(x => x.Cliente)
            .Include(x => x.Proveedor)
            .Include(x => x.NotaPedido)
            .Include(x => x.Comprobante)
            .Include(x => x.NotaCredito)
            .Include(x => x.Compra)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.DevolucionId == id, cancellationToken);

    public Task<bool> ExisteActivaPorNotaPedidoAsync(int empresaId, int notaPedidoId, CancellationToken cancellationToken) =>
        db.Devoluciones.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId
            && x.Origen == OrigenDevolucion.ANULACION_NOTA_PEDIDO
            && x.NotaPedidoId == notaPedidoId
            && x.EstadoDevolucion != EstadoDevolucion.ANULADO, cancellationToken);

    public Task<bool> ExisteActivaPorNotaCreditoAsync(int empresaId, int notaCreditoId, CancellationToken cancellationToken) =>
        db.Devoluciones.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId
            && x.Origen == OrigenDevolucion.NOTA_CREDITO
            && x.NotaCreditoId == notaCreditoId
            && x.EstadoDevolucion != EstadoDevolucion.ANULADO, cancellationToken);

    public Task<bool> ExisteActivaPorCompraAsync(int empresaId, int compraId, CancellationToken cancellationToken) =>
        db.Devoluciones.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId
            && x.Origen == OrigenDevolucion.ANULACION_COMPRA
            && x.CompraId == compraId
            && x.EstadoDevolucion != EstadoDevolucion.ANULADO, cancellationToken);

    public Task<bool> ExisteActivaPorComprobanteAsync(int empresaId, int comprobanteId, CancellationToken cancellationToken) =>
        db.Devoluciones.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId
            && x.Origen == OrigenDevolucion.ANULACION_COMPROBANTE
            && x.ComprobanteId == comprobanteId
            && x.EstadoDevolucion != EstadoDevolucion.ANULADO, cancellationToken);

    public async Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
            await operacion();
            await tx.CommitAsync(cancellationToken);
        });
    }

    public async Task GuardarAsync(Devolucion devolucion, CancellationToken cancellationToken)
    {
        if (devolucion.DevolucionId == 0) db.Devoluciones.Add(devolucion);
        await db.SaveChangesAsync(cancellationToken);
    }
}
