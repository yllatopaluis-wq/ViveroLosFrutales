using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class Gasto : EmpresaEntity
{
    public int GastoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public int? CategoriaGastoId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Importe { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public int? MovimientoCajaId { get; set; }
    public string MotivoAnulacion { get; set; } = string.Empty;
    public DateTime? FechaAnulacion { get; set; }
    public Empresa? Empresa { get; set; }
    public CategoriaGasto? CategoriaGasto { get; set; }
    public MovimientoCaja? MovimientoCaja { get; set; }
}
