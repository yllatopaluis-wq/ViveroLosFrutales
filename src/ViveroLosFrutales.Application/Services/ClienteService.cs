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
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<ClienteEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var cliente = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);
        return cliente is null ? null : ToDto(cliente);
    }

    public async Task GuardarAsync(ClienteEditDto dto, CancellationToken cancellationToken)
    {
        Validar(dto);
        var esNuevo = dto.ClienteId == 0;
        var cliente = esNuevo ? new Cliente { EmpresaId = empresaContext.EmpresaId } : await repository.ObtenerAsync(empresaContext.EmpresaId, dto.ClienteId, cancellationToken);
        if (cliente is null) throw new InvalidOperationException("Cliente no encontrado.");

        if (await repository.ExisteDocumentoAsync(empresaContext.EmpresaId, dto.TipoDocumento, dto.NumeroDocumento.Trim(), esNuevo ? null : dto.ClienteId, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un cliente con el mismo tipo y numero de documento en esta empresa.");
        }

        cliente.EmpresaId = empresaContext.EmpresaId;
        cliente.TipoDocumento = dto.TipoDocumento;
        cliente.NumeroDocumento = dto.NumeroDocumento.Trim();
        cliente.NombreCompleto = dto.NombreCompleto.Trim();
        cliente.Email = dto.Email?.Trim() ?? string.Empty;
        cliente.Direccion = dto.Direccion.Trim();
        cliente.Telefono = dto.Telefono?.Trim() ?? string.Empty;
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
        var cliente = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken) ?? throw new InvalidOperationException("Cliente no encontrado.");
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
