namespace ViveroLosFrutales.Domain.Entities;

public class CotizacionDetalle
{
    public int CotizacionDetalleId { get; set; }
    public int CotizacionId { get; set; }
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe { get; set; }
    public decimal ImporteIgv { get; set; }
    public Cotizacion? Cotizacion { get; set; }
    public Producto? Producto { get; set; }
}
