using Xunit;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Tests;

public class FormularioConfiguracionTests
{
    [Fact]
    public void Defaults_incluye_campos_controlados_de_cotizacion()
    {
        var config = FormularioConfiguracionService.Defaults("COTIZACION");
        Assert.Contains(config.Campos, x => x.Bloque == "GENERAL" && x.Campo == "Fecha" && x.Obligatorio);
        Assert.Contains(config.Campos, x => x.Bloque == "PRODUCTOS" && x.Campo == "Producto" && x.Visible);
        Assert.Contains(config.Campos, x => x.Bloque == "TOTALES" && x.Campo == "Total" && x.SoloLectura);
    }

    [Fact]
    public void Defaults_incluye_grilla_productos_configurable_de_compra()
    {
        var config = FormularioConfiguracionService.Defaults("COMPRA");

        Assert.Contains(config.Bloques, x => x.Bloque == "PRODUCTOS" && x.Visible);
        Assert.Contains(config.Campos, x => x.Bloque == "PRODUCTOS" && x.Campo == "Producto" && x.Visible);
        Assert.Contains(config.Campos, x => x.Bloque == "PRODUCTOS" && x.Campo == "PrecioUnitario" && x.Etiqueta == "Precio compra");
        Assert.Equal(1m, config.ProductoComportamiento.CantidadInicial);
    }
    [Fact]
    public void Defaults_incluye_bloques_de_nota_credito()
    {
        var config = FormularioConfiguracionService.Defaults("NOTA_CREDITO");

        Assert.Contains(config.Bloques, x => x.Bloque == "GENERAL" && x.Visible);
        Assert.Contains(config.Bloques, x => x.Bloque == "ORIGEN" && x.Visible);
        Assert.Contains(config.Campos, x => x.Bloque == "GENERAL" && x.Campo == "SustentoDescripcion" && x.Visible);
        Assert.Contains(config.Campos, x => x.Bloque == "PRODUCTOS" && x.Campo == "CantidadNc" && x.Visible);
        Assert.Contains(config.Campos, x => x.Bloque == "TOTALES" && x.Campo == "TotalNc" && x.Visible);
    }

    [Fact]
    public void ValidarCampo_rechaza_ocultar_producto()
    {
        var campo = new FormularioCampoEditDto
        {
            Bloque = "PRODUCTOS",
            Campo = "Producto",
            Etiqueta = "Producto",
            Visible = false,
            Obligatorio = true,
            SoloLectura = true,
            Orden = 20,
            Ancho = "4"
        };

        var ex = Assert.Throws<InvalidOperationException>(() => FormularioConfiguracionService.ValidarCampo(campo));
        Assert.Contains("no se puede ocultar", ex.Message);
    }

    [Fact]
    public void ValidarCampo_rechaza_ancho_fuera_de_rango()
    {
        var campo = new FormularioCampoEditDto
        {
            Bloque = "GENERAL",
            Campo = "Moneda",
            Etiqueta = "Moneda",
            Visible = true,
            Orden = 50,
            Ancho = "13"
        };

        var ex = Assert.Throws<InvalidOperationException>(() => FormularioConfiguracionService.ValidarCampo(campo));
        Assert.Contains("entre 1 y 12", ex.Message);
    }
}


