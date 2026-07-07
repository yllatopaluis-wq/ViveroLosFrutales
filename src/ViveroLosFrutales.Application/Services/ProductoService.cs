using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class ProductoService(IProductoRepository repository, IEmpresaContext empresaContext)
{
    public Task<PagedResult<ProductoListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<ProductoEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var producto = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);
        return producto is null ? null : ToDto(producto);
    }

    public async Task GuardarAsync(ProductoEditDto dto, CancellationToken cancellationToken)
    {
        Validar(dto);
        var producto = dto.ProductoId == 0 ? new Producto { EmpresaId = empresaContext.EmpresaId } : await repository.ObtenerAsync(empresaContext.EmpresaId, dto.ProductoId, cancellationToken);
        if (producto is null) throw new InvalidOperationException("Producto no encontrado.");

        producto.Categoria = dto.Categoria.Trim();
        producto.Nombre = dto.Nombre.Trim();
        producto.UnidadMedida = NormalizarUnidadMedida(dto.UnidadMedida);
        producto.Stock = EsCategoriaServicio(dto.Categoria) ? 0 : dto.Stock;
        producto.AfectoIgv = dto.AfectoIgv;
        producto.PrecioVentaConIgv = decimal.Round(dto.PrecioVentaConIgv, 2);
        producto.PrecioVentaSinIgv = CalcularPrecioSinIgv(producto.PrecioVentaConIgv, producto.AfectoIgv);
        producto.TieneDetraccion = dto.TieneDetraccion;
        producto.PorcentajeDetraccion = dto.TieneDetraccion ? dto.PorcentajeDetraccion : 0;
        producto.UsuarioRegistro = empresaContext.UsuarioNombre;
        producto.Estado = dto.Estado;

        await repository.GuardarAsync(producto, cancellationToken);
    }

    public async Task AnularAsync(int id, CancellationToken cancellationToken)
    {
        var producto = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken) ?? throw new InvalidOperationException("Producto no encontrado.");
        producto.Estado = EstadoRegistro.Anulado;
        await repository.GuardarAsync(producto, cancellationToken);
    }

    private static void Validar(ProductoEditDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Categoria)) throw new InvalidOperationException("Seleccione una categoria.");
        if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new InvalidOperationException("El nombre del producto es obligatorio.");
        if (dto.PrecioVentaConIgv < 0) throw new InvalidOperationException("El precio de venta no puede ser negativo.");
        if (!EsCategoriaServicio(dto.Categoria) && dto.Stock < 0) throw new InvalidOperationException("El stock no puede ser negativo.");
        if (dto.TieneDetraccion && dto.PorcentajeDetraccion <= 0) throw new InvalidOperationException("Ingrese el porcentaje de detraccion.");
    }

    private static bool EsCategoriaServicio(string? categoria) =>
        string.Equals(categoria?.Trim(), "SERVICIO", StringComparison.OrdinalIgnoreCase);

    private static decimal CalcularPrecioSinIgv(decimal precioVenta, bool afectoIgv) =>
        afectoIgv ? decimal.Round(precioVenta / 1.18m, 2) : decimal.Round(precioVenta, 2);

    private static string NormalizarUnidadMedida(string? unidad)
    {
        return unidad?.Trim().ToUpperInvariant() switch
        {
            null or "" => "NIU",
            "UNIDAD" or "UND" or "UNIDADES" => "NIU",
            "KILO" or "KILOS" or "KG" or "KILOGRAMO" or "KILOGRAMOS" => "KGM",
            "METRO" or "METROS" or "M" => "MTR",
            "LITRO" or "LITROS" or "LT" => "LTR",
            var valor => valor
        };
    }

    private static ProductoEditDto ToDto(Producto producto) => new()
    {
        ProductoId = producto.ProductoId,
        Categoria = producto.Categoria,
        Nombre = producto.Nombre,
        UnidadMedida = producto.UnidadMedida,
        Stock = producto.Stock,
        AfectoIgv = producto.AfectoIgv,
        PrecioVentaSinIgv = producto.PrecioVentaSinIgv,
        PrecioVentaConIgv = producto.PrecioVentaConIgv,
        TieneDetraccion = producto.TieneDetraccion,
        PorcentajeDetraccion = producto.PorcentajeDetraccion,
        Estado = producto.Estado
    };
}

