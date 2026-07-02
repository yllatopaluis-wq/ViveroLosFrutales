using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class TransferenciaFinanciera : EmpresaEntity
{
    public int TransferenciaFinancieraId { get; set; }
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public int CuentaOrigenId { get; set; }
    public int CuentaDestinoId { get; set; }
    public decimal Monto { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public int? MovimientoEgresoId { get; set; }
    public int? MovimientoIngresoId { get; set; }
    public DateTime? FechaAnulacion { get; set; }
    public string MotivoAnulacion { get; set; } = string.Empty;
    public string UsuarioAnulacion { get; set; } = string.Empty;

    public Empresa? Empresa { get; set; }
    public CuentaFinanciera? CuentaOrigen { get; set; }
    public CuentaFinanciera? CuentaDestino { get; set; }
    public MovimientoCaja? MovimientoEgreso { get; set; }
    public MovimientoCaja? MovimientoIngreso { get; set; }
}
