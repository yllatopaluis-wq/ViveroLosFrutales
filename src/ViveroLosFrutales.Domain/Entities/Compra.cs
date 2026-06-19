using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class Compra : EmpresaEntity
{
    public int CompraId { get; set; }
    public int ProveedorId { get; set; }
    public TipoDocumentoCompra TipoDocumento { get; set; } = TipoDocumentoCompra.FACTURA;
    public string Serie { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Today;
    public DateTime FechaEmision { get => Fecha; set => Fecha = value; }
    public FormaPagoCompra FormaPago { get; set; } = FormaPagoCompra.CREDITO;
    public string Observacion { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public EstadoPagoCompra EstadoPago { get; set; } = EstadoPagoCompra.PENDIENTE;
    public EstadoDocumentoCompra EstadoDocumento { get; set; } = EstadoDocumentoCompra.ACTIVO;
    public DateTime? FechaAnulacion { get; set; }
    public string MotivoAnulacion { get; set; } = string.Empty;
    public string UsuarioAnulacion { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
    public Proveedor? Proveedor { get; set; }
    public ICollection<CompraDetalle> Detalles { get; set; } = new List<CompraDetalle>();
    public ICollection<PagoProveedor> Pagos { get; set; } = new List<PagoProveedor>();
}
