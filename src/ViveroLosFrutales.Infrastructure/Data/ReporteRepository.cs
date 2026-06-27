using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ReporteRepository(ApplicationDbContext db) : IReporteRepository
{
    private static readonly string[] Meses =
    {
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    };

    private static readonly string[] MesesCortos =
    {
        "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"
    };

    public async Task<ReporteGeneralDto> ObtenerGeneralAsync(
        int empresaId,
        int anioDesde,
        int anioHasta,
        string indicador,
        CancellationToken cancellationToken)
    {
        var desde = new DateTime(anioDesde, 1, 1);
        var hasta = new DateTime(anioHasta + 1, 1, 1);
        var anioComparativo = anioDesde - 1;
        var desdeConsulta = new DateTime(anioComparativo, 1, 1);

        var ventas = await db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && x.FechaEmision >= desdeConsulta && x.FechaEmision < hasta
                && (x.TipoComprobante == TipoComprobante.BOL
                    || x.TipoComprobante == TipoComprobante.FAC
                    || x.TipoComprobante == TipoComprobante.NCR))
            .GroupBy(x => new { Anio = x.FechaEmision.Year, Mes = x.FechaEmision.Month })
            .Select(x => new MesMonto(
                x.Key.Anio,
                x.Key.Mes,
                x.Sum(d => d.TipoComprobante == TipoComprobante.NCR ? -d.Total : d.Total)))
            .ToListAsync(cancellationToken);

        var ingresos = await db.Ingresos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && x.Fecha >= desdeConsulta && x.Fecha < hasta)
            .GroupBy(x => new { Anio = x.Fecha.Year, Mes = x.Fecha.Month })
            .Select(x => new MesMonto(x.Key.Anio, x.Key.Mes, x.Sum(d => d.Importe)))
            .ToListAsync(cancellationToken);

        var gastos = await db.Gastos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && x.Fecha >= desdeConsulta && x.Fecha < hasta)
            .GroupBy(x => new { Anio = x.Fecha.Year, Mes = x.Fecha.Month })
            .Select(x => new MesMonto(x.Key.Anio, x.Key.Mes, x.Sum(d => d.Importe)))
            .ToListAsync(cancellationToken);

        var compras = await db.Compras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO
                && x.Fecha >= desdeConsulta && x.Fecha < hasta)
            .GroupBy(x => new { Anio = x.Fecha.Year, Mes = x.Fecha.Month })
            .Select(x => new MesMonto(x.Key.Anio, x.Key.Mes, x.Sum(d => d.Total)))
            .ToListAsync(cancellationToken);

        var comprobantesActivosPeriodo = db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && x.FechaEmision >= desde && x.FechaEmision < hasta
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC));

        var numeroComprobantes = await comprobantesActivosPeriodo.CountAsync(cancellationToken);
        var clientesAtendidos = await comprobantesActivosPeriodo.Select(x => x.ClienteId).Distinct().CountAsync(cancellationToken);
        var productosVendidos = await db.ComprobanteDetalles.AsNoTracking()
            .Where(x => x.Comprobante != null
                && x.Comprobante.EmpresaId == empresaId
                && x.Comprobante.Estado == EstadoRegistro.Activo
                && x.Comprobante.FechaEmision >= desde && x.Comprobante.FechaEmision < hasta
                && (x.Comprobante.TipoComprobante == TipoComprobante.BOL || x.Comprobante.TipoComprobante == TipoComprobante.FAC))
            .SumAsync(x => (decimal?)x.Cantidad, cancellationToken) ?? 0;

        var saldoCaja = await db.MovimientosCaja.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo && x.Fecha < hasta)
            .SumAsync(x => (decimal?)(x.TipoMovimiento == TipoMovimientoCaja.INGRESO ? x.Monto : -x.Monto), cancellationToken) ?? 0;

        var ventasMap = ventas.ToDictionary(x => (x.Anio, x.Mes), x => x.Monto);
        var ingresosMap = ingresos.ToDictionary(x => (x.Anio, x.Mes), x => x.Monto);
        var gastosMap = gastos.ToDictionary(x => (x.Anio, x.Mes), x => x.Monto);
        var comprasMap = compras.ToDictionary(x => (x.Anio, x.Mes), x => x.Monto);
        var anios = Enumerable.Range(anioDesde, anioHasta - anioDesde + 1).ToArray();

        decimal Monto(Dictionary<(int Anio, int Mes), decimal> origen, int anio, int mes) =>
            decimal.Round(origen.GetValueOrDefault((anio, mes)), 2);

        decimal VentasAnio(int anio) => Enumerable.Range(1, 12).Sum(mes => Monto(ventasMap, anio, mes));
        decimal IngresosAnio(int anio) => Enumerable.Range(1, 12).Sum(mes => Monto(ingresosMap, anio, mes));
        decimal GastosAnio(int anio) => Enumerable.Range(1, 12).Sum(mes => Monto(gastosMap, anio, mes));
        decimal ComprasAnio(int anio) => Enumerable.Range(1, 12).Sum(mes => Monto(comprasMap, anio, mes));
        decimal Valor(int anio, int mes)
        {
            var venta = Monto(ventasMap, anio, mes);
            var ingreso = Monto(ingresosMap, anio, mes);
            var gasto = Monto(gastosMap, anio, mes);
            var compra = Monto(comprasMap, anio, mes);
            return indicador switch
            {
                ReporteGeneralIndicadores.Ventas => venta,
                ReporteGeneralIndicadores.Ingresos => ingreso,
                ReporteGeneralIndicadores.Gastos => gasto,
                ReporteGeneralIndicadores.Compras => compra,
                _ => venta + ingreso - gasto - compra
            };
        }

        var filas = Enumerable.Range(1, 12)
            .Select(mes => new ReporteGeneralMesDto(
                mes,
                Meses[mes - 1],
                anios.Select(anio => decimal.Round(Valor(anio, mes), 2)).ToArray()))
            .ToArray();

        var dashboardMeses = Enumerable.Range(1, 12)
            .Select(mes =>
            {
                var venta = Monto(ventasMap, anioHasta, mes);
                var ingreso = Monto(ingresosMap, anioHasta, mes);
                var gasto = Monto(gastosMap, anioHasta, mes);
                var compra = Monto(comprasMap, anioHasta, mes);
                return new ReporteGeneralDashboardMesDto(mes, Meses[mes - 1], MesesCortos[mes - 1], venta, ingreso, gasto, compra, venta + ingreso - gasto - compra);
            })
            .ToArray();

        var resumen = new List<ReporteGeneralAnualDto>();
        foreach (var anio in anios)
        {
            var venta = VentasAnio(anio);
            var ingreso = IngresosAnio(anio);
            var gasto = GastosAnio(anio);
            var compra = ComprasAnio(anio);
            var resultado = venta + ingreso - gasto - compra;
            var anterior = resumen.LastOrDefault()?.Resultado;
            decimal? variacion = anterior is not null && anterior != 0
                ? decimal.Round((resultado - anterior.Value) / Math.Abs(anterior.Value) * 100, 2)
                : null;
            resumen.Add(new ReporteGeneralAnualDto(anio, venta, ingreso, gasto, compra, resultado, variacion));
        }

        var totalVentas = resumen.Sum(x => x.Ventas);
        var totalIngresos = resumen.Sum(x => x.Ingresos);
        var totalGastos = resumen.Sum(x => x.Gastos);
        var totalCompras = resumen.Sum(x => x.Compras);
        var resultadoTotal = totalVentas + totalIngresos - totalGastos - totalCompras;
        var ticketPromedio = numeroComprobantes == 0 ? 0 : decimal.Round(totalVentas / numeroComprobantes, 2);

        decimal? Variacion(decimal actual, decimal anterior) => anterior == 0 ? null : decimal.Round((actual - anterior) / Math.Abs(anterior) * 100, 2);
        var ventasAnterior = VentasAnio(anioComparativo);
        var ingresosAnterior = IngresosAnio(anioComparativo);
        var gastosAnterior = GastosAnio(anioComparativo);
        var comprasAnterior = ComprasAnio(anioComparativo);
        var resultadoAnterior = ventasAnterior + ingresosAnterior - gastosAnterior - comprasAnterior;

        return new ReporteGeneralDto
        {
            AnioDesde = anioDesde,
            AnioHasta = anioHasta,
            Indicador = indicador,
            Anios = anios,
            Meses = filas,
            DashboardMeses = dashboardMeses,
            ResumenAnual = resumen,
            TotalVentas = totalVentas,
            TotalIngresos = totalIngresos,
            TotalGastos = totalGastos,
            TotalCompras = totalCompras,
            SaldoCaja = saldoCaja,
            TicketPromedio = ticketPromedio,
            NumeroComprobantes = numeroComprobantes,
            ClientesAtendidos = clientesAtendidos,
            ProductosVendidos = productosVendidos,
            Kpis = new[]
            {
                new ReporteGeneralKpiDto("Ventas netas", totalVentas, "vs año anterior", Variacion(totalVentas, ventasAnterior), "sales"),
                new ReporteGeneralKpiDto("Ingresos", totalIngresos, "vs año anterior", Variacion(totalIngresos, ingresosAnterior), "income"),
                new ReporteGeneralKpiDto("Gastos", totalGastos, "vs año anterior", Variacion(totalGastos, gastosAnterior), "expense"),
                new ReporteGeneralKpiDto("Compras", totalCompras, "vs año anterior", Variacion(totalCompras, comprasAnterior), "purchases"),
                new ReporteGeneralKpiDto("Resultado neto", resultadoTotal, "vs año anterior", Variacion(resultadoTotal, resultadoAnterior), "result"),
                new ReporteGeneralKpiDto("Ticket promedio", ticketPromedio, "Por venta", null, "ticket"),
                new ReporteGeneralKpiDto("N° comprobantes", numeroComprobantes, "Emitidos", null, "docs", false),
                new ReporteGeneralKpiDto("Clientes atendidos", clientesAtendidos, "En el periodo", null, "clients", false),
                new ReporteGeneralKpiDto("Productos vendidos", productosVendidos, "Unidades", null, "products", false)
            }
        };
    }

    private sealed record MesMonto(int Anio, int Mes, decimal Monto);
}
