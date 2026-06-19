using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class EmpresaRepository(ApplicationDbContext db) : IEmpresaRepository
{
    public Task<PagedResult<EmpresaListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Empresas.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.RUC.Contains(term) || x.RazonSocial.Contains(term) || x.NombreComercial.Contains(term));
        }

        return query.OrderBy(x => x.RazonSocial)
            .Select(x => new EmpresaListDto(x.EmpresaId, x.RUC, x.RazonSocial, x.NombreComercial, x.Telefono, x.Estado))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<Empresa?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        db.Empresas.FirstOrDefaultAsync(x => x.EmpresaId == id, cancellationToken);

    public async Task GuardarAsync(Empresa empresa, CancellationToken cancellationToken)
    {
        if (empresa.EmpresaId == 0) db.Empresas.Add(empresa);
        await db.SaveChangesAsync(cancellationToken);
    }
}
