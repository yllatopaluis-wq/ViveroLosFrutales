using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class CategoriaIngreso : EmpresaEntity
{
    public int CategoriaIngresoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
}
