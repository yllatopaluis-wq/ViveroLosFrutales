using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class PagoProveedor : EmpresaEntity
{
    public int PagoProveedorId { get; set; }
    public int ProveedorId { get; set; }
    public int CompraId { get; set; }
    public DateTime FechaPago { get; set; } = PeruDateTime.Today;
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = "EFECTIVO";
    public string Observacion { get; set; } = string.Empty;
    public PagoProveedorEstado EstadoPago { get; set; } = PagoProveedorEstado.ACTIVO;
    public DateTime? FechaAnulacion { get; set; }
    public string MotivoAnulacion { get; set; } = string.Empty;
    public string UsuarioAnulacion { get; set; } = string.Empty;

    public Proveedor? Proveedor { get; set; }
    public Compra? Compra { get; set; }
}
