using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class ProveedorService(IProveedorRepository repository, IEmpresaContext empresaContext)
{
    public Task<PagedResult<ProveedorListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<IReadOnlyList<ProveedorListDto>> ListarActivosAsync(CancellationToken cancellationToken) =>
        repository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken);

    public async Task<ProveedorEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var proveedor = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);
        return proveedor is null ? null : ToDto(proveedor);
    }

    public async Task GuardarAsync(ProveedorEditDto dto, CancellationToken cancellationToken)
    {
        Validar(dto);
        var proveedor = dto.ProveedorId == 0
            ? new Proveedor { EmpresaId = empresaContext.EmpresaId }
            : await repository.ObtenerAsync(empresaContext.EmpresaId, dto.ProveedorId, cancellationToken);
        if (proveedor is null) throw new InvalidOperationException("Proveedor no encontrado.");

        proveedor.TipoDocumento = dto.TipoDocumento;
        proveedor.NumeroDocumento = dto.NumeroDocumento.Trim();
        proveedor.RazonSocial = dto.RazonSocial.Trim();
        proveedor.NombreComercial = dto.NombreComercial?.Trim() ?? string.Empty;
        proveedor.Direccion = dto.Direccion.Trim();
        proveedor.Telefono = dto.Telefono?.Trim() ?? string.Empty;
        proveedor.Email = dto.Email?.Trim() ?? string.Empty;
        proveedor.Estado = dto.Estado;
        proveedor.UsuarioRegistro = empresaContext.UsuarioNombre;

        await repository.GuardarAsync(proveedor, cancellationToken);
    }

    public async Task AnularAsync(int id, CancellationToken cancellationToken)
    {
        var proveedor = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken) ?? throw new InvalidOperationException("Proveedor no encontrado.");
        proveedor.Estado = EstadoRegistro.Anulado;
        await repository.GuardarAsync(proveedor, cancellationToken);
    }

    private static void Validar(ProveedorEditDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NumeroDocumento)) throw new InvalidOperationException("El numero de documento es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.RazonSocial)) throw new InvalidOperationException("La razon social es obligatoria.");
        if (dto.TipoDocumento == TipoDocumentoCliente.DNI && dto.NumeroDocumento.Trim().Length != 8) throw new InvalidOperationException("El DNI debe tener 8 digitos.");
        if (dto.TipoDocumento == TipoDocumentoCliente.RUC && dto.NumeroDocumento.Trim().Length != 11) throw new InvalidOperationException("El RUC debe tener 11 digitos.");
        if (!string.IsNullOrWhiteSpace(dto.Email) && !dto.Email.Contains('@')) throw new InvalidOperationException("Ingrese un correo valido.");
    }

    private static ProveedorEditDto ToDto(Proveedor proveedor) => new()
    {
        ProveedorId = proveedor.ProveedorId,
        TipoDocumento = proveedor.TipoDocumento,
        NumeroDocumento = proveedor.NumeroDocumento,
        RazonSocial = proveedor.RazonSocial,
        NombreComercial = proveedor.NombreComercial,
        Direccion = proveedor.Direccion,
        Telefono = proveedor.Telefono,
        Email = proveedor.Email,
        Estado = proveedor.Estado
    };
}
