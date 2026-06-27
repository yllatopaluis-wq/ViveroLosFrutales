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
        new PermissionDefinition("Ventas", "CobrosClientes", "Ver", "Crear", "Anular"),

        new PermissionDefinition("Maestros", "Productos", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Maestros", "Clientes", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Maestros", "Proveedores", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Maestros", "Categorias", "Ver", "Crear", "Editar", "Anular"),

        new PermissionDefinition("Operaciones", "Compras", "Ver", "Crear", "Anular", "RegistrarPago", "AnularPago"),
        new PermissionDefinition("Operaciones", "Gastos", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Operaciones", "Ingresos", "Ver", "Crear", "Editar", "Anular"),
        new PermissionDefinition("Operaciones", "Devoluciones", "Ver", "Registrar"),
        new PermissionDefinition("Operaciones", "Caja", "Ver"),

        new PermissionDefinition("Reportes", "ReporteGeneral", "Ver"),
        new PermissionDefinition("Reportes", "PropuestasComerciales", "Ver"),
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
        "Home" => "Tablero de control",
        "NotasPedido" => "Notas de pedido",
        "NotasCredito" => "Notas de crédito",
        "CobrosClientes" => "Cobros clientes",
        "Categorias" => "Categorías",
        "Configuracion" => "Configuración",
        "NubefactLogs" => "Log Nubefact",
        "ErroresAplicacion" => "Errores de aplicación",
        "ReporteGeneral" => "Reporte general",
        "PropuestasComerciales" => "Propuestas comerciales",
        "CuentasPorPagar" => "Cuentas por pagar",
        "DevolucionesProveedor" => "Devoluciones proveedor",
        "ReporteCaja" => "Reporte caja",
        "EstadoCuentaClientes" => "Estado cuenta clientes",
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
        "AnularPago" => "Anular pago",
        "ConsultarSunat" => "Consultar SUNAT",
        "RestablecerPassword" => "Restablecer contraseña",
        "Configurar" => "Configurar",
        "Revisar" => "Revisar",
        _ => action
    };
}
