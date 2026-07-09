using System.Text;
using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Web.Controllers;

public class ReportesController(
    CompraService compraService,
    DevolucionService devolucionService,
    ReporteGeneralService reporteGeneralService) : Controller
{
    public async Task<IActionResult> Reporte(int? anioDesde, int? anioHasta, string? indicador, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Reporte general";
        return View(await reporteGeneralService.ObtenerAsync(anioDesde, anioHasta, indicador, cancellationToken));
    }

    public IActionResult Ventas() => RedirectToAction(nameof(Reporte), new { indicador = ReporteGeneralIndicadores.Ventas });
    public IActionResult Compras() => RedirectToAction(nameof(Reporte), new { indicador = ReporteGeneralIndicadores.Compras });
    public IActionResult Productos() => RedirectToAction("Index", "Productos");
    public IActionResult Clientes() => RedirectToAction("Index", "Clientes");
    public IActionResult Gastos() => RedirectToAction(nameof(Reporte), new { indicador = ReporteGeneralIndicadores.Gastos });
    public IActionResult Ingresos() => RedirectToAction(nameof(Reporte), new { indicador = ReporteGeneralIndicadores.Ingresos });
    public IActionResult FlujoCaja() => RedirectToAction(nameof(Reporte), new { indicador = ReporteGeneralIndicadores.Resultado });
    public IActionResult Cotizaciones() => RedirectToAction("Index", "Cotizaciones");
    public async Task<IActionResult> CuentasPorPagar([FromQuery] SearchRequest request, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Cuentas por pagar";
        return View(await compraService.CuentasPorPagarAsync(request, cancellationToken));
    }

    public async Task<IActionResult> DevolucionesProveedor([FromQuery] SearchRequest request, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Devoluciones proveedor";
        return View(await devolucionService.BuscarAsync(request, cancellationToken));
    }

    public async Task<IActionResult> NotasPedido([FromQuery] ReporteNotasPedidoRequest request, CancellationToken cancellationToken)
    {
        request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        ViewData["Title"] = "Notas de pedido";
        return View(await reporteGeneralService.ObtenerNotasPedidoAsync(request, cancellationToken));
    }

    public async Task<IActionResult> ExportarNotasPedido([FromQuery] ReporteNotasPedidoRequest request, CancellationToken cancellationToken)
    {
        request.Page = 1;
        request.PageSize = 5000;
        var reporte = await reporteGeneralService.ObtenerNotasPedidoAsync(request, cancellationToken);
        var csv = new StringBuilder();
        csv.AppendLine("Fecha,Nro Nota Pedido,Cliente,Nro Documento,Estado Documento,Estado Pago,Vendedor,Tipo Venta,Subtotal,IGV,Total,Total Cobrado,Saldo,Observacion");

        foreach (var item in reporte.Notas.Items)
        {
            csv.AppendLine(string.Join(',', new[]
            {
                Csv(item.Fecha.ToString("dd/MM/yyyy")),
                Csv(item.Numero),
                Csv(item.Cliente),
                Csv(item.NumeroDocumento),
                Csv(item.EstadoDocumento.ToString()),
                Csv(EstadoPagoTexto(item.EstadoPago)),
                Csv(item.Vendedor),
                Csv(item.TipoVenta),
                Csv(item.Subtotal.ToString("0.00")),
                Csv(item.Igv.ToString("0.00")),
                Csv(item.Total.ToString("0.00")),
                Csv(item.TotalCobrado.ToString("0.00")),
                Csv(item.SaldoPendiente.ToString("0.00")),
                Csv(item.Observacion)
            }));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        return File(bytes, "text/csv; charset=utf-8", $"reporte-notas-pedido-{PeruDateTime.Today:yyyyMMdd}.csv");
    }


    public async Task<IActionResult> Comprobantes([FromQuery] ReporteComprobantesRequest request, CancellationToken cancellationToken)
    {
        request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        ViewData["Title"] = "Reporte de comprobantes";
        return View(await reporteGeneralService.ObtenerComprobantesAsync(request, cancellationToken));
    }

    public async Task<IActionResult> ExportarComprobantes([FromQuery] ReporteComprobantesRequest request, CancellationToken cancellationToken)
    {
        request.Page = 1;
        request.PageSize = 5000;
        var reporte = await reporteGeneralService.ObtenerComprobantesAsync(request, cancellationToken);
        var csv = new StringBuilder();
        csv.AppendLine("Fecha,Tipo,Serie Numero,Cliente,Documento,Moneda,Gravado,IGV,Exonerado,Importe Total,Cobrado,Saldo,Estado SUNAT,Estado,Vendedor,Medio de Pago");

        foreach (var item in reporte.Comprobantes.Items)
        {
            csv.AppendLine(string.Join(',', new[]
            {
                Csv(item.Fecha.ToString("dd/MM/yyyy")),
                Csv(TipoComprobanteTexto(item.TipoComprobante)),
                Csv(item.SerieNumero),
                Csv(item.Cliente),
                Csv(item.NumeroDocumento),
                Csv(item.Moneda),
                Csv(item.Gravado.ToString("0.00")),
                Csv(item.Igv.ToString("0.00")),
                Csv(item.Exonerado.ToString("0.00")),
                Csv(item.Total.ToString("0.00")),
                Csv(item.TotalCobrado.ToString("0.00")),
                Csv(item.SaldoPendiente.ToString("0.00")),
                Csv(EstadoSunatTexto(item.EstadoSunat)),
                Csv(EstadoRegistroTexto(item.Estado)),
                Csv(item.Vendedor),
                Csv(item.MedioPago)
            }));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        return File(bytes, "text/csv; charset=utf-8", $"reporte-comprobantes-{PeruDateTime.Today:yyyyMMdd}.csv");
    }

    public async Task<IActionResult> MovimientoCaja([FromQuery] ReporteMovimientoCajaRequest request, CancellationToken cancellationToken)
    {
        request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        ViewData["Title"] = "Reporte de movimiento de caja";
        return View(await reporteGeneralService.ObtenerMovimientoCajaAsync(request, cancellationToken));
    }

    public async Task<IActionResult> ExportarMovimientoCaja([FromQuery] ReporteMovimientoCajaRequest request, CancellationToken cancellationToken)
    {
        request.Page = 1;
        request.PageSize = 5000;
        var reporte = await reporteGeneralService.ObtenerMovimientoCajaAsync(request, cancellationToken);
        var csv = new StringBuilder();
        csv.AppendLine("Fecha,Tipo,Origen,Cliente Proveedor,Documento,Medio de Pago,Cuenta,Descripcion,Monto");

        foreach (var item in reporte.Movimientos.Items)
        {
            csv.AppendLine(string.Join(',', new[]
            {
                Csv(item.Fecha.ToString("dd/MM/yyyy")),
                Csv(TipoMovimientoTexto(item.TipoMovimiento)),
                Csv(item.OrigenDescripcion),
                Csv(item.ClienteProveedor),
                Csv(item.Documento),
                Csv(item.MedioPago),
                Csv(item.CuentaFinanciera),
                Csv(item.Descripcion),
                Csv(item.Monto.ToString("0.00"))
            }));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        return File(bytes, "text/csv; charset=utf-8", $"reporte-movimiento-caja-{PeruDateTime.Today:yyyyMMdd}.csv");
    }

    private static string Csv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
    private static string EstadoPagoTexto(EstadoPagoNotaPedido estado) => estado switch
    {
        EstadoPagoNotaPedido.PAGO_PARCIAL => "Parcial",
        EstadoPagoNotaPedido.PAGADO => "Pagado",
        _ => "Pendiente"
    };
    private static string EstadoSunatTexto(EstadoSunat estado) => estado switch
    {
        EstadoSunat.Aceptado => "Aceptado",
        EstadoSunat.Rechazado => "Rechazado",
        EstadoSunat.Observado => "Observado",
        EstadoSunat.Pendiente => "Pendiente",
        EstadoSunat.Anulado => "Anulado",
        _ => "No enviado"
    };

    private static string EstadoRegistroTexto(EstadoRegistro estado) => estado == EstadoRegistro.Activo ? "Activo" : "Anulado";

    private static string TipoMovimientoTexto(TipoMovimientoCaja tipo) => tipo == TipoMovimientoCaja.INGRESO ? "Ingreso" : "Egreso";

    private static string TipoComprobanteTexto(TipoComprobante tipo) => tipo switch
    {
        TipoComprobante.FAC => "Factura",
        TipoComprobante.BOL => "Boleta",
        TipoComprobante.NCR => "Nota de credito",
        _ => tipo.ToString()
    };
}


