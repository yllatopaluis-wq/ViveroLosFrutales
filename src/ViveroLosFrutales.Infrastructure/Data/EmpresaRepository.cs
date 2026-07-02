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

    public Task<EmpresaMarcaDto?> ObtenerMarcaActivaAsync(int empresaId, string usuarioId, CancellationToken cancellationToken) =>
        db.UsuarioEmpresas.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.UsuarioId == usuarioId && x.Empresa!.Estado == ViveroLosFrutales.Domain.Enums.EstadoRegistro.Activo)
            .Select(x => new EmpresaMarcaDto(
                x.EmpresaId,
                x.Empresa!.NombreComercial,
                x.Empresa.RazonSocial,
                x.Empresa.LogoContenido != null && x.Empresa.LogoContenido.Length > 0 && x.Empresa.LogoContentType != null && x.Empresa.LogoContentType != string.Empty))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Empresa?> ObtenerLogoActivaAsync(int empresaId, string usuarioId, CancellationToken cancellationToken) =>
        db.UsuarioEmpresas.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.UsuarioId == usuarioId && x.Empresa!.Estado == ViveroLosFrutales.Domain.Enums.EstadoRegistro.Activo)
            .Select(x => x.Empresa!)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task GuardarAsync(Empresa empresa, CancellationToken cancellationToken)
    {
        if (empresa.EmpresaId == 0) db.Empresas.Add(empresa);
        await db.SaveChangesAsync(cancellationToken);
    }
}
