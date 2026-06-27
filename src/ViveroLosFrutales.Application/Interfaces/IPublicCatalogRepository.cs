using ViveroLosFrutales.Application.DTOs;

namespace ViveroLosFrutales.Application.Interfaces;

public interface IPublicCatalogRepository
{
    Task<PublicCatalogDto> ObtenerAsync(string empresaRuc, int cantidadProductos, CancellationToken cancellationToken);
}
