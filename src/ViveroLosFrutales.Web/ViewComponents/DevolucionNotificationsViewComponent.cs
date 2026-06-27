using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;

namespace ViveroLosFrutales.Web.ViewComponents;

public class DevolucionNotificationsViewComponent(DevolucionService service, IMemoryCache cache) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
    {
        var empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 0;
        var cacheKey = $"devolucion-alertas:{empresaId}";
        var alertas = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            entry.Size = 1;
            return await service.ObtenerAlertasAsync(5, cancellationToken);
        }) ?? new DevolucionAlertasDto();

        return View(alertas);
    }
}