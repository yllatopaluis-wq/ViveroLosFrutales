using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Infrastructure.Identity;

namespace ViveroLosFrutales.Web.Controllers;

public class EmpresasController(EmpresaService service, UserManager<ApplicationUser> userManager) : Controller
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

    [HttpGet("Empresas/Logo/{id:int}")]
    public async Task<IActionResult> Logo(int id, CancellationToken cancellationToken)
    {
        var empresaActivaId = HttpContext.Session.GetInt32("EmpresaId");
        if (empresaActivaId is null || empresaActivaId.Value != id) return NotFound();

        var usuarioId = userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(usuarioId)) return NotFound();

        var empresa = await service.ObtenerLogoActivaAsync(id, usuarioId, cancellationToken);
        if (empresa?.LogoContenido is not { Length: > 0 } || string.IsNullOrWhiteSpace(empresa.LogoContentType)) return NotFound();

        return File(empresa.LogoContenido, empresa.LogoContentType);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EmpresaEditDto dto, IFormFile? logoArchivo, IFormFile? firmaArchivo, CancellationToken cancellationToken)
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

            if (firmaArchivo is { Length: > 0 })
            {
                if (!firmaArchivo.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(firmaArchivo), "El archivo de la firma debe ser una imagen.");
                    return View(dto);
                }

                if (firmaArchivo.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(firmaArchivo), "La firma no debe superar 2 MB.");
                    return View(dto);
                }

                using var firmaMemory = new MemoryStream();
                await firmaArchivo.CopyToAsync(firmaMemory, cancellationToken);
                dto.FirmaContenido = firmaMemory.ToArray();
                dto.FirmaContentType = firmaArchivo.ContentType;
                dto.FirmaNombre = Path.GetFileName(firmaArchivo.FileName);
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
