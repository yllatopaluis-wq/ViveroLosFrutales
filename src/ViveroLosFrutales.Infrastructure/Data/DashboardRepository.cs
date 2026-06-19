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
        var hastaExclusivo = fechaHasta.Date.AddDays(1);

        var ventasQuery = db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC)
                && x.FechaEmision >= desde
                && x.FechaEmision < hastaExclusivo);

        var cotizacionesQuery = db.Cotizaciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.FechaEmision >= desde
                && x.FechaEmision < hastaExclusivo);

        var comprasQuery = db.Compras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Fecha >= desde && x.Fecha < hastaExclusivo);

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

        var ventasPorDia = await ventasQuery
            .GroupBy(x => x.FechaEmision.Date)
            .OrderBy(x => x.Key)
            .Select(x => new DashboardSerieDto(x.Key, x.Sum(y => y.Total)))
            .ToListAsync(cancellationToken);

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
                Cliente = x.Cliente!.NombreCompleto,
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

        var alertas = BuildAlertas(comprobantesPendientesSunat, productosBajoStock, cotizaciones, totalVentas, totalCompras, totalGastos, totalIngresos);

        return new DashboardDto(
            desde,
            fechaHasta.Date,
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
            ventasPorDia,
            gastosPorCategoria,
            productosMasVendidos,
            ultimosComprobantes,
            alertas);
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
