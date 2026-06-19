using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class CategoriaGasto : EmpresaEntity
{
    public int CategoriaGastoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
}
