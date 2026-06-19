using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class MovimientoCaja : EmpresaEntity
{
    public int MovimientoCajaId { get; set; }
    public int? ClienteId { get; set; }
    public int? ProveedorId { get; set; }
    public TipoMovimientoCaja TipoMovimiento { get; set; } = TipoMovimientoCaja.INGRESO;
    public OrigenMovimientoCaja Origen { get; set; } = OrigenMovimientoCaja.COBRO_CLIENTE;
    public int OrigenId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
}
