using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ProveedorRepository(ApplicationDbContext db) : IProveedorRepository
{
    public Task<PagedResult<ProveedorListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Proveedores.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.RazonSocial.Contains(term) || x.NumeroDocumento.Contains(term) || x.NombreComercial.Contains(term));
        }

        return query.OrderBy(x => x.RazonSocial)
            .Select(x => new ProveedorListDto(x.ProveedorId, x.TipoDocumento, x.NumeroDocumento, x.RazonSocial, x.NombreComercial, x.Telefono, x.Estado))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<Proveedor?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Proveedores.FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.ProveedorId == id, cancellationToken);

    public async Task<IReadOnlyList<ProveedorListDto>> ListarActivosAsync(int empresaId, CancellationToken cancellationToken) =>
        await db.Proveedores.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo)
            .OrderBy(x => x.RazonSocial)
            .Select(x => new ProveedorListDto(x.ProveedorId, x.TipoDocumento, x.NumeroDocumento, x.RazonSocial, x.NombreComercial, x.Telefono, x.Estado))
            .ToListAsync(cancellationToken);

    public async Task GuardarAsync(Proveedor proveedor, CancellationToken cancellationToken)
    {
        if (proveedor.ProveedorId == 0) db.Proveedores.Add(proveedor);
        await db.SaveChangesAsync(cancellationToken);
    }
}
