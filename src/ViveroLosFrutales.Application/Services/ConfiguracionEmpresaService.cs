using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class ConfiguracionEmpresaService(
    IConfiguracionEmpresaRepository repository,
    IComprobanteRepository comprobanteRepository,
    IEmpresaRepository empresaRepository,
    IEmpresaContext empresaContext)
{
    public Task<PagedResult<ConfiguracionEmpresaListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<ConfiguracionEmpresaEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);

    public async Task GuardarAsync(ConfiguracionEmpresaEditDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Clave)) throw new InvalidOperationException("La clave es obligatoria.");
        await repository.GuardarAsync(empresaContext.EmpresaId, empresaContext.UsuarioNombre, dto, cancellationToken);
    }

    public async Task<CorrelativosEmpresaDto> ObtenerCorrelativosAsync(CancellationToken cancellationToken)
    {
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");

        var items = new List<CorrelativoEmpresaDto>();
        foreach (var item in ObtenerSeries(empresa))
        {
            var siguiente = await comprobanteRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, item.Tipo, item.Serie, cancellationToken);
            items.Add(new CorrelativoEmpresaDto
            {
                TipoComprobante = item.Tipo,
                Nombre = item.Nombre,
                Serie = item.Serie,
                SiguienteCorrelativo = siguiente,
                Clave = CrearClave(item.Tipo, item.Serie)
            });
        }

        return new CorrelativosEmpresaDto { Items = items };
    }

    public async Task GuardarCorrelativosAsync(CorrelativosEmpresaDto dto, CancellationToken cancellationToken)
    {
        foreach (var item in dto.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Serie))
            {
                continue;
            }

            if (item.SiguienteCorrelativo <= 0)
            {
                throw new InvalidOperationException($"Ingrese un correlativo valido para {item.Nombre}.");
            }

            var clave = CrearClave(item.TipoComprobante, item.Serie.Trim());
            await repository.GuardarPorClaveAsync(
                empresaContext.EmpresaId,
                empresaContext.UsuarioNombre,
                clave,
                item.SiguienteCorrelativo.ToString(),
                $"Siguiente correlativo para {item.Nombre} {item.Serie.Trim()}",
                cancellationToken);
        }
    }

    private static IEnumerable<(TipoComprobante Tipo, string Nombre, string Serie)> ObtenerSeries(Empresa empresa)
    {
        yield return (TipoComprobante.BOL, "Boleta", empresa.SerieBoleta);
        yield return (TipoComprobante.FAC, "Factura", empresa.SerieFactura);
        yield return (TipoComprobante.NCR, "Nota credito factura", string.IsNullOrWhiteSpace(empresa.SerieNotaCreditoFactura) ? empresa.SerieNotaCredito : empresa.SerieNotaCreditoFactura);
        yield return (TipoComprobante.NCR, "Nota credito boleta", string.IsNullOrWhiteSpace(empresa.SerieNotaCreditoBoleta) ? empresa.SerieNotaCredito : empresa.SerieNotaCreditoBoleta);
        yield return (TipoComprobante.COT, "Cotizacion", empresa.SerieCotizacion);
    }

    private static string CrearClave(TipoComprobante tipo, string serie) => $"Correlativo.{tipo}.{serie}";
}
