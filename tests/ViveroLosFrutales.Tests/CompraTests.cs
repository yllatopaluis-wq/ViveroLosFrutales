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
        compra.Pagos.Add(new PagoProveedor { Monto = 400m, EstadoPago = PagoProveedorEstado.ACTIVO });

        compra.TotalPagado = compra.Pagos.Where(x => x.EstadoPago == PagoProveedorEstado.ACTIVO).Sum(x => x.Monto);
        compra.SaldoPendiente = compra.Total - compra.TotalPagado;
        compra.EstadoPago = compra.TotalPagado >= compra.Total ? EstadoPagoCompra.PAGADO : EstadoPagoCompra.PARCIAL;

        Assert.Equal(400m, compra.TotalPagado);
        Assert.Equal(600m, compra.SaldoPendiente);
        Assert.Equal(EstadoPagoCompra.PARCIAL, compra.EstadoPago);
    }

    [Fact]
    public void AnularCompraConPagos_MantienePagoYCajaHistoricaYGeneraDevolucionProveedor()
    {
        var compra = new Compra { CompraId = 8, EmpresaId = 1, ProveedorId = 2, Total = 900m, EstadoDocumento = EstadoDocumentoCompra.ANULADO };
        var pago = new PagoProveedor { PagoProveedorId = 11, EmpresaId = 1, ProveedorId = 2, CompraId = 8, Monto = 900m, EstadoPago = PagoProveedorEstado.ACTIVO };
        var egresoHistorico = new MovimientoCaja
        {
            EmpresaId = 1,
            ProveedorId = 2,
            TipoMovimiento = TipoMovimientoCaja.EGRESO,
            Origen = OrigenMovimientoCaja.PAGO_PROVEEDOR,
            OrigenId = pago.PagoProveedorId,
            Monto = pago.Monto,
            Estado = EstadoRegistro.Activo
        };
        var devolucion = new Devolucion
        {
            EmpresaId = compra.EmpresaId,
            TipoTercero = TipoTerceroDevolucion.PROVEEDOR,
            ProveedorId = compra.ProveedorId,
            Origen = OrigenDevolucion.ANULACION_COMPRA,
            CompraId = compra.CompraId,
            MontoOriginal = pago.Monto,
            MontoPendiente = pago.Monto,
            EstadoDevolucion = EstadoDevolucion.PENDIENTE
        };

        Assert.Equal(PagoProveedorEstado.ACTIVO, pago.EstadoPago);
        Assert.Equal(EstadoRegistro.Activo, egresoHistorico.Estado);
        Assert.Equal(TipoTerceroDevolucion.PROVEEDOR, devolucion.TipoTercero);
        Assert.Equal(EstadoDevolucion.PENDIENTE, devolucion.EstadoDevolucion);
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
}
