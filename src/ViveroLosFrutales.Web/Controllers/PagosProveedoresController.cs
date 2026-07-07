using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class PagosProveedoresController(CompraService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken)
    {
        request.Page = request.Page <= 0 ? 1 : request.Page;
        request.PageSize = request.PageSize <= 0 || request.PageSize > 20 ? 20 : request.PageSize;
        request.FechaDesde ??= new DateTime(PeruDateTime.Today.Year, PeruDateTime.Today.Month, 1);
        request.FechaHasta ??= PeruDateTime.Today;
        return View(await service.BuscarPagosProveedorAsync(request, cancellationToken));
    }

    public async Task<IActionResult> Pendientes([FromQuery] SearchRequest request, CancellationToken cancellationToken)
    {
        request.FechaDesde ??= new DateTime(PeruDateTime.Today.Year, PeruDateTime.Today.Month, 1);
        request.FechaHasta ??= PeruDateTime.Today;
        return View(await service.CuentasPorPagarAsync(request, cancellationToken));
    }

    public async Task<IActionResult> Registrar(int id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await service.ObtenerFormularioPagoAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Pendientes));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistrarPagoProveedorDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.RegistrarPagoProveedorAsync(dto, cancellationToken);
            TempData["Success"] = "Pago a proveedor registrado correctamente. Se genero el egreso en caja.";
            return RedirectToAction(nameof(Index));
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
                form.CuentaFinancieraId = dto.CuentaFinancieraId;
                form.Observacion = dto.Observacion;
                return View(form);
            }
            catch
            {
                TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
                return RedirectToAction(nameof(Pendientes));
            }
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            await service.AnularPagoProveedorAsync(id, motivo ?? string.Empty, cancellationToken);
            TempData["Success"] = "Pago proveedor anulado. Se recalculo el saldo de la compra.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }
}
