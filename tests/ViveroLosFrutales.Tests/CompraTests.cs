using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
using Xunit;

namespace ViveroLosFrutales.Tests;

public class CompraTests
{
    [Fact]
    public void CompraContado_QuedaPagadaYCreaEgresoProveedor()
    {
        var compra = new Compra { CompraId = 10, EmpresaId = 1, ProveedorId = 3, Total = 118m, FormaPago = FormaPagoCompra.CONTADO };
        compra.TotalPagado = compra.Total;
        compra.SaldoPendiente = 0;
        compra.EstadoPago = EstadoPagoCompra.PAGADO;

        var pago = new PagoProveedor { PagoProveedorId = 7, EmpresaId = 1, ProveedorId = 3, CompraId = compra.CompraId, Monto = compra.Total };
        var movimiento = new MovimientoCaja
        {
            EmpresaId = 1,
            ProveedorId = 3,
            TipoMovimiento = TipoMovimientoCaja.EGRESO,
            Origen = OrigenMovimientoCaja.PAGO_PROVEEDOR,
            OrigenId = pago.PagoProveedorId,
            Monto = pago.Monto,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal(EstadoPagoCompra.PAGADO, compra.EstadoPago);
        Assert.Equal(0, compra.SaldoPendiente);
        Assert.Equal(TipoMovimientoCaja.EGRESO, movimiento.TipoMovimiento);
        Assert.Equal(OrigenMovimientoCaja.PAGO_PROVEEDOR, movimiento.Origen);
    }

    [Fact]
    public void CompraCredito_IniciaPendienteSinCaja()
    {
        var compra = new Compra { Total = 500m, FormaPago = FormaPagoCompra.CREDITO };
        compra.TotalPagado = 0;
        compra.SaldoPendiente = compra.Total;
        compra.EstadoPago = EstadoPagoCompra.PENDIENTE;

        Assert.Equal(EstadoPagoCompra.PENDIENTE, compra.EstadoPago);
        Assert.Equal(500m, compra.SaldoPendiente);
        Assert.Empty(compra.Pagos);
    }

    [Fact]
    public void PagoParcial_RecalculaSaldoYEstadoParcial()
    {
        var compra = new Compra { Total = 1000m };
        compra.PagoAplicaciones.Add(new PagoProveedorAplicacion { MontoAplicado = 400m, Estado = EstadoPagoProveedorAplicacion.ACTIVO });

        CompraService.RecalcularEstadoPago(compra);

        Assert.Equal(400m, compra.TotalPagado);
        Assert.Equal(600m, compra.SaldoPendiente);
        Assert.Equal(EstadoPagoCompra.PARCIAL, compra.EstadoPago);
    }

    [Fact]
    public void AnularCompraSinPagosYSinRecepcion_PermiteAnular()
    {
        var compra = CompraBase();

        CompraService.ValidarPuedeAnularCompra(compra, false);

        compra.Estado = EstadoRegistro.Anulado;
        compra.EstadoDocumento = EstadoDocumentoCompra.ANULADO;
        Assert.Equal(EstadoRegistro.Anulado, compra.Estado);
        Assert.Equal(EstadoDocumentoCompra.ANULADO, compra.EstadoDocumento);
    }

    [Fact]
    public void AnularCompraConPagosAplicados_PermiteProcesarReversionYSolicitudDevolucion()
    {
        var compra = CompraBase();
        compra.PagoAplicaciones.Add(new PagoProveedorAplicacion { MontoAplicado = 900m, Estado = EstadoPagoProveedorAplicacion.ACTIVO });

        CompraService.ValidarPuedeAnularCompra(compra, false);

        var montoSolicitudDevolucion = compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado);
        Assert.Equal(900m, montoSolicitudDevolucion);
    }

    [Fact]
    public void AnularCompraConVariasAplicaciones_CalculaMontoTotalParaSolicitudDevolucion()
    {
        var compra = CompraBase();
        compra.PagoAplicaciones.Add(new PagoProveedorAplicacion { MontoAplicado = 200m, Estado = EstadoPagoProveedorAplicacion.ACTIVO });
        compra.PagoAplicaciones.Add(new PagoProveedorAplicacion { MontoAplicado = 300m, Estado = EstadoPagoProveedorAplicacion.ACTIVO });

        CompraService.ValidarPuedeAnularCompra(compra, false);

        Assert.Equal(500m, compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado));
    }

    [Fact]
    public void AnularCompraConRecepcion_PermiteRevertirStockEnElFlujo()
    {
        var compra = CompraBase();
        compra.Detalles.Add(new CompraDetalle { Cantidad = 10m, CantidadRecibida = 5m });

        CompraService.ValidarPuedeAnularCompra(compra, true);

        Assert.Equal(5m, compra.Detalles.Sum(x => x.CantidadRecibida));
    }

    [Fact]
    public void AnularCompraConRecepcionYPago_PermiteProcesarStockPagosYDevolucion()
    {
        var compra = CompraBase();
        compra.Detalles.Add(new CompraDetalle { Cantidad = 10m, CantidadRecibida = 5m });
        compra.PagoAplicaciones.Add(new PagoProveedorAplicacion { MontoAplicado = 100m, Estado = EstadoPagoProveedorAplicacion.ACTIVO });

        CompraService.ValidarPuedeAnularCompra(compra, true);

        Assert.Equal(5m, compra.Detalles.Sum(x => x.CantidadRecibida));
        Assert.Equal(100m, compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado));
    }

    [Fact]
    public void RevertirAplicaciones_AnulaTodasLasActivasYMantienePagoYCaja()
    {
        var compra = CompraBase();
        var pago = new PagoProveedor { PagoProveedorId = 7, Monto = 1000m, EstadoPago = PagoProveedorEstado.ACTIVO };
        var movimiento = new MovimientoCaja { Origen = OrigenMovimientoCaja.PAGO_PROVEEDOR, OrigenId = pago.PagoProveedorId, Monto = 1000m, Estado = EstadoRegistro.Activo };
        var app1 = new PagoProveedorAplicacion { PagoProveedor = pago, MontoAplicado = 400m, Estado = EstadoPagoProveedorAplicacion.ACTIVO };
        var app2 = new PagoProveedorAplicacion { PagoProveedor = pago, MontoAplicado = 300m, Estado = EstadoPagoProveedorAplicacion.ACTIVO };
        pago.Aplicaciones.Add(app1);
        pago.Aplicaciones.Add(app2);
        compra.PagoAplicaciones.Add(app1);
        compra.PagoAplicaciones.Add(app2);

        CompraService.RevertirAplicacionesActivas(compra, "Correccion", "admin", new DateTime(2026, 7, 14));
        CompraService.RecalcularEstadoPago(compra);
        var aplicado = pago.Aplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado);
        var disponible = pago.Monto - aplicado;

        Assert.All(compra.PagoAplicaciones, x => Assert.Equal(EstadoPagoProveedorAplicacion.ANULADO, x.Estado));
        Assert.All(compra.PagoAplicaciones, x => Assert.Equal("Correccion", x.MotivoAnulacion));
        Assert.Equal(1000m, disponible);
        Assert.Equal(PagoProveedorEstado.ACTIVO, pago.EstadoPago);
        Assert.Equal(EstadoRegistro.Activo, movimiento.Estado);
        Assert.Equal(EstadoPagoCompra.PENDIENTE, compra.EstadoPago);
    }

    [Fact]
    public void RevertirAplicaciones_RecalculaSaldoYEstadoPago()
    {
        var compra = CompraBase();
        compra.Total = 1000m;
        compra.PagoAplicaciones.Add(new PagoProveedorAplicacion { MontoAplicado = 1000m, Estado = EstadoPagoProveedorAplicacion.ACTIVO });
        CompraService.RecalcularEstadoPago(compra);
        Assert.Equal(EstadoPagoCompra.PAGADO, compra.EstadoPago);

        CompraService.RevertirAplicacionesActivas(compra, "Reversion", "admin", DateTime.UtcNow);
        CompraService.RecalcularEstadoPago(compra);

        Assert.Equal(0m, compra.TotalPagado);
        Assert.Equal(1000m, compra.SaldoPendiente);
        Assert.Equal(EstadoPagoCompra.PENDIENTE, compra.EstadoPago);
    }

    [Fact]
    public void CompraYaAnulada_NoPermiteAnularOtraVez()
    {
        var compra = CompraBase();
        compra.Estado = EstadoRegistro.Anulado;
        compra.EstadoDocumento = EstadoDocumentoCompra.ANULADO;

        var ex = Assert.Throws<InvalidOperationException>(() => CompraService.ValidarPuedeAnularCompra(compra, false));

        Assert.Contains("ya esta anulada", ex.Message);
    }

    [Fact]
    public void RevertirAplicaciones_SinMotivo_HaceRollbackLogicoAntesDeCambiarEstados()
    {
        var compra = CompraBase();
        compra.PagoAplicaciones.Add(new PagoProveedorAplicacion { MontoAplicado = 100m, Estado = EstadoPagoProveedorAplicacion.ACTIVO });

        Assert.Throws<InvalidOperationException>(() => CompraService.RevertirAplicacionesActivas(compra, " ", "admin", DateTime.UtcNow));

        Assert.All(compra.PagoAplicaciones, x => Assert.Equal(EstadoPagoProveedorAplicacion.ACTIVO, x.Estado));
    }

    [Fact]
    public void RegistrarDevolucionProveedor_CreaIngresoCaja()
    {
        var devolucion = new Devolucion { DevolucionId = 5, EmpresaId = 1, TipoTercero = TipoTerceroDevolucion.PROVEEDOR, ProveedorId = 6, MontoOriginal = 300m, MontoPendiente = 300m };
        var ingreso = new MovimientoCaja
        {
            EmpresaId = devolucion.EmpresaId,
            ProveedorId = devolucion.ProveedorId,
            TipoMovimiento = TipoMovimientoCaja.INGRESO,
            Origen = OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR,
            OrigenId = devolucion.DevolucionId,
            Monto = 300m,
            Estado = EstadoRegistro.Activo
        };

        devolucion.MontoDevuelto += ingreso.Monto;
        devolucion.MontoPendiente = devolucion.MontoOriginal - devolucion.MontoDevuelto;
        devolucion.EstadoDevolucion = devolucion.MontoPendiente <= 0 ? EstadoDevolucion.DEVUELTO : EstadoDevolucion.PARCIAL;

        Assert.Equal(TipoMovimientoCaja.INGRESO, ingreso.TipoMovimiento);
        Assert.Equal(OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR, ingreso.Origen);
        Assert.Equal(EstadoDevolucion.DEVUELTO, devolucion.EstadoDevolucion);
    }

    private static Compra CompraBase() => new()
    {
        CompraId = 8,
        EmpresaId = 1,
        ProveedorId = 2,
        Total = 1000m,
        TotalPagado = 0m,
        SaldoPendiente = 1000m,
        Estado = EstadoRegistro.Activo,
        EstadoDocumento = EstadoDocumentoCompra.ACTIVO,
        EstadoPago = EstadoPagoCompra.PENDIENTE,
        EstadoEntrega = EstadoEntregaCompra.PENDIENTE
    };
}