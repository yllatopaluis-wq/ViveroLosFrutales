using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class TransferenciasController(TransferenciaService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await service.NuevoAsync(cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransferenciaEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.RegistrarAsync(dto, cancellationToken);
            TempData["Success"] = "Transferencia registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(await service.PrepararFormularioAsync(dto, cancellationToken));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            await service.AnularAsync(id, motivo, cancellationToken);
            TempData["Success"] = "Transferencia anulada correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }
}
