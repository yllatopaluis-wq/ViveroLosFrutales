using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
using ViveroLosFrutales.Domain.Security;

namespace ViveroLosFrutales.Infrastructure.Data;

public class RolRepository(ApplicationDbContext db) : IRolRepository
{
    public Task<PagedResult<RolListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.RolesNegocio.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Nombre.Contains(term) || x.Descripcion.Contains(term));
        }

        return query.OrderBy(x => x.Nombre)
            .Select(x => new RolListDto(x.RolId, x.Nombre, x.Descripcion, x.Estado))
            .ToPagedAsync(request, cancellationToken);
    }

    public async Task<RolEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var rol = await db.RolesNegocio.AsNoTracking()
            .Where(x => x.RolId == id)
            .Select(x => new RolEditDto
            {
                RolId = x.RolId,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                Estado = x.Estado,
                PermisoIds = x.RolPermisos.Select(p => p.PermisoId).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return rol;
    }

    public async Task<IReadOnlyList<PermisoDto>> ObtenerPermisosAsync(CancellationToken cancellationToken)
    {
        await EnsurePermisosAsync(cancellationToken);

        var permisos = await db.Permisos.AsNoTracking().ToListAsync(cancellationToken);
        var orden = PermissionCatalog.All()
            .Select((x, index) => new { Key = $"{x.Module}|{x.Action}", index })
            .ToDictionary(x => x.Key, x => x.index, StringComparer.OrdinalIgnoreCase);

        return permisos
            .OrderBy(x => orden.GetValueOrDefault($"{x.Modulo}|{x.Accion}", int.MaxValue))
            .Select(x => new PermisoDto(
                x.PermisoId,
                PermissionCatalog.GroupFor(x.Modulo),
                x.Modulo,
                PermissionCatalog.ModuleLabel(x.Modulo),
                x.Accion,
                PermissionCatalog.ActionLabel(x.Accion),
                x.Descripcion))
            .ToList();
    }

    public async Task GuardarAsync(RolEditDto dto, CancellationToken cancellationToken)
    {
        var rol = dto.RolId == 0
            ? new Rol()
            : await db.RolesNegocio.Include(x => x.RolPermisos).FirstOrDefaultAsync(x => x.RolId == dto.RolId, cancellationToken);

        if (rol is null) throw new InvalidOperationException("Rol no encontrado.");

        rol.Nombre = dto.Nombre.Trim();
        rol.Descripcion = dto.Descripcion.Trim();
        rol.Estado = dto.Estado;

        if (dto.RolId == 0) db.RolesNegocio.Add(rol);

        var permisosValidos = await db.Permisos
            .Where(x => x.Estado == EstadoRegistro.Activo)
            .Select(x => x.PermisoId)
            .ToListAsync(cancellationToken);
        var permisosValidosSet = permisosValidos.ToHashSet();

        rol.RolPermisos.Clear();
        foreach (var permisoId in dto.PermisoIds.Distinct().Where(permisosValidosSet.Contains))
        {
            rol.RolPermisos.Add(new RolPermiso { PermisoId = permisoId });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> TienePermisoAsync(int rolId, string modulo, string accion, CancellationToken cancellationToken)
    {
        return db.RolPermisos.AsNoTracking()
            .AnyAsync(x => x.RolId == rolId && x.Permiso!.Modulo == modulo && x.Permiso.Accion == accion, cancellationToken);
    }

    private async Task EnsurePermisosAsync(CancellationToken cancellationToken)
    {
        var catalogo = PermissionCatalog.All().ToArray();
        var catalogoSet = catalogo
            .Select(x => $"{x.Module}|{x.Action}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existentes = await db.Permisos
            .Include(x => x.RolPermisos)
            .ToListAsync(cancellationToken);

        var obsoletos = existentes
            .Where(x => !catalogoSet.Contains($"{x.Modulo}|{x.Accion}"))
            .ToList();
        if (obsoletos.Count > 0)
        {
            db.RolPermisos.RemoveRange(obsoletos.SelectMany(x => x.RolPermisos));
            db.Permisos.RemoveRange(obsoletos);
        }

        var existentesSet = existentes
            .Except(obsoletos)
            .Select(x => $"{x.Modulo}|{x.Accion}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var permiso in catalogo.Where(x => !existentesSet.Contains($"{x.Module}|{x.Action}")))
        {
            db.Permisos.Add(new Permiso
            {
                Modulo = permiso.Module,
                Accion = permiso.Action,
                Descripcion = $"{permiso.Action} {permiso.Module}",
                Estado = EstadoRegistro.Activo
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
