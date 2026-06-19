using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class CategoriaRepository(ApplicationDbContext db) : ICategoriaRepository
{
    public Task<PagedResult<CategoriaListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Categorias.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Nombre.Contains(term) || x.Descripcion.Contains(term));
        }

        return query.OrderBy(x => x.Nombre)
            .Select(x => new CategoriaListDto(x.CategoriaId, x.Nombre, x.Descripcion, x.Estado))
            .ToPagedAsync(request, cancellationToken);
    }

    public async Task<IReadOnlyList<CategoriaListDto>> ListarActivasAsync(int empresaId, CancellationToken cancellationToken)
    {
        return await db.Categorias.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new CategoriaListDto(x.CategoriaId, x.Nombre, x.Descripcion, x.Estado))
            .ToListAsync(cancellationToken);
    }

    public Task<Categoria?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Categorias.FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CategoriaId == id, cancellationToken);

    public async Task GuardarAsync(Categoria categoria, CancellationToken cancellationToken)
    {
        if (categoria.CategoriaId == 0) db.Categorias.Add(categoria);
        await db.SaveChangesAsync(cancellationToken);
    }
}
