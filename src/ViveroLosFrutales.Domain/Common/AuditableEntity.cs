using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Common;

public abstract class AuditableEntity
{
    public DateTime FechaRegistro { get; set; } = PeruDateTime.Now;
    public string UsuarioRegistro { get; set; } = string.Empty;
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}
