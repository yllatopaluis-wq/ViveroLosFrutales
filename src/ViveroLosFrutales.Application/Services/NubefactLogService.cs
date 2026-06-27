using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public class NubefactLogService(INubefactOperacionRepository repository, IEmpresaContext empresaContext)
{
    public Task<PagedResult<NubefactOperacionDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<NubefactOperacionDto?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);
}