namespace ViveroLosFrutales.Domain.Entities;

public class UsuarioEmpresa
{
    public int UsuarioEmpresaId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
}
