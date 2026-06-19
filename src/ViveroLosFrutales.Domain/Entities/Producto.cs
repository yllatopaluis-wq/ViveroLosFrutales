using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class Producto : EmpresaEntity
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
    public Empresa? Empresa { get; set; }

    public void RecalcularPrecioConIgv()
    {
        PrecioVentaConIgv = AfectoIgv ? decimal.Round(PrecioVentaSinIgv * 1.18m, 2) : PrecioVentaSinIgv;
    }
}
