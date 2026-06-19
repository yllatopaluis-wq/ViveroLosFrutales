using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
using Xunit;

namespace ViveroLosFrutales.Tests;

public class CotizacionTests
{
    [Fact]
    public void RecalcularTotales_SoloSumaDetalleComercial()
    {
        var cotizacion = new Cotizacion();
        cotizacion.Detalles.Add(new CotizacionDetalle { Importe = 1000m, ImporteIgv = 180m });

        cotizacion.RecalcularTotales();

        Assert.Equal(1000m, cotizacion.SubTotal);
        Assert.Equal(180m, cotizacion.Igv);
        Assert.Equal(1180m, cotizacion.Total);
    }

    [Fact]
    public void Cotizacion_NaceSinCajaStockNiDeuda()
    {
        var cotizacion = new Cotizacion { EstadoCotizacion = CotizacionEstado.ACTIVA, Total = 9000m };

        Assert.Equal(CotizacionEstado.ACTIVA, cotizacion.EstadoCotizacion);
        Assert.Empty(cotizacion.Detalles);
    }
}
