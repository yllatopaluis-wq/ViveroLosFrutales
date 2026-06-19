using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class ComprasController(CompraService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await service.NuevoAsync(cancellationToken));

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken) =>
        View(await service.ObtenerDetalleAsync(id, cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompraEditDto compra, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(compra, cancellationToken);
            TempData["Success"] = "Compra registrada correctamente. El stock fue actualizado.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            var model = await service.NuevoAsync(cancellationToken);
            model.Compra = compra;
            return View(model);
        }
    }

    public async Task<IActionResult> RegistrarPago(int id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await service.ObtenerFormularioPagoAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPago(RegistrarPagoProveedorDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.RegistrarPagoProveedorAsync(dto, cancellationToken);
            TempData["Success"] = "Pago registrado correctamente. Se genero el egreso en caja.";
            return RedirectToAction(nameof(Details), new { id = dto.CompraId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            try
            {
                var form = await service.ObtenerFormularioPagoAsync(dto.CompraId, cancellationToken);
                form.MontoPago = dto.MontoPago;
                form.FechaPago = dto.FechaPago;
                form.MedioPago = dto.MedioPago;
                form.Observacion = dto.Observacion;
                return View(form);
            }
            catch
            {
                TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
                return RedirectToAction(nameof(Index));
            }
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.AnularCompraAsync(id, motivo, cancellationToken);
            TempData["Success"] = result.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnularPago(int id, int compraId, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            await service.AnularPagoProveedorAsync(id, motivo, cancellationToken);
            TempData["Success"] = "Pago proveedor anulado. Se recalculo el saldo de la compra.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Details), new { id = compraId });
    }
}
