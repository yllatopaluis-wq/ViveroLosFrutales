using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class MotivoNotaCredito : AuditableEntity
{
    public int MotivoNotaCreditoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
