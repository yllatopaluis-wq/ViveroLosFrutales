namespace ViveroLosFrutales.Domain.Security;

public sealed record PermissionDefinition(string Group, string Module, params string[] Actions);

public static class PermissionCatalog
{
    public static IReadOnlyList<PermissionDefinition> Definitions { get; } = new[]
    {
        new PermissionDefinition("Inicio", "Home", "Ver"),

        new PermissionDefinition("Ventas", "Cotizaciones", "Ver", "Crear", "Editar", "Anular", "Imprimir", "Convertir"),
        new PermissionDefinition("Ventas", "NotasPedido", "Ver", "Crear", "Editar", "Anular", "Imprimir", "Convertir", "RegistrarPago"),
        new PermissionDefinition("Ventas", "Comprobantes", "Ver", "Crear", "Editar", "Anular", "Imprimir", "RegistrarPago", "ConsultarSunat"),
        new PermissionDefinition("Ventas", "NotasCredito", "Ver", "Crear", "Anular", "Imprimir"),
        new PermissionDefinition("Ventas", "TESORERIA_COBROS", "Ver", "Crear", "Anular"),
        new PermissionDefinition("Ventas", "TESORERIA_CUENTASCLIENTES", "Ver"),
        new PermissionDefinition("Ventas", "Clientes", "Ver", "Crear", "Editar", "Anular"),

        new PermissionDefinition("Compras", "OrdenesCompra", "Ver", "Registrar", "Editar", "Aprobar", "RegistrarPago", "RegistrarCompra", "AplicarPagos", "SolicitarDevolucion", "Cerrar", "Anular"),
        new PermissionDefinition("Compras", "Compras", "Ver", "Crear", "Anular", "RegistrarPago", "AnularPago"),
        new PermissionDefinition("Compras", "Devoluciones", "Ver", "Registrar"),
        new PermissionDefinition("Compras", "TESORERIA_PAGOSPROVEEDORES", "Ver", "Registrar"),
        new PermissionDefinition("Compras", "TESORERIA_CUENTASPROVEEDORES", "Ver"),
        new PermissionDefinition("Compras", "Proveedores", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Compras", "PagoProveedorAplicacion", "Aplicar", "AnularAplicacion"),

        new PermissionDefinition("Inventario", "Productos", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Inventario", "Categorias", "Ver", "Crear", "Editar", "Anular"),

        new PermissionDefinition("Tesorería", "TESORERIA", "Ver"),
        new PermissionDefinition("Tesorería", "TESORERIA_CAJA", "Ver"),
        new PermissionDefinition("Tesorería", "TESORERIA_CAJABANCOS", "Ver"),
        new PermissionDefinition("Tesorería", "TESORERIA_CUENTASFINANCIERAS", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Tesorería", "TESORERIA_TRANSFERENCIAS", "Ver", "Crear", "Anular"),
        new PermissionDefinition("Tesorería", "Ingresos", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Tesorería", "Gastos", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Tesorería", "Caja", "Ver"),
        new PermissionDefinition("Reportes", "ReporteGeneral", "Ver"),
        new PermissionDefinition("Reportes", "PropuestasComerciales", "Ver"),
        new PermissionDefinition("Reportes", "ReporteNotasPedido", "Ver"),
        new PermissionDefinition("Reportes", "ReporteComprobantes", "Ver"),
        new PermissionDefinition("Reportes", "CuentasPorPagar", "Ver"),
        new PermissionDefinition("Reportes", "DevolucionesProveedor", "Ver"),
        new PermissionDefinition("Reportes", "ReporteCaja", "Ver"),
        new PermissionDefinition("Reportes", "EstadoCuentaClientes", "Ver"),

        new PermissionDefinition("Administración", "Empresas", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Administración", "Usuarios", "Ver", "Crear", "Editar", "RestablecerPassword"),
        new PermissionDefinition("Administración", "Roles", "Ver", "Crear", "Editar"),
        new PermissionDefinition("Administración", "Configuracion", "Ver", "Crear", "Editar", "Configurar"),
        new PermissionDefinition("Administración", "NubefactLogs", "Ver"),
        new PermissionDefinition("Administración", "ErroresAplicacion", "Ver", "Revisar")
    };

    public static IEnumerable<(string Group, string Module, string Action)> All() =>
        Definitions.SelectMany(definition => definition.Actions.Select(action => (definition.Group, definition.Module, action)));

    public static string GroupFor(string module) =>
        Definitions.FirstOrDefault(x => x.Module.Equals(module, StringComparison.OrdinalIgnoreCase))?.Group ?? "Otros";

    public static string ModuleLabel(string module) => module switch
    {
        "Home" => "Inicio",
        "Cotizaciones" => "Cotizaciones",
        "NotasPedido" => "Notas pedido",
        "Comprobantes" => "Comprobantes",
        "NotasCredito" => "Notas crédito",
        "TESORERIA" => "Tesorería",
        "TESORERIA_CAJA" => "Caja",
        "TESORERIA_CAJABANCOS" => "Caja y Bancos",
        "TESORERIA_CUENTASFINANCIERAS" => "Cuentas financieras",
        "TESORERIA_COBROS" => "Cobros",
        "TESORERIA_PAGOSPROVEEDORES" => "Pagos proveedores",
        "TESORERIA_TRANSFERENCIAS" => "Transferencias",
        "TESORERIA_CUENTASCLIENTES" => "Estado cuenta clientes",
        "TESORERIA_CUENTASPROVEEDORES" => "Estado cuenta proveedores",
        "Categorias" => "Categorías",
        "Productos" => "Productos",
        "Clientes" => "Clientes",
        "Proveedores" => "Proveedores",
        "Compras" => "Compras",
        "OrdenesCompra" => "Ordenes compra",
        "PagoProveedorAplicacion" => "Aplicacion de pagos proveedor",
        "Gastos" => "Gastos",
        "Ingresos" => "Ingresos",
        "Devoluciones" => "Devoluciones",
        "Caja" => "Caja",
        "Configuracion" => "Configuración",
        "NubefactLogs" => "Log Nubefact",
        "ErroresAplicacion" => "Errores de aplicación",
        "ReporteGeneral" => "Reporte general",
        "PropuestasComerciales" => "Propuestas comerciales",
        "ReporteNotasPedido" => "Reporte de notas de pedido",
        "ReporteComprobantes" => "Reporte de comprobantes",
        "CuentasPorPagar" => "Estado de cuenta proveedores",
        "DevolucionesProveedor" => "Devoluciones proveedor",
        "ReporteCaja" => "Reporte caja",
        "EstadoCuentaClientes" => "Estado de cuenta clientes",
        _ => module
    };

    public static string ActionLabel(string action) => action switch
    {
        "Ver" => "Ver",
        "Crear" => "Crear",
        "Editar" => "Editar",
        "Anular" => "Anular",
        "Imprimir" => "Imprimir",
        "Convertir" => "Convertir",
        "Registrar" => "Registrar",
        "RegistrarPago" => "Registrar pago",
        "RegistrarCompra" => "Registrar compra",
        "AplicarPagos" => "Aplicar pagos",
        "SolicitarDevolucion" => "Solicitar devolucion",
        "Cerrar" => "Cerrar",
        "Aprobar" => "Aprobar",
        "Aplicar" => "Aplicar",
        "AnularAplicacion" => "Anular aplicacion",
        "AnularPago" => "Anular pago",
        "ConsultarSunat" => "Consultar SUNAT",
        "RestablecerPassword" => "Restablecer contraseña",
        "Configurar" => "Configurar",
        "Revisar" => "Revisar",
        _ => action
    };
}


