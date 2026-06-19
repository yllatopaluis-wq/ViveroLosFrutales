using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Infrastructure.Identity;

namespace ViveroLosFrutales.Infrastructure.Data;

public class UsuarioRepository(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager) : IUsuarioRepository
{
    public Task<PagedResult<UsuarioListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.UserName!.Contains(term) || x.Email!.Contains(term) || x.Nombres.Contains(term) || x.Apellidos.Contains(term));
        }

        var projected = query.OrderBy(x => x.Nombres)
            .Select(x => new UsuarioListDto(
                x.Id,
                x.Nombres,
                x.Apellidos,
                x.Email ?? string.Empty,
                x.UserName ?? string.Empty,
                db.RolesNegocio.Where(r => r.RolId == x.RolId).Select(r => r.Nombre).FirstOrDefault() ?? string.Empty,
                x.Activo));

        return projected.ToPagedAsync(request, cancellationToken);
    }

    public async Task<UsuarioEditDto?> ObtenerAsync(string id, CancellationToken cancellationToken)
    {
        return await db.Users.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new UsuarioEditDto
            {
                UsuarioId = x.Id,
                Nombres = x.Nombres,
                Apellidos = x.Apellidos,
                Correo = x.Email ?? string.Empty,
                Usuario = x.UserName ?? string.Empty,
                RolId = x.RolId,
                Activo = x.Activo,
                EmpresaIds = db.UsuarioEmpresas.Where(e => e.UsuarioId == x.Id).Select(e => e.EmpresaId).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task GuardarAsync(UsuarioEditDto dto, CancellationToken cancellationToken)
    {
        var user = string.IsNullOrEmpty(dto.UsuarioId)
            ? new ApplicationUser()
            : await userManager.FindByIdAsync(dto.UsuarioId);

        if (user is null) throw new InvalidOperationException("Usuario no encontrado.");

        user.Nombres = dto.Nombres.Trim();
        user.Apellidos = dto.Apellidos.Trim();
        user.Email = dto.Correo.Trim();
        user.UserName = dto.Usuario.Trim();
        user.RolId = dto.RolId;
        user.Activo = dto.Activo;

        IdentityResult result;
        if (string.IsNullOrEmpty(dto.UsuarioId))
        {
            result = await userManager.CreateAsync(user, dto.Password);
        }
        else
        {
            result = await userManager.UpdateAsync(user);
        }

        if (!result.Succeeded) throw new InvalidOperationException(string.Join(" ", result.Errors.Select(x => x.Description)));

        var actuales = await db.UsuarioEmpresas.Where(x => x.UsuarioId == user.Id).ToListAsync(cancellationToken);
        db.UsuarioEmpresas.RemoveRange(actuales);
        foreach (var empresaId in dto.EmpresaIds.Distinct())
        {
            db.UsuarioEmpresas.Add(new UsuarioEmpresa { UsuarioId = user.Id, EmpresaId = empresaId });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RestablecerPasswordAsync(string id, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id) ?? throw new InvalidOperationException("Usuario no encontrado.");
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join(" ", result.Errors.Select(x => x.Description)));
    }

    public async Task<IReadOnlyList<UsuarioEmpresaDto>> ObtenerEmpresasAsync(string usuarioId, CancellationToken cancellationToken)
    {
        return await db.UsuarioEmpresas.AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId)
            .OrderBy(x => x.Empresa!.RazonSocial)
            .Select(x => new UsuarioEmpresaDto(x.EmpresaId, x.Empresa!.RazonSocial, x.Empresa.NombreComercial))
            .ToListAsync(cancellationToken);
    }
}
