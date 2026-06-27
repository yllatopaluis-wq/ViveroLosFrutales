using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.PublicWeb.Services;

namespace ViveroLosFrutales.PublicWeb.Controllers;

public class ProductosController(PublicContentService contentService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => View(await contentService.ObtenerPaginaAsync(24, cancellationToken));
}
