USE ViveroLosFrutalesDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'erp.Permiso', N'U') IS NULL OR OBJECT_ID(N'erp.RolPermiso', N'U') IS NULL OR OBJECT_ID(N'erp.Rol', N'U') IS NULL
BEGIN
    THROW 51012, 'No existen las tablas de permisos y roles. Ejecute primero 001-create-database.sql.', 1;
END;

DECLARE @Permisos TABLE (
    Grupo nvarchar(80) NOT NULL,
    Modulo nvarchar(100) NOT NULL,
    Accion nvarchar(50) NOT NULL,
    Descripcion nvarchar(250) NOT NULL,
    Orden int IDENTITY(1,1) NOT NULL,
    PRIMARY KEY (Modulo, Accion)
);

INSERT INTO @Permisos (Grupo, Modulo, Accion, Descripcion)
VALUES
(N'Inicio', N'Home', N'Ver', N'Ver Inicio'),
(N'Ventas', N'Cotizaciones', N'Ver', N'Ver Cotizaciones'),
(N'Ventas', N'Cotizaciones', N'Crear', N'Crear Cotizaciones'),
(N'Ventas', N'Cotizaciones', N'Editar', N'Editar Cotizaciones'),
(N'Ventas', N'Cotizaciones', N'Anular', N'Anular Cotizaciones'),
(N'Ventas', N'Cotizaciones', N'Imprimir', N'Imprimir Cotizaciones'),
(N'Ventas', N'Cotizaciones', N'Convertir', N'Convertir Cotizaciones'),
(N'Ventas', N'NotasPedido', N'Ver', N'Ver Notas de pedido'),
(N'Ventas', N'NotasPedido', N'Crear', N'Crear Notas de pedido'),
(N'Ventas', N'NotasPedido', N'Editar', N'Editar Notas de pedido'),
(N'Ventas', N'NotasPedido', N'Anular', N'Anular Notas de pedido'),
(N'Ventas', N'NotasPedido', N'Imprimir', N'Imprimir Notas de pedido'),
(N'Ventas', N'NotasPedido', N'Convertir', N'Convertir Notas de pedido'),
(N'Ventas', N'NotasPedido', N'RegistrarPago', N'Registrar pago Notas de pedido'),
(N'Ventas', N'Comprobantes', N'Ver', N'Ver Comprobantes'),
(N'Ventas', N'Comprobantes', N'Crear', N'Crear Comprobantes'),
(N'Ventas', N'Comprobantes', N'Editar', N'Editar Comprobantes'),
(N'Ventas', N'Comprobantes', N'Anular', N'Anular Comprobantes'),
(N'Ventas', N'Comprobantes', N'Imprimir', N'Imprimir Comprobantes'),
(N'Ventas', N'Comprobantes', N'RegistrarPago', N'Registrar pago Comprobantes'),
(N'Ventas', N'Comprobantes', N'ConsultarSunat', N'Consultar SUNAT Comprobantes'),
(N'Ventas', N'NotasCredito', N'Ver', N'Ver Notas de credito'),
(N'Ventas', N'NotasCredito', N'Crear', N'Crear Notas de credito'),
(N'Ventas', N'NotasCredito', N'Anular', N'Anular Notas de credito'),
(N'Ventas', N'NotasCredito', N'Imprimir', N'Imprimir Notas de credito'),
(N'Tesoreria', N'TESORERIA', N'Ver', N'Ver Tesoreria'),
(N'Tesoreria', N'TESORERIA_CAJA', N'Ver', N'Ver Caja'),
(N'Tesoreria', N'TESORERIA_CAJABANCOS', N'Ver', N'Ver Caja y bancos'),
(N'Tesoreria', N'TESORERIA_CUENTASFINANCIERAS', N'Ver', N'Ver Cuentas financieras'),
(N'Tesoreria', N'TESORERIA_CUENTASFINANCIERAS', N'Crear', N'Crear Cuentas financieras'),
(N'Tesoreria', N'TESORERIA_CUENTASFINANCIERAS', N'Editar', N'Editar Cuentas financieras'),
(N'Tesoreria', N'TESORERIA_CUENTASFINANCIERAS', N'Anular', N'Anular Cuentas financieras'),
(N'Tesoreria', N'TESORERIA_COBROS', N'Ver', N'Ver Cobros clientes'),
(N'Tesoreria', N'TESORERIA_COBROS', N'Crear', N'Crear Cobros clientes'),
(N'Tesoreria', N'TESORERIA_COBROS', N'Anular', N'Anular Cobros clientes'),
(N'Tesoreria', N'TESORERIA_PAGOSPROVEEDORES', N'Ver', N'Ver Pagos proveedores'),
(N'Tesoreria', N'TESORERIA_PAGOSPROVEEDORES', N'Registrar', N'Registrar Pagos proveedores'),
(N'Tesoreria', N'TESORERIA_TRANSFERENCIAS', N'Ver', N'Ver Transferencias'),
(N'Tesoreria', N'TESORERIA_TRANSFERENCIAS', N'Crear', N'Crear Transferencias'),
(N'Tesoreria', N'TESORERIA_TRANSFERENCIAS', N'Anular', N'Anular Transferencias'),
(N'Tesoreria', N'TESORERIA_CUENTASCLIENTES', N'Ver', N'Ver Estado de cuenta clientes'),
(N'Tesoreria', N'TESORERIA_CUENTASPROVEEDORES', N'Ver', N'Ver Estado de cuenta proveedores'),
(N'Maestros', N'Categorias', N'Ver', N'Ver Categorias'),
(N'Maestros', N'Categorias', N'Crear', N'Crear Categorias'),
(N'Maestros', N'Categorias', N'Editar', N'Editar Categorias'),
(N'Maestros', N'Categorias', N'Anular', N'Anular Categorias'),
(N'Maestros', N'Productos', N'Ver', N'Ver Productos'),
(N'Maestros', N'Productos', N'Crear', N'Crear Productos'),
(N'Maestros', N'Productos', N'Editar', N'Editar Productos'),
(N'Maestros', N'Productos', N'Anular', N'Anular Productos'),
(N'Maestros', N'Clientes', N'Ver', N'Ver Clientes'),
(N'Maestros', N'Clientes', N'Crear', N'Crear Clientes'),
(N'Maestros', N'Clientes', N'Editar', N'Editar Clientes'),
(N'Maestros', N'Clientes', N'Anular', N'Anular Clientes'),
(N'Maestros', N'Proveedores', N'Ver', N'Ver Proveedores'),
(N'Maestros', N'Proveedores', N'Crear', N'Crear Proveedores'),
(N'Maestros', N'Proveedores', N'Editar', N'Editar Proveedores'),
(N'Maestros', N'Proveedores', N'Anular', N'Anular Proveedores'),
(N'Operaciones', N'Compras', N'Ver', N'Ver Compras'),
(N'Operaciones', N'Compras', N'Crear', N'Crear Compras'),
(N'Operaciones', N'Compras', N'Anular', N'Anular Compras'),
(N'Operaciones', N'Compras', N'RegistrarPago', N'Registrar pago Compras'),
(N'Operaciones', N'Compras', N'AnularPago', N'Anular pago Compras'),
(N'Operaciones', N'Gastos', N'Ver', N'Ver Gastos'),
(N'Operaciones', N'Gastos', N'Crear', N'Crear Gastos'),
(N'Operaciones', N'Gastos', N'Editar', N'Editar Gastos'),
(N'Operaciones', N'Gastos', N'Anular', N'Anular Gastos'),
(N'Operaciones', N'Ingresos', N'Ver', N'Ver Ingresos'),
(N'Operaciones', N'Ingresos', N'Crear', N'Crear Ingresos'),
(N'Operaciones', N'Ingresos', N'Editar', N'Editar Ingresos'),
(N'Operaciones', N'Ingresos', N'Anular', N'Anular Ingresos'),
(N'Operaciones', N'Devoluciones', N'Ver', N'Ver Devoluciones'),
(N'Operaciones', N'Devoluciones', N'Registrar', N'Registrar Devoluciones'),
(N'Operaciones', N'Caja', N'Ver', N'Ver Caja'),
(N'Reportes', N'ReporteGeneral', N'Ver', N'Ver Reporte general'),
(N'Reportes', N'PropuestasComerciales', N'Ver', N'Ver Propuestas comerciales'),
(N'Reportes', N'ReporteNotasPedido', N'Ver', N'Ver Reporte de notas de pedido'),
(N'Reportes', N'ReporteComprobantes', N'Ver', N'Ver Reporte de comprobantes'),
(N'Reportes', N'CuentasPorPagar', N'Ver', N'Ver Estado de cuenta proveedores'),
(N'Reportes', N'DevolucionesProveedor', N'Ver', N'Ver Devoluciones proveedor'),
(N'Reportes', N'ReporteCaja', N'Ver', N'Ver Reporte caja'),
(N'Reportes', N'EstadoCuentaClientes', N'Ver', N'Ver Estado de cuenta clientes'),
(N'Administracion', N'Empresas', N'Ver', N'Ver Empresas'),
(N'Administracion', N'Empresas', N'Crear', N'Crear Empresas'),
(N'Administracion', N'Empresas', N'Editar', N'Editar Empresas'),
(N'Administracion', N'Empresas', N'Anular', N'Anular Empresas'),
(N'Administracion', N'Usuarios', N'Ver', N'Ver Usuarios'),
(N'Administracion', N'Usuarios', N'Crear', N'Crear Usuarios'),
(N'Administracion', N'Usuarios', N'Editar', N'Editar Usuarios'),
(N'Administracion', N'Usuarios', N'RestablecerPassword', N'Restablecer password Usuarios'),
(N'Administracion', N'Roles', N'Ver', N'Ver Roles'),
(N'Administracion', N'Roles', N'Crear', N'Crear Roles'),
(N'Administracion', N'Roles', N'Editar', N'Editar Roles'),
(N'Administracion', N'Configuracion', N'Ver', N'Ver Configuracion'),
(N'Administracion', N'Configuracion', N'Crear', N'Crear Configuracion'),
(N'Administracion', N'Configuracion', N'Editar', N'Editar Configuracion'),
(N'Administracion', N'Configuracion', N'Configurar', N'Configurar Configuracion'),
(N'Administracion', N'NubefactLogs', N'Ver', N'Ver Log Nubefact'),
(N'Administracion', N'ErroresAplicacion', N'Ver', N'Ver Errores de aplicacion'),
(N'Administracion', N'ErroresAplicacion', N'Revisar', N'Revisar Errores de aplicacion');

UPDATE p
SET Descripcion = src.Descripcion,
    Estado = 1
FROM [erp].[Permiso] p
JOIN @Permisos src ON src.Modulo = p.Modulo AND src.Accion = p.Accion;

INSERT INTO [erp].[Permiso] ([Modulo], [Accion], [Descripcion], [Estado])
SELECT src.Modulo, src.Accion, src.Descripcion, 1
FROM @Permisos src
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[Permiso] p WHERE p.Modulo = src.Modulo AND p.Accion = src.Accion
);

DELETE rp
FROM [erp].[RolPermiso] rp
JOIN [erp].[Permiso] p ON p.PermisoId = rp.PermisoId
WHERE NOT EXISTS (
    SELECT 1 FROM @Permisos src WHERE src.Modulo = p.Modulo AND src.Accion = p.Accion
);

DELETE p
FROM [erp].[Permiso] p
WHERE NOT EXISTS (
    SELECT 1 FROM @Permisos src WHERE src.Modulo = p.Modulo AND src.Accion = p.Accion
);

IF EXISTS (SELECT 1 WHERE NOT EXISTS (SELECT 1 FROM [erp].[Rol] WHERE RolId = 1) OR NOT EXISTS (SELECT 1 FROM [erp].[Rol] WHERE RolId = 2))
BEGIN
    SET IDENTITY_INSERT [erp].[Rol] ON;

    IF NOT EXISTS (SELECT 1 FROM [erp].[Rol] WHERE RolId = 1)
        INSERT INTO [erp].[Rol] ([RolId], [Nombre], [Descripcion], [Estado]) VALUES (1, N'Administrador', N'Acceso total', 1);

    IF NOT EXISTS (SELECT 1 FROM [erp].[Rol] WHERE RolId = 2)
        INSERT INTO [erp].[Rol] ([RolId], [Nombre], [Descripcion], [Estado]) VALUES (2, N'Vendedor', N'Operacion comercial', 1);

    SET IDENTITY_INSERT [erp].[Rol] OFF;
END;

UPDATE [erp].[Rol]
SET Nombre = N'Administrador', Descripcion = N'Acceso total', Estado = 1
WHERE RolId = 1;

UPDATE [erp].[Rol]
SET Nombre = N'Vendedor', Descripcion = N'Operacion comercial', Estado = 1
WHERE RolId = 2;

INSERT INTO [erp].[RolPermiso] ([RolId], [PermisoId])
SELECT 1, p.PermisoId
FROM [erp].[Permiso] p
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[RolPermiso] rp WHERE rp.RolId = 1 AND rp.PermisoId = p.PermisoId
);

DECLARE @ModulosVendedor TABLE (Modulo nvarchar(100) NOT NULL PRIMARY KEY);
INSERT INTO @ModulosVendedor (Modulo)
VALUES
(N'Home'), (N'Categorias'), (N'Productos'), (N'Clientes'), (N'Cotizaciones'), (N'Comprobantes'),
(N'NotasCredito'), (N'NotasPedido'), (N'Devoluciones'), (N'Caja'),
(N'TESORERIA'), (N'TESORERIA_CAJA'), (N'TESORERIA_CAJABANCOS'), (N'TESORERIA_COBROS'),
(N'TESORERIA_PAGOSPROVEEDORES'), (N'TESORERIA_TRANSFERENCIAS'), (N'TESORERIA_CUENTASCLIENTES');

DECLARE @AccionesVendedor TABLE (Accion nvarchar(50) NOT NULL PRIMARY KEY);
INSERT INTO @AccionesVendedor (Accion)
VALUES (N'Ver'), (N'Crear'), (N'Editar'), (N'Anular'), (N'Imprimir'), (N'Convertir'), (N'Registrar'), (N'RegistrarPago');

INSERT INTO [erp].[RolPermiso] ([RolId], [PermisoId])
SELECT 2, p.PermisoId
FROM [erp].[Permiso] p
JOIN @ModulosVendedor m ON m.Modulo = p.Modulo
JOIN @AccionesVendedor a ON a.Accion = p.Accion
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[RolPermiso] rp WHERE rp.RolId = 2 AND rp.PermisoId = p.PermisoId
);

PRINT N'Roles y permisos sincronizados correctamente.';
GO

