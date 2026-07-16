using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public class DocumentoConfiguracionService(
    IFormularioConfiguracionService formularioService,
    ICondicionComercialService condicionService,
    IDocumentoConfiguracionRepository repository,
    IEmpresaContext empresaContext)
{
    public Task<DocumentoConfiguracionEditDto> ObtenerCotizacionAsync(CancellationToken cancellationToken) =>
        ObtenerAsync(FormularioConfiguracionService.TipoCotizacion, cancellationToken);

    public Task<DocumentoConfiguracionEditDto> ObtenerNotaPedidoAsync(CancellationToken cancellationToken) =>
        ObtenerAsync(FormularioConfiguracionService.TipoNotaPedido, cancellationToken);

    public Task<DocumentoConfiguracionEditDto> ObtenerComprobanteAsync(CancellationToken cancellationToken) =>
        ObtenerAsync(FormularioConfiguracionService.TipoComprobante, cancellationToken);

    public async Task<DocumentoConfiguracionEditDto> ObtenerAsync(string tipoDocumento, CancellationToken cancellationToken)
    {
        tipoDocumento = FormularioConfiguracionService.NormalizarTipoDocumento(tipoDocumento);
        var formulario = await formularioService.ObtenerConfiguracionAsync(tipoDocumento, empresaContext.EmpresaId, null, cancellationToken);
        var condiciones = tipoDocumento == FormularioConfiguracionService.TipoCotizacion
            ? await condicionService.ObtenerPlantillaAsync(tipoDocumento, empresaContext.EmpresaId, null, cancellationToken)
            : new CondicionComercialPlantillaDto(null, tipoDocumento, string.Empty, Array.Empty<CondicionComercialItemDto>());

        return new DocumentoConfiguracionEditDto
        {
            TipoDocumento = tipoDocumento,
            Nombre = formulario.Nombre,
            Bloques = formulario.Bloques
                .OrderBy(x => x.Orden)
                .Select(x => new FormularioBloqueEditDto
                {
                    Bloque = x.Bloque,
                    Titulo = x.Titulo,
                    Visible = x.Visible,
                    Orden = x.Orden,
                    Colapsado = x.Colapsado
                })
                .ToList(),
            Campos = formulario.Campos
                .OrderBy(x => x.Bloque)
                .ThenBy(x => x.Orden)
                .Select(x => new FormularioCampoEditDto
                {
                    Bloque = x.Bloque,
                    Campo = x.Campo,
                    Etiqueta = x.Etiqueta,
                    Visible = x.Visible,
                    Obligatorio = x.Obligatorio,
                    SoloLectura = x.SoloLectura,
                    Orden = x.Orden,
                    Ancho = x.Ancho,
                    ValorDefecto = x.ValorDefecto
                })
                .Select(x => FormularioConfiguracionService.AplicarCatalogo(tipoDocumento, x))
                .ToList(),
            ProductoComportamiento = formulario.ProductoComportamiento,
            Condiciones = condiciones.Items
                .OrderBy(x => x.Orden)
                .Select(x => new CondicionComercialEditDto
                {
                    Etiqueta = x.Etiqueta,
                    Texto = x.Texto,
                    Orden = x.Orden,
                    Visible = x.Visible
                })
                .ToList()
        };
    }

    public Task GuardarCotizacionAsync(DocumentoConfiguracionEditDto dto, CancellationToken cancellationToken) =>
        GuardarAsync(dto, cancellationToken);

    public async Task GuardarAsync(DocumentoConfiguracionEditDto dto, CancellationToken cancellationToken)
    {
        var tipoDocumento = FormularioConfiguracionService.NormalizarTipoDocumento(dto.TipoDocumento);
        if (dto.Bloques.Count == 0) throw new InvalidOperationException("Debe registrar al menos un bloque.");
        if (tipoDocumento == FormularioConfiguracionService.TipoCotizacion && dto.Condiciones.Count == 0)
        {
            throw new InvalidOperationException("Debe registrar al menos una condicion comercial.");
        }

        dto.TipoDocumento = tipoDocumento;
        dto.Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? FormularioConfiguracionService.NombreDocumento(tipoDocumento) : dto.Nombre.Trim();

        foreach (var bloque in dto.Bloques)
        {
            if (string.IsNullOrWhiteSpace(bloque.Bloque)) throw new InvalidOperationException("Existe un bloque sin codigo.");
            bloque.Bloque = bloque.Bloque.Trim().ToUpperInvariant();
            bloque.Titulo = bloque.Titulo?.Trim() ?? string.Empty;
        }

        var camposNormalizados = new List<FormularioCampoEditDto>();
        foreach (var campo in dto.Campos)
        {
            if (string.IsNullOrWhiteSpace(campo.Bloque)) throw new InvalidOperationException("Existe un campo sin bloque.");
            if (string.IsNullOrWhiteSpace(campo.Campo)) throw new InvalidOperationException("Existe un campo sin codigo.");
            campo.Bloque = campo.Bloque.Trim().ToUpperInvariant();
            campo.Campo = campo.Campo.Trim();
            campo.Etiqueta = campo.Etiqueta?.Trim() ?? string.Empty;
            campo.Ancho = campo.Ancho?.Trim() ?? string.Empty;
            campo.ValorDefecto = string.IsNullOrWhiteSpace(campo.ValorDefecto) ? null : campo.ValorDefecto.Trim();
            FormularioConfiguracionService.AplicarCatalogo(tipoDocumento, campo);
            FormularioConfiguracionService.ValidarCampo(tipoDocumento, campo);
            camposNormalizados.Add(campo);
        }

        var duplicado = camposNormalizados
            .GroupBy(x => new { Bloque = x.Bloque.ToUpperInvariant(), Campo = x.Campo.ToUpperInvariant() })
            .FirstOrDefault(x => x.Count() > 1);
        if (duplicado is not null) throw new InvalidOperationException($"El campo {duplicado.Key.Campo} esta duplicado en el bloque {duplicado.Key.Bloque}.");

        NormalizarOrdenesDuplicados(camposNormalizados);

        dto.Campos = camposNormalizados;
        dto.ProductoComportamiento.CantidadInicial = dto.ProductoComportamiento.CantidadInicial <= 0 ? 1m : dto.ProductoComportamiento.CantidadInicial;

        foreach (var condicion in dto.Condiciones)
        {
            condicion.Etiqueta = condicion.Etiqueta?.Trim() ?? string.Empty;
            condicion.Texto = condicion.Texto?.Trim() ?? string.Empty;
        }

        if (tipoDocumento != FormularioConfiguracionService.TipoCotizacion)
        {
            dto.Condiciones.Clear();
        }

        await repository.GuardarConfiguracionDocumentoAsync(empresaContext.EmpresaId, empresaContext.UsuarioNombre, dto, cancellationToken);
    }

    private static void NormalizarOrdenesDuplicados(List<FormularioCampoEditDto> campos)
    {
        var bloquesConOrdenDuplicado = campos
            .GroupBy(x => x.Bloque.ToUpperInvariant())
            .Where(bloque => bloque.GroupBy(x => x.Orden).Any(orden => orden.Count() > 1))
            .Select(x => x.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (bloquesConOrdenDuplicado.Count == 0) return;

        foreach (var bloque in bloquesConOrdenDuplicado)
        {
            var orden = 10;
            foreach (var campo in campos
                .Where(x => string.Equals(x.Bloque, bloque, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Orden)
                .ThenBy(x => x.Campo, StringComparer.OrdinalIgnoreCase))
            {
                campo.Orden = orden;
                orden += 10;
            }
        }
    }
}

