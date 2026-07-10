using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
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
    public async Task<ReporteNotasPedidoDto> ObtenerNotasPedidoAsync(
        int empresaId,
        ReporteNotasPedidoRequest request,
        CancellationToken cancellationToken)
    {
        var query = db.NotasPedido.AsNoTracking().Where(x => x.EmpresaId == empresaId);

        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.Fecha < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => (x.ClienteNombre ?? string.Empty).Contains(term)
                || (x.ClienteNumeroDocumento ?? string.Empty).Contains(term)
                || x.Cliente!.NombreCompleto.Contains(term)
                || x.Cliente.NumeroDocumento.Contains(term)
                || x.Serie.Contains(term)
                || (x.Serie + "-" + x.Correlativo).Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Numero))
        {
            var numero = request.Numero.Trim();
            query = query.Where(x => x.Serie.Contains(numero) || (x.Serie + "-" + x.Correlativo).Contains(numero));
        }

        if (request.EstadoDocumento is not null) query = query.Where(x => x.EstadoDocumento == request.EstadoDocumento.Value);

        var shaped = query.Select(x => new
        {
            Nota = x,
            TotalCobrado = x.Cobros
                .Where(c => c.Estado != CobroClienteEstado.ANULADO)
                .Sum(c => (decimal?)c.Monto) ?? 0
        })
        .Select(x => new
        {
            x.Nota,
            x.TotalCobrado,
            Saldo = x.Nota.Total - x.TotalCobrado < 0 ? 0 : x.Nota.Total - x.TotalCobrado,
            EstadoPagoCalculado = x.TotalCobrado <= 0
                ? EstadoPagoNotaPedido.PENDIENTE
                : x.Nota.Total - x.TotalCobrado <= 0
                    ? EstadoPagoNotaPedido.PAGADO
                    : EstadoPagoNotaPedido.PAGO_PARCIAL
        });

        if (request.EstadoPago is not null) shaped = shaped.Where(x => x.EstadoPagoCalculado == request.EstadoPago.Value);

        var resumenFilas = await shaped
            .Select(x => new
            {
                x.Nota.Total,
                x.TotalCobrado,
                x.Saldo,
                x.EstadoPagoCalculado,
                x.Nota.EstadoDocumento
            })
            .ToListAsync(cancellationToken);

        var resumen = new ReporteNotasPedidoResumenDto(
            resumenFilas.Count,
            resumenFilas.Sum(x => x.Total),
            resumenFilas.Sum(x => x.TotalCobrado),
            resumenFilas.Sum(x => x.Saldo),
            resumenFilas.Count(x => x.EstadoPagoCalculado == EstadoPagoNotaPedido.PENDIENTE),
            resumenFilas.Count(x => x.EstadoPagoCalculado == EstadoPagoNotaPedido.PAGO_PARCIAL),
            resumenFilas.Count(x => x.EstadoPagoCalculado == EstadoPagoNotaPedido.PAGADO),
            resumenFilas.Count(x => x.EstadoDocumento == NotaPedidoEstado.ANULADO));

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, 5000);
        var total = resumen.TotalNotas;

        var items = await shaped
            .OrderByDescending(x => x.Nota.Fecha)
            .ThenByDescending(x => x.Nota.NotaPedidoId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ReporteNotaPedidoRowDto(
                x.Nota.NotaPedidoId,
                x.Nota.Fecha,
                x.Nota.Serie + "-" + x.Nota.Correlativo,
                x.Nota.ClienteNombre != null && x.Nota.ClienteNombre != string.Empty ? x.Nota.ClienteNombre : x.Nota.Cliente!.NombreCompleto,
                x.Nota.ClienteNumeroDocumento != null && x.Nota.ClienteNumeroDocumento != string.Empty ? x.Nota.ClienteNumeroDocumento : x.Nota.Cliente!.NumeroDocumento,
                x.Nota.EstadoDocumento,
                x.EstadoPagoCalculado,
                x.Nota.UsuarioModificacion == "" ? "-" : x.Nota.UsuarioModificacion,
                x.Saldo > 0 ? "Credito" : "Contado",
                x.Nota.Subtotal,
                x.Nota.Igv,
                x.Nota.Total,
                x.TotalCobrado,
                x.Saldo,
                x.Nota.ComprobanteId == null ? "-" : "Convertida a comprobante"))
            .ToListAsync(cancellationToken);

        return new ReporteNotasPedidoDto
        {
            Request = request,
            Notas = new PagedResult<ReporteNotaPedidoRowDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            },
            Resumen = resumen
        };
    }
    public async Task<ReporteComprobantesDto> ObtenerComprobantesAsync(
        int empresaId,
        ReporteComprobantesRequest request,
        CancellationToken cancellationToken)
    {
        var baseQuery = db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC));

        var series = await baseQuery
            .Select(x => x.Serie)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var vendedores = await baseQuery
            .Where(x => x.UsuarioRegistro != "")
            .Select(x => x.UsuarioRegistro)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var mediosDb = await db.CobrosCliente.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == CobroClienteEstado.ACTIVO && x.ComprobanteId != null && x.MedioPago != "")
            .Select(x => x.MedioPago)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
        var mediosPago = new[] { "Contado", "Credito" }.Concat(mediosDb).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToArray();

        var query = baseQuery;

        if (request.TipoComprobante is not null) query = query.Where(x => x.TipoComprobante == request.TipoComprobante.Value);
        if (!string.IsNullOrWhiteSpace(request.Serie))
        {
            var serie = request.Serie.Trim();
            query = query.Where(x => x.Serie == serie);
        }

        if (!string.IsNullOrWhiteSpace(request.Numero))
        {
            var numero = request.Numero.Trim();
            query = query.Where(x => x.Serie.Contains(numero) || (x.Serie + "-" + x.Correlativo).Contains(numero));
        }

        if (request.FechaDesde is not null) query = query.Where(x => x.FechaEmision >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaEmision < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Cliente))
        {
            var cliente = request.Cliente.Trim();
            query = query.Where(x => (x.ClienteNombre ?? string.Empty).Contains(cliente)
                || (x.ClienteNumeroDocumento ?? string.Empty).Contains(cliente)
                || x.Cliente!.NombreCompleto.Contains(cliente)
                || x.Cliente.NumeroDocumento.Contains(cliente));
        }

        if (request.EstadoSunat is not null) query = query.Where(x => x.EstadoSunat == request.EstadoSunat.Value);
        if (request.EstadoComprobante is not null) query = query.Where(x => x.Estado == request.EstadoComprobante.Value);
        if (!string.IsNullOrWhiteSpace(request.Vendedor))
        {
            var vendedor = request.Vendedor.Trim();
            query = query.Where(x => x.UsuarioRegistro == vendedor);
        }

        var shaped = query.Select(x => new
        {
            Comprobante = x,
            TotalPagado = x.Cobros
                .Where(c => c.Estado == CobroClienteEstado.ACTIVO
                    && !x.CobrosAplicados.Any(a => a.CobroClienteId == c.CobroClienteId))
                .Sum(c => (decimal?)c.Monto) ?? 0,
            TotalAplicado = x.CobrosAplicados
                .Where(c => c.CobroCliente != null && c.CobroCliente.Estado == CobroClienteEstado.ACTIVO)
                .Sum(c => (decimal?)c.MontoAplicado) ?? 0,
            TotalNotasCredito = db.Comprobantes
                .Where(nc => nc.EmpresaId == x.EmpresaId
                    && nc.TipoComprobante == TipoComprobante.NCR
                    && nc.Estado == EstadoRegistro.Activo
                    && nc.ComprobanteReferenciaId == x.ComprobanteId)
                .Sum(nc => (decimal?)nc.Total) ?? 0,
            MedioPagoCobro = x.Cobros
                .Where(c => c.Estado == CobroClienteEstado.ACTIVO && c.MedioPago != "")
                .OrderByDescending(c => c.CobroClienteId)
                .Select(c => c.MedioPago)
                .FirstOrDefault()
        })
        .Select(x => new
        {
            x.Comprobante,
            TotalCobrado = x.TotalPagado + x.TotalAplicado,
            Saldo = x.Comprobante.Total - x.TotalNotasCredito - (x.TotalPagado + x.TotalAplicado) < 0
                ? 0
                : x.Comprobante.Total - x.TotalNotasCredito - (x.TotalPagado + x.TotalAplicado),
            Gravado = x.Comprobante.Igv > 0 ? x.Comprobante.SubTotal : 0,
            Exonerado = x.Comprobante.Igv > 0 ? 0 : x.Comprobante.SubTotal,
            MedioPago = x.MedioPagoCobro ?? (x.Comprobante.FormaPago == FormaPago.Credito ? "Credito" : "Contado")
        });

        if (!string.IsNullOrWhiteSpace(request.MedioPago))
        {
            var medioPago = request.MedioPago.Trim();
            shaped = shaped.Where(x => x.MedioPago == medioPago);
        }

        var resumenFilas = await shaped
            .Select(x => new
            {
                x.Gravado,
                x.Comprobante.Igv,
                x.Exonerado,
                x.Comprobante.Total,
                x.TotalCobrado,
                x.Saldo
            })
            .ToListAsync(cancellationToken);

        var resumen = new ReporteComprobantesResumenDto(
            resumenFilas.Count,
            resumenFilas.Sum(x => x.Total),
            resumenFilas.Sum(x => x.Igv),
            resumenFilas.Sum(x => x.Gravado),
            resumenFilas.Sum(x => x.Exonerado),
            resumenFilas.Sum(x => x.TotalCobrado),
            resumenFilas.Sum(x => x.Saldo));

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, 5000);

        var items = await shaped
            .OrderByDescending(x => x.Comprobante.FechaEmision)
            .ThenByDescending(x => x.Comprobante.ComprobanteId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ReporteComprobanteRowDto(
                x.Comprobante.ComprobanteId,
                x.Comprobante.FechaEmision,
                x.Comprobante.TipoComprobante,
                x.Comprobante.Serie + "-" + x.Comprobante.Correlativo,
                x.Comprobante.ClienteNombre != null && x.Comprobante.ClienteNombre != string.Empty ? x.Comprobante.ClienteNombre : x.Comprobante.Cliente!.NombreCompleto,
                x.Comprobante.ClienteNumeroDocumento != null && x.Comprobante.ClienteNumeroDocumento != string.Empty ? x.Comprobante.ClienteNumeroDocumento : x.Comprobante.Cliente!.NumeroDocumento,
                "Soles",
                x.Gravado,
                x.Comprobante.Igv,
                x.Exonerado,
                x.Comprobante.Total,
                x.TotalCobrado,
                x.Saldo,
                x.Comprobante.EstadoSunat,
                x.Comprobante.Estado,
                x.Comprobante.UsuarioRegistro == "" ? "-" : x.Comprobante.UsuarioRegistro,
                x.MedioPago))
            .ToListAsync(cancellationToken);

        return new ReporteComprobantesDto
        {
            Request = request,
            Comprobantes = new PagedResult<ReporteComprobanteRowDto>
            {
                Items = items,
                Total = resumen.TotalComprobantes,
                Page = page,
                PageSize = pageSize
            },
            Resumen = resumen,
            Series = series,
            MediosPago = mediosPago,
            Vendedores = vendedores
        };
    }


    public async Task<ReporteMovimientoCajaDto> ObtenerMovimientoCajaAsync(
        int empresaId,
        ReporteMovimientoCajaRequest request,
        CancellationToken cancellationToken)
    {
        var cuentasFinancieras = await db.CuentasFinancieras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Activo)
            .OrderBy(x => x.Tipo)
            .ThenBy(x => x.Nombre)
            .Select(x => new CuentaFinancieraOptionDto(x.CuentaFinancieraId, x.Nombre, x.Tipo, x.Moneda))
            .ToListAsync(cancellationToken);

        var mediosPago = await db.MovimientosCaja.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo && x.MedioPago != "")
            .Select(x => x.MedioPago)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var query = db.MovimientosCaja.AsNoTracking()
            .Include(x => x.CuentaFinanciera)
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo);

        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.Fecha < hasta);
        }

        if (request.TipoMovimiento is not null) query = query.Where(x => x.TipoMovimiento == request.TipoMovimiento.Value);
        if (request.Origen is not null) query = query.Where(x => x.Origen == request.Origen.Value);
        if (request.CuentaFinancieraId is int cuentaId && cuentaId > 0) query = query.Where(x => x.CuentaFinancieraId == cuentaId);
        if (!string.IsNullOrWhiteSpace(request.MedioPago))
        {
            var medio = request.MedioPago.Trim();
            query = query.Where(x => x.MedioPago == medio);
        }

        var movimientosBase = await query
            .OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.MovimientoCajaId)
            .ToListAsync(cancellationToken);

        var cobroIds = movimientosBase.Where(x => x.Origen == OrigenMovimientoCaja.COBRO_CLIENTE).Select(x => x.OrigenId).Distinct().ToArray();
        var devolucionIds = movimientosBase.Where(x => x.Origen is OrigenMovimientoCaja.DEVOLUCION_CLIENTE or OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR).Select(x => x.OrigenId).Distinct().ToArray();
        var gastoIds = movimientosBase.Where(x => x.Origen == OrigenMovimientoCaja.GASTO).Select(x => x.OrigenId).Distinct().ToArray();
        var pagoProveedorIds = movimientosBase.Where(x => x.Origen == OrigenMovimientoCaja.PAGO_PROVEEDOR).Select(x => x.OrigenId).Distinct().ToArray();
        var ingresoIds = movimientosBase.Where(x => x.Origen == OrigenMovimientoCaja.INGRESO_MANUAL).Select(x => x.OrigenId).Distinct().ToArray();

        var cobros = await db.CobrosCliente.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && cobroIds.Contains(x.CobroClienteId))
            .Select(x => new
            {
                x.CobroClienteId,
                Cliente = x.Cliente!.NombreCompleto,
                NotaSerie = x.NotaPedido == null ? string.Empty : x.NotaPedido.Serie,
                NotaCorrelativo = x.NotaPedido == null ? 0 : x.NotaPedido.Correlativo,
                ComprobanteSerie = x.Comprobante == null ? string.Empty : x.Comprobante.Serie,
                ComprobanteCorrelativo = x.Comprobante == null ? 0 : x.Comprobante.Correlativo,
                Aplicado = x.Aplicaciones
                    .OrderByDescending(a => a.FechaAplicacion)
                    .Select(a => new { a.Comprobante!.Serie, a.Comprobante.Correlativo })
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.CobroClienteId, cancellationToken);

        var devoluciones = await db.Devoluciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && devolucionIds.Contains(x.DevolucionId))
            .Select(x => new
            {
                x.DevolucionId,
                Tercero = x.TipoTercero == TipoTerceroDevolucion.CLIENTE ? x.Cliente!.NombreCompleto : x.Proveedor!.RazonSocial,
                NotaSerie = x.NotaPedido == null ? string.Empty : x.NotaPedido.Serie,
                NotaCorrelativo = x.NotaPedido == null ? 0 : x.NotaPedido.Correlativo,
                ComprobanteSerie = x.Comprobante == null ? string.Empty : x.Comprobante.Serie,
                ComprobanteCorrelativo = x.Comprobante == null ? 0 : x.Comprobante.Correlativo,
                NotaCreditoSerie = x.NotaCredito == null ? string.Empty : x.NotaCredito.Serie,
                NotaCreditoCorrelativo = x.NotaCredito == null ? 0 : x.NotaCredito.Correlativo,
                CompraDocumento = x.Compra == null ? string.Empty : x.Compra.Documento
            })
            .ToDictionaryAsync(x => x.DevolucionId, cancellationToken);

        var gastos = await db.Gastos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && gastoIds.Contains(x.GastoId))
            .Select(x => new { x.GastoId, x.Descripcion, x.Categoria })
            .ToDictionaryAsync(x => x.GastoId, cancellationToken);

        var pagosProveedor = await db.PagosProveedor.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && pagoProveedorIds.Contains(x.PagoProveedorId))
            .Select(x => new
            {
                x.PagoProveedorId,
                Proveedor = x.Proveedor!.RazonSocial,
                Documento = x.Compra == null ? string.Empty : x.Compra.Documento,
                x.CompraId
            })
            .ToDictionaryAsync(x => x.PagoProveedorId, cancellationToken);

        var ingresosManuales = await db.Ingresos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && ingresoIds.Contains(x.IngresoId))
            .Select(x => new { x.IngresoId, x.TipoIngreso, x.Descripcion })
            .ToDictionaryAsync(x => x.IngresoId, cancellationToken);

        var movimientos = movimientosBase.Select(x =>
        {
            var tercero = "-";
            var documento = "-";
            var descripcion = x.Descripcion;
            var extraBusqueda = string.Empty;

            if (x.Origen == OrigenMovimientoCaja.COBRO_CLIENTE && cobros.TryGetValue(x.OrigenId, out var cobro))
            {
                tercero = cobro.Cliente;
                documento = DocumentoCobro(
                    Numero(cobro.NotaSerie, cobro.NotaCorrelativo),
                    Numero(cobro.ComprobanteSerie, cobro.ComprobanteCorrelativo),
                    cobro.Aplicado is null ? string.Empty : Numero(cobro.Aplicado.Serie, cobro.Aplicado.Correlativo));
                extraBusqueda = $"{tercero} {documento}";
            }
            else if ((x.Origen == OrigenMovimientoCaja.DEVOLUCION_CLIENTE || x.Origen == OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR) && devoluciones.TryGetValue(x.OrigenId, out var devolucion))
            {
                tercero = devolucion.Tercero;
                documento = DocumentoDevolucion(
                    Numero(devolucion.NotaSerie, devolucion.NotaCorrelativo),
                    Numero(devolucion.ComprobanteSerie, devolucion.ComprobanteCorrelativo),
                    Numero(devolucion.NotaCreditoSerie, devolucion.NotaCreditoCorrelativo),
                    devolucion.CompraDocumento);
                extraBusqueda = $"{tercero} {documento}";
            }
            else if (x.Origen == OrigenMovimientoCaja.GASTO && gastos.TryGetValue(x.OrigenId, out var gasto))
            {
                documento = $"GASTO-{gasto.GastoId:000000}";
                descripcion = string.IsNullOrWhiteSpace(descripcion) ? gasto.Descripcion : descripcion;
                extraBusqueda = $"{documento} {gasto.Categoria} {gasto.Descripcion}";
            }
            else if (x.Origen == OrigenMovimientoCaja.PAGO_PROVEEDOR && pagosProveedor.TryGetValue(x.OrigenId, out var pagoProveedor))
            {
                tercero = pagoProveedor.Proveedor;
                documento = string.IsNullOrWhiteSpace(pagoProveedor.Documento) ? $"COMPRA-{pagoProveedor.CompraId:000000}" : pagoProveedor.Documento;
                extraBusqueda = $"{tercero} {documento}";
            }
            else if (x.Origen == OrigenMovimientoCaja.INGRESO_MANUAL && ingresosManuales.TryGetValue(x.OrigenId, out var ingreso))
            {
                documento = $"INGRESO-{ingreso.IngresoId:000000}";
                descripcion = string.IsNullOrWhiteSpace(descripcion) ? ingreso.Descripcion : descripcion;
                extraBusqueda = $"{documento} {ingreso.TipoIngreso} {ingreso.Descripcion}";
            }
            else if (x.Origen == OrigenMovimientoCaja.TRANSFERENCIA)
            {
                documento = $"TRF-{x.OrigenId:000000}";
                extraBusqueda = $"{documento} {descripcion}";
            }

            var cuenta = x.CuentaFinanciera?.Nombre ?? string.Empty;
            var dto = new ReporteMovimientoCajaRowDto(
                x.MovimientoCajaId,
                x.Fecha,
                x.TipoMovimiento,
                x.Origen,
                OrigenDescripcion(x.Origen),
                tercero,
                documento,
                x.MedioPago,
                cuenta,
                x.Monto,
                descripcion);

            return new MovimientoCajaReporteItem(dto, $"{extraBusqueda} {x.MedioPago} {cuenta} {descripcion}");
        });

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            movimientos = movimientos.Where(x => x.SearchText.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var filas = movimientos.ToList();
        var ingresos = filas.Where(x => x.Dto.TipoMovimiento == TipoMovimientoCaja.INGRESO).Sum(x => x.Dto.Monto);
        var egresos = filas.Where(x => x.Dto.TipoMovimiento == TipoMovimientoCaja.EGRESO).Sum(x => x.Dto.Monto);
        var resumen = new ReporteMovimientoCajaResumenDto(
            filas.Count,
            ingresos,
            egresos,
            ingresos - egresos,
            filas.Count(x => x.Dto.TipoMovimiento == TipoMovimientoCaja.INGRESO),
            filas.Count(x => x.Dto.TipoMovimiento == TipoMovimientoCaja.EGRESO));

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, 5000);
        var items = filas.Select(x => x.Dto).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new ReporteMovimientoCajaDto
        {
            Request = request,
            Movimientos = new PagedResult<ReporteMovimientoCajaRowDto>
            {
                Items = items,
                Total = resumen.TotalMovimientos,
                Page = page,
                PageSize = pageSize
            },
            Resumen = resumen,
            MediosPago = mediosPago,
            CuentasFinancieras = cuentasFinancieras
        };
    }

    private static string DocumentoCobro(string notaNumero, string comprobanteNumero, string aplicadoNumero)
    {
        if (!string.IsNullOrWhiteSpace(notaNumero) && !string.IsNullOrWhiteSpace(aplicadoNumero)) return $"{notaNumero} -> {aplicadoNumero}";
        if (!string.IsNullOrWhiteSpace(notaNumero)) return notaNumero;
        if (!string.IsNullOrWhiteSpace(comprobanteNumero)) return comprobanteNumero;
        if (!string.IsNullOrWhiteSpace(aplicadoNumero)) return aplicadoNumero;
        return "-";
    }

    private static string Numero(string serie, int correlativo) =>
        string.IsNullOrWhiteSpace(serie) || correlativo <= 0 ? string.Empty : $"{serie}-{correlativo}";

    private static string DocumentoDevolucion(string notaNumero, string comprobanteNumero, string notaCreditoNumero, string compraDocumento)
    {
        if (!string.IsNullOrWhiteSpace(notaNumero)) return notaNumero;
        if (!string.IsNullOrWhiteSpace(comprobanteNumero) && !string.IsNullOrWhiteSpace(notaCreditoNumero)) return $"{comprobanteNumero} -> {notaCreditoNumero}";
        if (!string.IsNullOrWhiteSpace(notaCreditoNumero)) return notaCreditoNumero;
        if (!string.IsNullOrWhiteSpace(comprobanteNumero)) return comprobanteNumero;
        if (!string.IsNullOrWhiteSpace(compraDocumento)) return compraDocumento;
        return "-";
    }

    private static string OrigenDescripcion(OrigenMovimientoCaja origen) => origen switch
    {
        OrigenMovimientoCaja.COBRO_CLIENTE => "Cobro Cliente",
        OrigenMovimientoCaja.PAGO_PROVEEDOR => "Pago Proveedor",
        OrigenMovimientoCaja.GASTO => "Gasto",
        OrigenMovimientoCaja.INGRESO_MANUAL => "Ingreso Manual",
        OrigenMovimientoCaja.DEVOLUCION_CLIENTE => "Devolucion Cliente",
        OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR => "Devolucion Proveedor",
        OrigenMovimientoCaja.TRANSFERENCIA => "Transferencia",
        OrigenMovimientoCaja.OTRO => "Otro",
        _ => origen.ToString()
    };

    private sealed record MovimientoCajaReporteItem(ReporteMovimientoCajaRowDto Dto, string SearchText);

    private sealed record MesMonto(int Anio, int Mes, decimal Monto);
}
