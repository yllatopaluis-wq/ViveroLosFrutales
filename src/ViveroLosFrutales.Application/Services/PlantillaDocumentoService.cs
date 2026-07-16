using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public interface IPlantillaDocumentoService
{
    Task<PlantillaDocumentoDto> ObtenerPlantillaAsync(string tipoDocumento, int? empresaId = null, int? teamId = null, CancellationToken cancellationToken = default);
}

public class PlantillaDocumentoService(IDocumentoConfiguracionRepository repository, IEmpresaContext empresaContext) : IPlantillaDocumentoService
{
    public async Task<PlantillaDocumentoDto> ObtenerPlantillaAsync(string tipoDocumento, int? empresaId = null, int? teamId = null, CancellationToken cancellationToken = default)
    {
        var empresa = empresaId ?? empresaContext.EmpresaId;
        var plantilla = await repository.ObtenerPlantillaDocumentoAsync(tipoDocumento, empresa, teamId, cancellationToken);
        if (plantilla is null) return Defaults(tipoDocumento);

        return new PlantillaDocumentoDto(
            plantilla.TipoDocumento,
            plantilla.Nombre,
            plantilla.Version,
            plantilla.Bloques.OrderBy(x => x.Orden).Select(x => new PlantillaDocumentoBloqueDto(x.Bloque, x.Titulo, x.Visible, x.Orden)).ToArray());
    }

    public static PlantillaDocumentoDto Defaults(string tipoDocumento)
    {
        var bloques = new[]
        {
            new PlantillaDocumentoBloqueDto("ENCABEZADO", "Encabezado", true, 10),
            new PlantillaDocumentoBloqueDto("CLIENTE", "Cliente", true, 20),
            new PlantillaDocumentoBloqueDto("DETALLE", "Detalle", true, 30),
            new PlantillaDocumentoBloqueDto("TOTALES", "Totales", true, 40),
            new PlantillaDocumentoBloqueDto("CONDICIONES", "Condiciones", true, 50),
            new PlantillaDocumentoBloqueDto("FIRMA", "Firma", true, 60),
            new PlantillaDocumentoBloqueDto("PIE", "Pie", true, 70)
        };
        return new PlantillaDocumentoDto(tipoDocumento, "PDF Cotizacion estandar SaaS", 1, bloques);
    }
}
