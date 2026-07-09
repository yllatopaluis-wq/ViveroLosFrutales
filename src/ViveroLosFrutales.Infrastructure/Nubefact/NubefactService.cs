using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
using ViveroLosFrutales.Infrastructure.Data;

namespace ViveroLosFrutales.Infrastructure.Nubefact;

public class NubefactService(
    HttpClient httpClient,
    ApplicationDbContext db,
    IOptions<NubefactOptions> nubefactOptions,
    ILogger<NubefactService> logger) : INubefactService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly NubefactOptions options = nubefactOptions.Value;

    public async Task<NubefactResponseDto> EmitirComprobanteAsync(Comprobante comprobante, CancellationToken cancellationToken)
    {
        if (!EsComprobanteElectronico(comprobante.TipoComprobante))
        {
            return new NubefactResponseDto
            {
                Exitoso = false,
                EstadoSunat = EstadoSunat.NoAplica,
                Mensaje = "Nubefact solo aplica para boletas, facturas y notas de credito.",
                RespuestaCompleta = "Tipo de comprobante no electronico."
            };
        }

        if (!TieneCredenciales(comprobante))
        {
            return CredencialesNoConfiguradas();
        }

        object? payload = null;
        try
        {
            payload = CrearPayloadEmision(comprobante);
            var result = await EnviarAsync(comprobante, payload, cancellationToken);
            var estadoExitoso = EstadoSunatDesdeRespuesta(result.Body, comprobante.EstadoSunat);
            var respuesta = CrearRespuesta(result, "Comprobante emitido.", estadoExitoso);
            if (string.IsNullOrWhiteSpace(respuesta.PdfUrl))
            {
                respuesta.Exitoso = false;
                respuesta.Mensaje = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                    ? "Nubefact no devolvio enlace PDF. Revise la respuesta completa del comprobante."
                    : respuesta.Mensaje;
            }

            await RegistrarOperacionAsync(comprobante, "generar_comprobante", payload, respuesta, cancellationToken);
            return respuesta;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al emitir comprobante {Serie}-{Correlativo} en Nubefact.", comprobante.Serie, comprobante.Correlativo);
            var respuesta = ErrorPorExcepcion(ex);
            await RegistrarOperacionAsync(comprobante, "generar_comprobante", payload, respuesta, cancellationToken);
            return respuesta;
        }
    }

    public async Task<NubefactResponseDto> ConsultarEstadoAsync(Comprobante comprobante, CancellationToken cancellationToken)
    {
        if (!EsComprobanteElectronico(comprobante.TipoComprobante))
        {
            return new NubefactResponseDto { Exitoso = true, EstadoSunat = EstadoSunat.NoAplica, Mensaje = "Consulta no aplica para este comprobante." };
        }

        if (!TieneCredenciales(comprobante))
        {
            return CredencialesNoConfiguradas();
        }

        object? payload = null;
        try
        {
            payload = new
            {
                operacion = "consultar_comprobante",
                tipo_de_comprobante = MapTipoComprobante(comprobante.TipoComprobante),
                serie = comprobante.Serie,
                numero = comprobante.Correlativo
            };

            var result = await EnviarAsync(comprobante, payload, cancellationToken);
            var respuesta = CrearRespuesta(
                result,
                "Consulta realizada.",
                EstadoSunatDesdeRespuesta(result.Body, comprobante.EstadoSunat));
            await RegistrarOperacionAsync(comprobante, "consultar_comprobante", payload, respuesta, cancellationToken);
            return respuesta;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consultar comprobante {Serie}-{Correlativo} en Nubefact.", comprobante.Serie, comprobante.Correlativo);
            var respuesta = ErrorPorExcepcion(ex);
            await RegistrarOperacionAsync(comprobante, "consultar_comprobante", payload, respuesta, cancellationToken);
            return respuesta;
        }
    }

    public async Task<NubefactResponseDto> AnularComprobanteAsync(Comprobante comprobante, CancellationToken cancellationToken)
    {
        if (!EsComprobanteElectronico(comprobante.TipoComprobante))
        {
            return new NubefactResponseDto { Exitoso = true, EstadoSunat = EstadoSunat.NoAplica, Mensaje = "Anulacion no aplica para este comprobante." };
        }

        if (!TieneCredenciales(comprobante))
        {
            return CredencialesNoConfiguradas();
        }

        object? payload = null;
        try
        {
            payload = new
            {
                operacion = "generar_anulacion",
                tipo_de_comprobante = MapTipoComprobante(comprobante.TipoComprobante),
                serie = comprobante.Serie,
                numero = comprobante.Correlativo,
                motivo = string.IsNullOrWhiteSpace(comprobante.MotivoAnulacion)
                    ? "ANULACION DE COMPROBANTE"
                    : comprobante.MotivoAnulacion,
                codigo_unico = string.Empty
            };

            var result = await EnviarAsync(comprobante, payload, cancellationToken);
            var respuesta = CrearRespuesta(result, "Comprobante anulado.", EstadoSunat.Anulado);
            await RegistrarOperacionAsync(comprobante, "generar_anulacion", payload, respuesta, cancellationToken);
            return respuesta;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al anular comprobante {Serie}-{Correlativo} en Nubefact.", comprobante.Serie, comprobante.Correlativo);
            var respuesta = ErrorPorExcepcion(ex);
            await RegistrarOperacionAsync(comprobante, "generar_anulacion", payload, respuesta, cancellationToken);
            return respuesta;
        }
    }

    private object CrearPayloadEmision(Comprobante comprobante)
    {
        var detalles = comprobante.Detalles.ToList();
        var totalGravada = Round(detalles.Where(EsGravado).Sum(x => x.Importe));
        var totalExonerada = Round(detalles.Where(EsExonerado).Sum(x => x.Importe));
        var detraccion = ObtenerDetraccion(comprobante, detalles);

        return new
        {
            operacion = "generar_comprobante",
            tipo_de_comprobante = MapTipoComprobante(comprobante.TipoComprobante),
            serie = comprobante.Serie,
            numero = comprobante.Correlativo,
            sunat_transaction = detraccion.Aplica ? 30 : 1,
            cliente_tipo_de_documento = MapTipoDocumento(comprobante.ClienteTipoDocumentoMostrar),
            cliente_numero_de_documento = comprobante.ClienteNumeroDocumentoMostrar,
            cliente_denominacion = comprobante.ClienteNombreMostrar,
            cliente_direccion = comprobante.ClienteDireccionMostrar,
            cliente_email = comprobante.ClienteEmailMostrar,
            cliente_email_1 = string.Empty,
            cliente_email_2 = string.Empty,
            fecha_de_emision = comprobante.FechaEmision.ToString("dd-MM-yyyy"),
            fecha_de_vencimiento = string.Empty,
            moneda = MapMoneda(comprobante.Empresa?.MonedaPredeterminada),
            tipo_de_cambio = string.Empty,
            porcentaje_de_igv = 18,
            descuento_global = string.Empty,
            total_descuento = string.Empty,
            total_anticipo = string.Empty,
            total_gravada = ValorNubefact(totalGravada),
            total_inafecta = string.Empty,
            total_exonerada = ValorNubefact(totalExonerada),
            total_igv = Round(comprobante.Igv),
            total_gratuita = string.Empty,
            total_otros_cargos = string.Empty,
            total = Round(comprobante.Total),
            percepcion_tipo = string.Empty,
            percepcion_base_imponible = string.Empty,
            total_percepcion = string.Empty,
            total_incluido_percepcion = string.Empty,
            detraccion = detraccion.Aplica,
            detraccion_tipo = detraccion.Aplica ? detraccion.Tipo : (object)string.Empty,
            detraccion_total = detraccion.Aplica ? detraccion.Total : (object)string.Empty,
            detraccion_porcentaje = detraccion.Aplica ? detraccion.Porcentaje : (object)string.Empty,
            medio_de_pago_detraccion = detraccion.Aplica ? options.MedioPagoDetraccion : (object)string.Empty,
            observaciones = comprobante.TipoComprobante == TipoComprobante.NCR && !string.IsNullOrWhiteSpace(comprobante.MotivoNotaCredito)
                ? comprobante.MotivoNotaCredito
                : comprobante.CondicionesVenta,
            documento_que_se_modifica_tipo = comprobante.TipoComprobante == TipoComprobante.NCR ? MapTipoComprobante(comprobante.ComprobanteReferencia?.TipoComprobante) : (object)string.Empty,
            documento_que_se_modifica_serie = comprobante.TipoComprobante == TipoComprobante.NCR ? comprobante.ComprobanteReferencia?.Serie ?? string.Empty : string.Empty,
            documento_que_se_modifica_numero = comprobante.TipoComprobante == TipoComprobante.NCR ? comprobante.ComprobanteReferencia?.Correlativo.ToString() ?? string.Empty : string.Empty,
            tipo_de_nota_de_credito = comprobante.TipoComprobante == TipoComprobante.NCR ? 1 : (object)string.Empty,
            tipo_de_nota_de_debito = string.Empty,
            enviar_automaticamente_a_sunat = true,
            enviar_automaticamente_al_cliente = false,
            codigo_unico = string.Empty,
            condiciones_de_pago = comprobante.FormaPago.ToString().ToUpperInvariant(),
            medio_de_pago = string.Empty,
            formato_de_pdf = string.IsNullOrWhiteSpace(options.FormatoPdf) ? "A4" : options.FormatoPdf,
            items = detalles.Select(CrearItem)
        };
    }

    private object CrearItem(ComprobanteDetalle detalle)
    {
        var total = Round(detalle.Importe + detalle.ImporteIgv);
        var precioUnitario = detalle.Cantidad <= 0 ? 0 : Round(total / detalle.Cantidad);
        var valorUnitario = detalle.Cantidad <= 0 ? 0 : RoundUnitario(detalle.Importe / detalle.Cantidad);

        return new
        {
            unidad_de_medida = NormalizarUnidadMedida(detalle.Producto?.UnidadMedida),
            codigo = detalle.ProductoId.ToString("000"),
            codigo_producto_sunat = string.Empty,
            descripcion = detalle.Producto?.Nombre ?? string.Empty,
            cantidad = Round(detalle.Cantidad),
            valor_unitario = valorUnitario,
            precio_unitario = precioUnitario,
            descuento = string.Empty,
            subtotal = Round(detalle.Importe),
            tipo_de_igv = EsGravado(detalle) ? 1 : 8,
            igv = Round(detalle.ImporteIgv),
            total,
            anticipo_regularizacion = false,
            anticipo_documento_serie = string.Empty,
            anticipo_documento_numero = string.Empty
        };
    }

    private async Task<NubefactHttpResult> EnviarAsync(Comprobante comprobante, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, comprobante.Empresa!.UrlNubefact)
        {
            Content = JsonContent.Create(payload, options: JsonOptions)
        };
        request.Headers.Add("Authorization", BuildAuthorizationHeader(comprobante.Empresa.TokenNubefact));

        var response = await httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return new NubefactHttpResult(response.IsSuccessStatusCode, body);
    }

    private async Task RegistrarOperacionAsync(Comprobante comprobante, string tipoOperacion, object? payload, NubefactResponseDto respuesta, CancellationToken cancellationToken)
    {
        if (comprobante.ComprobanteId == 0 || comprobante.EmpresaId == 0)
        {
            return;
        }

        db.NubefactOperaciones.Add(new NubefactOperacion
        {
            EmpresaId = comprobante.EmpresaId,
            ComprobanteId = comprobante.ComprobanteId,
            TipoOperacion = tipoOperacion,
            EstadoSunat = respuesta.EstadoSunat,
            PdfUrl = respuesta.PdfUrl,
            XmlUrl = respuesta.XmlUrl,
            Hash = respuesta.Hash,
            SolicitudJson = payload is null ? string.Empty : JsonSerializer.Serialize(payload, JsonOptions),
            RespuestaCompleta = respuesta.RespuestaCompleta,
            UsuarioRegistro = comprobante.UsuarioRegistro
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    private NubefactResponseDto CrearRespuesta(NubefactHttpResult result, string mensajeExitoso, EstadoSunat estadoExitoso)
    {
        var mensajeError = TryGetErrors(result.Body);
        var tieneError = !string.IsNullOrWhiteSpace(mensajeError);
        var exitoso = result.IsSuccessStatusCode && !tieneError;

        return new NubefactResponseDto
        {
            Exitoso = exitoso,
            EstadoSunat = exitoso ? estadoExitoso : EstadoSunat.Observado,
            PdfUrl = TryGetString(result.Body, "enlace_del_pdf"),
            XmlUrl = TryGetString(result.Body, "enlace_del_xml"),
            Hash = TryGetString(result.Body, "codigo_hash"),
            RespuestaCompleta = result.Body,
            Mensaje = exitoso ? (TryGetString(result.Body, "sunat_description") is { Length: > 0 } sunatMessage ? sunatMessage : mensajeExitoso) : mensajeError
        };
    }

    private static EstadoSunat EstadoSunatDesdeRespuesta(string json, EstadoSunat estadoActual)
    {
        if (TryGetBool(json, "anulado")) return EstadoSunat.Anulado;
        if (TieneFlagVerdadero(json, "aceptada_por_sunat", "aceptado_por_sunat", "aceptada_sunat", "aceptado_sunat", "sunat_aceptado", "sunat_aceptada"))
        {
            return EstadoSunat.Aceptado;
        }

        var sunatResponseCode = TryGetString(json, "sunat_responsecode");
        if (EsCodigoSunatAceptado(sunatResponseCode)) return EstadoSunat.Aceptado;

        foreach (var estadoCampo in new[] { "estado", "estado_sunat", "sunat_estado" })
        {
            var estado = TryGetString(json, estadoCampo);
            if (MapEstadoTexto(estado) is { } estadoMapeado) return estadoMapeado;
        }

        var descripcionSunat = string.Empty;
        foreach (var descripcionCampo in new[] { "sunat_description", "sunat_descripcion", "descripcion", "mensaje", "message" })
        {
            var descripcion = TryGetString(json, descripcionCampo);
            if (string.IsNullOrWhiteSpace(descripcion)) continue;

            descripcionSunat = descripcion;
            if (descripcion.Contains("acept", StringComparison.OrdinalIgnoreCase)) return EstadoSunat.Aceptado;
            if (descripcion.Contains("rechaz", StringComparison.OrdinalIgnoreCase)) return EstadoSunat.Rechazado;
            if (descripcion.Contains("observ", StringComparison.OrdinalIgnoreCase)) return EstadoSunat.Observado;
        }

        if (TieneFlagFalso(json, "aceptada_por_sunat", "aceptado_por_sunat", "aceptada_sunat", "aceptado_sunat", "sunat_aceptado", "sunat_aceptada")
            && !string.IsNullOrWhiteSpace(descripcionSunat))
        {
            return EstadoSunat.Rechazado;
        }

        return estadoActual;
    }

    private static bool TieneFlagVerdadero(string json, params string[] properties) =>
        properties.Any(property => TryGetBool(json, property));

    private static bool TieneFlagFalso(string json, params string[] properties) =>
        properties.Any(property => TryGetBoolNullable(json, property) == false);

    private static bool EsCodigoSunatAceptado(string codigo)
    {
        var normalizado = codigo.Trim();
        return normalizado == "0" || normalizado == "0000";
    }

    private static EstadoSunat? MapEstadoTexto(string estado)
    {
        var normalizado = estado.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalizado)) return null;

        return normalizado switch
        {
            "ACEPTADO" or "ACEPTADA" or "ACEPTADA_POR_SUNAT" => EstadoSunat.Aceptado,
            "PENDIENTE" or "EN_PROCESO" => EstadoSunat.Pendiente,
            "OBSERVADO" or "OBSERVADA" => EstadoSunat.Observado,
            "RECHAZADO" or "RECHAZADA" => EstadoSunat.Rechazado,
            "ANULADO" or "ANULADA" => EstadoSunat.Anulado,
            _ => null
        };
    }

    private DetraccionInfo ObtenerDetraccion(Comprobante comprobante, IReadOnlyCollection<ComprobanteDetalle> detalles)
    {
        if (comprobante.TipoComprobante != TipoComprobante.FAC || comprobante.Total <= options.MontoMinimoDetraccion)
        {
            return DetraccionInfo.NoAplica;
        }

        var detallesConDetraccion = detalles
            .Where(x => x.Producto is { PorcentajeDetraccion: > 0 }
                && (x.Producto.TieneDetraccion || EsExonerado(x)))
            .ToList();

        if (detallesConDetraccion.Count == 0)
        {
            return DetraccionInfo.NoAplica;
        }

        var porcentaje = Round(detallesConDetraccion.Max(x => x.Producto?.PorcentajeDetraccion ?? 0m));
        var total = Round(detallesConDetraccion.Sum(CalcularMontoDetraccion));
        if (total <= 0)
        {
            return DetraccionInfo.NoAplica;
        }

        var tipo = detallesConDetraccion.Any(EsGravado)
            ? options.TipoDetraccionGravada
            : options.TipoDetraccionExonerada;

        return new DetraccionInfo(true, tipo, porcentaje, total);
    }

    private static bool EsGravado(ComprobanteDetalle detalle) => detalle.ImporteIgv > 0;

    private static bool EsExonerado(ComprobanteDetalle detalle) => detalle.ImporteIgv <= 0 && detalle.Producto?.AfectoIgv == false;

    private static decimal CalcularMontoDetraccion(ComprobanteDetalle detalle)
    {
        if (detalle.MontoDetraccion > 0)
        {
            return detalle.MontoDetraccion;
        }

        var porcentaje = detalle.Producto?.PorcentajeDetraccion ?? 0m;
        return porcentaje <= 0
            ? 0
            : Round((detalle.Importe + detalle.ImporteIgv) * porcentaje / 100m);
    }

    private static bool EsComprobanteElectronico(TipoComprobante tipo) => tipo is TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR;

    private static bool TieneCredenciales(Comprobante comprobante) =>
        comprobante.Empresa is not null
        && !string.IsNullOrWhiteSpace(comprobante.Empresa.UrlNubefact)
        && !string.IsNullOrWhiteSpace(comprobante.Empresa.TokenNubefact);

    private static NubefactResponseDto CredencialesNoConfiguradas() => new()
    {
        Exitoso = false,
        EstadoSunat = EstadoSunat.Pendiente,
        Mensaje = "Credenciales Nubefact no configuradas.",
        RespuestaCompleta = "Credenciales Nubefact no configuradas."
    };

    private static NubefactResponseDto ErrorPorExcepcion(Exception ex) => new()
    {
        Exitoso = false,
        EstadoSunat = EstadoSunat.Pendiente,
        Mensaje = ex.Message,
        RespuestaCompleta = ex.ToString()
    };

    private static int MapTipoComprobante(TipoComprobante? tipo) => tipo switch
    {
        TipoComprobante.FAC => 1,
        TipoComprobante.BOL => 2,
        TipoComprobante.NCR => 3,
        _ => 0
    };

    private static int MapTipoDocumento(TipoDocumentoCliente? tipo) => tipo switch
    {
        TipoDocumentoCliente.RUC => 6,
        TipoDocumentoCliente.DNI => 1,
        TipoDocumentoCliente.CE => 4,
        _ => 0
    };

    private static int MapMoneda(string? moneda)
    {
        return moneda?.Trim().ToUpperInvariant() is "USD" or "US$" or "DOLARES" or "DOLARS" ? 2 : 1;
    }

    private static object ValorNubefact(decimal value) => value > 0 ? value : string.Empty;

    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);

    private static decimal RoundUnitario(decimal value) => decimal.Round(value, 6, MidpointRounding.AwayFromZero);

    private static string NormalizarUnidadMedida(string? unidad)
    {
        return unidad?.Trim().ToUpperInvariant() switch
        {
            null or "" => "NIU",
            "UNIDAD" or "UND" or "UNIDADES" => "NIU",
            "KILO" or "KILOS" or "KG" or "KILOGRAMO" or "KILOGRAMOS" => "KGM",
            "METRO" or "METROS" or "M" => "MTR",
            "LITRO" or "LITROS" or "LT" => "LTR",
            var valor => valor
        };
    }

    private static string BuildAuthorizationHeader(string token)
    {
        var normalized = token.Trim();
        if (normalized.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            || normalized.StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
        {
            return normalized;
        }

        return $"Token token=\"{normalized}\"";
    }

    private static string TryGetString(string json, string property)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty(property, out var value))
            {
                return string.Empty;
            }

            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString() ?? string.Empty,
                JsonValueKind.Number => value.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => string.Empty
            };
        }
        catch
        {
            return string.Empty;
        }
    }

    private static bool TryGetBool(string json, string property) =>
        TryGetBoolNullable(json, property) == true;

    private static bool? TryGetBoolNullable(string json, string property)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty(property, out var value)) return null;

            return value.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Number => value.TryGetInt32(out var number) ? number == 1 : null,
                JsonValueKind.String => EsValorVerdadero(value.GetString()) ? true : EsValorFalso(value.GetString()) ? false : null,
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private static bool EsValorVerdadero(string? value)
    {
        var normalizado = value?.Trim().ToUpperInvariant();
        return normalizado is "TRUE" or "1" or "SI" or "SÍ" or "S" or "YES" or "Y" or "ACEPTADO" or "ACEPTADA";
    }

    private static bool EsValorFalso(string? value)
    {
        var normalizado = value?.Trim().ToUpperInvariant();
        return normalizado is "FALSE" or "0" or "NO" or "N" or "RECHAZADO" or "RECHAZADA" or "OBSERVADO" or "OBSERVADA";
    }

    private static string TryGetErrors(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("errors", out var errors))
            {
                return string.Empty;
            }

            return errors.ValueKind switch
            {
                JsonValueKind.String => errors.GetString() ?? string.Empty,
                JsonValueKind.Array => string.Join(" ", errors.EnumerateArray().Select(x => x.ToString())),
                JsonValueKind.Object => errors.ToString(),
                _ => errors.ToString()
            };
        }
        catch
        {
            return string.Empty;
        }
    }

    private sealed record NubefactHttpResult(bool IsSuccessStatusCode, string Body);

    private sealed record DetraccionInfo(bool Aplica, int Tipo, decimal Porcentaje, decimal Total)
    {
        public static DetraccionInfo NoAplica { get; } = new(false, 0, 0, 0);
    }
}

