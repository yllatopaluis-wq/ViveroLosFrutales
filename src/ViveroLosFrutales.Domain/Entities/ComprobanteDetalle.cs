namespace ViveroLosFrutales.Domain.Entities;

public class ComprobanteDetalle
{
    public int ComprobanteDetalleId { get; set; }
    public int ComprobanteId { get; set; }
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe { get; set; }
    public decimal ImporteIgv { get; set; }
    public decimal MontoDetraccion { get; set; }
    public Comprobante? Comprobante { get; set; }
    public Producto? Producto { get; set; }
}
