using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class EstadoCuentaClientesController(EstadoCuentaClienteService service) : Controller
{
    public async Task<IActionResult> Index(int? clienteId, string? search, CancellationToken cancellationToken)
    {
        ViewData["Clientes"] = await service.BuscarClientesAsync(search, cancellationToken);
        ViewData["Search"] = search;

        if (clienteId is null) return View();

        var estadoCuenta = await service.ObtenerAsync(clienteId.Value, cancellationToken);
        if (estadoCuenta is null)
        {
            TempData["Error"] = "Cliente no encontrado.";
            return View();
        }

        return View(estadoCuenta);
    }
}
