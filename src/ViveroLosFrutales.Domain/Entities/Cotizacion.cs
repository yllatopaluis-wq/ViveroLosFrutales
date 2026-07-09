using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class Cotizacion : EmpresaEntity
{
    public int CotizacionId { get; set; }
    public int ClienteId { get; set; }
    public TipoDocumentoCliente? ClienteTipoDocumento { get; set; }
    public string? ClienteNumeroDocumento { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteNombreComercial { get; set; }
    public string? ClienteDireccion { get; set; }
    public string? ClienteTelefono { get; set; }
    public string? ClienteEmail { get; set; }
    public string Serie { get; set; } = string.Empty;
    public int Correlativo { get; set; }
    public DateTime FechaEmision { get; set; } = PeruDateTime.Today;
    public string Direccion { get; set; } = string.Empty;
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
    public CotizacionEstado EstadoCotizacion { get; set; } = CotizacionEstado.ACTIVA;
    public string PdfUrl { get; set; } = string.Empty;
    public DateTime? FechaModificacion { get; set; }
    public string UsuarioModificacion { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
    public Cliente? Cliente { get; set; }
    public ICollection<CotizacionDetalle> Detalles { get; set; } = new List<CotizacionDetalle>();

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

    public void RecalcularTotales()
    {
        SubTotal = decimal.Round(Detalles.Sum(x => x.Importe), 2);
        Igv = decimal.Round(Detalles.Sum(x => x.ImporteIgv), 2);
        Total = decimal.Round(SubTotal + Igv, 2);
    }

    private static string FirstNotEmpty(params string?[] values) =>
        values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim() ?? string.Empty;
}
