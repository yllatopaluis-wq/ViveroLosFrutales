using System.Security.Claims;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Web.Services;

public class EmpresaContext(IHttpContextAccessor accessor) : IEmpresaContext
{
    public int EmpresaId
    {
        get
        {
            var http = accessor.HttpContext;
            var value = http?.Session.GetInt32("EmpresaId");
            if (value is > 0) return value.Value;
            throw new InvalidOperationException("Seleccione una empresa activa para continuar.");
        }
    }

    public string EmpresaNombre => accessor.HttpContext?.Session.GetString("EmpresaNombre") ?? string.Empty;
    public string UsuarioId => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
    public string UsuarioNombre => accessor.HttpContext?.User.Identity?.Name ?? "system";
}
