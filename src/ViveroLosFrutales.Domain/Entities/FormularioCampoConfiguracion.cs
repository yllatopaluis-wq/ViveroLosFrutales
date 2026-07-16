namespace ViveroLosFrutales.Domain.Entities;

public class FormularioCampoConfiguracion
{
    public int FormularioCampoConfiguracionId { get; set; }
    public int FormularioConfiguracionId { get; set; }
    public string Bloque { get; set; } = string.Empty;
    public string Campo { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public bool Obligatorio { get; set; }
    public bool SoloLectura { get; set; }
    public int Orden { get; set; }
    public string Ancho { get; set; } = string.Empty;
public string? ValorDefecto { get; set; }
    public FormularioConfiguracion? FormularioConfiguracion { get; set; }
}




