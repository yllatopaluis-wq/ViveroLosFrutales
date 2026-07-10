using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ProductoRepository(ApplicationDbContext db) : IProductoRepository
{
    public Task<PagedResult<ProductoListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Productos.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
                        var codigoTerm = term.ToUpper().StartsWith("PROD-") ? term.Substring(5) : term;
            var productoIdTerm = int.TryParse(codigoTerm.TrimStart('0'), out var parsedProductoId) ? parsedProductoId : 0;
            query = query.Where(x => x.Nombre.Contains(term)
                || x.Categoria.Contains(term)
                || x.UnidadMedida.Contains(term)
                || (productoIdTerm > 0 && x.ProductoId == productoIdTerm));
        }

        return query.OrderBy(x => x.Nombre)
            .Select(x => new ProductoListDto(x.ProductoId, x.Categoria, x.Nombre, x.UnidadMedida, x.PrecioVentaSinIgv, x.PrecioVentaConIgv > 0 ? x.PrecioVentaConIgv : (x.AfectoIgv ? decimal.Round(x.PrecioVentaSinIgv * 1.18m, 2) : x.PrecioVentaSinIgv), x.Stock, x.AfectoIgv, x.Estado))
            .ToPagedAsync(request, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductoListDto>> ListarActivosAsync(int empresaId, CancellationToken cancellationToken)
    {
        return await db.Productos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new ProductoListDto(x.ProductoId, x.Categoria, x.Nombre, x.UnidadMedida, x.PrecioVentaSinIgv, x.PrecioVentaConIgv > 0 ? x.PrecioVentaConIgv : (x.AfectoIgv ? decimal.Round(x.PrecioVentaSinIgv * 1.18m, 2) : x.PrecioVentaSinIgv), x.Stock, x.AfectoIgv, x.Estado))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductoListDto>> BuscarActivosAsync(int empresaId, string? search, int take, CancellationToken cancellationToken)
    {
        var query = db.Productos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
                        var codigoTerm = term.ToUpper().StartsWith("PROD-") ? term.Substring(5) : term;
            var productoIdTerm = int.TryParse(codigoTerm.TrimStart('0'), out var parsedProductoId) ? parsedProductoId : 0;
            query = query.Where(x => x.Nombre.Contains(term)
                || x.Categoria.Contains(term)
                || x.UnidadMedida.Contains(term)
                || (productoIdTerm > 0 && x.ProductoId == productoIdTerm));
        }

        return await query
            .OrderBy(x => x.Nombre)
            .ThenBy(x => x.ProductoId)
            .Take(Math.Clamp(take, 1, 50))
            .Select(x => new ProductoListDto(x.ProductoId, x.Categoria, x.Nombre, x.UnidadMedida, x.PrecioVentaSinIgv, x.PrecioVentaConIgv > 0 ? x.PrecioVentaConIgv : (x.AfectoIgv ? decimal.Round(x.PrecioVentaSinIgv * 1.18m, 2) : x.PrecioVentaSinIgv), x.Stock, x.AfectoIgv, x.Estado))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductoListDto>> BuscarPorIdsAsync(int empresaId, IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        var productoIds = ids.Where(x => x > 0).Distinct().ToArray();
        if (productoIds.Length == 0) return Array.Empty<ProductoListDto>();

        return await db.Productos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && productoIds.Contains(x.ProductoId))
            .OrderBy(x => x.Nombre)
            .Select(x => new ProductoListDto(x.ProductoId, x.Categoria, x.Nombre, x.UnidadMedida, x.PrecioVentaSinIgv, x.PrecioVentaConIgv > 0 ? x.PrecioVentaConIgv : (x.AfectoIgv ? decimal.Round(x.PrecioVentaSinIgv * 1.18m, 2) : x.PrecioVentaSinIgv), x.Stock, x.AfectoIgv, x.Estado))
            .ToListAsync(cancellationToken);
    }

    public Task<Producto?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Productos.FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.ProductoId == id, cancellationToken);

    public async Task GuardarAsync(Producto producto, CancellationToken cancellationToken)
    {
        if (producto.ProductoId == 0) db.Productos.Add(producto);
        await db.SaveChangesAsync(cancellationToken);
    }
}

