using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class CobroClienteRepository(ApplicationDbContext db) : ICobroClienteRepository
{
    public Task<PagedResult<CobroClienteListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = BuildQuery(empresaId);
        if (request.FechaDesde is not null) query = query.Where(x => x.FechaCobro >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaCobro < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.Cliente!.NombreCompleto.Contains(term)
                || x.MedioPago.Contains(term)
                || x.Observacion.Contains(term)
                || (x.NotaPedidoId != null && (x.NotaPedido!.Serie.Contains(term) || (x.NotaPedido.Serie + "-" + x.NotaPedido.Correlativo).Contains(term)))
                || (x.ComprobanteId != null && (x.Comprobante!.Serie.Contains(term) || (x.Comprobante.Serie + "-" + x.Comprobante.Correlativo).Contains(term)))
                || (x.ComprobanteId != null && x.Comprobante!.NotaPedidoId != null && (x.Comprobante.NotaPedido!.Serie.Contains(term) || (x.Comprobante.NotaPedido.Serie + "-" + x.Comprobante.NotaPedido.Correlativo).Contains(term)))
                || x.Aplicaciones.Any(a =>
                    a.Comprobante!.Serie.Contains(term)
                    || (a.Comprobante.Serie + "-" + a.Comprobante.Correlativo).Contains(term)
                    || (a.Comprobante.NotaPedidoId != null && (a.Comprobante.NotaPedido!.Serie.Contains(term) || (a.Comprobante.NotaPedido.Serie + "-" + a.Comprobante.NotaPedido.Correlativo).Contains(term)))));
        }

        return ToDto(query.OrderByDescending(x => x.FechaCobro).ThenByDescending(x => x.CobroClienteId))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<CobroCliente?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.CobrosCliente.Include(x => x.NotaPedido)
            .Include(x => x.Comprobante)
            .Include(x => x.Aplicaciones)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CobroClienteId == id, cancellationToken);

    public async Task<IReadOnlyList<CobroClienteListDto>> ListarPorNotaPedidoAsync(int empresaId, int notaPedidoId, CancellationToken cancellationToken) =>
        await ToDto(BuildQuery(empresaId).Where(x => x.NotaPedidoId == notaPedidoId).OrderByDescending(x => x.FechaCobro))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CobroClienteListDto>> ListarPorComprobanteAsync(int empresaId, int comprobanteId, CancellationToken cancellationToken) =>
        await ToDto(BuildQuery(empresaId).Where(x => x.ComprobanteId == comprobanteId).OrderByDescending(x => x.FechaCobro))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CobroClienteListDto>> ListarPorClienteAsync(int empresaId, int clienteId, CancellationToken cancellationToken) =>
        await ToDto(BuildQuery(empresaId).Where(x => x.ClienteId == clienteId).OrderByDescending(x => x.FechaCobro))
            .ToListAsync(cancellationToken);

    public async Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken)
    {
        if (db.Database.CurrentTransaction is not null)
        {
            await operacion();
            return;
        }

        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            await operacion();
            await transaction.CommitAsync(cancellationToken);
        });
    }

    public async Task GuardarAsync(CobroCliente cobro, CancellationToken cancellationToken)
    {
        if (cobro.CobroClienteId == 0) db.CobrosCliente.Add(cobro);
        await db.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<CobroCliente> BuildQuery(int empresaId) =>
        db.CobrosCliente.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId);

    private static IQueryable<CobroClienteListDto> ToDto(IQueryable<CobroCliente> query) =>
        query.Select(x => new CobroClienteListDto(
            x.CobroClienteId,
            x.FechaCobro,
            x.Cliente!.NombreCompleto,
            x.NotaPedidoId != null && x.Aplicaciones.Any()
                ? x.NotaPedido!.Serie + "-" + x.NotaPedido.Correlativo + " -> " + x.Aplicaciones
                    .OrderByDescending(a => a.FechaAplicacion)
                    .Select(a => a.Comprobante!.Serie + "-" + a.Comprobante.Correlativo)
                    .FirstOrDefault()
            : x.NotaPedidoId != null
                ? x.NotaPedido!.Serie + "-" + x.NotaPedido.Correlativo
                : x.ComprobanteId != null
                    ? x.Comprobante!.Serie + "-" + x.Comprobante.Correlativo
                    : "Sin referencia",
            x.Monto,
            x.MedioPago,
            x.Estado,
            x.Observacion));
}
