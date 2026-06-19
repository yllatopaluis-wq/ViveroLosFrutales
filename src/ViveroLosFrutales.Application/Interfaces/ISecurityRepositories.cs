using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;

namespace ViveroLosFrutales.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<PagedResult<UsuarioListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken);
    Task<UsuarioEditDto?> ObtenerAsync(string id, CancellationToken cancellationToken);
    Task GuardarAsync(UsuarioEditDto dto, CancellationToken cancellationToken);
    Task RestablecerPasswordAsync(string id, string password, CancellationToken cancellationToken);
    Task<IReadOnlyList<UsuarioEmpresaDto>> ObtenerEmpresasAsync(string usuarioId, CancellationToken cancellationToken);
}

public interface IRolRepository
{
    Task<PagedResult<RolListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken);
    Task<RolEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PermisoDto>> ObtenerPermisosAsync(CancellationToken cancellationToken);
    Task GuardarAsync(RolEditDto dto, CancellationToken cancellationToken);
    Task<bool> TienePermisoAsync(int rolId, string modulo, string accion, CancellationToken cancellationToken);
}

public interface IConfiguracionEmpresaRepository
{
    Task<PagedResult<ConfiguracionEmpresaListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<ConfiguracionEmpresaEditDto?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<ConfiguracionEmpresaEditDto?> ObtenerPorClaveAsync(int empresaId, string clave, CancellationToken cancellationToken);
    Task GuardarAsync(int empresaId, string usuario, ConfiguracionEmpresaEditDto dto, CancellationToken cancellationToken);
    Task GuardarPorClaveAsync(int empresaId, string usuario, string clave, string valor, string descripcion, CancellationToken cancellationToken);
}
