using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class NotaPedido : EmpresaEntity
{
    public int NotaPedidoId { get; set; }
    public int ClienteId { get; set; }
    public int? CotizacionId { get; set; }
    public int? ComprobanteId { get; set; }
    public string Serie { get; set; } = string.Empty;
    public int Correlativo { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal TotalCobrado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public NotaPedidoEstado EstadoDocumento { get; set; } = NotaPedidoEstado.ACTIVO;
    public EstadoPagoNotaPedido EstadoPago { get; set; } = EstadoPagoNotaPedido.PENDIENTE;
    public DateTime? FechaModificacion { get; set; }
    public string UsuarioModificacion { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
    public Cliente? Cliente { get; set; }
    public Cotizacion? Cotizacion { get; set; }
    public ICollection<NotaPedidoDetalle> Detalles { get; set; } = new List<NotaPedidoDetalle>();
    public ICollection<CobroCliente> Cobros { get; set; } = new List<CobroCliente>();
    public ICollection<Comprobante> Comprobantes { get; set; } = new List<Comprobante>();

    public void RecalcularTotales()
    {
        Subtotal = decimal.Round(Detalles.Sum(x => x.Subtotal), 2);
        Igv = decimal.Round(Detalles.Sum(x => x.Igv), 2);
        Total = decimal.Round(Detalles.Sum(x => x.Total), 2);
        SaldoPendiente = decimal.Round(Total - TotalCobrado < 0 ? 0 : Total - TotalCobrado, 2);
        EstadoPago = TotalCobrado <= 0
            ? EstadoPagoNotaPedido.PENDIENTE
            : SaldoPendiente <= 0
                ? EstadoPagoNotaPedido.PAGADO
                : EstadoPagoNotaPedido.PAGO_PARCIAL;
    }
}
