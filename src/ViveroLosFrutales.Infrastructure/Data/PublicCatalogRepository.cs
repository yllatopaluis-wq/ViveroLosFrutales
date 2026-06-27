using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class PublicCatalogRepository(ApplicationDbContext db) : IPublicCatalogRepository
{
    public async Task<PublicCatalogDto> ObtenerAsync(string empresaRuc, int cantidadProductos, CancellationToken cancellationToken)
    {
        var empresa = await db.Empresas.AsNoTracking()
            .Where(x => x.RUC == empresaRuc && x.Estado == EstadoRegistro.Activo)
            .Select(x => new { x.EmpresaId, Dto = new PublicEmpresaDto(x.RazonSocial, x.NombreComercial, x.RUC, x.Direccion, x.Telefono, x.Email) })
            .FirstOrDefaultAsync(cancellationToken);

        if (empresa is null) return new PublicCatalogDto();

        var productos = await db.Productos.AsNoTracking()
            .Where(x => x.EmpresaId == empresa.EmpresaId && x.Estado == EstadoRegistro.Activo)
            .OrderBy(x => x.Categoria)
            .ThenBy(x => x.Nombre)
            .Take(cantidadProductos)
            .Select(x => new PublicProductoDto(x.ProductoId, x.Nombre, x.Categoria, x.UnidadMedida, x.PrecioVentaConIgv))
            .ToListAsync(cancellationToken);

        return new PublicCatalogDto { Empresa = empresa.Dto, Productos = productos };
    }
}
