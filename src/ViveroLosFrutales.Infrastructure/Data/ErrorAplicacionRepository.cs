using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ErrorAplicacionRepository(ApplicationDbContext db) : IErrorAplicacionRepository
{
    public Task<PagedResult<ErrorAplicacionListDto>> BuscarAsync(int empresaId, ErrorAplicacionSearchDto request, CancellationToken cancellationToken)
    {
        var query = db.ErroresAplicacion.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId || x.EmpresaId == null);

        if (request.FechaDesde is not null)
        {
            var desde = request.FechaDesde.Value.Date;
            query = query.Where(x => x.FechaUtc >= desde);
        }

        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaUtc < hasta);
        }

        if (request.Estado is not null) query = query.Where(x => x.Estado == request.Estado);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Mensaje.Contains(term)
                || x.TipoExcepcion.Contains(term)
                || x.Ruta.Contains(term)
                || x.Usuario.Contains(term)
                || x.Identificador.Contains(term));
        }

        return query.OrderByDescending(x => x.FechaUtc)
            .ThenByDescending(x => x.ErrorAplicacionId)
            .Select(x => new ErrorAplicacionListDto(
                x.ErrorAplicacionId,
                x.FechaUtc,
                x.Usuario,
                x.Ruta,
                x.TipoExcepcion,
                x.Mensaje,
                x.Estado))
            .ToPagedAsync(new SearchRequest
            {
                Page = request.Page,
                PageSize = request.PageSize
            }, cancellationToken);
    }

    public Task<ErrorAplicacion?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.ErroresAplicacion
            .Include(x => x.Empresa)
            .FirstOrDefaultAsync(x => x.ErrorAplicacionId == id && (x.EmpresaId == empresaId || x.EmpresaId == null), cancellationToken);

    public async Task GuardarAsync(ErrorAplicacion error, CancellationToken cancellationToken)
    {
        if (error.ErrorAplicacionId == 0) db.ErroresAplicacion.Add(error);
        await db.SaveChangesAsync(cancellationToken);
    }
}
