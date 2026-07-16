using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;

namespace ViveroLosFrutales.Application.Services;

public interface IFormularioConfiguracionService
{
    Task<FormularioConfiguracionDto> ObtenerConfiguracionAsync(string tipoDocumento, int? empresaId = null, int? teamId = null, CancellationToken cancellationToken = default);
}

public class FormularioConfiguracionService(IDocumentoConfiguracionRepository repository, IEmpresaContext empresaContext) : IFormularioConfiguracionService
{
    public const string TipoCotizacion = "COTIZACION";
    public const string TipoNotaPedido = "NOTA_PEDIDO";
    public const string TipoComprobante = "COMPROBANTE";
    public const string TipoNotaCredito = "NOTA_CREDITO";
    public const string TipoCompra = "COMPRA";
    public const string TipoGasto = "GASTO";
    public const string TipoIngreso = "INGRESO";
    public static readonly string[] AnchosPermitidos = ["1", "2", "3", "4", "5", "6", "8", "9", "12"];

    public async Task<FormularioConfiguracionDto> ObtenerConfiguracionAsync(string tipoDocumento, int? empresaId = null, int? teamId = null, CancellationToken cancellationToken = default)
    {
        tipoDocumento = NormalizarTipoDocumento(tipoDocumento);
        var empresa = empresaId ?? empresaContext.EmpresaId;
        var config = await repository.ObtenerFormularioAsync(tipoDocumento, empresa, teamId, cancellationToken);
        var defaults = Defaults(tipoDocumento);
        if (config is null) return defaults;

        var bloques = defaults.Bloques
            .GroupJoin(config.Bloques, d => d.Bloque, c => c.Bloque, (d, values) => values.FirstOrDefault() is { } c
                ? new FormularioBloqueDto(d.Bloque, string.IsNullOrWhiteSpace(c.Titulo) ? d.Titulo : c.Titulo, c.Visible, c.Orden, c.Colapsado)
                : d)
            .OrderBy(x => x.Orden)
            .ToArray();

        var campos = defaults.Campos
            .GroupJoin(config.Campos, d => FieldKey(d.Bloque, d.Campo), c => FieldKey(c.Bloque, c.Campo), (d, values) => values.FirstOrDefault() is { } c
                ? new FormularioCampoDto(d.Bloque, d.Campo, string.IsNullOrWhiteSpace(c.Etiqueta) ? d.Etiqueta : c.Etiqueta, c.Visible, c.Obligatorio, c.SoloLectura, c.Orden, NormalizarAncho(c.Ancho, d.Ancho), c.ValorDefecto)
                : d)
            .OrderBy(x => BloqueOrden(bloques, x.Bloque))
            .ThenBy(x => x.Orden)
            .ToArray();

        return new FormularioConfiguracionDto(tipoDocumento, string.IsNullOrWhiteSpace(config.Nombre) ? defaults.Nombre : config.Nombre, config.Version, bloques, campos, ProductoBehavior(config.ProductoConfiguracion));
    }

    public static FormularioConfiguracionDto Defaults(string tipoDocumento)
    {
        tipoDocumento = NormalizarTipoDocumento(tipoDocumento);
        var bloques = BloquesDefault(tipoDocumento);

        var campos = Catalogo(tipoDocumento)
            .Select(x => new FormularioCampoDto(x.Bloque, x.Campo, x.EtiquetaDefault, x.VisibleDefault, x.ObligatorioDefault, x.SoloLecturaDefault, x.OrdenDefault, x.AnchoDefault.ToString(), null))
            .OrderBy(x => BloqueOrden(bloques, x.Bloque))
            .ThenBy(x => x.Orden)
            .ToArray();

        return new FormularioConfiguracionDto(tipoDocumento, NombreDefault(tipoDocumento), 1, bloques, campos, ProductoBehavior(null));
    }

    public static IReadOnlyList<FormularioCampoCatalogoDto> Catalogo(string tipoDocumento)
    {
        tipoDocumento = NormalizarTipoDocumento(tipoDocumento);
        return tipoDocumento switch
        {
            TipoNotaPedido => CatalogoNotaPedido(tipoDocumento),
            TipoComprobante => CatalogoComprobante(tipoDocumento),
            TipoNotaCredito => CatalogoNotaCredito(tipoDocumento),
            TipoCompra => CatalogoCompra(tipoDocumento),
            TipoGasto => CatalogoGasto(tipoDocumento),
            TipoIngreso => CatalogoIngreso(tipoDocumento),
            _ => CatalogoCotizacion(tipoDocumento)
        };
    }

    public static string NormalizarTipoDocumento(string? tipoDocumento)
    {
        var value = string.IsNullOrWhiteSpace(tipoDocumento) ? TipoCotizacion : tipoDocumento.Trim().ToUpperInvariant();
        value = value.Replace("-", "_").Replace(" ", "_");
        return value switch
        {
            "COT" or "COTIZACION" or "COTIZACION_ESTANDAR" => TipoCotizacion,
            "NOTAPEDIDO" or "NOTA_PEDIDO" or "NPE" => TipoNotaPedido,
            "COMPROBANTES" or "COMPROBANTE" or "BOL" or "FAC" => TipoComprobante,
            "NOTACREDITO" or "NOTA_CREDITO" or "NOTAS_CREDITO" or "NCR" => TipoNotaCredito,
            "COMPRA" or "COMPRAS" => TipoCompra,
            "GASTO" or "GASTOS" => TipoGasto,
            "INGRESO" or "INGRESOS" => TipoIngreso,
            _ => value
        };
    }

    public static string NombreDocumento(string tipoDocumento) => NormalizarTipoDocumento(tipoDocumento) switch
    {
        TipoNotaPedido => "Nota de pedido",
        TipoComprobante => "Comprobante",
        TipoNotaCredito => "Nota de credito",
        TipoCompra => "Compra",
        TipoGasto => "Gasto",
        TipoIngreso => "Ingreso",
        _ => "Cotizacion"
    };

    public static void ValidarCampo(FormularioCampoEditDto campo) => ValidarCampo(TipoCotizacion, campo);

    public static void ValidarCampo(string tipoDocumento, FormularioCampoEditDto campo)
    {
        tipoDocumento = NormalizarTipoDocumento(tipoDocumento);
        var catalogo = Catalogo(tipoDocumento).FirstOrDefault(x => SameField(x.Bloque, campo.Bloque) && SameField(x.Campo, campo.Campo))
            ?? throw new InvalidOperationException($"El campo {campo.Campo} no pertenece al catalogo controlado de {NombreDocumento(tipoDocumento)}.");

        if (!int.TryParse(campo.Ancho, out var ancho) || ancho < 1 || ancho > 12)
        {
            throw new InvalidOperationException($"El ancho del campo {catalogo.EtiquetaDefault} debe estar entre 1 y 12.");
        }
        if (campo.Orden <= 0) throw new InvalidOperationException($"El orden del campo {catalogo.EtiquetaDefault} debe ser mayor que cero.");
        if (campo.Visible && string.IsNullOrWhiteSpace(campo.Etiqueta)) throw new InvalidOperationException($"La etiqueta del campo {catalogo.EtiquetaDefault} no puede estar vacia.");
        if (!catalogo.Ocultable && !campo.Visible) throw new InvalidOperationException($"El campo {catalogo.EtiquetaDefault} no se puede ocultar.");
        if (!catalogo.PuedeSerObligatorio && campo.Obligatorio) throw new InvalidOperationException($"El campo {catalogo.EtiquetaDefault} no puede marcarse como obligatorio.");
        if (catalogo.EsCampoSistema && catalogo.SoloLecturaDefault && !campo.SoloLectura) throw new InvalidOperationException($"El campo {catalogo.EtiquetaDefault} es de sistema y debe ser solo lectura.");
    }

    public static FormularioCampoEditDto AplicarCatalogo(FormularioCampoEditDto campo) => AplicarCatalogo(TipoCotizacion, campo);

    public static FormularioCampoEditDto AplicarCatalogo(string tipoDocumento, FormularioCampoEditDto campo)
    {
        tipoDocumento = NormalizarTipoDocumento(tipoDocumento);
        var catalogo = Catalogo(tipoDocumento).FirstOrDefault(x => SameField(x.Bloque, campo.Bloque) && SameField(x.Campo, campo.Campo));
        if (catalogo is null) return campo;
        campo.Bloque = catalogo.Bloque;
        campo.Campo = catalogo.Campo;
        campo.Etiqueta = string.IsNullOrWhiteSpace(campo.Etiqueta) ? catalogo.EtiquetaDefault : campo.Etiqueta.Trim();
        campo.Ancho = NormalizarAncho(campo.Ancho, catalogo.AnchoDefault.ToString());
        campo.Ocultable = catalogo.Ocultable;
        campo.PuedeSerObligatorio = catalogo.PuedeSerObligatorio;
        campo.PuedeSerSoloLectura = catalogo.PuedeSerSoloLectura;
        campo.EsCampoSistema = catalogo.EsCampoSistema;
        campo.TipoVisual = catalogo.TipoVisual;
        if (!catalogo.Ocultable) campo.Visible = true;
        if (!catalogo.PuedeSerObligatorio) campo.Obligatorio = catalogo.ObligatorioDefault;
        if (!catalogo.PuedeSerSoloLectura || catalogo.EsCampoSistema) campo.SoloLectura = catalogo.SoloLecturaDefault;
        return campo;
    }

    private static FormularioBloqueDto[] BloquesDefault(string tipoDocumento)
    {
        if (tipoDocumento == TipoGasto)
        {
            return
            [
                new("GENERAL", "Informacion general", true, 10, false),
                new("CLIENTE", "Cliente", true, 20, false),
                new("OBSERVACIONES", "Observaciones", true, 35, false),
                new("TOTALES", "Totales", true, 50, false),
                new("ACCIONES", "Acciones", true, 60, false)
            ];
        }

        if (tipoDocumento == TipoIngreso)
        {
            return
            [
                new("GENERAL", "Informacion general", true, 10, false),
                new("PROVEEDOR", "Proveedor", true, 20, false),
                new("OBSERVACIONES", "Observaciones", true, 35, false),
                new("TOTALES", "Totales", true, 50, false),
                new("ACCIONES", "Acciones", true, 60, false)
            ];
        }

        var bloques = new List<FormularioBloqueDto>
        {
            new("GENERAL", tipoDocumento == TipoNotaCredito ? "Informacion de nota de credito" : "Informacion general", true, 10, false),
            tipoDocumento == TipoNotaCredito
                ? new("ORIGEN", "Comprobante origen", true, 20, false)
                : tipoDocumento == TipoCompra
                    ? new("PROVEEDOR", "Proveedor", true, 20, false)
                    : new("CLIENTE", "Cliente", true, 20, false),
            new("PRODUCTOS", "Detalle de productos", true, 30, false),
            new("OBSERVACIONES", "Observaciones", tipoDocumento != TipoNotaCredito, 35, false),
            new("TOTALES", "Totales", true, 50, false),
            new("ACCIONES", "Acciones", true, 60, false)
        };

        if (tipoDocumento == TipoCotizacion)
        {
            bloques.Insert(4, new FormularioBloqueDto("CONDICIONES", "Condiciones comerciales", true, 40, false));
        }

        return bloques.ToArray();
    }

    private static string NombreDefault(string tipoDocumento) => NormalizarTipoDocumento(tipoDocumento) switch
    {
        TipoNotaPedido => "Nota de pedido estandar",
        TipoComprobante => "Comprobante estandar",
        TipoNotaCredito => "Nota de credito estandar",
        TipoCompra => "Compra estandar",
        TipoGasto => "Gasto estandar",
        TipoIngreso => "Ingreso estandar",
        _ => "Cotizacion estandar SaaS"
    };

    private static FormularioCampoCatalogoDto[] CatalogoCotizacion(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "GENERAL", "Serie", "Serie", 10, true, false, true, 4, false, false, false, true, "Texto"),
        Campo(tipoDocumento, "GENERAL", "Numero", "Numero", 20, true, false, true, 4, false, false, false, true, "Numero"),
        Campo(tipoDocumento, "GENERAL", "Fecha", "Fecha", 30, true, true, false, 3, false, true, true, false, "Fecha"),
        Campo(tipoDocumento, "GENERAL", "ValidezDias", "Validez", 40, true, false, true, 3, true, false, true, false, "Texto"),
        Campo(tipoDocumento, "GENERAL", "Moneda", "Moneda", 50, true, false, true, 3, true, false, true, false, "Texto"),
        Campo(tipoDocumento, "GENERAL", "FormaPago", "Forma de pago", 60, true, false, false, 3, true, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "Vendedor", "Vendedor", 70, false, false, true, 3, true, true, true, false, "Texto"),
        .. CamposCliente(tipoDocumento),
        .. CamposProducto(tipoDocumento),
        Campo(tipoDocumento, "OBSERVACIONES", "CaracteristicasTecnicas", "Observaciones", 10, true, false, false, 12, true, false, true, false, "Textarea"),
        Campo(tipoDocumento, "CONDICIONES", "CondicionesComerciales", "Condiciones comerciales", 10, true, false, false, 12, true, false, true, false, "Textarea"),
        .. CamposTotales(tipoDocumento),
        Campo(tipoDocumento, "ACCIONES", "Guardar", "Guardar cotizacion", 10, true, false, false, 4, false, false, true, false, "Accion"),
        Campo(tipoDocumento, "ACCIONES", "GuardarPdf", "Guardar PDF", 20, false, false, false, 4, true, false, true, false, "Accion"),
        Campo(tipoDocumento, "ACCIONES", "Volver", "Volver", 30, true, false, false, 4, true, false, true, false, "Accion")
    ];

    private static FormularioCampoCatalogoDto[] CatalogoNotaPedido(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "GENERAL", "Serie", "Serie", 10, true, false, true, 3, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "GENERAL", "Numero", "Numero", 20, true, false, true, 3, true, false, false, true, "Numero"),
        Campo(tipoDocumento, "GENERAL", "Fecha", "Fecha", 30, true, true, false, 3, false, true, true, false, "Fecha"),
        Campo(tipoDocumento, "GENERAL", "Vendedor", "Vendedor", 40, true, false, true, 3, true, false, true, false, "Texto"),
        Campo(tipoDocumento, "GENERAL", "Moneda", "Moneda", 50, true, false, true, 3, true, false, true, false, "Texto"),
        Campo(tipoDocumento, "GENERAL", "FormaPago", "Forma de pago", 60, true, false, false, 3, true, true, true, false, "Select"),
        .. CamposCliente(tipoDocumento),
        .. CamposProducto(tipoDocumento),
        Campo(tipoDocumento, "OBSERVACIONES", "Observacion", "Observaciones", 10, true, false, false, 12, true, false, true, false, "Textarea"),
        .. CamposTotales(tipoDocumento),
        Campo(tipoDocumento, "ACCIONES", "Guardar", "Guardar nota de pedido", 10, true, false, false, 4, false, false, true, false, "Accion"),
        Campo(tipoDocumento, "ACCIONES", "Volver", "Volver", 20, true, false, false, 4, true, false, true, false, "Accion")
    ];

    private static FormularioCampoCatalogoDto[] CatalogoComprobante(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "GENERAL", "TipoComprobante", "Tipo de comprobante", 10, true, true, false, 3, false, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "Serie", "Serie", 20, true, false, true, 3, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "GENERAL", "Numero", "Numero", 30, true, false, true, 3, true, false, false, true, "Numero"),
        Campo(tipoDocumento, "GENERAL", "FechaEmision", "Fecha de emision", 40, true, true, false, 3, false, true, true, false, "Fecha"),
        Campo(tipoDocumento, "GENERAL", "FormaPago", "Forma de pago", 50, true, true, false, 3, true, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "MedioPago", "Medio de pago", 60, true, true, false, 3, true, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "CuentaDestino", "Cuenta destino", 70, true, false, false, 3, true, false, true, false, "Select"),
        .. CamposCliente(tipoDocumento),
        .. CamposProducto(tipoDocumento),
        Campo(tipoDocumento, "OBSERVACIONES", "CondicionesVenta", "Observaciones", 10, true, false, false, 12, true, false, true, false, "Textarea"),
        .. CamposTotales(tipoDocumento),
        Campo(tipoDocumento, "ACCIONES", "Guardar", "Guardar comprobante", 10, true, false, false, 4, false, false, true, false, "Accion"),
        Campo(tipoDocumento, "ACCIONES", "Volver", "Volver", 20, true, false, false, 4, true, false, true, false, "Accion")
    ];


    private static FormularioCampoCatalogoDto[] CatalogoNotaCredito(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "GENERAL", "Serie", "Serie", 10, true, false, true, 2, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "GENERAL", "Numero", "Numero", 20, true, false, true, 2, true, false, false, true, "Numero"),
        Campo(tipoDocumento, "GENERAL", "FechaEmision", "Fecha de emision", 30, true, true, false, 3, false, true, true, false, "Fecha"),
        Campo(tipoDocumento, "GENERAL", "Motivo", "Motivo", 40, true, true, false, 3, false, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "SustentoDescripcion", "Sustento / descripcion", 50, true, false, false, 12, true, false, true, false, "Textarea"),
        Campo(tipoDocumento, "ORIGEN", "Comprobante", "Comprobante", 10, true, false, true, 2, false, false, false, true, "Texto"),
        Campo(tipoDocumento, "ORIGEN", "Cliente", "Cliente", 20, true, false, true, 3, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "ORIGEN", "Documento", "Documento", 30, true, false, true, 2, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "ORIGEN", "FechaEmisionOrigen", "Fecha emision", 40, true, false, true, 2, true, false, false, true, "Fecha"),
        Campo(tipoDocumento, "ORIGEN", "TotalOriginal", "Total original", 50, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "ORIGEN", "NotasCreditoEmitidas", "NC emitidas", 60, true, false, true, 1, true, false, false, true, "Total"),
        Campo(tipoDocumento, "ORIGEN", "SaldoDisponible", "Saldo disponible", 70, true, false, true, 2, true, false, false, true, "Total"),
        .. CamposProductoNotaCredito(tipoDocumento),
        .. CamposTotalesNotaCredito(tipoDocumento),
        Campo(tipoDocumento, "ACCIONES", "Guardar", "Generar nota de credito", 10, true, false, false, 4, false, false, true, false, "Accion"),
        Campo(tipoDocumento, "ACCIONES", "Volver", "Volver", 20, true, false, false, 4, true, false, true, false, "Accion")
    ];
    private static FormularioCampoCatalogoDto[] CatalogoCompra(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "GENERAL", "TipoDocumento", "Tipo de documento", 10, true, true, false, 3, false, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "Serie", "Serie", 20, true, false, false, 3, true, false, true, false, "Texto"),
        Campo(tipoDocumento, "GENERAL", "Numero", "Numero", 30, true, false, false, 3, true, false, true, false, "Numero"),
        Campo(tipoDocumento, "GENERAL", "Fecha", "Fecha", 40, true, true, false, 3, false, true, true, false, "Fecha"),
        Campo(tipoDocumento, "GENERAL", "FormaPago", "Forma de pago", 50, true, true, false, 3, true, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "MedioPago", "Medio de pago", 60, true, true, false, 3, true, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "CuentaDestino", "Cuenta destino", 70, true, false, false, 3, true, false, true, false, "Select"),
        .. CamposProveedor(tipoDocumento),
        .. CamposProductoCompra(tipoDocumento),
        Campo(tipoDocumento, "OBSERVACIONES", "Observacion", "Observaciones", 10, true, false, false, 12, true, false, true, false, "Textarea"),
        .. CamposTotales(tipoDocumento),
        Campo(tipoDocumento, "ACCIONES", "Guardar", "Guardar compra", 10, true, false, false, 4, false, false, true, false, "Accion"),
        Campo(tipoDocumento, "ACCIONES", "Volver", "Volver", 20, true, false, false, 4, true, false, true, false, "Accion")
    ];
    private static FormularioCampoCatalogoDto[] CatalogoGasto(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "GENERAL", "Fecha", "Fecha", 10, true, true, false, 2, false, true, true, false, "Fecha"),
        Campo(tipoDocumento, "GENERAL", "TipoGasto", "Tipo gasto", 20, true, true, false, 2, false, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "MedioPago", "Medio de pago", 30, true, true, false, 2, true, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "CuentaOrigen", "Cuenta origen", 40, true, false, false, 2, true, false, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "Importe", "Importe", 50, true, true, false, 2, false, true, true, false, "Numero"),
        Campo(tipoDocumento, "GENERAL", "Descripcion", "Descripcion", 60, true, true, false, 2, true, true, true, false, "Texto"),
        Campo(tipoDocumento, "GENERAL", "DocumentoReferencia", "Documento referencia", 70, true, false, false, 2, true, false, true, false, "Texto"),
        .. CamposCliente(tipoDocumento),
        Campo(tipoDocumento, "OBSERVACIONES", "Observacion", "Observaciones", 10, true, false, false, 12, true, false, true, false, "Textarea"),
        Campo(tipoDocumento, "TOTALES", "Total", "Total", 10, true, false, true, 4, false, false, false, true, "Total"),
        Campo(tipoDocumento, "ACCIONES", "Guardar", "Guardar gasto", 10, true, false, false, 4, false, false, true, false, "Accion"),
        Campo(tipoDocumento, "ACCIONES", "Volver", "Volver", 20, true, false, false, 4, true, false, true, false, "Accion")
    ];

    private static FormularioCampoCatalogoDto[] CatalogoIngreso(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "GENERAL", "Fecha", "Fecha", 10, true, true, false, 2, false, true, true, false, "Fecha"),
        Campo(tipoDocumento, "GENERAL", "TipoIngreso", "Tipo ingreso", 20, true, true, false, 2, false, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "MedioPago", "Medio de pago", 30, true, true, false, 2, true, true, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "CuentaOrigen", "Cuenta origen", 40, true, false, false, 2, true, false, true, false, "Select"),
        Campo(tipoDocumento, "GENERAL", "Importe", "Importe", 50, true, true, false, 2, false, true, true, false, "Numero"),
        Campo(tipoDocumento, "GENERAL", "Descripcion", "Descripcion", 60, true, true, false, 2, true, true, true, false, "Texto"),
        Campo(tipoDocumento, "GENERAL", "DocumentoReferencia", "Documento referencia", 70, true, false, false, 2, true, false, true, false, "Texto"),
        .. CamposProveedor(tipoDocumento),
        Campo(tipoDocumento, "OBSERVACIONES", "Observacion", "Observaciones", 10, true, false, false, 12, true, false, true, false, "Textarea"),
        Campo(tipoDocumento, "TOTALES", "Total", "Total", 10, true, false, true, 4, false, false, false, true, "Total"),
        Campo(tipoDocumento, "ACCIONES", "Guardar", "Guardar ingreso", 10, true, false, false, 4, false, false, true, false, "Accion"),
        Campo(tipoDocumento, "ACCIONES", "Volver", "Volver", 20, true, false, false, 4, true, false, true, false, "Accion")
    ];
    private static FormularioCampoCatalogoDto[] CamposCliente(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "CLIENTE", "ClienteBuscador", "Buscar cliente", 10, true, true, false, 12, false, true, true, false, "Buscador"),
        Campo(tipoDocumento, "CLIENTE", "ClienteNombre", "Seleccione cliente", 15, true, false, true, 3, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "CLIENTE", "ClienteDocumento", "Documento", 20, true, false, true, 2, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "CLIENTE", "ClienteTelefono", "Telefono", 30, true, false, true, 2, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "CLIENTE", "ClienteEmail", "Email", 40, true, false, true, 2, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "CLIENTE", "ClienteDireccion", "Direccion", 50, true, false, true, 3, true, false, false, true, "Texto")
    ];

    private static FormularioCampoCatalogoDto[] CamposProveedor(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "PROVEEDOR", "ProveedorBuscador", "Buscar proveedor", 10, true, true, false, 12, false, true, true, false, "Buscador"),
        Campo(tipoDocumento, "PROVEEDOR", "ProveedorTipoDocumento", "Tipo documento", 15, true, false, true, 2, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "PROVEEDOR", "ProveedorNumeroDocumento", "Numero documento", 20, true, false, true, 2, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "PROVEEDOR", "ProveedorRazonSocial", "Razon social", 30, true, false, true, 4, true, false, false, true, "Texto"),
        Campo(tipoDocumento, "PROVEEDOR", "ProveedorDireccion", "Direccion", 40, true, false, true, 4, true, false, false, true, "Texto")
    ];
    private static FormularioCampoCatalogoDto[] CamposProductoCompra(string tipoDocumento) =>
        CamposProducto(tipoDocumento)
            .Select(x => x.Campo == "PrecioUnitario"
                ? Campo(tipoDocumento, x.Bloque, x.Campo, "Precio compra", x.OrdenDefault, x.VisibleDefault, x.ObligatorioDefault, x.SoloLecturaDefault, x.AnchoDefault, x.Ocultable, x.PuedeSerObligatorio, x.PuedeSerSoloLectura, x.EsCampoSistema, x.TipoVisual)
                : x)
            .ToArray();
    private static FormularioCampoCatalogoDto[] CamposProducto(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "PRODUCTOS", "BuscadorProducto", "Buscar producto", 5, true, false, false, 12, true, false, true, false, "ControlGrilla"),
        Campo(tipoDocumento, "PRODUCTOS", "AgregarProducto", "Agregar producto", 6, true, false, false, 3, true, false, true, false, "ControlGrilla"),
        Campo(tipoDocumento, "PRODUCTOS", "Item", "#", 8, true, false, true, 1, true, false, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "Codigo", "Codigo", 10, false, false, true, 2, true, false, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "Producto", "Producto", 20, true, true, true, 4, false, true, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "Unidad", "Unidad", 30, true, false, true, 1, true, false, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "Stock", "Stock", 40, true, false, true, 1, true, false, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "Cantidad", "Cantidad", 50, true, true, false, 1, false, true, true, false, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "PrecioUnitario", "Precio unitario", 60, true, true, false, 2, false, true, true, false, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "DescuentoPorcentaje", "Descuento %", 70, true, false, false, 1, true, false, true, false, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "TotalLinea", "Total", 80, true, false, true, 2, false, false, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "Accion", "Accion", 90, true, false, false, 1, true, false, true, false, "ColumnaSistema")
    ];


    private static FormularioCampoCatalogoDto[] CamposProductoNotaCredito(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "PRODUCTOS", "Producto", "Producto", 10, true, false, true, 4, false, false, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "CantidadOriginal", "Cantidad original", 20, true, false, true, 2, true, false, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "CantidadNc", "Cantidad NC", 30, true, true, false, 2, false, true, true, false, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "Precio", "Precio", 40, true, false, true, 2, true, false, false, true, "Columna"),
        Campo(tipoDocumento, "PRODUCTOS", "TotalNc", "Total NC", 50, true, false, true, 2, false, false, false, true, "Columna")
    ];

    private static FormularioCampoCatalogoDto[] CamposTotalesNotaCredito(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "TOTALES", "SubtotalExonerado", "Subtotal exonerado", 10, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "TOTALES", "SubtotalGravado", "Subtotal gravado", 20, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "TOTALES", "Descuento", "Descuento", 30, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "TOTALES", "Igv", "IGV", 40, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "TOTALES", "TotalNc", "Total NC", 50, true, false, true, 4, false, false, false, true, "Total")
    ];
    private static FormularioCampoCatalogoDto[] CamposTotales(string tipoDocumento) =>
    [
        Campo(tipoDocumento, "TOTALES", "SubtotalExonerado", "Subtotal exonerado", 10, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "TOTALES", "SubtotalGravado", "Subtotal gravado", 20, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "TOTALES", "Descuento", "Descuento", 30, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "TOTALES", "Igv", "IGV (18%)", 40, true, false, true, 2, true, false, false, true, "Total"),
        Campo(tipoDocumento, "TOTALES", "Total", "Total", 50, true, false, true, 4, false, false, false, true, "Total")
    ];

    private static FormularioProductoComportamientoEditDto ProductoBehavior(ViveroLosFrutales.Domain.Entities.FormularioBloqueProductoConfiguracion? config) => new()
    {
        UnirProductosDuplicados = config?.UnirProductosDuplicados ?? true,
        CantidadInicial = config?.CantidadInicial > 0 ? config.CantidadInicial : 1m,
        PermitirEditarPrecio = config?.PermitirEditarPrecio ?? true,
        PermitirDescuento = config?.PermitirDescuento ?? true,
        MostrarStock = config?.MostrarStock ?? true,
        BloquearSinStock = config?.BloquearSinStock ?? false
    };

    private static FormularioCampoCatalogoDto Campo(string tipoDocumento, string bloque, string campo, string etiqueta, int orden, bool visible, bool obligatorio, bool soloLectura, int ancho, bool ocultable, bool puedeObligatorio, bool puedeSoloLectura, bool sistema, string tipoVisual) =>
        new(tipoDocumento, bloque, campo, etiqueta, orden, visible, obligatorio, soloLectura, ancho, ocultable, puedeObligatorio, puedeSoloLectura, sistema, tipoVisual);

    private static bool SameField(string left, string right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

    private static string FieldKey(string bloque, string campo) => $"{bloque.Trim().ToUpperInvariant()}::{campo.Trim().ToUpperInvariant()}";

    private static int BloqueOrden(IEnumerable<FormularioBloqueDto> bloques, string bloque) => bloques.FirstOrDefault(x => SameField(x.Bloque, bloque))?.Orden ?? int.MaxValue;

    private static string NormalizarAncho(string? ancho, string fallback)
    {
        if (string.Equals(ancho, "full", StringComparison.OrdinalIgnoreCase)) return "12";
        if (int.TryParse(ancho, out var valor) && valor >= 1 && valor <= 12) return valor.ToString();
        return fallback;
    }
}








