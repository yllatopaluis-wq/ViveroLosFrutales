using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class CategoriaService(ICategoriaRepository repository, IEmpresaContext empresaContext)
{
    public Task<PagedResult<CategoriaListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<IReadOnlyList<CategoriaListDto>> ListarActivasAsync(CancellationToken cancellationToken) =>
        repository.ListarActivasAsync(empresaContext.EmpresaId, cancellationToken);

    public async Task<CategoriaEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var categoria = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);
        return categoria is null ? null : ToDto(categoria);
    }

    public async Task GuardarAsync(CategoriaEditDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new InvalidOperationException("El nombre de la categoria es obligatorio.");

        var categoria = dto.CategoriaId == 0
            ? new Categoria { EmpresaId = empresaContext.EmpresaId }
            : await repository.ObtenerAsync(empresaContext.EmpresaId, dto.CategoriaId, cancellationToken);

        if (categoria is null) throw new InvalidOperationException("Categoria no encontrada.");

        categoria.Nombre = dto.Nombre.Trim();
        categoria.Descripcion = dto.Descripcion?.Trim() ?? string.Empty;
        categoria.Estado = dto.Estado;
        categoria.UsuarioRegistro = empresaContext.UsuarioNombre;

        await repository.GuardarAsync(categoria, cancellationToken);
    }

    public async Task AnularAsync(int id, CancellationToken cancellationToken)
    {
        var categoria = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken) ?? throw new InvalidOperationException("Categoria no encontrada.");
        categoria.Estado = EstadoRegistro.Anulado;
        await repository.GuardarAsync(categoria, cancellationToken);
    }

    private static CategoriaEditDto ToDto(Categoria categoria) => new()
    {
        CategoriaId = categoria.CategoriaId,
        Nombre = categoria.Nombre,
        Descripcion = categoria.Descripcion,
        Estado = categoria.Estado
    };
}
