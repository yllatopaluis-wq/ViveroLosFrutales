using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Web.Controllers;

public class CuentasFinancierasController(CuentaFinancieraService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, TipoCuentaFinanciera? tipo, bool? activo, CancellationToken cancellationToken)
    {
        ViewBag.Tipo = tipo;
        ViewBag.Activo = activo;
        return View(await service.BuscarAsync(request, tipo, activo, cancellationToken));
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await service.NuevoAsync(cancellationToken));

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerAsync(id, cancellationToken);
        return dto is null ? NotFound() : View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CuentaFinancieraEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = "Cuenta financiera guardada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, bool activo, CancellationToken cancellationToken)
    {
        try
        {
            await service.CambiarEstadoAsync(id, activo, cancellationToken);
            TempData["Success"] = activo ? "Cuenta financiera activada." : "Cuenta financiera anulada.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }
}

