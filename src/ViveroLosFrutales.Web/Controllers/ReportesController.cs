using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

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

}
