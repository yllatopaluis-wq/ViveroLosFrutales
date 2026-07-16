using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class OrdenCompra : EmpresaEntity
{
    public int OrdenCompraId { get; set; }
    public int ProveedorId { get; set; }
    public string Serie { get; set; } = "OC001";
    public int Correlativo { get; set; }
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public string Moneda { get; set; } = "Soles";
    public DateTime? FechaEntregaEsperada { get; set; }
    public string LugarEntrega { get; set; } = string.Empty;
    public FormaPagoCompra FormaPago { get; set; } = FormaPagoCompra.CREDITO;
    public string CondicionPago { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public string CondicionEntrega { get; set; } = string.Empty;
    public string Garantia { get; set; } = string.Empty;
    public decimal PorcentajeAdelanto { get; set; }
    public int PlazoDias { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal TotalFacturado { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal TotalAplicado { get; set; }
    public decimal TotalDevuelto { get; set; }
    public decimal SaldoDisponible { get; set; }
    public decimal PendienteFacturar { get; set; }
    public EstadoDocumentoOrdenCompra EstadoDocumento { get; set; } = EstadoDocumentoOrdenCompra.ACTIVO;
    public EstadoAprobacionOrdenCompra EstadoAprobacion { get; set; } = EstadoAprobacionOrdenCompra.APROBADO;
    public EstadoFacturacionOrdenCompra EstadoFacturacion { get; set; } = EstadoFacturacionOrdenCompra.NO_FACTURADO;
    public EstadoRecepcionOrdenCompra EstadoRecepcion { get; set; } = EstadoRecepcionOrdenCompra.NO_RECIBIDO;
    public EstadoFinancieroOrdenCompra EstadoFinanciero { get; set; } = EstadoFinancieroOrdenCompra.SIN_PAGOS;
    public DateTime? FechaAnulacion { get; set; }
    public string MotivoAnulacion { get; set; } = string.Empty;
    public string UsuarioAnulacion { get; set; } = string.Empty;
    public string ProveedorTipoDocumento { get; set; } = string.Empty;
    public string ProveedorNumeroDocumento { get; set; } = string.Empty;
    public string ProveedorRazonSocial { get; set; } = string.Empty;
    public string ProveedorNombreComercial { get; set; } = string.Empty;
    public string ProveedorDireccion { get; set; } = string.Empty;
    public string ProveedorTelefono { get; set; } = string.Empty;
    public string ProveedorEmail { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Empresa? Empresa { get; set; }
    public Proveedor? Proveedor { get; set; }
    public ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    public ICollection<PagoProveedor> Pagos { get; set; } = new List<PagoProveedor>();
    public ICollection<Devolucion> Devoluciones { get; set; } = new List<Devolucion>();
}