using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class NotaPedido : EmpresaEntity
{
    public int NotaPedidoId { get; set; }
    public int ClienteId { get; set; }
    public TipoDocumentoCliente? ClienteTipoDocumento { get; set; }
    public string? ClienteNumeroDocumento { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteNombreComercial { get; set; }
    public string? ClienteDireccion { get; set; }
    public string? ClienteTelefono { get; set; }
    public string? ClienteEmail { get; set; }
    public int? CotizacionId { get; set; }
    public int? ComprobanteId { get; set; }
    public string Serie { get; set; } = string.Empty;
    public int Correlativo { get; set; }
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public FormaPago FormaPago { get; set; } = FormaPago.Contado;
    public string Observacion { get; set; } = string.Empty;
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

    public TipoDocumentoCliente? ClienteTipoDocumentoMostrar => ClienteTipoDocumento ?? Cliente?.TipoDocumento;
    public string ClienteNumeroDocumentoMostrar => FirstNotEmpty(ClienteNumeroDocumento, Cliente?.NumeroDocumento);
    public string ClienteNombreMostrar => FirstNotEmpty(ClienteNombre, Cliente?.NombreCompleto);
    public string ClienteNombreComercialMostrar => FirstNotEmpty(ClienteNombreComercial);
    public string ClienteDireccionMostrar => FirstNotEmpty(ClienteDireccion, Cliente?.Direccion);
    public string ClienteTelefonoMostrar => FirstNotEmpty(ClienteTelefono, Cliente?.Telefono);
    public string ClienteEmailMostrar => FirstNotEmpty(ClienteEmail, Cliente?.Email);

    public void AplicarSnapshotCliente(Cliente cliente)
    {
        ClienteTipoDocumento = cliente.TipoDocumento;
        ClienteNumeroDocumento = cliente.NumeroDocumento;
        ClienteNombre = cliente.NombreCompleto;
        ClienteNombreComercial = string.Empty;
        ClienteDireccion = cliente.Direccion;
        ClienteTelefono = cliente.Telefono;
        ClienteEmail = cliente.Email;
    }

    public void AplicarSnapshotClienteDesde(Cotizacion cotizacion)
    {
        ClienteTipoDocumento = cotizacion.ClienteTipoDocumento ?? cotizacion.Cliente?.TipoDocumento;
        ClienteNumeroDocumento = FirstNotEmpty(cotizacion.ClienteNumeroDocumento, cotizacion.Cliente?.NumeroDocumento);
        ClienteNombre = FirstNotEmpty(cotizacion.ClienteNombre, cotizacion.Cliente?.NombreCompleto);
        ClienteNombreComercial = FirstNotEmpty(cotizacion.ClienteNombreComercial);
        ClienteDireccion = FirstNotEmpty(cotizacion.ClienteDireccion, cotizacion.Direccion, cotizacion.Cliente?.Direccion);
        ClienteTelefono = FirstNotEmpty(cotizacion.ClienteTelefono, cotizacion.Cliente?.Telefono);
        ClienteEmail = FirstNotEmpty(cotizacion.ClienteEmail, cotizacion.Cliente?.Email);
    }

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

    private static string FirstNotEmpty(params string?[] values) =>
        values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim() ?? string.Empty;
}
