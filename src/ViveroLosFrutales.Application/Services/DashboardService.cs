using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public class DashboardService(IDashboardRepository dashboardRepository, IEmpresaContext empresaContext)
{
    public Task<DashboardDto> ObtenerMesActualAsync(CancellationToken cancellationToken)
    {
        var hoy = PeruDateTime.Today;
        var fechaDesde = new DateTime(hoy.Year, hoy.Month, 1);
        var fechaHasta = fechaDesde.AddMonths(1).AddDays(-1);

        return dashboardRepository.ObtenerAsync(empresaContext.EmpresaId, fechaDesde, fechaHasta, cancellationToken);
    }
}
