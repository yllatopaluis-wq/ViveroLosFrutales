using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record RolListDto(int RolId, string Nombre, string Descripcion, EstadoRegistro Estado);

public class RolEditDto
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    public List<int> PermisoIds { get; set; } = new();
}

public record PermisoDto(int PermisoId, string Modulo, string Accion, string Descripcion);
