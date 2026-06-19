using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record UsuarioListDto(string UsuarioId, string Nombres, string Apellidos, string Correo, string Usuario, string Rol, bool Activo);

public class UsuarioEditDto
{
    public string UsuarioId { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; } = true;
    public List<int> EmpresaIds { get; set; } = new();
}

public record UsuarioEmpresaDto(int EmpresaId, string RazonSocial, string NombreComercial);

public class LoginDto
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
    public bool Recordarme { get; set; }
}

public class SeleccionarEmpresaDto
{
    public int EmpresaId { get; set; }
    public IReadOnlyList<UsuarioEmpresaDto> Empresas { get; set; } = Array.Empty<UsuarioEmpresaDto>();
}
