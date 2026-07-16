namespace ViveroLosFrutales.Domain.Entities;

public class OrdenCompraDetalle
{
    public int OrdenCompraDetalleId { get; set; }
    public int OrdenCompraId { get; set; }
    public int ProductoId { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public int Orden { get; set; }
    public decimal CantidadFacturada { get; set; }
    public decimal CantidadRecibida { get; set; }

    public OrdenCompra? OrdenCompra { get; set; }
    public Producto? Producto { get; set; }
}
