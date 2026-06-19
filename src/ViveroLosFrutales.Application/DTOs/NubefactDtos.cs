using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public class NubefactResponseDto
{
    public bool Exitoso { get; set; }
    public EstadoSunat EstadoSunat { get; set; } = EstadoSunat.Pendiente;
    public string PdfUrl { get; set; } = string.Empty;
    public string XmlUrl { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string RespuestaCompleta { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}

public record NubefactOperacionDto(
    int NubefactOperacionId,
    string NumeroComprobante,
    string TipoOperacion,
    string EstadoSunat,
    string PdfUrl,
    string XmlUrl,
    string Hash,
    string SolicitudJson,
    string RespuestaCompleta,
    DateTime FechaRegistro);
