using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class NotasCreditoController(ComprobanteService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarNotasCreditoAsync(request, cancellationToken));

    public async Task<IActionResult> Create([FromQuery] NotaCreditoOrigenSearchRequest request, int? comprobanteReferenciaId, CancellationToken cancellationToken)
    {
        var model = new NotaCreditoCreatePageDto
        {
            NotaCredito = await service.PrepararNotaCreditoInicialAsync(cancellationToken)
        };
        if (comprobanteReferenciaId is int id)
        {
            try
            {
                model.NotaCredito = await service.PrepararNotaCreditoAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
                model.Origenes = await service.BuscarOrigenesNotaCreditoAsync(request, cancellationToken);
            }
        }
        else
        {
            model.Origenes = await service.BuscarOrigenesNotaCreditoAsync(request, cancellationToken);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NotaCreditoEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.EmitirNotaCreditoAsync(dto, cancellationToken);
            TempData["Success"] = string.IsNullOrWhiteSpace(result.Mensaje)
                ? $"Nota de credito {result.Serie}-{result.Correlativo:000000} emitida. Total S/ {result.Total:N2}."
                : $"Nota de credito {result.Serie}-{result.Correlativo:000000} emitida. Total S/ {result.Total:N2}. {result.Mensaje}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(new NotaCreditoCreatePageDto
            {
                NotaCredito = dto.ComprobanteReferenciaId > 0
                    ? await service.PrepararNotaCreditoAsync(dto.ComprobanteReferenciaId, cancellationToken)
                    : await service.PrepararNotaCreditoInicialAsync(cancellationToken)
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Imprimir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.ImprimirComprobanteAsync(id, cancellationToken);
            if (!string.IsNullOrWhiteSpace(result.PdfUrl))
            {
                if (Path.IsPathRooted(result.PdfUrl) && System.IO.File.Exists(result.PdfUrl))
                {
                    return PhysicalFile(result.PdfUrl, "application/pdf", Path.GetFileName(result.PdfUrl));
                }

                return Redirect(result.PdfUrl);
            }

            return Content(string.IsNullOrWhiteSpace(result.Mensaje)
                ? "La nota de credito no tiene PDF disponible."
                : $"La nota de credito no tiene PDF disponible. {result.Mensaje}", "text/plain");
        }
        catch (Exception ex)
        {
            return Content(ErrorMessageHelper.ToSpanish(ex), "text/plain");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.AnularComprobanteAsync(id, motivo, cancellationToken);
            TempData["Success"] = string.IsNullOrWhiteSpace(result.Mensaje) ? "Nota de credito anulada." : result.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }
}
