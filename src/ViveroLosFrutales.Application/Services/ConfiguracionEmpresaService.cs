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
    public async Task<PagedResult<ConfiguracionEmpresaListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        await AsegurarCorrelativosAsync(cancellationToken);
        return await repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);
    }

    public Task<ConfiguracionEmpresaEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);

    public async Task GuardarAsync(ConfiguracionEmpresaEditDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Clave)) throw new InvalidOperationException("La clave es obligatoria.");
        await repository.GuardarAsync(empresaContext.EmpresaId, empresaContext.UsuarioNombre, dto, cancellationToken);
    }

    public async Task<CorrelativosEmpresaDto> ObtenerCorrelativosAsync(CancellationToken cancellationToken)
    {
        await AsegurarCorrelativosAsync(cancellationToken);

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

    private async Task AsegurarCorrelativosAsync(CancellationToken cancellationToken)
    {
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");

        foreach (var item in ObtenerSeries(empresa))
        {
            if (string.IsNullOrWhiteSpace(item.Serie))
            {
                continue;
            }

            var serie = item.Serie.Trim();
            var clave = CrearClave(item.Tipo, serie);
            var existente = await repository.ObtenerPorClaveAsync(empresaContext.EmpresaId, clave, cancellationToken);
            if (existente is not null)
            {
                continue;
            }

            var siguiente = await comprobanteRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, item.Tipo, serie, cancellationToken);
            await repository.GuardarPorClaveAsync(
                empresaContext.EmpresaId,
                empresaContext.UsuarioNombre,
                clave,
                siguiente.ToString(),
                $"Siguiente correlativo para {item.Nombre} {serie}",
                cancellationToken);
        }
    }

    private static IEnumerable<(TipoComprobante Tipo, string Nombre, string Serie)> ObtenerSeries(Empresa empresa)
    {
        var series = new[]
        {
            (Tipo: TipoComprobante.BOL, Nombre: "Boleta", Serie: empresa.SerieBoleta),
            (Tipo: TipoComprobante.FAC, Nombre: "Factura", Serie: empresa.SerieFactura),
            (Tipo: TipoComprobante.NCR, Nombre: "Nota de credito factura", Serie: string.IsNullOrWhiteSpace(empresa.SerieNotaCreditoFactura) ? empresa.SerieNotaCredito : empresa.SerieNotaCreditoFactura),
            (Tipo: TipoComprobante.NCR, Nombre: "Nota de credito boleta", Serie: string.IsNullOrWhiteSpace(empresa.SerieNotaCreditoBoleta) ? empresa.SerieNotaCredito : empresa.SerieNotaCreditoBoleta),
            (Tipo: TipoComprobante.NPE, Nombre: "Nota de pedido", Serie: empresa.SerieNotaPedido),
            (Tipo: TipoComprobante.COT, Nombre: "Cotizacion", Serie: empresa.SerieCotizacion)
        };

        return series
            .Where(x => !string.IsNullOrWhiteSpace(x.Serie))
            .Select(x => (x.Tipo, x.Nombre, x.Serie.Trim()))
            .GroupBy(x => CrearClave(x.Tipo, x.Item3), StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First());
    }

    private static string CrearClave(TipoComprobante tipo, string serie) => $"Correlativo.{tipo}.{serie}";
}
