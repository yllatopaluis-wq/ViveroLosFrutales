using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class UsuariosController(
    UsuarioService service,
    RolService rolService,
    EmpresaService empresaService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Edit(string? id, CancellationToken cancellationToken)
    {
        await CargarCombosAsync(cancellationToken);
        if (string.IsNullOrEmpty(id)) return View(new UsuarioEditDto());
        var dto = await service.ObtenerAsync(id, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsuarioEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = "Usuario guardado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await CargarCombosAsync(cancellationToken);
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(dto);
        }
    }

    public IActionResult RestablecerPassword(string id) => View((id, string.Empty));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestablecerPassword(string id, string password, CancellationToken cancellationToken)
    {
        await service.RestablecerPasswordAsync(id, password, cancellationToken);
        TempData["Success"] = "Contrasena restablecida correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCombosAsync(CancellationToken cancellationToken)
    {
        ViewBag.Roles = (await rolService.BuscarAsync(new SearchRequest { PageSize = 500 }, cancellationToken)).Items;
        ViewBag.Empresas = (await empresaService.BuscarAsync(new SearchRequest { PageSize = 500 }, cancellationToken)).Items;
    }
}
