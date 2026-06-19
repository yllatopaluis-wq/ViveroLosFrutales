using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Application.Interfaces;

public interface INubefactService
{
    Task<NubefactResponseDto> EmitirComprobanteAsync(Comprobante comprobante, CancellationToken cancellationToken);
    Task<NubefactResponseDto> ConsultarEstadoAsync(Comprobante comprobante, CancellationToken cancellationToken);
    Task<NubefactResponseDto> AnularComprobanteAsync(Comprobante comprobante, CancellationToken cancellationToken);
}

public interface IPdfService
{
    Task<string> GenerarComprobanteLocalAsync(Comprobante comprobante, CancellationToken cancellationToken);
    Task<string> GenerarCotizacionAsync(Cotizacion cotizacion, CancellationToken cancellationToken);
}
