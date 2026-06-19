using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Web.Controllers;

public class CajaController(MovimientoCajaService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, string? medioPago, TipoMovimientoCaja? tipoMovimiento, CancellationToken cancellationToken) =>
        View(await service.BuscarAsync(request, medioPago, tipoMovimiento, cancellationToken));

    public Task<IActionResult> ResumenDiario([FromQuery] SearchRequest request, string? medioPago, TipoMovimientoCaja? tipoMovimiento, CancellationToken cancellationToken) =>
        Index(request, medioPago, tipoMovimiento, cancellationToken);

    public Task<IActionResult> ResumenMensual([FromQuery] SearchRequest request, string? medioPago, TipoMovimientoCaja? tipoMovimiento, CancellationToken cancellationToken) =>
        Index(request, medioPago, tipoMovimiento, cancellationToken);
}
