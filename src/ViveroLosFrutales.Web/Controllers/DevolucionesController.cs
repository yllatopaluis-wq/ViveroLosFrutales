using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Web.Controllers;

public class DevolucionesController(DevolucionService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken) =>
        View(await service.ObtenerDetalleAsync(id, cancellationToken));

    public async Task<IActionResult> Registrar(int id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await service.ObtenerFormularioRegistroAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistrarDevolucionDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.RegistrarDevolucionAsync(dto, cancellationToken);
            TempData["Success"] = dto.TipoTercero == TipoTerceroDevolucion.CLIENTE
                ? "Devolucion registrada correctamente. Se genero el egreso en caja."
                : "Devolucion registrada correctamente. Se genero el ingreso en caja.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            try
            {
                var form = await service.ObtenerFormularioRegistroAsync(dto.DevolucionId, cancellationToken);
                form.MontoDevolver = dto.MontoDevolver;
                form.MedioDevolucion = dto.MedioDevolucion;
                form.CuentaFinancieraId = dto.CuentaFinancieraId;
                form.Fecha = dto.Fecha;
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
    public IActionResult Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        TempData["Error"] = "No se puede anular una solicitud de devolucion. Registre la devolucion para mantener el sustento del dinero pendiente.";
        return RedirectToAction(nameof(Index));
    }
}

