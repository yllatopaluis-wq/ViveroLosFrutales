using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

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

        return await db.Permisos.AsNoTracking()
            .OrderBy(x => x.Modulo)
            .ThenBy(x => x.Accion)
            .Select(x => new PermisoDto(x.PermisoId, x.Modulo, x.Accion, x.Descripcion))
            .ToListAsync(cancellationToken);
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

        rol.RolPermisos.Clear();
        foreach (var permisoId in dto.PermisoIds.Distinct())
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
        var modulos = new[]
        {
            "Cotizaciones",
            "Comprobantes",
            "NotasCredito",
            "Categorias",
            "Productos",
            "Clientes",
            "Proveedores",
            "Compras",
            "NotasPedido",
            "CobrosClientes",
            "Caja",
            "EstadoCuentaClientes",
            "Gastos",
            "Ingresos",
            "Empresas",
            "Usuarios",
            "Roles",
            "Configuracion",
            "NubefactLogs",
            "Reportes"
        };
        var acciones = new[] { "Ver", "Crear", "Editar", "Anular", "Imprimir", "Configurar", "Convertir", "RegistrarPago" };
        var existentes = await db.Permisos
            .Select(x => new { x.Modulo, x.Accion })
            .ToListAsync(cancellationToken);
        var existentesSet = existentes.Select(x => $"{x.Modulo}|{x.Accion}").ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var modulo in modulos)
        {
            foreach (var accion in acciones)
            {
                if (existentesSet.Contains($"{modulo}|{accion}")) continue;

                db.Permisos.Add(new Permiso
                {
                    Modulo = modulo,
                    Accion = accion,
                    Descripcion = $"{accion} {modulo}",
                    Estado = EstadoRegistro.Activo
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
