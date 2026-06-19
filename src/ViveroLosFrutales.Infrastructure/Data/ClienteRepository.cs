using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ClienteRepository(ApplicationDbContext db) : IClienteRepository
{
    public async Task<PagedResult<ClienteListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Clientes.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.NombreCompleto.Contains(term) || x.NumeroDocumento.Contains(term));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 100 : Math.Min(request.PageSize, 100);
        var skip = (page - 1) * pageSize;

        var items = await query.OrderBy(x => x.NombreCompleto)
            .ThenBy(x => x.ClienteId)
            .Select(x => new ClienteListDto(x.ClienteId, x.NombreCompleto, x.TipoDocumento, x.NumeroDocumento, x.Direccion, x.Telefono, x.Estado))
            .Skip(skip)
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        var hasNextPage = items.Count > pageSize;
        if (hasNextPage) items.RemoveAt(items.Count - 1);

        return new PagedResult<ClienteListDto>
        {
            Items = items,
            Total = skip + items.Count + (hasNextPage ? 1 : 0),
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IReadOnlyList<ClienteListDto>> ListarActivosAsync(CancellationToken cancellationToken)
    {
        return await db.Clientes.AsNoTracking()
            .Where(x => x.Estado == EstadoRegistro.Activo)
            .OrderBy(x => x.NombreCompleto)
            .Select(x => new ClienteListDto(x.ClienteId, x.NombreCompleto, x.TipoDocumento, x.NumeroDocumento, x.Direccion, x.Telefono, x.Estado))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClienteListDto>> BuscarActivosAsync(string? search, int take, CancellationToken cancellationToken)
    {
        var query = db.Clientes.AsNoTracking()
            .Where(x => x.Estado == EstadoRegistro.Activo);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.NombreCompleto.Contains(term) || x.NumeroDocumento.Contains(term));
        }

        return await query
            .OrderBy(x => x.NombreCompleto)
            .ThenBy(x => x.ClienteId)
            .Take(Math.Clamp(take, 1, 50))
            .Select(x => new ClienteListDto(x.ClienteId, x.NombreCompleto, x.TipoDocumento, x.NumeroDocumento, x.Direccion, x.Telefono, x.Estado))
            .ToListAsync(cancellationToken);
    }

    public Task<Cliente?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        db.Clientes.FirstOrDefaultAsync(x => x.ClienteId == id, cancellationToken);

    public async Task GuardarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        if (cliente.ClienteId == 0) db.Clientes.Add(cliente);
        await db.SaveChangesAsync(cancellationToken);
    }
}
