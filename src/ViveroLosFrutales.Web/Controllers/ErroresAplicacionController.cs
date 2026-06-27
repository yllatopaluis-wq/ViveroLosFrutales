using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class ErroresAplicacionController(ErrorAplicacionService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] ErrorAplicacionSearchDto request, CancellationToken cancellationToken)
    {
        request.FechaDesde ??= PeruDateTime.Today.AddDays(-30);
        request.FechaHasta ??= PeruDateTime.Today;
        ViewBag.Filtros = request;
        return View(await service.BuscarAsync(request, cancellationToken));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken) =>
        View(await service.ObtenerAsync(id, cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revisar(int id, string observacion, CancellationToken cancellationToken)
    {
        await service.MarcarRevisadoAsync(id, observacion, cancellationToken);
        TempData["Success"] = "El error fue marcado como revisado.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
