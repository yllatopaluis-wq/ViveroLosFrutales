using ViveroLosFrutales.Domain.Security;
using Xunit;

namespace ViveroLosFrutales.Tests;

public class PermissionCatalogTests
{
    [Fact]
    public void Catalogo_NoContienePermisosDuplicados()
    {
        var permisos = PermissionCatalog.All()
            .Select(x => $"{x.Module}|{x.Action}")
            .ToArray();

        Assert.Equal(permisos.Length, permisos.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void Catalogo_UsaAccionesRealesPorFormulario()
    {
        var permisos = PermissionCatalog.All()
            .Select(x => $"{x.Module}|{x.Action}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("Compras|RegistrarPago", permisos);
        Assert.Contains("Devoluciones|Registrar", permisos);
        Assert.Contains("Usuarios|RestablecerPassword", permisos);
        Assert.DoesNotContain("Categorias|RegistrarPago", permisos);
        Assert.DoesNotContain("Usuarios|Convertir", permisos);
        Assert.DoesNotContain("DevolucionesClientes|Ver", permisos);
    }
}
