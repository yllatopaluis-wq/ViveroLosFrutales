using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Infrastructure.Data;

public class NubefactOperacionRepository(ApplicationDbContext db) : INubefactOperacionRepository
{
    public Task<PagedResult<NubefactOperacionDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.NubefactOperaciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.TipoOperacion.Contains(term)
                || x.RespuestaCompleta.Contains(term)
                || x.SolicitudJson.Contains(term)
                || x.Comprobante!.Serie.Contains(term));
        }

        return query.OrderByDescending(x => x.FechaRegistro)
            .Select(x => new NubefactOperacionDto(
                x.NubefactOperacionId,
                x.Comprobante!.Serie + "-" + x.Comprobante.Correlativo.ToString(),
                x.TipoOperacion,
                x.EstadoSunat.ToString(),
                x.PdfUrl,
                x.XmlUrl,
                x.Hash,
                x.SolicitudJson,
                x.RespuestaCompleta,
                x.FechaRegistro))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<NubefactOperacionDto?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken)
    {
        return db.NubefactOperaciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.NubefactOperacionId == id)
            .Select(x => new NubefactOperacionDto(
                x.NubefactOperacionId,
                x.Comprobante!.Serie + "-" + x.Comprobante.Correlativo.ToString(),
                x.TipoOperacion,
                x.EstadoSunat.ToString(),
                x.PdfUrl,
                x.XmlUrl,
                x.Hash,
                x.SolicitudJson,
                x.RespuestaCompleta,
                x.FechaRegistro))
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<NubefactOperacionDto>> ListarPorComprobanteAsync(int empresaId, int comprobanteId, CancellationToken cancellationToken)
    {
        return await db.NubefactOperaciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.ComprobanteId == comprobanteId)
            .OrderByDescending(x => x.FechaRegistro)
            .Select(x => new NubefactOperacionDto(
                x.NubefactOperacionId,
                x.Comprobante!.Serie + "-" + x.Comprobante.Correlativo.ToString(),
                x.TipoOperacion,
                x.EstadoSunat.ToString(),
                x.PdfUrl,
                x.XmlUrl,
                x.Hash,
                x.SolicitudJson,
                x.RespuestaCompleta,
                x.FechaRegistro))
            .ToListAsync(cancellationToken);
    }
}
