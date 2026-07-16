using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Application.Services;

public interface ICondicionComercialService
{
    Task<CondicionComercialPlantillaDto> ObtenerPlantillaAsync(string tipoDocumento, int? empresaId = null, int? teamId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CotizacionCondicionSnapshot>> GenerarSnapshotAsync(Cotizacion cotizacion, int? plantillaId, IReadOnlyDictionary<string, string>? valoresContexto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CotizacionCondicionSnapshotDto>> ObtenerCondicionesParaPdfAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default);
}

public class CondicionComercialService(IDocumentoConfiguracionRepository repository, IEmpresaContext empresaContext) : ICondicionComercialService
{
    public async Task<CondicionComercialPlantillaDto> ObtenerPlantillaAsync(string tipoDocumento, int? empresaId = null, int? teamId = null, CancellationToken cancellationToken = default)
    {
        var empresa = empresaId ?? empresaContext.EmpresaId;
        var plantilla = await repository.ObtenerCondicionPlantillaAsync(tipoDocumento, empresa, teamId, cancellationToken);
        if (plantilla is null) return Defaults(tipoDocumento);

        return new CondicionComercialPlantillaDto(
            plantilla.CondicionComercialPlantillaId,
            plantilla.TipoDocumento,
            plantilla.Nombre,
            plantilla.Items.OrderBy(x => x.Orden).Select(x => new CondicionComercialItemDto(x.Etiqueta, x.Texto, x.Orden, x.Visible)).ToArray());
    }

    public async Task<IReadOnlyList<CotizacionCondicionSnapshot>> GenerarSnapshotAsync(Cotizacion cotizacion, int? plantillaId, IReadOnlyDictionary<string, string>? valoresContexto, CancellationToken cancellationToken = default)
    {
        var plantilla = await ObtenerPlantillaAsync("COTIZACION", cotizacion.EmpresaId, null, cancellationToken);
        var values = valoresContexto ?? new Dictionary<string, string>();
        return plantilla.Items.Where(x => x.Visible).OrderBy(x => x.Orden).Select(x => new CotizacionCondicionSnapshot
        {
            CotizacionId = cotizacion.CotizacionId,
            Etiqueta = x.Etiqueta,
            Texto = ReplaceTokens(x.Texto, values),
            Orden = x.Orden
        }).ToArray();
    }

    public async Task<IReadOnlyList<CotizacionCondicionSnapshotDto>> ObtenerCondicionesParaPdfAsync(Cotizacion cotizacion, CancellationToken cancellationToken = default)
    {
        var snapshot = cotizacion.CondicionesSnapshot.OrderBy(x => x.Orden).ToArray();
        if (snapshot.Length == 0)
        {
            snapshot = (await repository.ListarCondicionesSnapshotAsync(cotizacion.CotizacionId, cancellationToken)).ToArray();
        }

        if (snapshot.Length > 0)
            return snapshot.Select(x => new CotizacionCondicionSnapshotDto(x.Etiqueta, x.Texto, x.Orden)).ToArray();

        var plantilla = await ObtenerPlantillaAsync("COTIZACION", cotizacion.EmpresaId, null, cancellationToken);
        var valores = ContextoCotizacion(cotizacion);
        return plantilla.Items.Where(x => x.Visible).OrderBy(x => x.Orden)
            .Select(x => new CotizacionCondicionSnapshotDto(x.Etiqueta, ReplaceTokens(x.Texto, valores), x.Orden))
            .ToArray();
    }

    public static CondicionComercialPlantillaDto Defaults(string tipoDocumento)
    {
        var items = new[]
        {
            new CondicionComercialItemDto("Validez", "Cotizacion valida por: {ValidezDias} dias calendario.", 10, true),
            new CondicionComercialItemDto("Precios", "Precios: sujetos a disponibilidad de stock.", 20, true),
            new CondicionComercialItemDto("Entrega", "Plazo de entrega: a coordinar.", 30, true),
            new CondicionComercialItemDto("Lugar", "Lugar de entrega: segun acuerdo.", 40, true),
            new CondicionComercialItemDto("Pago", "Forma de pago: contado o segun acuerdo.", 50, true),
            new CondicionComercialItemDto("Garantia", "Garantia: segun condiciones del producto.", 60, true)
        };
        return new CondicionComercialPlantillaDto(null, tipoDocumento, "Condiciones estandar SaaS", items);
    }

    public static IReadOnlyDictionary<string, string> ContextoCotizacion(Cotizacion cotizacion) => new Dictionary<string, string>
    {
        ["ValidezDias"] = "15",
        ["Serie"] = cotizacion.Serie,
        ["Numero"] = cotizacion.Correlativo.ToString(),
        ["Cliente"] = cotizacion.ClienteNombreMostrar,
        ["Total"] = cotizacion.Total.ToString("N2")
    };

    private static string ReplaceTokens(string text, IReadOnlyDictionary<string, string> values)
    {
        var result = text;
        foreach (var item in values) result = result.Replace("{" + item.Key + "}", item.Value);
        return result;
    }
}
