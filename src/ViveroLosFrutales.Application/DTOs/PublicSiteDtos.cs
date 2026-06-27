namespace ViveroLosFrutales.Application.DTOs;

public record PublicEmpresaDto(
    string RazonSocial,
    string NombreComercial,
    string Ruc,
    string Direccion,
    string Telefono,
    string Email);

public record PublicProductoDto(
    int ProductoId,
    string Nombre,
    string Categoria,
    string UnidadMedida,
    decimal PrecioReferencial);

public class PublicCatalogDto
{
    public PublicEmpresaDto? Empresa { get; set; }
    public IReadOnlyList<PublicProductoDto> Productos { get; set; } = Array.Empty<PublicProductoDto>();
}
