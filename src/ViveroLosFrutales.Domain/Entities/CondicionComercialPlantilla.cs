using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class CondicionComercialPlantilla
{
    public int CondicionComercialPlantillaId { get; set; }
    public int? EmpresaId { get; set; }
    public int? TeamId { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool EsPredeterminada { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = PeruDateTime.Now;
    public string UsuarioRegistro { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
    public ICollection<CondicionComercialItem> Items { get; set; } = new List<CondicionComercialItem>();
}
