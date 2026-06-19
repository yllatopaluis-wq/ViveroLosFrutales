using ViveroLosFrutales.Domain.Common;

namespace ViveroLosFrutales.Domain.Entities;

public class Categoria : EmpresaEntity
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
}
