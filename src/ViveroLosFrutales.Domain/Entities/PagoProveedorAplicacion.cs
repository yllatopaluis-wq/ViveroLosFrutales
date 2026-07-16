using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class PagoProveedorAplicacion
{
    public int PagoProveedorAplicacionId { get; set; }
    public int EmpresaId { get; set; }
    public int PagoProveedorId { get; set; }
    public int CompraId { get; set; }
    public decimal MontoAplicado { get; set; }
    public DateTime FechaAplicacion { get; set; } = DateTime.UtcNow;
    public EstadoPagoProveedorAplicacion Estado { get; set; } = EstadoPagoProveedorAplicacion.ACTIVO;
    public string MotivoAnulacion { get; set; } = string.Empty;
    public DateTime? FechaAnulacion { get; set; }
    public string UsuarioRegistro { get; set; } = string.Empty;
    public string UsuarioAnulacion { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public PagoProveedor? PagoProveedor { get; set; }
    public Compra? Compra { get; set; }
}
