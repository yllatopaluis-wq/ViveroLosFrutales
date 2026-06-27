using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.PublicWeb.Services;

namespace ViveroLosFrutales.PublicWeb.Controllers;

public class HomeController(PublicContentService contentService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => View(await contentService.ObtenerPaginaAsync(6, cancellationToken));

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
