using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class FormularioConfiguracion
{
    public int FormularioConfiguracionId { get; set; }
    public int? EmpresaId { get; set; }
    public int? TeamId { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = PeruDateTime.Now;
    public string UsuarioRegistro { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
    public ICollection<FormularioBloqueConfiguracion> Bloques { get; set; } = new List<FormularioBloqueConfiguracion>();
    public ICollection<FormularioCampoConfiguracion> Campos { get; set; } = new List<FormularioCampoConfiguracion>();
    public FormularioBloqueProductoConfiguracion? ProductoConfiguracion { get; set; }
}

