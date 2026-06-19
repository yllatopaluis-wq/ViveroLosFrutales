namespace ViveroLosFrutales.Application.Interfaces;

public interface IEmpresaContext
{
    int EmpresaId { get; }
    string EmpresaNombre { get; }
    string UsuarioId { get; }
    string UsuarioNombre { get; }
}
