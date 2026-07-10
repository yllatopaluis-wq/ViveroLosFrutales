using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class DashboardRepository(ApplicationDbContext db) : IDashboardRepository
{
    public async Task<DashboardDto> ObtenerAsync(int empresaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken cancellationToken)
    {
        var desde = fechaDesde.Date;
        var hasta = fechaHasta.Date;
        var hastaExclusivo = hasta.AddDays(1);
        var hoy = PeruDateTime.Today;
        var manana = hoy.AddDays(1);
        var ayer = hoy.AddDays(-1);
        var sieteDiasDesde = hoy.AddDays(-6);

        var ventasQuery = db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC)
                && x.FechaEmision >= desde
                && x.FechaEmision < hastaExclusivo);

        var cotizacionesQuery = db.Cotizaciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.EstadoCotizacion != CotizacionEstado.ANULADA
                && x.FechaEmision >= desde
                && x.FechaEmision < hastaExclusivo);

        var comprasQuery = db.Compras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO
                && x.Fecha >= desde
                && x.Fecha < hastaExclusivo);

        var gastosQuery = db.Gastos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && x.Fecha >= desde
                && x.Fecha < hastaExclusivo);

        var ingresosQuery = db.Ingresos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && x.Fecha >= desde
                && x.Fecha < hastaExclusivo);

        var totalVentas = await ventasQuery.SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0;
        var totalCompras = await comprasQuery.SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0;
        var totalGastos = await gastosQuery.SumAsync(x => (decimal?)x.Importe, cancellationToken) ?? 0;
        var totalIngresos = await ingresosQuery.SumAsync(x => (decimal?)x.Importe, cancellationToken) ?? 0;
        var comprobantesEmitidos = await ventasQuery.CountAsync(cancellationToken);
        var cotizaciones = await cotizacionesQuery.CountAsync(cancellationToken);
        var clientesAtendidos = await ventasQuery.Select(x => x.ClienteId).Distinct().CountAsync(cancellationToken);

        var comprobantesPendientesSunat = await db.Comprobantes.AsNoTracking()
            .CountAsync(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC)
                && x.EstadoSunat == EstadoSunat.Pendiente, cancellationToken);

        var productosBajoStock = await db.Productos.AsNoTracking()
            .CountAsync(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo && x.Stock <= 5, cancellationToken);

        var ventasHoy = await SumVentasAsync(empresaId, hoy, manana, cancellationToken);
        var ventasAyer = await SumVentasAsync(empresaId, ayer, hoy, cancellationToken);
        var comprasHoy = await SumComprasAsync(empresaId, hoy, manana, cancellationToken);
        var comprasAyer = await SumComprasAsync(empresaId, ayer, hoy, cancellationToken);
        var gastosHoy = await SumGastosAsync(empresaId, hoy, manana, cancellationToken);
        var gastosAyer = await SumGastosAsync(empresaId, ayer, hoy, cancellationToken);
        var notasPedidoHoy = await SumNotasPedidoAsync(empresaId, hoy, manana, cancellationToken);
        var notasPedidoAyer = await SumNotasPedidoAsync(empresaId, ayer, hoy, cancellationToken);

        var ventasPorDiaRaw = await db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC)
                && x.FechaEmision >= sieteDiasDesde
                && x.FechaEmision < manana)
            .GroupBy(x => x.FechaEmision.Date)
            .Select(x => new DashboardSerieDto(x.Key, x.Sum(y => y.Total)))
            .ToListAsync(cancellationToken);

        var comprasPorDiaRaw = await db.Compras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO
                && x.Fecha >= sieteDiasDesde
                && x.Fecha < manana)
            .GroupBy(x => x.Fecha.Date)
            .Select(x => new DashboardSerieDto(x.Key, x.Sum(y => y.Total)))
            .ToListAsync(cancellationToken);

        var ventasPorDia = CompletarSerieDiaria(sieteDiasDesde, hoy, ventasPorDiaRaw);
        var comprasPorDia = CompletarSerieDiaria(sieteDiasDesde, hoy, comprasPorDiaRaw);

        var gastosPorCategoria = await gastosQuery
            .GroupBy(x => x.Categoria)
            .OrderByDescending(x => x.Sum(y => y.Importe))
            .Take(5)
            .Select(x => new DashboardCategoriaImporteDto(x.Key, x.Sum(y => y.Importe)))
            .ToListAsync(cancellationToken);

        var productosMasVendidos = await db.ComprobanteDetalles.AsNoTracking()
            .Where(x => x.Comprobante!.EmpresaId == empresaId
                && x.Comprobante.Estado == EstadoRegistro.Activo
                && x.Comprobante.TipoComprobante != TipoComprobante.COT
                && x.Comprobante.FechaEmision >= desde
                && x.Comprobante.FechaEmision < hastaExclusivo)
            .GroupBy(x => x.Producto!.Nombre)
            .OrderByDescending(x => x.Sum(y => y.Importe + y.ImporteIgv))
            .Take(5)
            .Select(x => new DashboardProductoDto(x.Key, x.Sum(y => y.Cantidad), x.Sum(y => y.Importe + y.ImporteIgv)))
            .ToListAsync(cancellationToken);

        var ultimosComprobantesData = await ventasQuery
            .OrderByDescending(x => x.FechaEmision)
            .ThenByDescending(x => x.ComprobanteId)
            .Take(6)
            .Select(x => new
            {
                x.FechaEmision,
                x.TipoComprobante,
                x.Serie,
                x.Correlativo,
                Cliente = x.ClienteNombre != null && x.ClienteNombre != string.Empty ? x.ClienteNombre : x.Cliente!.NombreCompleto,
                x.Total,
                x.EstadoSunat
            })
            .ToListAsync(cancellationToken);
        var ultimosComprobantes = ultimosComprobantesData
            .Select(x => new DashboardComprobanteRecienteDto(
                x.FechaEmision,
                $"{x.TipoComprobante}-{x.Serie}-{x.Correlativo}",
                x.Cliente,
                x.Total,
                x.EstadoSunat))
            .ToList();

        var ultimosMovimientos = await ObtenerUltimosMovimientosAsync(empresaId, cancellationToken);
        var alertas = BuildAlertas(comprobantesPendientesSunat, productosBajoStock, cotizaciones, totalVentas, totalCompras, totalGastos, totalIngresos);

        return new DashboardDto(
            desde,
            hasta,
            new DashboardResumenDto(
                totalVentas,
                totalCompras,
                totalGastos,
                totalIngresos,
                totalVentas + totalIngresos - totalCompras - totalGastos,
                comprobantesEmitidos,
                cotizaciones,
                comprobantesPendientesSunat,
                productosBajoStock,
                clientesAtendidos),
            new DashboardResumenDiarioDto(
                new DashboardIndicadorDiarioDto("Ventas del dia", ventasHoy, ventasAyer, CalcularVariacion(ventasHoy, ventasAyer), true),
                new DashboardIndicadorDiarioDto("Compras del dia", comprasHoy, comprasAyer, CalcularVariacion(comprasHoy, comprasAyer), true),
                new DashboardIndicadorDiarioDto("Gastos del dia", gastosHoy, gastosAyer, CalcularVariacion(gastosHoy, gastosAyer), true),
                new DashboardIndicadorDiarioDto("Notas de pedido del dia", notasPedidoHoy, notasPedidoAyer, CalcularVariacion(notasPedidoHoy, notasPedidoAyer), true)),
            ventasPorDia,
            comprasPorDia,
            gastosPorCategoria,
            productosMasVendidos,
            ultimosComprobantes,
            ultimosMovimientos,
            alertas);
    }

    private async Task<decimal> SumVentasAsync(int empresaId, DateTime desde, DateTime hasta, CancellationToken cancellationToken) =>
        await db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC)
                && x.FechaEmision >= desde
                && x.FechaEmision < hasta)
            .SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0;

    private async Task<decimal> SumComprasAsync(int empresaId, DateTime desde, DateTime hasta, CancellationToken cancellationToken) =>
        await db.Compras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO
                && x.Fecha >= desde
                && x.Fecha < hasta)
            .SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0;

    private async Task<decimal> SumGastosAsync(int empresaId, DateTime desde, DateTime hasta, CancellationToken cancellationToken) =>
        await db.Gastos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && x.Fecha >= desde
                && x.Fecha < hasta)
            .SumAsync(x => (decimal?)x.Importe, cancellationToken) ?? 0;

    private async Task<decimal> SumNotasPedidoAsync(int empresaId, DateTime desde, DateTime hasta, CancellationToken cancellationToken) =>
        await db.NotasPedido.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.EstadoDocumento == NotaPedidoEstado.ACTIVO
                && x.Fecha >= desde
                && x.Fecha < hasta)
            .SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0;

    private async Task<IReadOnlyList<DashboardMovimientoDto>> ObtenerUltimosMovimientosAsync(int empresaId, CancellationToken cancellationToken)
    {
        var comprobantes = await db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC || x.TipoComprobante == TipoComprobante.NCR))
            .OrderByDescending(x => x.FechaEmision)
            .ThenByDescending(x => x.ComprobanteId)
            .Take(8)
            .Select(x => new DashboardMovimientoDto(
                x.FechaEmision,
                x.TipoComprobante == TipoComprobante.NCR ? "Nota de credito" : "Comprobante",
                x.Serie + "-" + x.Correlativo,
                x.Cliente!.NombreCompleto,
                x.Total,
                "/Comprobantes",
                x.TipoComprobante == TipoComprobante.NCR ? "red" : "green"))
            .ToListAsync(cancellationToken);

        var notasPedido = await db.NotasPedido.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.EstadoDocumento == NotaPedidoEstado.ACTIVO)
            .OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.NotaPedidoId)
            .Take(8)
            .Select(x => new DashboardMovimientoDto(
                x.Fecha,
                "Nota de pedido",
                x.Serie + "-" + x.Correlativo,
                x.Cliente!.NombreCompleto,
                x.Total,
                "/NotasPedido",
                "purple"))
            .ToListAsync(cancellationToken);

        var cotizaciones = await db.Cotizaciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.EstadoCotizacion != CotizacionEstado.ANULADA)
            .OrderByDescending(x => x.FechaEmision)
            .ThenByDescending(x => x.CotizacionId)
            .Take(8)
            .Select(x => new DashboardMovimientoDto(
                x.FechaEmision,
                "Cotizacion",
                x.Serie + "-" + x.Correlativo,
                x.Cliente!.NombreCompleto,
                x.Total,
                "/Cotizaciones",
                "blue"))
            .ToListAsync(cancellationToken);

        var compras = await db.Compras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO)
            .OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.CompraId)
            .Take(8)
            .Select(x => new DashboardMovimientoDto(
                x.Fecha,
                "Compra",
                x.Serie + "-" + x.Numero,
                x.Proveedor!.RazonSocial,
                x.Total,
                "/Compras",
                "orange"))
            .ToListAsync(cancellationToken);

        return comprobantes
            .Concat(notasPedido)
            .Concat(cotizaciones)
            .Concat(compras)
            .OrderByDescending(x => x.Fecha)
            .Take(6)
            .ToList();
    }

    private static IReadOnlyList<DashboardSerieDto> CompletarSerieDiaria(DateTime desde, DateTime hasta, IReadOnlyList<DashboardSerieDto> datos)
    {
        var mapa = datos.ToDictionary(x => x.Fecha.Date, x => x.Importe);
        var serie = new List<DashboardSerieDto>();
        for (var fecha = desde.Date; fecha <= hasta.Date; fecha = fecha.AddDays(1))
        {
            serie.Add(new DashboardSerieDto(fecha, mapa.TryGetValue(fecha, out var importe) ? importe : 0));
        }
        return serie;
    }

    private static decimal CalcularVariacion(decimal hoy, decimal ayer)
    {
        if (ayer == 0) return hoy == 0 ? 0 : 100;
        return decimal.Round((hoy - ayer) / ayer * 100, 1);
    }

    private static IReadOnlyList<DashboardAlertaDto> BuildAlertas(
        int comprobantesPendientesSunat,
        int productosBajoStock,
        int cotizaciones,
        decimal totalVentas,
        decimal totalCompras,
        decimal totalGastos,
        decimal totalIngresos)
    {
        var alertas = new List<DashboardAlertaDto>();

        if (comprobantesPendientesSunat > 0)
        {
            alertas.Add(new DashboardAlertaDto(
                "SUNAT pendiente",
                $"{comprobantesPendientesSunat} comprobante(s) esperan sincronizacion.",
                "warning",
                "/Comprobantes"));
        }

        if (productosBajoStock > 0)
        {
            alertas.Add(new DashboardAlertaDto(
                "Stock bajo",
                $"{productosBajoStock} producto(s) estan en 5 unidades o menos.",
                "danger",
                "/Productos"));
        }

        if (cotizaciones > 0)
        {
            alertas.Add(new DashboardAlertaDto(
                "Cotizaciones activas",
                $"{cotizaciones} cotizacion(es) del mes pueden convertirse a nota de pedido.",
                "info",
                "/Cotizaciones"));
        }

        if (totalVentas + totalIngresos < totalCompras + totalGastos)
        {
            alertas.Add(new DashboardAlertaDto(
                "Flujo negativo",
                "Las salidas superan las entradas del mes.",
                "danger",
                "/Reportes/Reporte"));
        }

        if (alertas.Count == 0)
        {
            alertas.Add(new DashboardAlertaDto(
                "Operacion estable",
                "No hay alertas criticas para el mes actual.",
                "success",
                "/Reportes/Reporte"));
        }

        return alertas;
    }
}