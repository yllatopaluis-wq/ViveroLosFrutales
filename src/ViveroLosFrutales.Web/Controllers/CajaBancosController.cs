using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Web.Controllers;

public class CajaBancosController(CuentaFinancieraService service) : Controller
{
    public async Task<IActionResult> Index([FromQuery] SearchRequest request, TipoCuentaFinanciera? tipo, CancellationToken cancellationToken)
    {
        ViewBag.Tipo = tipo;
        return View(await service.ObtenerCajaBancosAsync(request.FechaDesde, request.FechaHasta, tipo, request.Search, cancellationToken));
    }
}
