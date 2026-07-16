using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record OrdenCompraListDto(
    int OrdenCompraId,
    DateTime Fecha,
    string Documento,
    string Proveedor,
    decimal Total,
    decimal TotalFacturado,
    decimal TotalPagado,
    decimal TotalAplicado,
    decimal SaldoDisponible,
    decimal PendienteFacturar,
    EstadoFacturacionOrdenCompra EstadoFacturacion,
    EstadoFinancieroOrdenCompra EstadoFinanciero,
    EstadoDocumentoOrdenCompra EstadoDocumento,
    bool PuedeRegistrarPago,
    bool PuedeRegistrarCompra,
    bool PuedeAplicarPagos,
    bool PuedeSolicitarDevolucion,
    bool PuedeCerrar,
    bool PuedeAnular);

public class OrdenCompraEditDto
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
    public decimal PorcentajeAdelanto { get; set; }
    public int PlazoDias { get; set; }
    public string CondicionEntrega { get; set; } = string.Empty;
    public string Garantia { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public List<OrdenCompraDetalleEditDto> Detalles { get; set; } = new() { new OrdenCompraDetalleEditDto() };
}

public class OrdenCompraDetalleEditDto
{
    public int ProductoId { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
}

public class OrdenCompraFormDataDto
{
    public OrdenCompraEditDto OrdenCompra { get; set; } = new();
    public IReadOnlyList<ProveedorListDto> Proveedores { get; set; } = Array.Empty<ProveedorListDto>();
    public IReadOnlyList<ProductoListDto> Productos { get; set; } = Array.Empty<ProductoListDto>();
    public FormularioConfiguracionDto FormularioConfiguracion { get; set; } = ViveroLosFrutales.Application.Services.FormularioConfiguracionService.Defaults("ORDEN_COMPRA");
}

public record OrdenCompraDetalleDto(
    string Producto,
    string UnidadMedida,
    decimal Cantidad,
    decimal CantidadFacturada,
    decimal CantidadRecibida,
    decimal CostoUnitario,
    decimal Subtotal,
    decimal Igv,
    decimal Total);

public class OrdenCompraDetalleViewDto
{
    public int OrdenCompraId { get; set; }
    public DateTime Fecha { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public string ProveedorDocumento { get; set; } = string.Empty;
    public string ProveedorDireccion { get; set; } = string.Empty;
    public string Moneda { get; set; } = "Soles";
    public DateTime? FechaEntregaEsperada { get; set; }
    public string LugarEntrega { get; set; } = string.Empty;
    public FormaPagoCompra FormaPago { get; set; }
    public string CondicionPago { get; set; } = string.Empty;
    public string Garantia { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal TotalFacturado { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal TotalAplicado { get; set; }
    public decimal TotalDevuelto { get; set; }
    public decimal SaldoDisponible { get; set; }
    public decimal PendienteFacturar { get; set; }
    public EstadoDocumentoOrdenCompra EstadoDocumento { get; set; }
    public EstadoFacturacionOrdenCompra EstadoFacturacion { get; set; }
    public EstadoRecepcionOrdenCompra EstadoRecepcion { get; set; }
    public EstadoFinancieroOrdenCompra EstadoFinanciero { get; set; }
    public IReadOnlyList<OrdenCompraDetalleDto> Detalles { get; set; } = Array.Empty<OrdenCompraDetalleDto>();
    public IReadOnlyList<CompraListDto> Compras { get; set; } = Array.Empty<CompraListDto>();
    public IReadOnlyList<PagoProveedorOrdenDto> Pagos { get; set; } = Array.Empty<PagoProveedorOrdenDto>();
    public IReadOnlyList<PagoProveedorAplicacionListDto> Aplicaciones { get; set; } = Array.Empty<PagoProveedorAplicacionListDto>();
    public IReadOnlyList<DevolucionListDto> Devoluciones { get; set; } = Array.Empty<DevolucionListDto>();
}

public record PagoProveedorOrdenDto(
    int PagoProveedorId,
    DateTime FechaPago,
    string MedioPago,
    string CuentaFinanciera,
    decimal Monto,
    decimal MontoAplicado,
    decimal MontoReservadoDevolucion,
    decimal MontoDevuelto,
    decimal SaldoDisponible,
    PagoProveedorEstado Estado,
    string Observacion);

public record PagoProveedorAplicacionListDto(
    int PagoProveedorAplicacionId,
    DateTime FechaAplicacion,
    string Pago,
    string Compra,
    decimal MontoAplicado,
    EstadoPagoProveedorAplicacion Estado);

public class RegistrarPagoOrdenCompraDto
{
    public int OrdenCompraId { get; set; }
    public string OrdenCompra { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public decimal TotalOrden { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal TotalAplicado { get; set; }
    public decimal SaldoDisponible { get; set; }
    public DateTime FechaPago { get; set; } = PeruDateTime.Today;
    public decimal MontoPago { get; set; }
    public string MedioPago { get; set; } = "EFECTIVO";
    public int? CuentaFinancieraId { get; set; }
    public string NumeroOperacion { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public IReadOnlyList<CuentaFinancieraOptionDto> CuentasFinancieras { get; set; } = Array.Empty<CuentaFinancieraOptionDto>();
}

public class AplicarPagoProveedorDto
{
    public int CompraId { get; set; }
    public decimal MontoAplicar { get; set; }
    public List<AplicarPagoProveedorItemDto> Pagos { get; set; } = new();
}


public class AplicarPagoProveedorFormDto : AplicarPagoProveedorDto
{
    public string Compra { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public string OrdenCompra { get; set; } = string.Empty;
    public decimal TotalCompra { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public IReadOnlyList<PagoProveedorDisponibleDto> PagosDisponibles { get; set; } = Array.Empty<PagoProveedorDisponibleDto>();
}
public class AplicarPagoProveedorItemDto
{
    public int PagoProveedorId { get; set; }
    public decimal MontoAplicar { get; set; }
}

public record PagoProveedorDisponibleDto(
    int PagoProveedorId,
    DateTime FechaPago,
    string Referencia,
    decimal MontoOriginal,
    decimal MontoAplicado,
    decimal MontoReservadoDevolucion,
    decimal MontoDevuelto,
    decimal SaldoDisponible);