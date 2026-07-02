using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class CuentaFinanciera : EmpresaEntity
{
    public int CuentaFinancieraId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoCuentaFinanciera Tipo { get; set; } = TipoCuentaFinanciera.CAJA;
    public string Banco { get; set; } = string.Empty;
    public string NumeroCuenta { get; set; } = string.Empty;
    public string Moneda { get; set; } = "PEN";
    public decimal SaldoInicial { get; set; }
    public DateTime FechaSaldoInicial { get; set; } = PeruDateTime.Today;
    public bool Activo { get; set; } = true;
    public DateTime? FechaModificacion { get; set; }
    public string UsuarioModificacion { get; set; } = string.Empty;

    public Empresa? Empresa { get; set; }
    public ICollection<MovimientoCaja> MovimientosCaja { get; set; } = new List<MovimientoCaja>();
}
