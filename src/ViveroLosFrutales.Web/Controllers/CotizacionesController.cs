using System.IO;
using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Web.Controllers;

public class CotizacionesController(CotizacionService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await service.CrearFormularioInicialAsync(cancellationToken));

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

    public async Task<IActionResult> BuscarProductos(string? search, CancellationToken cancellationToken)
    {
        var productos = await service.BuscarProductosAsync(search, cancellationToken);
        return Json(productos.Select(x => new
        {
            id = x.ProductoId,
            nombre = x.Nombre,
            categoria = x.Categoria,
            codigo = $"PROD-{x.ProductoId:D6}",
            sku = string.Empty,
            codigoBarras = string.Empty,
            unidad = x.UnidadMedida,
            precio = x.PrecioVentaConIgv,
            stock = x.Stock,
            afectoIgv = x.AfectoIgv,
            texto = string.IsNullOrWhiteSpace(x.Categoria) ? x.Nombre : $"{x.Nombre} ({x.Categoria})"
        }));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await service.ObtenerParaEditarAsync(id, cancellationToken);
            return View("Create", await service.ObtenerFormularioAsync(dto, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        try
        {
            return View(await service.ObtenerDetalleAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CotizacionEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = $"Cotizacion {result.Serie}-{result.Correlativo:000000} guardada.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View(await service.ObtenerFormularioAsync(dto, cancellationToken));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Edit(CotizacionEditDto dto, CancellationToken cancellationToken) =>
        Create(dto, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Imprimir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.ImprimirAsync(id, cancellationToken);
            if (!string.IsNullOrWhiteSpace(result.PdfUrl))
            {
                if (Path.IsPathRooted(result.PdfUrl) && System.IO.File.Exists(result.PdfUrl))
                    return PhysicalFile(result.PdfUrl, "application/pdf", Path.GetFileName(result.PdfUrl));
                return Redirect(result.PdfUrl);
            }
            TempData["Error"] = "La cotizacion no tiene PDF disponible.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Convertir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var notaId = await service.ConvertirANotaPedidoAsync(id, cancellationToken);
            TempData["Success"] = "Cotizacion convertida a nota de pedido.";
            return RedirectToAction("Details", "NotasPedido", new { id = notaId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, CancellationToken cancellationToken)
    {
        try
        {
            await service.CambiarEstadoAsync(id, CotizacionEstado.ANULADA, cancellationToken);
            TempData["Success"] = "Cotizacion anulada.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }
        return RedirectToAction(nameof(Index));
    }
}


