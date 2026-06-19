using System.Text.RegularExpressions;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public class UsuarioService(IUsuarioRepository repository)
{
    public Task<PagedResult<UsuarioListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(request, cancellationToken);

    public Task<UsuarioEditDto?> ObtenerAsync(string id, CancellationToken cancellationToken) =>
        repository.ObtenerAsync(id, cancellationToken);

    public Task<IReadOnlyList<UsuarioEmpresaDto>> ObtenerEmpresasAsync(string usuarioId, CancellationToken cancellationToken) =>
        repository.ObtenerEmpresasAsync(usuarioId, cancellationToken);

    public async Task GuardarAsync(UsuarioEditDto dto, CancellationToken cancellationToken)
    {
        Validar(dto);
        await repository.GuardarAsync(dto, cancellationToken);
    }

    public async Task RestablecerPasswordAsync(string id, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new InvalidOperationException("La contrasena debe tener al menos 8 caracteres.");
        }

        await repository.RestablecerPasswordAsync(id, password, cancellationToken);
    }

    private static void Validar(UsuarioEditDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombres)) throw new InvalidOperationException("Los nombres son obligatorios.");
        if (string.IsNullOrWhiteSpace(dto.Apellidos)) throw new InvalidOperationException("Los apellidos son obligatorios.");
        if (string.IsNullOrWhiteSpace(dto.Usuario)) throw new InvalidOperationException("El usuario es obligatorio.");
        if (!Regex.IsMatch(dto.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new InvalidOperationException("El correo no tiene un formato valido.");
        if (dto.RolId == 0) throw new InvalidOperationException("Seleccione un rol.");
        if (dto.EmpresaIds.Count == 0) throw new InvalidOperationException("Asigne al menos una empresa.");
        if (string.IsNullOrEmpty(dto.UsuarioId) && (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8))
        {
            throw new InvalidOperationException("La contrasena inicial debe tener al menos 8 caracteres.");
        }
    }
}
