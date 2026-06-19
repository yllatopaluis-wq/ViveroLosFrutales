using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class MovimientoCajaService(IMovimientoCajaRepository repository, IEmpresaContext empresaContext)
{
    public Task<CajaIndexDto> BuscarAsync(SearchRequest request, string? medioPago, TipoMovimientoCaja? tipoMovimiento, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, medioPago, tipoMovimiento, cancellationToken);
}
