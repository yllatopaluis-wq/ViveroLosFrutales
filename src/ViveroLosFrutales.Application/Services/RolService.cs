using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class RolService(IRolRepository repository)
{
    public Task<PagedResult<RolListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(request, cancellationToken);

    public Task<RolEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        repository.ObtenerAsync(id, cancellationToken);

    public Task<IReadOnlyList<PermisoDto>> ObtenerPermisosAsync(CancellationToken cancellationToken) =>
        repository.ObtenerPermisosAsync(cancellationToken);

    public Task<bool> TienePermisoAsync(int rolId, string modulo, string accion, CancellationToken cancellationToken) =>
        repository.TienePermisoAsync(rolId, modulo, accion, cancellationToken);

    public async Task GuardarAsync(RolEditDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new InvalidOperationException("El nombre del rol es obligatorio.");
        dto.Estado = dto.Estado == 0 ? EstadoRegistro.Activo : dto.Estado;
        await repository.GuardarAsync(dto, cancellationToken);
    }
}
