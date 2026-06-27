using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class NubefactLogsController(NubefactLogService service) : Controller
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public async Task<IActionResult> Index([FromQuery] SearchRequest request, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, cancellationToken));

    public async Task<IActionResult> Descargar(int id, CancellationToken cancellationToken)
    {
        var log = await service.ObtenerAsync(id, cancellationToken);
        if (log is null) return NotFound();

        var contenido = new
        {
            log.NubefactOperacionId,
            log.NumeroComprobante,
            log.TipoOperacion,
            log.EstadoSunat,
            log.PdfUrl,
            log.XmlUrl,
            log.Hash,
            FechaRegistro = log.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"),
            request = ParseJsonOrRaw(log.SolicitudJson),
            response = ParseJsonOrRaw(log.RespuestaCompleta)
        };

        var json = JsonSerializer.Serialize(contenido, JsonOptions);
        var fileName = $"nubefact-{Sanitize(log.NumeroComprobante)}-{log.NubefactOperacionId}.txt";
        return File(Encoding.UTF8.GetBytes(json), "text/plain; charset=utf-8", fileName);
    }

    private static object? ParseJsonOrRaw(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        try
        {
            return JsonSerializer.Deserialize<JsonElement>(value);
        }
        catch
        {
            return value;
        }
    }

    private static string Sanitize(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new string(value.Select(ch => invalid.Contains(ch) ? '-' : ch).ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "log" : sanitized;
    }
}