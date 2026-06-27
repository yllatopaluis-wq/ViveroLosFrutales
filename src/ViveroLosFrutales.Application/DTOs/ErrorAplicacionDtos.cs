using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record ErrorAplicacionListDto(
    int ErrorAplicacionId,
    DateTime FechaUtc,
    string Usuario,
    string Ruta,
    string TipoExcepcion,
    string Mensaje,
    EstadoErrorAplicacion Estado);

public class ErrorAplicacionDetalleDto
{
    public int ErrorAplicacionId { get; set; }
    public DateTime FechaUtc { get; set; }
    public string Empresa { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public string MetodoHttp { get; set; } = string.Empty;
    public string TipoExcepcion { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public string Identificador { get; set; } = string.Empty;
    public EstadoErrorAplicacion Estado { get; set; }
    public DateTime? FechaRevisionUtc { get; set; }
    public string UsuarioRevision { get; set; } = string.Empty;
    public string ObservacionRevision { get; set; } = string.Empty;
}

public class ErrorAplicacionSearchDto
{
    public string? Search { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public EstadoErrorAplicacion? Estado { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 30;
}
