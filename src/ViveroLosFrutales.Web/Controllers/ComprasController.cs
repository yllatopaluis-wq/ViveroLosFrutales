using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class ComprasController(CompraService service, PagoProveedorAplicacionService aplicacionService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await service.NuevoAsync(cancellationToken));

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken) =>
        View(await service.ObtenerDetalleAsync(id, cancellationToken));

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await service.ObtenerCamposEditablesAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CompraCamposEditablesDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.ActualizarCamposEditablesAsync(dto, cancellationToken);
            TempData["Success"] = "Compra actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            try
            {
                var form = await service.ObtenerCamposEditablesAsync(dto.CompraId, cancellationToken);
                form.TipoDocumento = dto.TipoDocumento;
                form.Serie = dto.Serie;
                form.Numero = dto.Numero;
                form.FormaPago = dto.FormaPago;
                form.DiasCredito = dto.DiasCredito;
                form.EstadoEntrega = dto.EstadoEntrega;
                return View(form);
            }
            catch
            {
                return View(dto);
            }
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompraFormDataDto model, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(model.Compra, cancellationToken);
            TempData["Success"] = "Compra registrada correctamente. El stock fue actualizado.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            var form = await service.NuevoAsync(cancellationToken);
            form.Compra = model.Compra;
            return View(form);
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
                form.CuentaFinancieraId = dto.CuentaFinancieraId;
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


    public async Task<IActionResult> AplicarPagos(int id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await aplicacionService.ObtenerFormularioAplicacionAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AplicarPagos(AplicarPagoProveedorFormDto model, CancellationToken cancellationToken)
    {
        try
        {
            await aplicacionService.AplicarAsync(model, cancellationToken);
            TempData["Success"] = "Pagos aplicados correctamente.";
            return RedirectToAction(nameof(Details), new { id = model.CompraId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            var form = await aplicacionService.ObtenerFormularioAplicacionAsync(model.CompraId, cancellationToken);
            for (var i = 0; i < form.Pagos.Count && i < model.Pagos.Count; i++)
            {
                form.Pagos[i].MontoAplicar = model.Pagos[i].MontoAplicar;
            }
            return View(form);
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevertirAplicaciones(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            await service.RevertirAplicacionesPagoCompraAsync(id, motivo, cancellationToken);
            TempData["Success"] = "Aplicaciones de pago revertidas. El pago proveedor quedo disponible para otra compra.";
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

