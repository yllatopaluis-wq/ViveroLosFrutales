namespace ViveroLosFrutales.Domain.Entities;

public class FormularioBloqueConfiguracion
{
    public int FormularioBloqueConfiguracionId { get; set; }
    public int FormularioConfiguracionId { get; set; }
    public string Bloque { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public int Orden { get; set; }
    public bool Colapsado { get; set; }
    public FormularioConfiguracion? FormularioConfiguracion { get; set; }
}
