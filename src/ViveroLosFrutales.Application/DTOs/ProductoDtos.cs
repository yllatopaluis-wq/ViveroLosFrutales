using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record ProductoListDto(int ProductoId, string Categoria, string Nombre, string UnidadMedida, decimal PrecioVentaSinIgv, decimal PrecioVentaConIgv, decimal Stock, bool AfectoIgv, EstadoRegistro Estado);

public class ProductoEditDto
{
    public int ProductoId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = "NIU";
    public decimal Stock { get; set; }
    public bool AfectoIgv { get; set; } = true;
    public decimal PrecioVentaSinIgv { get; set; }
    public decimal PrecioVentaConIgv { get; set; }
    public bool TieneDetraccion { get; set; }
    public decimal PorcentajeDetraccion { get; set; }
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}

