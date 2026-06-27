using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record CompraListDto(
    int CompraId,
    DateTime Fecha,
    string Proveedor,
    string Documento,
    decimal SubTotal,
    decimal Igv,
    decimal Total,
    decimal TotalPagado,
    decimal SaldoPendiente,
    EstadoPagoCompra EstadoPago,
    EstadoDocumentoCompra EstadoDocumento,
    bool PuedeRegistrarPago,
    bool PuedeAnular);

public class CompraEditDto
{
    public int CompraId { get; set; }
    public int ProveedorId { get; set; }
    public TipoDocumentoCompra TipoDocumento { get; set; } = TipoDocumentoCompra.FACTURA;
    public string Serie { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public FormaPagoCompra FormaPago { get; set; } = FormaPagoCompra.CREDITO;
    public string MedioPago { get; set; } = "EFECTIVO";
    public string Observacion { get; set; } = string.Empty;
    public List<CompraDetalleEditDto> Detalles { get; set; } = new() { new CompraDetalleEditDto() };
}

public class CompraDetalleEditDto
{
    public int ProductoId { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
}

public class CompraFormDataDto
{
    public CompraEditDto Compra { get; set; } = new();
    public IReadOnlyList<ProveedorListDto> Proveedores { get; set; } = Array.Empty<ProveedorListDto>();
    public IReadOnlyList<ProductoListDto> Productos { get; set; } = Array.Empty<ProductoListDto>();
}

public record CompraDetalleDto(string Producto, string UnidadMedida, decimal Cantidad, decimal CostoUnitario, decimal Importe, decimal Igv, decimal TotalLinea);

public record PagoProveedorListDto(
    int PagoProveedorId,
    DateTime FechaPago,
    string Proveedor,
    string DocumentoCompra,
    decimal Monto,
    string MedioPago,
    PagoProveedorEstado Estado,
    string Observacion,
    bool PuedeAnular);

public class CompraDetalleViewDto
{
    public int CompraId { get; set; }
    public DateTime Fecha { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public TipoDocumentoCompra TipoDocumento { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public FormaPagoCompra FormaPago { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public EstadoPagoCompra EstadoPago { get; set; }
    public EstadoDocumentoCompra EstadoDocumento { get; set; }
    public IReadOnlyList<CompraDetalleDto> Detalles { get; set; } = Array.Empty<CompraDetalleDto>();
    public IReadOnlyList<PagoProveedorListDto> Pagos { get; set; } = Array.Empty<PagoProveedorListDto>();
    public DevolucionListDto? DevolucionProveedor { get; set; }
}

public class RegistrarPagoProveedorDto
{
    public int CompraId { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public string DocumentoCompra { get; set; } = string.Empty;
    public decimal TotalCompra { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public DateTime FechaPago { get; set; } = PeruDateTime.Today;
    public string MedioPago { get; set; } = "EFECTIVO";
    public decimal MontoPago { get; set; }
    public string Observacion { get; set; } = string.Empty;
}
