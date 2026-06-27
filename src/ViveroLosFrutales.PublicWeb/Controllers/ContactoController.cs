using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.PublicWeb.Models;
using ViveroLosFrutales.PublicWeb.Services;

namespace ViveroLosFrutales.PublicWeb.Controllers;

public class ContactoController(PublicContentService contentService, ILogger<ContactoController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => View(await contentService.ObtenerContactoAsync(cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactoViewModel model, CancellationToken cancellationToken)
    {
        model.Empresa = (await contentService.ObtenerContactoAsync(cancellationToken)).Empresa;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        logger.LogInformation("Consulta pública recibida de {Nombre} ({Email}, {Telefono})", model.Nombre, model.Email, model.Telefono);
        TempData["ContactoEnviado"] = "Gracias por escribirnos. Nuestro equipo se comunicará contigo muy pronto.";
        return RedirectToAction(nameof(Index));
    }
}
