namespace ViveroLosFrutales.Domain.Entities;

public class FormularioBloqueProductoConfiguracion
{
    public int FormularioBloqueProductoConfiguracionId { get; set; }
    public int FormularioConfiguracionId { get; set; }
    public bool UnirProductosDuplicados { get; set; } = true;
    public decimal CantidadInicial { get; set; } = 1m;
    public bool PermitirEditarPrecio { get; set; } = true;
    public bool PermitirDescuento { get; set; } = true;
    public bool MostrarStock { get; set; } = true;
    public bool BloquearSinStock { get; set; }
    public FormularioConfiguracion? FormularioConfiguracion { get; set; }
}
