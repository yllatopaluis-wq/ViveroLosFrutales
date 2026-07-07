using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public class ReporteGeneralService(IReporteRepository repository, IEmpresaContext empresaContext)
{
    public Task<ReporteGeneralDto> ObtenerAsync(int? anioDesde, int? anioHasta, string? indicador, CancellationToken cancellationToken)
    {
        var actual = PeruDateTime.Today.Year;
        var desde = Math.Clamp(anioDesde ?? actual, 2000, actual + 1);
        var hasta = Math.Clamp(anioHasta ?? actual, 2000, actual + 1);
        if (desde > hasta) (desde, hasta) = (hasta, desde);
        if (hasta - desde > 9) desde = hasta - 9;

        var indicadorValido = ReporteGeneralIndicadores.Todos
            .FirstOrDefault(x => x.Equals(indicador, StringComparison.OrdinalIgnoreCase))
            ?? ReporteGeneralIndicadores.Resultado;

        return repository.ObtenerGeneralAsync(empresaContext.EmpresaId, desde, hasta, indicadorValido, cancellationToken);
    }
    public Task<ReporteNotasPedidoDto> ObtenerNotasPedidoAsync(ReporteNotasPedidoRequest request, CancellationToken cancellationToken) =>
        repository.ObtenerNotasPedidoAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<ReporteComprobantesDto> ObtenerComprobantesAsync(ReporteComprobantesRequest request, CancellationToken cancellationToken) =>
        repository.ObtenerComprobantesAsync(empresaContext.EmpresaId, request, cancellationToken);
}



