using System.IO;
using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Web.Controllers;

public class NotasPedidoController(NotaPedidoService service, CobroClienteService cobroClienteService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public IActionResult Create() => RedirectToAction(nameof(Nuevo));

    public async Task<IActionResult> Nuevo(CancellationToken cancellationToken) =>
        View("Create", await service.NuevoAsync(cancellationToken));

    public async Task<IActionResult> BuscarClientes(string? search, CancellationToken cancellationToken)
    {
        var clientes = await service.BuscarClientesAsync(search, cancellationToken);
        return Json(clientes.Select(x => new
        {
            id = x.ClienteId,
            nombre = x.NombreCompleto,
            documento = x.NumeroDocumento,
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
            return View("Create", await service.FormDataAsync(dto, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken) =>
        View(await service.ObtenerDetalleAsync(id, cancellationToken));

    public async Task<IActionResult> RegistrarCobro(int id, CancellationToken cancellationToken)
    {
        var nota = await service.ObtenerDetalleAsync(id, cancellationToken);
        ViewData["SaldoPendiente"] = nota.SaldoPendiente;
        ViewData["Referencia"] = nota.Numero;
        return View(await cobroClienteService.PrepararFormularioAsync(new RegistrarCobroDto { NotaPedidoId = id, Monto = nota.SaldoPendiente }, cancellationToken));
    }

    public IActionResult VerCobros(int id) =>
        RedirectToAction(nameof(Details), new { id });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nuevo(NotaPedidoEditDto dto, CancellationToken cancellationToken)
    {
        dto.NotaPedidoId = 0;
        return await GuardarFormularioAsync(dto, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Create(NotaPedidoEditDto dto, CancellationToken cancellationToken) =>
        Nuevo(dto, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Edit(NotaPedidoEditDto dto, CancellationToken cancellationToken) =>
        GuardarFormularioAsync(dto, cancellationToken);

    private async Task<IActionResult> GuardarFormularioAsync(NotaPedidoEditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.GuardarAsync(dto, cancellationToken);
            TempData["Success"] = "Nota de pedido guardada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            return View("Create", await service.FormDataAsync(dto, cancellationToken));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarCobro(RegistrarCobroDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await service.RegistrarCobroAsync(dto, cancellationToken);
            TempData["Success"] = "Cobro registrado correctamente.";
            return RedirectToAction(nameof(Details), new { id = dto.NotaPedidoId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            var notaId = dto.NotaPedidoId ?? 0;
            var nota = await service.ObtenerDetalleAsync(notaId, cancellationToken);
            ViewData["SaldoPendiente"] = nota.SaldoPendiente;
            ViewData["Referencia"] = nota.Numero;
            return View(await cobroClienteService.PrepararFormularioAsync(dto, cancellationToken));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConvertirAComprobante(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.ConvertirAsync(id, cancellationToken);
            TempData["Success"] = $"Nota convertida a comprobante {result.Serie}-{result.Correlativo:000000}.";
            return RedirectToAction("Index", "Comprobantes");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.AnularAsync(id, motivo, cancellationToken);
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
    public async Task<IActionResult> Imprimir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.ImprimirAsync(id, cancellationToken);
            if (!string.IsNullOrWhiteSpace(result.PdfUrl))
            {
                if (Path.IsPathRooted(result.PdfUrl) && System.IO.File.Exists(result.PdfUrl))
                {
                    return PhysicalFile(result.PdfUrl, "application/pdf", Path.GetFileName(result.PdfUrl));
                }

                return Redirect(result.PdfUrl);
            }

            TempData["Error"] = "La nota de pedido no tiene PDF disponible.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }
}

