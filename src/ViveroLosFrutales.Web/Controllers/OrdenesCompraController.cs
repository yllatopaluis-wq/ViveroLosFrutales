using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class OrdenesCompraController(OrdenCompraService service, PagoProveedorAplicacionService aplicacionService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await service.NuevoAsync(cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrdenCompraFormDataDto model, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(model.OrdenCompra, cancellationToken);
            TempData["Success"] = "Orden de compra registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            var form = await service.NuevoAsync(cancellationToken);
            form.OrdenCompra = model.OrdenCompra;
            return View(form);
        }
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken) =>
        View(await service.ObtenerDetalleAsync(id, cancellationToken));

    public async Task<IActionResult> RegistrarPago(int id, CancellationToken cancellationToken) =>
        View(await service.ObtenerFormularioPagoAsync(id, cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPago(RegistrarPagoOrdenCompraDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.RegistrarPagoAsync(dto, cancellationToken);
            TempData["Success"] = "Pago adelantado registrado correctamente.";
            return RedirectToAction(nameof(Details), new { id = dto.OrdenCompraId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(dto);
        }
    }

    public async Task<IActionResult> RegistrarCompra(int id, CancellationToken cancellationToken) =>
        View("~/Views/Compras/Create.cshtml", await service.PrepararCompraAsync(id, cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AplicarAutomaticamente(int compraId, int ordenCompraId, CancellationToken cancellationToken)
    {
        try
        {
            await aplicacionService.AplicarAutomaticamenteAsync(compraId, cancellationToken);
            TempData["Success"] = "Pagos aplicados correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }
        return RedirectToAction(nameof(Details), new { id = ordenCompraId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar(int id, bool solicitarDevolucion, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            await service.CerrarAsync(id, solicitarDevolucion, motivo ?? string.Empty, cancellationToken);
            TempData["Success"] = "Orden de compra cerrada correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            await service.AnularAsync(id, motivo, cancellationToken);
            TempData["Success"] = "Orden de compra anulada correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }
        return RedirectToAction(nameof(Index));
    }
}

