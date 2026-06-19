using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record CategoriaListDto(int CategoriaId, string Nombre, string Descripcion, EstadoRegistro Estado);

public class CategoriaEditDto
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}
