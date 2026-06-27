using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.PublicWeb.Models;

namespace ViveroLosFrutales.PublicWeb.Services;

public class PublicContentService(
    PublicCatalogService catalogService,
    IConfiguration configuration,
    ILogger<PublicContentService> logger)
{
    public async Task<PublicPageViewModel> ObtenerPaginaAsync(int cantidadProductos, CancellationToken cancellationToken)
    {
        var fallback = CrearFallback(cantidadProductos);
        try
        {
            var ruc = configuration["PublicSite:EmpresaRuc"] ?? string.Empty;
            var catalogo = await catalogService.ObtenerAsync(ruc, cantidadProductos, cancellationToken);
            return new PublicPageViewModel
            {
                Empresa = catalogo.Empresa ?? fallback.Empresa,
                Productos = catalogo.Productos.Count > 0 ? catalogo.Productos : fallback.Productos
            };
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "No se pudo cargar el catalogo publico; se usara contenido informativo.");
            return fallback;
        }
    }

    public async Task<ContactoViewModel> ObtenerContactoAsync(CancellationToken cancellationToken)
    {
        var pagina = await ObtenerPaginaAsync(1, cancellationToken);
        return new ContactoViewModel { Empresa = pagina.Empresa };
    }

    private PublicPageViewModel CrearFallback(int cantidad)
    {
        var empresa = new PublicEmpresaDto(
            configuration["PublicSite:Nombre"] ?? "Vivero Los Frutales",
            configuration["PublicSite:Nombre"] ?? "Vivero Los Frutales",
            configuration["PublicSite:EmpresaRuc"] ?? string.Empty,
            configuration["PublicSite:Direccion"] ?? "Huaral, Lima, Peru",
            configuration["PublicSite:Telefono"] ?? string.Empty,
            configuration["PublicSite:Email"] ?? string.Empty);

        var productos = new[]
        {
            new PublicProductoDto(0, "Plantones de palta", "Frutales", "UNIDAD", 0),
            new PublicProductoDto(0, "Plantones de cítricos", "Frutales", "UNIDAD", 0),
            new PublicProductoDto(0, "Mango y frutales tropicales", "Frutales", "UNIDAD", 0),
            new PublicProductoDto(0, "Plantas ornamentales", "Ornamentales", "UNIDAD", 0),
            new PublicProductoDto(0, "Árboles para jardín", "Ornamentales", "UNIDAD", 0),
            new PublicProductoDto(0, "Abonos y sustratos", "Insumos", "UNIDAD", 0),
            new PublicProductoDto(0, "Materiales de riego", "Riego", "UNIDAD", 0),
            new PublicProductoDto(0, "Productos para manejo agrícola", "Insumos", "UNIDAD", 0)
        };

        return new PublicPageViewModel { Empresa = empresa, Productos = productos.Take(cantidad).ToArray() };
    }
}
