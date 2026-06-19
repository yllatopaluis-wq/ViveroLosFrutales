using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class ProductosController(ProductoService service, CategoriaService categoriaService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        ViewBag.Categorias = await categoriaService.ListarActivasAsync(cancellationToken);
        if (id is null) return View(new ProductoEditDto());
        var dto = await service.ObtenerAsync(id.Value, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    public async Task<IActionResult> Visualizar(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerAsync(id, cancellationToken);
        if (dto is null) return NotFound();
        ViewBag.Categorias = await categoriaService.ListarActivasAsync(cancellationToken);
        ViewData["ReadOnly"] = true;
        return View("Edit", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductoEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = "Producto guardado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Categorias = await categoriaService.ListarActivasAsync(cancellationToken);
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, CancellationToken cancellationToken)
    {
        await service.AnularAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
