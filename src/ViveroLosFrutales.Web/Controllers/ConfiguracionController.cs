using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class ConfiguracionController(ConfiguracionEmpresaService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Correlativos(CancellationToken cancellationToken) =>
        View(await service.ObtenerCorrelativosAsync(cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Correlativos(CorrelativosEmpresaDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarCorrelativosAsync(dto, cancellationToken);
            TempData["Success"] = "Correlativos guardados correctamente.";
            return RedirectToAction(nameof(Correlativos));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(dto);
        }
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id is null) return View(new ConfiguracionEmpresaEditDto());
        var dto = await service.ObtenerAsync(id.Value, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ConfiguracionEmpresaEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = "Configuracion guardada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(dto);
        }
    }
}
