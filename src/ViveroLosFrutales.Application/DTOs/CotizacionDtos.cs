using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record CotizacionListDto(
    int CotizacionId,
    string Serie,
    int Correlativo,
    DateTime FechaEmision,
    string Cliente,
    TipoDocumentoCliente TipoDocumento,
    string Documento,
    decimal Total,
    CotizacionEstado EstadoCotizacion);

public class CotizacionEditDto
{
    public int CotizacionId { get; set; }
    public int ClienteId { get; set; }
    public TipoDocumentoCliente? ClienteTipoDocumento { get; set; }
    public string ClienteNumeroDocumento { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string ClienteNombreComercial { get; set; } = string.Empty;
    public string ClienteDireccion { get; set; } = string.Empty;
    public string ClienteTelefono { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public int Correlativo { get; set; }
    public DateTime FechaEmision { get; set; } = PeruDateTime.Today;
    public string Direccion { get; set; } = string.Empty;
    public FormaPago FormaPago { get; set; } = FormaPago.Contado;
    public string CondicionesVenta { get; set; } = string.Empty;
    public string CaracteristicasTecnicas { get; set; } = string.Empty;
    public List<ComprobanteDetalleDto> Detalles { get; set; } = new() { new ComprobanteDetalleDto() };
}

public record CotizacionNumeracionDto(string Serie, int Correlativo);
public record CotizacionResultadoDto(int CotizacionId, string Serie, int Correlativo, decimal Total, string PdfUrl, CotizacionEstado Estado);
public record CotizacionFormDataDto(
    CotizacionEditDto Cotizacion,
    CotizacionNumeracionDto Numeracion,
    IReadOnlyCollection<ComprobanteClienteOptionDto> Clientes,
    IReadOnlyCollection<ComprobanteProductoOptionDto> Productos,
    FormularioConfiguracionDto FormularioConfiguracion,
    CondicionComercialPlantillaDto CondicionesComerciales);

public class CotizacionDetalleViewDto
{
    public int CotizacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public TipoDocumentoCliente TipoDocumento { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public FormaPago FormaPago { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public CotizacionEstado EstadoCotizacion { get; set; }
    public bool TieneDocumentosRelacionados { get; set; }
    public IReadOnlyList<NotaPedidoDetalleDto> Detalles { get; set; } = Array.Empty<NotaPedidoDetalleDto>();
}

