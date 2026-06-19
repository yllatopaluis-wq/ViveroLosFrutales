namespace ViveroLosFrutales.Domain.Entities;

public class NotaPedidoDetalle
{
    public int NotaPedidoDetalleId { get; set; }
    public int NotaPedidoId { get; set; }
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public NotaPedido? NotaPedido { get; set; }
    public Producto? Producto { get; set; }
}
