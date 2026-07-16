using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class GastosController(GastoService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> BuscarClientes(string? search, CancellationToken cancellationToken)
    {
        var clientes = await service.BuscarClientesAsync(search, cancellationToken);
        return Json(clientes.Select(x => new
        {
            id = x.ClienteId,
            nombre = x.NombreCompleto,
            documento = x.NumeroDocumento,
            telefono = x.Telefono,
            email = x.Email,
            direccion = x.Direccion,
            texto = $"{x.NombreCompleto} - {x.NumeroDocumento}"
        }));
    }
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewBag.CategoriasGasto = await service.ListarCategoriasAsync(cancellationToken);
        return View(await PrepararFormularioAsync(new GastoEditDto(), cancellationToken));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerAsync(id, cancellationToken);
        ViewBag.CategoriasGasto = await service.ListarCategoriasAsync(cancellationToken);
        return dto is null ? NotFound() : View("Create", await PrepararFormularioAsync(dto, cancellationToken));
    }

    public async Task<IActionResult> Visualizar(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerAsync(id, cancellationToken);
        if (dto is null) return NotFound();
        ViewData["ReadOnly"] = true;
        ViewBag.CategoriasGasto = await service.ListarCategoriasAsync(cancellationToken);
        return View("Create", await PrepararFormularioAsync(dto, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GastoEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = "Gasto guardado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            ViewBag.CategoriasGasto = await service.ListarCategoriasAsync(cancellationToken);
            return View(await PrepararFormularioAsync(dto, cancellationToken));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            await service.AnularAsync(id, motivo, cancellationToken);
            TempData["Success"] = "Gasto anulado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<GastoEditDto> PrepararFormularioAsync(GastoEditDto dto, CancellationToken cancellationToken)
    {
        dto.CuentasFinancieras = await service.ListarCuentasFinancierasAsync(cancellationToken);
        dto.Clientes = await service.ListarClientesAsync(cancellationToken);
        dto.FormularioConfiguracion = await service.ObtenerFormularioConfiguracionAsync(cancellationToken);
        return dto;
    }
}





