using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class Devolucion : EmpresaEntity
{
    public int DevolucionId { get; set; }
    public TipoTerceroDevolucion TipoTercero { get; set; } = TipoTerceroDevolucion.CLIENTE;
    public int? ClienteId { get; set; }
    public int? ProveedorId { get; set; }
    public OrigenDevolucion Origen { get; set; } = OrigenDevolucion.ANULACION_NOTA_PEDIDO;
    public int? NotaPedidoId { get; set; }
    public int? ComprobanteId { get; set; }
    public int? NotaCreditoId { get; set; }
    public int? CompraId { get; set; }
    public DateTime FechaGeneracion { get; set; } = DateTime.Today;
    public decimal MontoOriginal { get; set; }
    public decimal MontoDevuelto { get; set; }
    public decimal MontoPendiente { get; set; }
    public EstadoDevolucion EstadoDevolucion { get; set; } = EstadoDevolucion.PENDIENTE;
    public string Observacion { get; set; } = string.Empty;
    public string MotivoGeneracion { get; set; } = string.Empty;
    public DateTime? FechaModificacion { get; set; }
    public string UsuarioModificacion { get; set; } = string.Empty;

    public Empresa? Empresa { get; set; }
    public Cliente? Cliente { get; set; }
    public Proveedor? Proveedor { get; set; }
    public NotaPedido? NotaPedido { get; set; }
    public Comprobante? Comprobante { get; set; }
    public Comprobante? NotaCredito { get; set; }
    public Compra? Compra { get; set; }
}
