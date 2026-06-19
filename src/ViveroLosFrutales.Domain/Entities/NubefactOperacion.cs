using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class NubefactOperacion : EmpresaEntity
{
    public int NubefactOperacionId { get; set; }
    public int ComprobanteId { get; set; }
    public string TipoOperacion { get; set; } = string.Empty;
    public EstadoSunat EstadoSunat { get; set; } = EstadoSunat.Pendiente;
    public string PdfUrl { get; set; } = string.Empty;
    public string XmlUrl { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string SolicitudJson { get; set; } = string.Empty;
    public string RespuestaCompleta { get; set; } = string.Empty;
    public Comprobante? Comprobante { get; set; }
}
