using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class ConfiguracionEmpresa : EmpresaEntity
{
    public int ConfiguracionEmpresaId { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
}
