namespace ViveroLosFrutales.Domain.Entities;

public class PlantillaDocumentoBloque
{
    public int PlantillaDocumentoBloqueId { get; set; }
    public int PlantillaDocumentoId { get; set; }
    public string Bloque { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public int Orden { get; set; }
    public PlantillaDocumento? PlantillaDocumento { get; set; }
}
