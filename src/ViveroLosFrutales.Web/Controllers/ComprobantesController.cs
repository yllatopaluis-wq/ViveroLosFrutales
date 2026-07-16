using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;
using ViveroLosFrutales.Web.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class ComprobantesController(
    ComprobanteService service,
    CobroClienteService cobroClienteService,
    SunatPendingSyncService sunatPendingSyncService) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await service.ObtenerFormularioAsync(new ComprobanteEditDto(), cancellationToken));

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

    public IActionResult Visualizar(int id) =>
        RedirectToAction(nameof(Details), new { id });

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerComprobanteParaVisualizarAsync(id, cancellationToken);
        ViewData["ReadOnly"] = true;
        return View(await service.ObtenerFormularioLecturaAsync(dto, cancellationToken));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await service.ObtenerComprobanteParaEditarAsync(id, cancellationToken);
            return View("Create", await service.ObtenerFormularioAsync(dto, cancellationToken));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> RegistrarPago(int id, CancellationToken cancellationToken)
    {
        var dto = await service.ObtenerComprobanteParaVisualizarAsync(id, cancellationToken);
        if (dto.TipoComprobante == TipoComprobante.NCR)
        {
            TempData["Error"] = "No se registran pagos sobre notas de credito.";
            return RedirectToAction(nameof(Index));
        }

        if (dto.EstadoPago == EstadoPagoComprobante.PAGADO || dto.SaldoPendiente <= 0)
        {
            TempData["Error"] = "El comprobante ya se encuentra pagado.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Referencia"] = $"{dto.TipoComprobante}-{dto.Serie}-{dto.Correlativo:000000}";
        ViewData["SaldoPendiente"] = dto.SaldoPendiente;
        return View(await cobroClienteService.PrepararFormularioAsync(new RegistrarCobroDto { ComprobanteId = id, FechaCobro = PeruDateTime.Today, Monto = dto.SaldoPendiente, MedioPago = "EFECTIVO" }, cancellationToken));
    }

    public async Task<IActionResult> Numeracion(TipoComprobante tipoComprobante, CancellationToken cancellationToken) =>
        Json(await service.ObtenerNumeracionAsync(tipoComprobante, cancellationToken));

    public IActionResult NotaCredito(int id)
    {
        return RedirectToAction("Create", "NotasCredito", new { comprobanteReferenciaId = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ComprobanteEditDto dto, string submit, CancellationToken cancellationToken)
    {
        try
        {
            if (dto.TipoComprobante is TipoComprobante.COT or TipoComprobante.NPE or TipoComprobante.NCR)
            {
                throw new InvalidOperationException("Desde este formulario solo se registran boletas y facturas.");
            }

            var result = await service.GuardarAsync(dto, imprimir: false, cancellationToken);
            TempData["Success"] = $"Comprobante {result.Serie}-{result.Correlativo:000000} guardado. Total S/ {result.Total:N2}.";
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
    public IActionResult NotaCredito(NotaCreditoEditDto dto)
    {
        return RedirectToAction("Create", "NotasCredito", new { comprobanteReferenciaId = dto.ComprobanteReferenciaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPago(RegistrarCobroDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await cobroClienteService.RegistrarAsync(dto, cancellationToken);
            TempData["Success"] = "Pago registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ErrorMessageHelper.ToSpanish(ex));
            ViewData["Referencia"] = $"Comprobante #{dto.ComprobanteId}";
            if (dto.ComprobanteId is int comprobanteId)
            {
                var comprobante = await service.ObtenerComprobanteParaVisualizarAsync(comprobanteId, cancellationToken);
                ViewData["SaldoPendiente"] = comprobante.SaldoPendiente;
            }
            return View(await cobroClienteService.PrepararFormularioAsync(dto, cancellationToken));
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

                return AbrirPdfYActualizarListado(result.PdfUrl);
            }

            return Content(string.IsNullOrWhiteSpace(result.Mensaje)
                ? "El comprobante no tiene PDF disponible."
                : $"El comprobante no tiene PDF disponible. {result.Mensaje}", "text/plain");
        }
        catch (Exception ex)
        {
            return Content(ErrorMessageHelper.ToSpanish(ex), "text/plain");
        }
    }


    private ContentResult AbrirPdfYActualizarListado(string pdfUrl)
    {
        var pdfJson = JsonSerializer.Serialize(pdfUrl);
        var messageJson = JsonSerializer.Serialize(new { type = "comprobante-impreso" });
        var html = $$"""
<!doctype html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <title>Abriendo comprobante</title>
</head>
<body>
    <p>Abriendo comprobante...</p>
    <script>
        try {
            if (window.opener && !window.opener.closed) {
                window.opener.postMessage({{messageJson}}, window.location.origin);
            }
        } catch (e) { }
        window.location.replace({{pdfJson}});
    </script>
</body>
</html>
""";
        return Content(html, "text/html; charset=utf-8");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConsultarSunat(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.ConsultarEstadoSunatAsync(id, cancellationToken);
            TempData["Success"] = $"Estado SUNAT actualizado: {result.EstadoSunat}.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult SincronizarPendientes()
    {
        var empresaId = HttpContext.Session.GetInt32("EmpresaId");
        if (empresaId is null)
        {
            return Unauthorized();
        }

        sunatPendingSyncService.Start(empresaId.Value);
        return Accepted();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anular(int id, string motivo, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.AnularComprobanteAsync(id, motivo, cancellationToken);
            TempData["Success"] = result.Mensaje;
        }
        catch (Exception ex)
        {
            TempData["Error"] = ErrorMessageHelper.ToSpanish(ex);
        }

        return RedirectToAction(nameof(Index));
    }
}





