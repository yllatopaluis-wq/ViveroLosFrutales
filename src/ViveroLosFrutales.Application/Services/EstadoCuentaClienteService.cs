using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public class EstadoCuentaClienteService(
    IEstadoCuentaClienteRepository repository,
    IClienteRepository clienteRepository,
    IEmpresaContext empresaContext)
{
    public async Task<IReadOnlyList<ClienteListDto>> BuscarClientesAsync(string? search, CancellationToken cancellationToken)
    {
        var result = await clienteRepository.BuscarAsync(new SearchRequest { Search = search, PageSize = 100 }, cancellationToken);
        return result.Items;
    }

    public Task<EstadoCuentaClienteDto?> ObtenerAsync(int clienteId, CancellationToken cancellationToken) =>
        repository.ObtenerAsync(empresaContext.EmpresaId, clienteId, cancellationToken);
}
