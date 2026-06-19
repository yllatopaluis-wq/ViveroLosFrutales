using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
using Xunit;

namespace ViveroLosFrutales.Tests;

public class FinanzaTests
{
    [Fact]
    public void GastoManual_CreaEgresoCaja()
    {
        var gasto = new Gasto
        {
            GastoId = 12,
            EmpresaId = 1,
            Fecha = new DateTime(2026, 6, 17),
            Categoria = "MOVILIDAD",
            Descripcion = "Traslado operativo",
            MedioPago = "EFECTIVO",
            Importe = 50m
        };

        var movimiento = new MovimientoCaja
        {
            EmpresaId = gasto.EmpresaId,
            TipoMovimiento = TipoMovimientoCaja.EGRESO,
            Origen = OrigenMovimientoCaja.GASTO,
            OrigenId = gasto.GastoId,
            Fecha = gasto.Fecha,
            Monto = gasto.Importe,
            MedioPago = gasto.MedioPago,
            Descripcion = gasto.Descripcion,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal(TipoMovimientoCaja.EGRESO, movimiento.TipoMovimiento);
        Assert.Equal(OrigenMovimientoCaja.GASTO, movimiento.Origen);
        Assert.Equal(gasto.Importe, movimiento.Monto);
        Assert.Equal(EstadoRegistro.Activo, movimiento.Estado);
    }

    [Fact]
    public void IngresoManual_CreaIngresoCaja()
    {
        var ingreso = new Ingreso
        {
            IngresoId = 15,
            EmpresaId = 1,
            Fecha = new DateTime(2026, 6, 17),
            TipoIngreso = "APORTE DE SOCIOS",
            Descripcion = "Aporte inicial",
            MedioPago = "TRANSFERENCIA",
            Importe = 300m
        };

        var movimiento = new MovimientoCaja
        {
            EmpresaId = ingreso.EmpresaId,
            TipoMovimiento = TipoMovimientoCaja.INGRESO,
            Origen = OrigenMovimientoCaja.INGRESO_MANUAL,
            OrigenId = ingreso.IngresoId,
            Fecha = ingreso.Fecha,
            Monto = ingreso.Importe,
            MedioPago = ingreso.MedioPago,
            Descripcion = ingreso.Descripcion,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal(TipoMovimientoCaja.INGRESO, movimiento.TipoMovimiento);
        Assert.Equal(OrigenMovimientoCaja.INGRESO_MANUAL, movimiento.Origen);
        Assert.Equal(ingreso.Importe, movimiento.Monto);
        Assert.Equal(EstadoRegistro.Activo, movimiento.Estado);
    }

    [Fact]
    public void MovimientoAnulado_NoSumaEnCaja()
    {
        var movimientos = new[]
        {
            new MovimientoCaja { TipoMovimiento = TipoMovimientoCaja.INGRESO, Monto = 100m, Estado = EstadoRegistro.Activo },
            new MovimientoCaja { TipoMovimiento = TipoMovimientoCaja.INGRESO, Monto = 200m, Estado = EstadoRegistro.Anulado },
            new MovimientoCaja { TipoMovimiento = TipoMovimientoCaja.EGRESO, Monto = 40m, Estado = EstadoRegistro.Activo },
            new MovimientoCaja { TipoMovimiento = TipoMovimientoCaja.EGRESO, Monto = 80m, Estado = EstadoRegistro.Anulado }
        };

        var activos = movimientos.Where(x => x.Estado == EstadoRegistro.Activo).ToArray();
        var ingresos = activos.Where(x => x.TipoMovimiento == TipoMovimientoCaja.INGRESO).Sum(x => x.Monto);
        var egresos = activos.Where(x => x.TipoMovimiento == TipoMovimientoCaja.EGRESO).Sum(x => x.Monto);

        Assert.Equal(100m, ingresos);
        Assert.Equal(40m, egresos);
        Assert.Equal(60m, ingresos - egresos);
    }
}
