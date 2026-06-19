using ViveroLosFrutales.Domain.Entities;
using Xunit;

namespace ViveroLosFrutales.Tests;

public class ProductoTests
{
    [Fact]
    public void RecalcularPrecioConIgv_CuandoAfectoIgv_AplicaDieciochoPorCiento()
    {
        var producto = new Producto { AfectoIgv = true, PrecioVentaSinIgv = 100m };

        producto.RecalcularPrecioConIgv();

        Assert.Equal(118m, producto.PrecioVentaConIgv);
    }

    [Fact]
    public void RecalcularPrecioConIgv_CuandoNoAfectoIgv_MantienePrecio()
    {
        var producto = new Producto { AfectoIgv = false, PrecioVentaSinIgv = 100m };

        producto.RecalcularPrecioConIgv();

        Assert.Equal(100m, producto.PrecioVentaConIgv);
    }
}
