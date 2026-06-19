using System.Text.RegularExpressions;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class ClienteService(IClienteRepository repository, IEmpresaContext empresaContext)
{
    public Task<PagedResult<ClienteListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(request, cancellationToken);

    public async Task<ClienteEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var cliente = await repository.ObtenerAsync(id, cancellationToken);
        return cliente is null ? null : ToDto(cliente);
    }

    public async Task GuardarAsync(ClienteEditDto dto, CancellationToken cancellationToken)
    {
        Validar(dto);
        var esNuevo = dto.ClienteId == 0;
        var cliente = esNuevo ? new Cliente() : await repository.ObtenerAsync(dto.ClienteId, cancellationToken);
        if (cliente is null) throw new InvalidOperationException("Cliente no encontrado.");

        cliente.TipoDocumento = dto.TipoDocumento;
        cliente.NumeroDocumento = dto.NumeroDocumento.Trim();
        cliente.NombreCompleto = dto.NombreCompleto.Trim();
        cliente.Email = dto.Email.Trim();
        cliente.Direccion = dto.Direccion.Trim();
        cliente.Telefono = dto.Telefono.Trim();
        cliente.Estado = dto.Estado;
        if (esNuevo)
        {
            cliente.UsuarioRegistro = empresaContext.UsuarioNombre;
        }
        else
        {
            cliente.FechaModificacion = DateTime.UtcNow;
            cliente.UsuarioModificacion = empresaContext.UsuarioNombre;
        }

        await repository.GuardarAsync(cliente, cancellationToken);
    }

    public async Task AnularAsync(int id, CancellationToken cancellationToken)
    {
        var cliente = await repository.ObtenerAsync(id, cancellationToken) ?? throw new InvalidOperationException("Cliente no encontrado.");
        cliente.Estado = EstadoRegistro.Anulado;
        cliente.FechaModificacion = DateTime.UtcNow;
        cliente.UsuarioModificacion = empresaContext.UsuarioNombre;
        await repository.GuardarAsync(cliente, cancellationToken);
    }

    private static void Validar(ClienteEditDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NombreCompleto)) throw new InvalidOperationException("El cliente es obligatorio.");
        if (dto.TipoDocumento == TipoDocumentoCliente.DNI && dto.NumeroDocumento.Trim().Length != 8) throw new InvalidOperationException("El DNI debe tener 8 digitos.");
        if (dto.TipoDocumento == TipoDocumentoCliente.RUC && dto.NumeroDocumento.Trim().Length != 11) throw new InvalidOperationException("El RUC debe tener 11 digitos.");
        if (!string.IsNullOrWhiteSpace(dto.Email) && !Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new InvalidOperationException("El email no tiene un formato valido.");
    }

    private static ClienteEditDto ToDto(Cliente cliente) => new()
    {
        ClienteId = cliente.ClienteId,
        TipoDocumento = cliente.TipoDocumento,
        NumeroDocumento = cliente.NumeroDocumento,
        NombreCompleto = cliente.NombreCompleto,
        Email = cliente.Email,
        Direccion = cliente.Direccion,
        Telefono = cliente.Telefono,
        Estado = cliente.Estado
    };
}
