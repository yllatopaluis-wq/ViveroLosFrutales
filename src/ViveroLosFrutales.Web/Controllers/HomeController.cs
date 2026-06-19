using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.Controllers;

public class HomeController(DashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboard = await dashboardService.ObtenerMesActualAsync(cancellationToken);
        return View(dashboard);
    }

    [AllowAnonymous]
    public IActionResult Error() => View();
}
