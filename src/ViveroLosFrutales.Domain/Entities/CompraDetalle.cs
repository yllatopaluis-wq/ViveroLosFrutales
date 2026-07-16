namespace ViveroLosFrutales.Domain.Entities;

public class CompraDetalle
{
    public int CompraDetalleId { get; set; }
    public int CompraId { get; set; }
    public int ProductoId { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal CantidadRecibida { get; set; }
    public decimal CantidadPendiente => Math.Max(Cantidad - CantidadRecibida, 0);
    public decimal CostoUnitario { get; set; }
    public decimal Importe { get; set; }
    public decimal Igv { get; set; }
    public decimal TotalLinea { get; set; }
    public Compra? Compra { get; set; }
    public Producto? Producto { get; set; }
}



