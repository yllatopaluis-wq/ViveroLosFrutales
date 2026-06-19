using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class MovimientoInventario : EmpresaEntity
{
    public int MovimientoInventarioId { get; set; }
    public int ProductoId { get; set; }
    public TipoMovimientoInventario Tipo { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public decimal Cantidad { get; set; }
    public decimal StockAnterior { get; set; }
    public decimal StockNuevo { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public Producto? Producto { get; set; }
}
