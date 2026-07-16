using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record ComprobanteListDto(
    int ComprobanteId,
    TipoComprobante TipoComprobante,
    string Serie,
    int Correlativo,
    DateTime FechaEmision,
    string Cliente,
    string Documento,
    string Direccion,
    decimal Total,
    decimal TotalPagado,
    decimal SaldoPendiente,
    EstadoPagoComprobante EstadoPago,
    EstadoSunat EstadoSunat,
    bool DocumentoImpreso,
    bool DocumentoAceptadoSunat,
    EstadoRegistro Estado,
    bool PuedeEditar,
    bool PuedeRegistrarPago,
    bool PuedeAnular);

public class ComprobanteEditDto
{
    public int ComprobanteId { get; set; }
    public TipoComprobante TipoComprobante { get; set; } = TipoComprobante.BOL;
    public string Serie { get; set; } = string.Empty;
    public int Correlativo { get; set; }
    public int ClienteId { get; set; }
    public TipoDocumentoCliente? ClienteTipoDocumento { get; set; }
    public string ClienteNumeroDocumento { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string ClienteNombreComercial { get; set; } = string.Empty;
    public string ClienteDireccion { get; set; } = string.Empty;
    public string ClienteTelefono { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public int? CotizacionId { get; set; }
    public int? NotaPedidoId { get; set; }
    public string DocumentoOrigen { get; set; } = string.Empty;
    public int? ComprobanteReferenciaId { get; set; }
    public string ComprobanteReferenciaNumero { get; set; } = string.Empty;
    public string MotivoNotaCredito { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; } = PeruDateTime.Today;
    public FormaPago FormaPago { get; set; } = FormaPago.Contado;
    public EstadoPagoComprobante EstadoPago { get; set; } = EstadoPagoComprobante.PENDIENTE;
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    public EstadoSunat EstadoSunat { get; set; } = EstadoSunat.NoAplica;
    public bool DocumentoImpreso { get; set; }
    public string MedioPago { get; set; } = "EFECTIVO";
    public int? CuentaFinancieraId { get; set; }

    public string EmpresaRazonSocial { get; set; } = string.Empty;
    public string EmpresaNombreComercial { get; set; } = string.Empty;
    public string EmpresaRuc { get; set; } = string.Empty;
    public string EmpresaDireccion { get; set; } = string.Empty;
    public string EmpresaTelefono { get; set; } = string.Empty;
    public string EmpresaEmail { get; set; } = string.Empty;
    public string CondicionesVenta { get; set; } = string.Empty;
    public string CaracteristicasTecnicas { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }

    public List<ComprobanteDetalleDto> Detalles { get; set; } = new();
}

public class ComprobanteDetalleDto
{
    public int ProductoId { get; set; }
    public string Producto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public record ComprobanteResultadoDto(int ComprobanteId, string Serie, int Correlativo, decimal Total, string PdfUrl, EstadoSunat EstadoSunat, string Mensaje = "");
public class NotaCreditoEditDto
{
    public int ComprobanteReferenciaId { get; set; }
    public int MotivoNotaCreditoId { get; set; }
    public string Serie { get; set; } = string.Empty;
    public int Correlativo { get; set; }
    public DateTime FechaEmision { get; set; } = PeruDateTime.Today;
    public string Referencia { get; set; } = string.Empty;
    public TipoComprobante TipoComprobanteOrigen { get; set; }
    public DateTime FechaOrigen { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal TotalCobradoOrigen { get; set; }
    public decimal TotalNotasCreditoEmitidas { get; set; }
    public decimal SaldoDisponible { get; set; }
    public string DocumentoCliente { get; set; } = string.Empty;
    public string SustentoDescripcion { get; set; } = string.Empty;
    public decimal NuevoTotalValido { get; set; }
    public decimal MontoDevolucionEstimado { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public IReadOnlyList<MotivoNotaCreditoOptionDto> Motivos { get; set; } = Array.Empty<MotivoNotaCreditoOptionDto>();
    public List<NotaCreditoDetalleDto> Detalles { get; set; } = new();
}

public class NotaCreditoDetalleDto
{
    public int ProductoId { get; set; }
    public string Producto { get; set; } = string.Empty;
    public decimal CantidadOriginal { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public bool AfectoIgv { get; set; }
    public decimal Total => decimal.Round(Cantidad * PrecioUnitario, 2);
}

public record MotivoNotaCreditoOptionDto(int MotivoNotaCreditoId, string Nombre);

public record NotaCreditoOrigenDto(
    int ComprobanteId,
    TipoComprobante TipoComprobante,
    string Serie,
    int Correlativo,
    string Numero,
    DateTime FechaEmision,
    string Cliente,
    string DocumentoCliente,
    decimal Total,
    EstadoSunat EstadoSunat);

public class NotaCreditoOrigenSearchRequest : SearchRequest
{
    public string? Serie { get; set; }
    public int? Correlativo { get; set; }
    public string? Cliente { get; set; }
    public TipoComprobante? TipoComprobante { get; set; }
}

public class NotaCreditoCreatePageDto
{
    public NotaCreditoEditDto NotaCredito { get; set; } = new();
    public PagedResult<NotaCreditoOrigenDto>? Origenes { get; set; }
    public FormularioConfiguracionDto FormularioConfiguracion { get; set; } = FormularioConfiguracionService.Defaults(FormularioConfiguracionService.TipoNotaCredito);
}
public record ComprobanteNumeracionDto(string Serie, int Correlativo);
public record ComprobanteClienteOptionDto(int ClienteId, string NombreCompleto, string NumeroDocumento, string Direccion, string Telefono, string Email);
public record ComprobanteProductoOptionDto(int ProductoId, string Nombre, string Categoria, string UnidadMedida, decimal PrecioVentaConIgv, decimal Stock, bool AfectoIgv);
public record ComprobanteFormDataDto(
    ComprobanteEditDto Comprobante,
    ComprobanteNumeracionDto Numeracion,
    IReadOnlyCollection<ComprobanteClienteOptionDto> Clientes,
    IReadOnlyCollection<ComprobanteProductoOptionDto> Productos,
    IReadOnlyCollection<CuentaFinancieraOptionDto> CuentasFinancieras,
    FormularioConfiguracionDto FormularioConfiguracion);






