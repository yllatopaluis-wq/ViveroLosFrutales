using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class PlantillaDocumento
{
    public int PlantillaDocumentoId { get; set; }
    public int? EmpresaId { get; set; }
    public int? TeamId { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool Activa { get; set; } = true;
    public bool EsPredeterminada { get; set; }
    public DateTime FechaRegistro { get; set; } = PeruDateTime.Now;
    public string UsuarioRegistro { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
    public ICollection<PlantillaDocumentoBloque> Bloques { get; set; } = new List<PlantillaDocumentoBloque>();
}
