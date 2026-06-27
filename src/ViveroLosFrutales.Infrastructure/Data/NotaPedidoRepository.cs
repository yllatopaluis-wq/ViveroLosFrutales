using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class NotaPedidoRepository(ApplicationDbContext db) : INotaPedidoRepository
{
    public Task<PagedResult<NotaPedidoListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.NotasPedido.AsNoTracking().Where(x => x.EmpresaId == empresaId);

        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.Fecha < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Serie.Contains(term) || x.Cliente!.NombreCompleto.Contains(term) || x.Cliente.NumeroDocumento.Contains(term));
        }

        return query.OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.NotaPedidoId)
            .Select(x => new
            {
                NotaPedido = x,
                TotalCobrado = x.Cobros
                    .Where(c => c.Estado != CobroClienteEstado.ANULADO)
                    .Sum(c => (decimal?)c.Monto) ?? 0,
                EstadoDevolucion = db.Devoluciones
                    .Where(d => d.EmpresaId == empresaId
                        && d.Origen == OrigenDevolucion.ANULACION_NOTA_PEDIDO
                        && d.NotaPedidoId == x.NotaPedidoId
                        && d.EstadoDevolucion != EstadoDevolucion.ANULADO)
                    .OrderByDescending(d => d.DevolucionId)
                    .Select(d => (int?)d.EstadoDevolucion)
                    .FirstOrDefault()
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
                x.EstadoDevolucion == null
                    ? string.Empty
                    : x.EstadoDevolucion == (int)EstadoDevolucion.PENDIENTE
                        ? "Pendiente"
                        : x.EstadoDevolucion == (int)EstadoDevolucion.PARCIAL
                            ? "Parcial"
                            : x.EstadoDevolucion == (int)EstadoDevolucion.DEVUELTO
                                ? "Devuelto"
                                : "Anulado",
                x.NotaPedido.ComprobanteId))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<NotaPedido?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.NotasPedido
            .Include(x => x.Cliente)
            .Include(x => x.Detalles).ThenInclude(x => x.Producto)
            .Include(x => x.Cobros)
            .Include(x => x.Comprobantes).ThenInclude(x => x.Cliente)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.NotaPedidoId == id, cancellationToken);

    public async Task<int> SiguienteCorrelativoAsync(int empresaId, string serie, CancellationToken cancellationToken)
    {
        var ultimo = await db.NotasPedido.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Serie == serie)
            .OrderByDescending(x => x.Correlativo)
            .Select(x => x.Correlativo)
            .FirstOrDefaultAsync(cancellationToken);

        return ultimo + 1;
    }

    public async Task GuardarAsync(NotaPedido notaPedido, CancellationToken cancellationToken)
    {
        if (notaPedido.NotaPedidoId == 0) db.NotasPedido.Add(notaPedido);
        await db.SaveChangesAsync(cancellationToken);
    }
}
