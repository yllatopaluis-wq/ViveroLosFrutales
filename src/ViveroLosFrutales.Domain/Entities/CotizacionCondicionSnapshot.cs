namespace ViveroLosFrutales.Domain.Entities;

public class CotizacionCondicionSnapshot
{
    public int CotizacionCondicionSnapshotId { get; set; }
    public int CotizacionId { get; set; }
    public string Etiqueta { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public int Orden { get; set; }
    public Cotizacion? Cotizacion { get; set; }
}
