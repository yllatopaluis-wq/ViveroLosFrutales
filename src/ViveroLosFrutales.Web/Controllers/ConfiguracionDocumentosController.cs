using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class ConfiguracionDocumentosController(DocumentoConfiguracionService service) : Controller
{
    public async Task<IActionResult> Index(string? tipoDocumento, CancellationToken cancellationToken)
    {
        var tipo = FormularioConfiguracionService.NormalizarTipoDocumento(tipoDocumento);
        var dto = await service.ObtenerAsync(tipo, cancellationToken);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(DocumentoConfiguracionEditDto dto, CancellationToken cancellationToken)
    {
        dto.TipoDocumento = FormularioConfiguracionService.NormalizarTipoDocumento(dto.TipoDocumento);
        try
        {
            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = $"Configuracion de {FormularioConfiguracionService.NombreDocumento(dto.TipoDocumento).ToLowerInvariant()} guardada correctamente.";
            return RedirectToAction(nameof(Index), new { tipoDocumento = dto.TipoDocumento });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(dto);
        }
    }
}
