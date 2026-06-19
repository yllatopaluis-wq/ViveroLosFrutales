using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
using Xunit;

namespace ViveroLosFrutales.Tests;

public class NotaPedidoTests
{
    [Fact]
    public void RecalcularTotales_ConDetalleCalculaSaldoPendiente()
    {
        var nota = new NotaPedido { TotalCobrado = 1000m };
        nota.Detalles.Add(new NotaPedidoDetalle { Subtotal = 9000m, Igv = 0m, Total = 9000m });

        nota.RecalcularTotales();

        Assert.Equal(9000m, nota.Total);
        Assert.Equal(1000m, nota.TotalCobrado);
        Assert.Equal(8000m, nota.SaldoPendiente);
    }

    [Fact]
    public void CobroCliente_ActivoDebeSerOrigenUnicoParaMovimientoCaja()
    {
        var cobro = new CobroCliente
        {
            CobroClienteId = 12,
            EmpresaId = 1,
            ClienteId = 5,
            Monto = 3000m,
            Estado = CobroClienteEstado.ACTIVO
        };

        var movimiento = new MovimientoCaja
        {
            EmpresaId = cobro.EmpresaId,
            TipoMovimiento = TipoMovimientoCaja.INGRESO,
            Origen = OrigenMovimientoCaja.COBRO_CLIENTE,
            OrigenId = cobro.CobroClienteId,
            Monto = cobro.Monto
        };

        Assert.Equal(OrigenMovimientoCaja.COBRO_CLIENTE, movimiento.Origen);
        Assert.Equal(cobro.CobroClienteId, movimiento.OrigenId);
        Assert.Equal(cobro.Monto, movimiento.Monto);
    }

    [Fact]
    public void ComprobanteDesdeNotaPedido_PuedeQuedarPagadoPorCobrosAplicados()
    {
        var comprobante = new Comprobante
        {
            NotaPedidoId = 10,
            Total = 9000m,
            EstadoPago = EstadoPagoComprobante.PENDIENTE
        };

        comprobante.CobrosAplicados.Add(new ComprobanteCobroAplicado { MontoAplicado = 4000m });
        comprobante.CobrosAplicados.Add(new ComprobanteCobroAplicado { MontoAplicado = 5000m });

        var aplicado = comprobante.CobrosAplicados.Sum(x => x.MontoAplicado);
        comprobante.EstadoPago = aplicado >= comprobante.Total ? EstadoPagoComprobante.PAGADO : EstadoPagoComprobante.PAGO_PARCIAL;

        Assert.Equal(EstadoPagoComprobante.PAGADO, comprobante.EstadoPago);
    }

    [Fact]
    public void AnulacionNotaPedidoConCobro_GeneraDevolucionPendienteSinAnularIngreso()
    {
        var nota = new NotaPedido
        {
            NotaPedidoId = 20,
            EmpresaId = 1,
            ClienteId = 8,
            EstadoDocumento = NotaPedidoEstado.ANULADO
        };
        var cobro = new CobroCliente
        {
            CobroClienteId = 30,
            EmpresaId = 1,
            ClienteId = 8,
            NotaPedidoId = 20,
            Monto = 500m,
            Estado = CobroClienteEstado.ACTIVO
        };
        var ingresoHistorico = new MovimientoCaja
        {
            EmpresaId = 1,
            TipoMovimiento = TipoMovimientoCaja.INGRESO,
            Origen = OrigenMovimientoCaja.COBRO_CLIENTE,
            OrigenId = cobro.CobroClienteId,
            Monto = cobro.Monto,
            Estado = EstadoRegistro.Activo
        };

        var devolucion = new Devolucion
        {
            EmpresaId = nota.EmpresaId,
            TipoTercero = TipoTerceroDevolucion.CLIENTE,
            ClienteId = nota.ClienteId,
            NotaPedidoId = nota.NotaPedidoId,
            Origen = OrigenDevolucion.ANULACION_NOTA_PEDIDO,
            MontoOriginal = cobro.Monto,
            MontoPendiente = cobro.Monto,
            EstadoDevolucion = EstadoDevolucion.PENDIENTE
        };

        Assert.Equal(CobroClienteEstado.ACTIVO, cobro.Estado);
        Assert.Equal(EstadoRegistro.Activo, ingresoHistorico.Estado);
        Assert.Equal(EstadoDevolucion.PENDIENTE, devolucion.EstadoDevolucion);
        Assert.Equal(500m, devolucion.MontoPendiente);
    }

    [Fact]
    public void RegistrarDevolucion_CreaEgresoCajaContraDevolucion()
    {
        var devolucion = new Devolucion
        {
            DevolucionId = 9,
            EmpresaId = 1,
            TipoTercero = TipoTerceroDevolucion.CLIENTE,
            ClienteId = 3,
            MontoOriginal = 300m,
            MontoDevuelto = 100m,
            MontoPendiente = 200m,
            EstadoDevolucion = EstadoDevolucion.PARCIAL
        };

        var egreso = new MovimientoCaja
        {
            EmpresaId = devolucion.EmpresaId,
            TipoMovimiento = TipoMovimientoCaja.EGRESO,
            Origen = OrigenMovimientoCaja.DEVOLUCION_CLIENTE,
            OrigenId = devolucion.DevolucionId,
            Monto = 100m,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal(TipoMovimientoCaja.EGRESO, egreso.TipoMovimiento);
        Assert.Equal(OrigenMovimientoCaja.DEVOLUCION_CLIENTE, egreso.Origen);
        Assert.Equal(devolucion.DevolucionId, egreso.OrigenId);
        Assert.Equal(200m, devolucion.MontoPendiente);
    }

    [Fact]
    public void RegistrarDevolucionProveedor_CreaIngresoCajaContraDevolucion()
    {
        var devolucion = new Devolucion
        {
            DevolucionId = 15,
            EmpresaId = 1,
            TipoTercero = TipoTerceroDevolucion.PROVEEDOR,
            ProveedorId = 4,
            CompraId = 7,
            Origen = OrigenDevolucion.ANULACION_COMPRA,
            MontoOriginal = 800m,
            MontoDevuelto = 0m,
            MontoPendiente = 800m,
            EstadoDevolucion = EstadoDevolucion.PENDIENTE
        };

        var ingreso = new MovimientoCaja
        {
            EmpresaId = devolucion.EmpresaId,
            TipoMovimiento = TipoMovimientoCaja.INGRESO,
            Origen = OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR,
            OrigenId = devolucion.DevolucionId,
            Monto = 800m,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal(TipoMovimientoCaja.INGRESO, ingreso.TipoMovimiento);
        Assert.Equal(OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR, ingreso.Origen);
        Assert.Equal(devolucion.DevolucionId, ingreso.OrigenId);
    }

    [Fact]
    public void RecalcularDevolucion_TotalDevueltoMarcaDevuelto()
    {
        var devolucion = new Devolucion
        {
            MontoOriginal = 500m,
            MontoDevuelto = 200m,
            MontoPendiente = 300m,
            EstadoDevolucion = EstadoDevolucion.PARCIAL
        };

        var monto = 300m;
        devolucion.MontoDevuelto += monto;
        devolucion.MontoPendiente = devolucion.MontoOriginal - devolucion.MontoDevuelto;
        devolucion.EstadoDevolucion = devolucion.MontoPendiente <= 0
            ? EstadoDevolucion.DEVUELTO
            : EstadoDevolucion.PARCIAL;

        Assert.Equal(0m, devolucion.MontoPendiente);
        Assert.Equal(EstadoDevolucion.DEVUELTO, devolucion.EstadoDevolucion);
    }
}
