namespace ViveroLosFrutales.Domain.Entities;

public class CondicionComercialItem
{
    public int CondicionComercialItemId { get; set; }
    public int CondicionComercialPlantillaId { get; set; }
    public string Etiqueta { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool Visible { get; set; } = true;
    public CondicionComercialPlantilla? Plantilla { get; set; }
}
