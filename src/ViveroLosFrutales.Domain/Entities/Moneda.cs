using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class Moneda
{
    public int MonedaId { get; set; }
    public string Codigo { get; set; } = "PEN";
    public string Descripcion { get; set; } = "Soles";
    public string Simbolo { get; set; } = "S/";
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}
