using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class ReportesController(CompraService compraService, DevolucionService devolucionService) : Controller
{
    public IActionResult Ventas() => Reporte("Reporte Ventas");
    public IActionResult Compras() => Reporte("Reporte Compras");
    public IActionResult Productos() => Reporte("Reporte Productos");
    public IActionResult Clientes() => Reporte("Reporte Clientes");
    public IActionResult Gastos() => Reporte("Reporte Gastos");
    public IActionResult Ingresos() => Reporte("Reporte Ingresos");
    public IActionResult FlujoCaja() => Reporte("Flujo de Caja");
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

    private IActionResult Reporte(string title)
    {
        ViewData["Title"] = title;
        return View("Reporte");
    }
}
