using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class ErrorAplicacion
{
    public int ErrorAplicacionId { get; set; }
    public int? EmpresaId { get; set; }
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
    public string Usuario { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public string MetodoHttp { get; set; } = string.Empty;
    public string TipoExcepcion { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public string Identificador { get; set; } = string.Empty;
    public EstadoErrorAplicacion Estado { get; set; } = EstadoErrorAplicacion.PENDIENTE;
    public DateTime? FechaRevisionUtc { get; set; }
    public string UsuarioRevision { get; set; } = string.Empty;
    public string ObservacionRevision { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
}
