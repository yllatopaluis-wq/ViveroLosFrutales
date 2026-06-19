using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ConfiguracionEmpresaRepository(ApplicationDbContext db) : IConfiguracionEmpresaRepository
{
    public Task<PagedResult<ConfiguracionEmpresaListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.ConfiguracionesEmpresa.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Clave.Contains(term) || x.Descripcion.Contains(term));
        }

        return query.OrderBy(x => x.Clave)
            .Select(x => new ConfiguracionEmpresaListDto(x.ConfiguracionEmpresaId, x.Clave, x.Valor, x.Descripcion))
            .ToPagedAsync(request, cancellationToken);
    }

    public async Task<ConfiguracionEmpresaEditDto?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken)
    {
        return await db.ConfiguracionesEmpresa.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.ConfiguracionEmpresaId == id)
            .Select(x => new ConfiguracionEmpresaEditDto
            {
                ConfiguracionEmpresaId = x.ConfiguracionEmpresaId,
                Clave = x.Clave,
                Valor = x.Valor,
                Descripcion = x.Descripcion
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ConfiguracionEmpresaEditDto?> ObtenerPorClaveAsync(int empresaId, string clave, CancellationToken cancellationToken)
    {
        return await db.ConfiguracionesEmpresa.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Clave == clave)
            .Select(x => new ConfiguracionEmpresaEditDto
            {
                ConfiguracionEmpresaId = x.ConfiguracionEmpresaId,
                Clave = x.Clave,
                Valor = x.Valor,
                Descripcion = x.Descripcion
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task GuardarAsync(int empresaId, string usuario, ConfiguracionEmpresaEditDto dto, CancellationToken cancellationToken)
    {
        var config = dto.ConfiguracionEmpresaId == 0
            ? new ConfiguracionEmpresa { EmpresaId = empresaId, UsuarioRegistro = usuario }
            : await db.ConfiguracionesEmpresa.FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.ConfiguracionEmpresaId == dto.ConfiguracionEmpresaId, cancellationToken);

        if (config is null) throw new InvalidOperationException("Configuracion no encontrada.");

        config.Clave = dto.Clave.Trim();
        config.Valor = dto.Valor.Trim();
        config.Descripcion = dto.Descripcion.Trim();

        if (dto.ConfiguracionEmpresaId == 0) db.ConfiguracionesEmpresa.Add(config);

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task GuardarPorClaveAsync(int empresaId, string usuario, string clave, string valor, string descripcion, CancellationToken cancellationToken)
    {
        var config = await db.ConfiguracionesEmpresa
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.Clave == clave, cancellationToken);

        if (config is null)
        {
            config = new ConfiguracionEmpresa
            {
                EmpresaId = empresaId,
                Clave = clave,
                UsuarioRegistro = usuario
            };
            db.ConfiguracionesEmpresa.Add(config);
        }

        config.Valor = valor.Trim();
        config.Descripcion = descripcion.Trim();
        await db.SaveChangesAsync(cancellationToken);
    }
}
