using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class EmpresasController(EmpresaService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken)
    {
        var result = await service.BuscarAsync(request, cancellationToken);
        return View(result);
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id is null) return View(new EmpresaEditDto());
        var dto = await service.ObtenerAsync(id.Value, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    public async Task<IActionResult> Visualizar(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerAsync(id, cancellationToken);
        if (dto is null) return NotFound();
        ViewData["ReadOnly"] = true;
        return View("Edit", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EmpresaEditDto dto, IFormFile? logoArchivo, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            if (logoArchivo is { Length: > 0 })
            {
                if (!logoArchivo.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(logoArchivo), "El archivo del logo debe ser una imagen.");
                    return View(dto);
                }

                if (logoArchivo.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(logoArchivo), "El logo no debe superar 2 MB.");
                    return View(dto);
                }

                using var memory = new MemoryStream();
                await logoArchivo.CopyToAsync(memory, cancellationToken);
                dto.LogoContenido = memory.ToArray();
                dto.LogoContentType = logoArchivo.ContentType;
                dto.LogoNombre = Path.GetFileName(logoArchivo.FileName);
            }

            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = "Empresa guardada correctamente.";
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
    public async Task<IActionResult> Anular(int id, CancellationToken cancellationToken)
    {
        await service.AnularAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
