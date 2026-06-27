using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public class PublicCatalogService(IPublicCatalogRepository repository)
{
    public Task<PublicCatalogDto> ObtenerAsync(string empresaRuc, int cantidadProductos, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(empresaRuc)) throw new InvalidOperationException("Configure el RUC de la empresa para la web publica.");
        return repository.ObtenerAsync(empresaRuc.Trim(), Math.Clamp(cantidadProductos, 1, 24), cancellationToken);
    }
}
