using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class Comprobante : EmpresaEntity
{
    public int ComprobanteId { get; set; }
    public TipoComprobante TipoComprobante { get; set; }
    public string Serie { get; set; } = string.Empty;
    public int Correlativo { get; set; }
    public int ClienteId { get; set; }
    public TipoDocumentoCliente? ClienteTipoDocumento { get; set; }
    public string? ClienteNumeroDocumento { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteNombreComercial { get; set; }
    public string? ClienteDireccion { get; set; }
    public string? ClienteTelefono { get; set; }
    public string? ClienteEmail { get; set; }
    public int? CotizacionId { get; set; }
    public int? NotaPedidoId { get; set; }
    public int? ComprobanteReferenciaId { get; set; }
    public int? MotivoNotaCreditoId { get; set; }
    public string MotivoNotaCredito { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; } = PeruDateTime.Today;
    public FormaPago FormaPago { get; set; } = FormaPago.Contado;

    public string EmpresaRazonSocial { get; set; } = string.Empty;
    public string EmpresaNombreComercial { get; set; } = string.Empty;
    public string EmpresaRuc { get; set; } = string.Empty;
    public string EmpresaDireccion { get; set; } = string.Empty;
    public string EmpresaTelefono { get; set; } = string.Empty;
    public string EmpresaEmail { get; set; } = string.Empty;
    public string CondicionesVenta { get; set; } = string.Empty;
    public string CaracteristicasTecnicas { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public decimal MontoDetraccion { get; set; }
    public EstadoSunat EstadoSunat { get; set; } = EstadoSunat.NoAplica;
    public EstadoPagoComprobante EstadoPago { get; set; } = EstadoPagoComprobante.PENDIENTE;
    public bool DocumentoImpreso { get; set; }
    public string PdfUrl { get; set; } = string.Empty;
    public string XmlUrl { get; set; } = string.Empty;
    public string NubefactHash { get; set; } = string.Empty;
    public string NubefactRespuesta { get; set; } = string.Empty;
    public string MotivoAnulacion { get; set; } = string.Empty;

    public Empresa? Empresa { get; set; }
    public Cliente? Cliente { get; set; }
    public Cotizacion? Cotizacion { get; set; }
    public NotaPedido? NotaPedido { get; set; }
    public Comprobante? ComprobanteReferencia { get; set; }
    public MotivoNotaCredito? MotivoNotaCreditoCatalogo { get; set; }
    public ICollection<ComprobanteDetalle> Detalles { get; set; } = new List<ComprobanteDetalle>();
    public ICollection<CobroCliente> Cobros { get; set; } = new List<CobroCliente>();
    public ICollection<ComprobanteCobroAplicado> CobrosAplicados { get; set; } = new List<ComprobanteCobroAplicado>();

    public TipoDocumentoCliente? ClienteTipoDocumentoMostrar => ClienteTipoDocumento ?? Cliente?.TipoDocumento;
    public string ClienteNumeroDocumentoMostrar => FirstNotEmpty(ClienteNumeroDocumento, Cliente?.NumeroDocumento);
    public string ClienteNombreMostrar => FirstNotEmpty(ClienteNombre, Cliente?.NombreCompleto);
    public string ClienteNombreComercialMostrar => FirstNotEmpty(ClienteNombreComercial);
    public string ClienteDireccionMostrar => FirstNotEmpty(ClienteDireccion, Direccion, Cliente?.Direccion);
    public string ClienteTelefonoMostrar => FirstNotEmpty(ClienteTelefono, Cliente?.Telefono);
    public string ClienteEmailMostrar => FirstNotEmpty(ClienteEmail, Cliente?.Email);

    public void AplicarSnapshotCliente(Cliente cliente, string? direccionDocumento = null)
    {
        ClienteTipoDocumento = cliente.TipoDocumento;
        ClienteNumeroDocumento = cliente.NumeroDocumento;
        ClienteNombre = cliente.NombreCompleto;
        ClienteNombreComercial = string.Empty;
        ClienteDireccion = FirstNotEmpty(direccionDocumento, cliente.Direccion);
        ClienteTelefono = cliente.Telefono;
        ClienteEmail = cliente.Email;
    }

    public void AplicarSnapshotClienteDesde(Comprobante origen)
    {
        ClienteTipoDocumento = origen.ClienteTipoDocumento ?? origen.Cliente?.TipoDocumento;
        ClienteNumeroDocumento = FirstNotEmpty(origen.ClienteNumeroDocumento, origen.Cliente?.NumeroDocumento);
        ClienteNombre = FirstNotEmpty(origen.ClienteNombre, origen.Cliente?.NombreCompleto);
        ClienteNombreComercial = FirstNotEmpty(origen.ClienteNombreComercial);
        ClienteDireccion = FirstNotEmpty(origen.ClienteDireccion, origen.Direccion, origen.Cliente?.Direccion);
        ClienteTelefono = FirstNotEmpty(origen.ClienteTelefono, origen.Cliente?.Telefono);
        ClienteEmail = FirstNotEmpty(origen.ClienteEmail, origen.Cliente?.Email);
    }


    public void AplicarSnapshotClienteDesde(NotaPedido origen)
    {
        ClienteTipoDocumento = origen.ClienteTipoDocumento ?? origen.Cliente?.TipoDocumento;
        ClienteNumeroDocumento = FirstNotEmpty(origen.ClienteNumeroDocumento, origen.Cliente?.NumeroDocumento);
        ClienteNombre = FirstNotEmpty(origen.ClienteNombre, origen.Cliente?.NombreCompleto);
        ClienteNombreComercial = FirstNotEmpty(origen.ClienteNombreComercial);
        ClienteDireccion = FirstNotEmpty(origen.ClienteDireccion, origen.Cliente?.Direccion);
        ClienteTelefono = FirstNotEmpty(origen.ClienteTelefono, origen.Cliente?.Telefono);
        ClienteEmail = FirstNotEmpty(origen.ClienteEmail, origen.Cliente?.Email);
    }
    public void RecalcularTotales()
    {
        SubTotal = Detalles.Sum(x => x.Importe);
        Igv = decimal.Round(Detalles.Sum(x => x.ImporteIgv), 2);
        Total = decimal.Round(SubTotal + Igv, 2);
        MontoDetraccion = decimal.Round(Detalles.Sum(x => x.MontoDetraccion), 2);
    }

    private static string FirstNotEmpty(params string?[] values) =>
        values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim() ?? string.Empty;
}
