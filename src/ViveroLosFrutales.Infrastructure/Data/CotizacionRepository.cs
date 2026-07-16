using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class CotizacionRepository(ApplicationDbContext db) : ICotizacionRepository
{
    public Task<PagedResult<CotizacionListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Cotizaciones.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (request.FechaDesde is not null) query = query.Where(x => x.FechaEmision >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaEmision < hasta);
        }
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Serie.Contains(term)
                || (x.ClienteNombre ?? string.Empty).Contains(term)
                || (x.ClienteNumeroDocumento ?? string.Empty).Contains(term)
                || x.Cliente!.NombreCompleto.Contains(term)
                || x.Cliente.NumeroDocumento.Contains(term));
        }

        return query.OrderByDescending(x => x.FechaEmision)
            .ThenByDescending(x => x.CotizacionId)
            .Select(x => new CotizacionListDto(
                x.CotizacionId,
                x.Serie,
                x.Correlativo,
                x.FechaEmision,
                x.ClienteNombre != null && x.ClienteNombre != string.Empty ? x.ClienteNombre : x.Cliente!.NombreCompleto,
                x.ClienteTipoDocumento ?? x.Cliente!.TipoDocumento,
                x.ClienteNumeroDocumento != null && x.ClienteNumeroDocumento != string.Empty ? x.ClienteNumeroDocumento : x.Cliente!.NumeroDocumento,
                x.Total,
                x.EstadoCotizacion))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<Cotizacion?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Cotizaciones.Include(x => x.Detalles).ThenInclude(x => x.Producto)
            .Include(x => x.Cliente)
            .Include(x => x.Empresa)
            .Include(x => x.CondicionesSnapshot)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CotizacionId == id, cancellationToken);

    public async Task<int> SiguienteCorrelativoAsync(int empresaId, string serie, CancellationToken cancellationToken)
    {
        var ultimo = await db.Cotizaciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Serie == serie)
            .OrderByDescending(x => x.Correlativo)
            .Select(x => x.Correlativo)
            .FirstOrDefaultAsync(cancellationToken);
        return ultimo + 1;
    }

    public async Task<bool> TieneDocumentosRelacionadosAsync(int empresaId, int cotizacionId, CancellationToken cancellationToken)
    {
        return await db.NotasPedido.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId && x.CotizacionId == cotizacionId, cancellationToken)
            || await db.Comprobantes.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId && x.CotizacionId == cotizacionId, cancellationToken);
    }

    public async Task GuardarAsync(Cotizacion cotizacion, CancellationToken cancellationToken)
    {
        if (cotizacion.CotizacionId == 0) db.Cotizaciones.Add(cotizacion);
        await db.SaveChangesAsync(cancellationToken);
    }

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
}

