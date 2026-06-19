using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class IngresosController(IngresoService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewBag.CategoriasIngreso = await service.ListarCategoriasAsync(cancellationToken);
        return View(new IngresoEditDto());
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerAsync(id, cancellationToken);
        ViewBag.CategoriasIngreso = await service.ListarCategoriasAsync(cancellationToken);
        return dto is null ? NotFound() : View("Create", dto);
    }

    public async Task<IActionResult> Visualizar(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerAsync(id, cancellationToken);
        if (dto is null) return NotFound();
        ViewData["ReadOnly"] = true;
        ViewBag.CategoriasIngreso = await service.ListarCategoriasAsync(cancellationToken);
        return View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IngresoEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = "Ingreso guardado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            ViewBag.CategoriasIngreso = await service.ListarCategoriasAsync(cancellationToken);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            await service.AnularAsync(id, motivo, cancellationToken);
            TempData["Success"] = "Ingreso anulado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }
}
