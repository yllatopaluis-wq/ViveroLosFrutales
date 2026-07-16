BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[erp].[PagoProveedor]') AND [c].[name] = N'CompraId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [erp].[PagoProveedor] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [erp].[PagoProveedor] ALTER COLUMN [CompraId] int NULL;
GO

ALTER TABLE [erp].[PagoProveedor] ADD [CuentaFinancieraId] int NULL;
GO

ALTER TABLE [erp].[PagoProveedor] ADD [OrdenCompraId] int NULL;
GO

ALTER TABLE [erp].[NotaPedido] ADD [FormaPago] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [erp].[NotaPedido] ADD [Observacion] nvarchar(1000) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[MovimientoCaja] ADD [CuentaFinancieraId] int NULL;
GO

ALTER TABLE [erp].[Ingreso] ADD [ClienteId] int NULL;
GO

ALTER TABLE [erp].[Ingreso] ADD [CuentaFinancieraId] int NULL;
GO

ALTER TABLE [erp].[Ingreso] ADD [DocumentoReferencia] nvarchar(120) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[Gasto] ADD [CuentaFinancieraId] int NULL;
GO

ALTER TABLE [erp].[Gasto] ADD [DocumentoReferencia] nvarchar(120) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[Gasto] ADD [ProveedorId] int NULL;
GO

ALTER TABLE [erp].[Empresa] ADD [FirmaContenido] varbinary(max) NULL;
GO

ALTER TABLE [erp].[Empresa] ADD [FirmaContentType] nvarchar(120) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[Empresa] ADD [FirmaNombre] nvarchar(260) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[Empresa] ADD [RepresentanteLegalCargo] nvarchar(120) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[Empresa] ADD [RepresentanteLegalDocumento] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[Empresa] ADD [RepresentanteLegalNombre] nvarchar(200) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[Devolucion] ADD [OrdenCompraId] int NULL;
GO

ALTER TABLE [erp].[Devolucion] ADD [PagoProveedorId] int NULL;
GO

ALTER TABLE [erp].[CompraDetalle] ADD [CantidadRecibida] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [erp].[Compra] ADD [DiasCredito] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [erp].[Compra] ADD [EstadoEntrega] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [erp].[Compra] ADD [FechaVencimiento] datetime2 NULL;
GO

ALTER TABLE [erp].[Compra] ADD [Moneda] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [erp].[Compra] ADD [OrdenCompraId] int NULL;
GO

ALTER TABLE [erp].[Compra] ADD [TipoCambio] decimal(18,4) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [erp].[CobroCliente] ADD [CuentaFinancieraId] int NULL;
GO

ALTER TABLE [erp].[Cliente] ADD [EmpresaId] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [erp].[CondicionComercialPlantilla] (
    [CondicionComercialPlantillaId] int NOT NULL IDENTITY,
    [EmpresaId] int NULL,
    [TeamId] int NULL,
    [TipoDocumento] nvarchar(40) NOT NULL,
    [Nombre] nvarchar(160) NOT NULL,
    [EsPredeterminada] bit NOT NULL,
    [Activa] bit NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(120) NOT NULL,
    CONSTRAINT [PK_CondicionComercialPlantilla] PRIMARY KEY ([CondicionComercialPlantillaId]),
    CONSTRAINT [FK_CondicionComercialPlantilla_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[CotizacionCondicionSnapshot] (
    [CotizacionCondicionSnapshotId] int NOT NULL IDENTITY,
    [CotizacionId] int NOT NULL,
    [Etiqueta] nvarchar(120) NOT NULL,
    [Texto] nvarchar(1000) NOT NULL,
    [Orden] int NOT NULL,
    CONSTRAINT [PK_CotizacionCondicionSnapshot] PRIMARY KEY ([CotizacionCondicionSnapshotId]),
    CONSTRAINT [FK_CotizacionCondicionSnapshot_Cotizacion_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [erp].[Cotizacion] ([CotizacionId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[CuentaFinanciera] (
    [CuentaFinancieraId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(120) NOT NULL,
    [Tipo] int NOT NULL,
    [Banco] nvarchar(120) NOT NULL,
    [NumeroCuenta] nvarchar(80) NOT NULL,
    [Moneda] nvarchar(3) NOT NULL,
    [SaldoInicial] decimal(18,2) NOT NULL,
    [FechaSaldoInicial] datetime2 NOT NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_CuentaFinanciera] PRIMARY KEY ([CuentaFinancieraId]),
    CONSTRAINT [FK_CuentaFinanciera_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[FormularioConfiguracion] (
    [FormularioConfiguracionId] int NOT NULL IDENTITY,
    [EmpresaId] int NULL,
    [TeamId] int NULL,
    [TipoDocumento] nvarchar(40) NOT NULL,
    [Nombre] nvarchar(160) NOT NULL,
    [Version] int NOT NULL,
    [Activo] bit NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(120) NOT NULL,
    CONSTRAINT [PK_FormularioConfiguracion] PRIMARY KEY ([FormularioConfiguracionId]),
    CONSTRAINT [FK_FormularioConfiguracion_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[OrdenCompra] (
    [OrdenCompraId] int NOT NULL IDENTITY,
    [ProveedorId] int NOT NULL,
    [Serie] nvarchar(20) NOT NULL,
    [Correlativo] int NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [Moneda] nvarchar(20) NOT NULL,
    [FechaEntregaEsperada] datetime2 NULL,
    [LugarEntrega] nvarchar(250) NOT NULL,
    [FormaPago] int NOT NULL,
    [CondicionPago] nvarchar(120) NOT NULL,
    [Observacion] nvarchar(1000) NOT NULL,
    [CondicionEntrega] nvarchar(250) NOT NULL,
    [Garantia] nvarchar(250) NOT NULL,
    [PorcentajeAdelanto] decimal(5,2) NOT NULL,
    [PlazoDias] int NOT NULL,
    [SubTotal] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [TotalFacturado] decimal(18,2) NOT NULL,
    [TotalPagado] decimal(18,2) NOT NULL,
    [TotalAplicado] decimal(18,2) NOT NULL,
    [TotalDevuelto] decimal(18,2) NOT NULL,
    [SaldoDisponible] decimal(18,2) NOT NULL,
    [PendienteFacturar] decimal(18,2) NOT NULL,
    [EstadoDocumento] int NOT NULL,
    [EstadoAprobacion] int NOT NULL,
    [EstadoFacturacion] int NOT NULL,
    [EstadoRecepcion] int NOT NULL,
    [EstadoFinanciero] int NOT NULL,
    [FechaAnulacion] datetime2 NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [UsuarioAnulacion] nvarchar(120) NOT NULL,
    [ProveedorTipoDocumento] nvarchar(30) NOT NULL,
    [ProveedorNumeroDocumento] nvarchar(20) NOT NULL,
    [ProveedorRazonSocial] nvarchar(250) NOT NULL,
    [ProveedorNombreComercial] nvarchar(250) NOT NULL,
    [ProveedorDireccion] nvarchar(500) NOT NULL,
    [ProveedorTelefono] nvarchar(40) NOT NULL,
    [ProveedorEmail] nvarchar(120) NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_OrdenCompra] PRIMARY KEY ([OrdenCompraId]),
    CONSTRAINT [FK_OrdenCompra_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrdenCompra_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[PagoProveedorAplicacion] (
    [PagoProveedorAplicacionId] int NOT NULL IDENTITY,
    [EmpresaId] int NOT NULL,
    [PagoProveedorId] int NOT NULL,
    [CompraId] int NOT NULL,
    [MontoAplicado] decimal(18,2) NOT NULL,
    [FechaAplicacion] datetime2 NOT NULL,
    [Estado] int NOT NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [FechaAnulacion] datetime2 NULL,
    [UsuarioRegistro] nvarchar(120) NOT NULL,
    [UsuarioAnulacion] nvarchar(120) NOT NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_PagoProveedorAplicacion] PRIMARY KEY ([PagoProveedorAplicacionId]),
    CONSTRAINT [FK_PagoProveedorAplicacion_Compra_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [erp].[Compra] ([CompraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PagoProveedorAplicacion_PagoProveedor_PagoProveedorId] FOREIGN KEY ([PagoProveedorId]) REFERENCES [erp].[PagoProveedor] ([PagoProveedorId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[PlantillaDocumento] (
    [PlantillaDocumentoId] int NOT NULL IDENTITY,
    [EmpresaId] int NULL,
    [TeamId] int NULL,
    [TipoDocumento] nvarchar(40) NOT NULL,
    [Nombre] nvarchar(160) NOT NULL,
    [Version] int NOT NULL,
    [Activa] bit NOT NULL,
    [EsPredeterminada] bit NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(120) NOT NULL,
    CONSTRAINT [PK_PlantillaDocumento] PRIMARY KEY ([PlantillaDocumentoId]),
    CONSTRAINT [FK_PlantillaDocumento_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[CondicionComercialItem] (
    [CondicionComercialItemId] int NOT NULL IDENTITY,
    [CondicionComercialPlantillaId] int NOT NULL,
    [Etiqueta] nvarchar(120) NOT NULL,
    [Texto] nvarchar(1000) NOT NULL,
    [Orden] int NOT NULL,
    [Visible] bit NOT NULL,
    CONSTRAINT [PK_CondicionComercialItem] PRIMARY KEY ([CondicionComercialItemId]),
    CONSTRAINT [FK_CondicionComercialItem_CondicionComercialPlantilla_CondicionComercialPlantillaId] FOREIGN KEY ([CondicionComercialPlantillaId]) REFERENCES [erp].[CondicionComercialPlantilla] ([CondicionComercialPlantillaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[TransferenciaFinanciera] (
    [TransferenciaFinancieraId] int NOT NULL IDENTITY,
    [Fecha] datetime2 NOT NULL,
    [CuentaOrigenId] int NOT NULL,
    [CuentaDestinoId] int NOT NULL,
    [Monto] decimal(18,2) NOT NULL,
    [Observacion] nvarchar(500) NOT NULL,
    [MovimientoEgresoId] int NULL,
    [MovimientoIngresoId] int NULL,
    [FechaAnulacion] datetime2 NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [UsuarioAnulacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_TransferenciaFinanciera] PRIMARY KEY ([TransferenciaFinancieraId]),
    CONSTRAINT [FK_TransferenciaFinanciera_CuentaFinanciera_CuentaDestinoId] FOREIGN KEY ([CuentaDestinoId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TransferenciaFinanciera_CuentaFinanciera_CuentaOrigenId] FOREIGN KEY ([CuentaOrigenId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TransferenciaFinanciera_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransferenciaFinanciera_MovimientoCaja_MovimientoEgresoId] FOREIGN KEY ([MovimientoEgresoId]) REFERENCES [erp].[MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TransferenciaFinanciera_MovimientoCaja_MovimientoIngresoId] FOREIGN KEY ([MovimientoIngresoId]) REFERENCES [erp].[MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[FormularioBloqueConfiguracion] (
    [FormularioBloqueConfiguracionId] int NOT NULL IDENTITY,
    [FormularioConfiguracionId] int NOT NULL,
    [Bloque] nvarchar(60) NOT NULL,
    [Titulo] nvarchar(120) NOT NULL,
    [Visible] bit NOT NULL,
    [Orden] int NOT NULL,
    [Colapsado] bit NOT NULL,
    CONSTRAINT [PK_FormularioBloqueConfiguracion] PRIMARY KEY ([FormularioBloqueConfiguracionId]),
    CONSTRAINT [FK_FormularioBloqueConfiguracion_FormularioConfiguracion_FormularioConfiguracionId] FOREIGN KEY ([FormularioConfiguracionId]) REFERENCES [erp].[FormularioConfiguracion] ([FormularioConfiguracionId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[FormularioBloqueProductoConfiguracion] (
    [FormularioBloqueProductoConfiguracionId] int NOT NULL IDENTITY,
    [FormularioConfiguracionId] int NOT NULL,
    [UnirProductosDuplicados] bit NOT NULL,
    [CantidadInicial] decimal(18,2) NOT NULL,
    [PermitirEditarPrecio] bit NOT NULL,
    [PermitirDescuento] bit NOT NULL,
    [MostrarStock] bit NOT NULL,
    [BloquearSinStock] bit NOT NULL,
    CONSTRAINT [PK_FormularioBloqueProductoConfiguracion] PRIMARY KEY ([FormularioBloqueProductoConfiguracionId]),
    CONSTRAINT [FK_FormularioBloqueProductoConfiguracion_FormularioConfiguracion_FormularioConfiguracionId] FOREIGN KEY ([FormularioConfiguracionId]) REFERENCES [erp].[FormularioConfiguracion] ([FormularioConfiguracionId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[FormularioCampoConfiguracion] (
    [FormularioCampoConfiguracionId] int NOT NULL IDENTITY,
    [FormularioConfiguracionId] int NOT NULL,
    [Bloque] nvarchar(60) NOT NULL,
    [Campo] nvarchar(80) NOT NULL,
    [Etiqueta] nvarchar(120) NOT NULL,
    [Visible] bit NOT NULL,
    [Obligatorio] bit NOT NULL,
    [SoloLectura] bit NOT NULL,
    [Orden] int NOT NULL,
    [Ancho] nvarchar(40) NOT NULL,
    [ValorDefecto] nvarchar(500) NULL,
    CONSTRAINT [PK_FormularioCampoConfiguracion] PRIMARY KEY ([FormularioCampoConfiguracionId]),
    CONSTRAINT [FK_FormularioCampoConfiguracion_FormularioConfiguracion_FormularioConfiguracionId] FOREIGN KEY ([FormularioConfiguracionId]) REFERENCES [erp].[FormularioConfiguracion] ([FormularioConfiguracionId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[OrdenCompraDetalle] (
    [OrdenCompraDetalleId] int NOT NULL IDENTITY,
    [OrdenCompraId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [UnidadMedida] nvarchar(20) NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [CostoUnitario] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [Orden] int NOT NULL,
    [CantidadFacturada] decimal(18,2) NOT NULL,
    [CantidadRecibida] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrdenCompraDetalle] PRIMARY KEY ([OrdenCompraDetalleId]),
    CONSTRAINT [FK_OrdenCompraDetalle_OrdenCompra_OrdenCompraId] FOREIGN KEY ([OrdenCompraId]) REFERENCES [erp].[OrdenCompra] ([OrdenCompraId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrdenCompraDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [erp].[Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[PlantillaDocumentoBloque] (
    [PlantillaDocumentoBloqueId] int NOT NULL IDENTITY,
    [PlantillaDocumentoId] int NOT NULL,
    [Bloque] nvarchar(60) NOT NULL,
    [Titulo] nvarchar(120) NOT NULL,
    [Visible] bit NOT NULL,
    [Orden] int NOT NULL,
    CONSTRAINT [PK_PlantillaDocumentoBloque] PRIMARY KEY ([PlantillaDocumentoBloqueId]),
    CONSTRAINT [FK_PlantillaDocumentoBloque_PlantillaDocumento_PlantillaDocumentoId] FOREIGN KEY ([PlantillaDocumentoId]) REFERENCES [erp].[PlantillaDocumento] ([PlantillaDocumentoId]) ON DELETE CASCADE
);
GO

UPDATE [erp].[MotivoNotaCredito] SET [FechaRegistro] = '2026-07-15T18:45:34.6338957'
WHERE [MotivoNotaCreditoId] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[MotivoNotaCredito] SET [FechaRegistro] = '2026-07-15T18:45:34.6339009'
WHERE [MotivoNotaCreditoId] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[MotivoNotaCredito] SET [FechaRegistro] = '2026-07-15T18:45:34.6339012'
WHERE [MotivoNotaCreditoId] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[MotivoNotaCredito] SET [FechaRegistro] = '2026-07-15T18:45:34.6339014'
WHERE [MotivoNotaCreditoId] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Ver TESORERIA', [Modulo] = N'TESORERIA'
WHERE [PermisoId] = 26;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver TESORERIA_CAJA', [Modulo] = N'TESORERIA_CAJA'
WHERE [PermisoId] = 27;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver TESORERIA_CAJABANCOS', [Modulo] = N'TESORERIA_CAJABANCOS'
WHERE [PermisoId] = 28;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Ver TESORERIA_CUENTASFINANCIERAS', [Modulo] = N'TESORERIA_CUENTASFINANCIERAS'
WHERE [PermisoId] = 29;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Crear TESORERIA_CUENTASFINANCIERAS', [Modulo] = N'TESORERIA_CUENTASFINANCIERAS'
WHERE [PermisoId] = 30;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Editar TESORERIA_CUENTASFINANCIERAS', [Modulo] = N'TESORERIA_CUENTASFINANCIERAS'
WHERE [PermisoId] = 31;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Anular TESORERIA_CUENTASFINANCIERAS', [Modulo] = N'TESORERIA_CUENTASFINANCIERAS'
WHERE [PermisoId] = 32;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Ver TESORERIA_COBROS', [Modulo] = N'TESORERIA_COBROS'
WHERE [PermisoId] = 33;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Crear TESORERIA_COBROS', [Modulo] = N'TESORERIA_COBROS'
WHERE [PermisoId] = 34;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular TESORERIA_COBROS', [Modulo] = N'TESORERIA_COBROS'
WHERE [PermisoId] = 35;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver TESORERIA_PAGOSPROVEEDORES', [Modulo] = N'TESORERIA_PAGOSPROVEEDORES'
WHERE [PermisoId] = 36;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Registrar', [Descripcion] = N'Registrar TESORERIA_PAGOSPROVEEDORES', [Modulo] = N'TESORERIA_PAGOSPROVEEDORES'
WHERE [PermisoId] = 37;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver TESORERIA_TRANSFERENCIAS', [Modulo] = N'TESORERIA_TRANSFERENCIAS'
WHERE [PermisoId] = 38;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Crear', [Descripcion] = N'Crear TESORERIA_TRANSFERENCIAS', [Modulo] = N'TESORERIA_TRANSFERENCIAS'
WHERE [PermisoId] = 39;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Anular TESORERIA_TRANSFERENCIAS', [Modulo] = N'TESORERIA_TRANSFERENCIAS'
WHERE [PermisoId] = 40;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Ver TESORERIA_CUENTASCLIENTES', [Modulo] = N'TESORERIA_CUENTASCLIENTES'
WHERE [PermisoId] = 41;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver TESORERIA_CUENTASPROVEEDORES', [Modulo] = N'TESORERIA_CUENTASPROVEEDORES'
WHERE [PermisoId] = 42;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver Categorias'
WHERE [PermisoId] = 43;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Crear', [Descripcion] = N'Crear Categorias'
WHERE [PermisoId] = 44;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Editar', [Descripcion] = N'Editar Categorias', [Modulo] = N'Categorias'
WHERE [PermisoId] = 45;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular Categorias', [Modulo] = N'Categorias'
WHERE [PermisoId] = 46;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver Productos', [Modulo] = N'Productos'
WHERE [PermisoId] = 47;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Crear', [Descripcion] = N'Crear Productos', [Modulo] = N'Productos'
WHERE [PermisoId] = 48;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Editar', [Descripcion] = N'Editar Productos', [Modulo] = N'Productos'
WHERE [PermisoId] = 49;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular Productos', [Modulo] = N'Productos'
WHERE [PermisoId] = 50;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver Clientes', [Modulo] = N'Clientes'
WHERE [PermisoId] = 51;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Crear', [Descripcion] = N'Crear Clientes', [Modulo] = N'Clientes'
WHERE [PermisoId] = 52;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Editar', [Descripcion] = N'Editar Clientes', [Modulo] = N'Clientes'
WHERE [PermisoId] = 53;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular Clientes', [Modulo] = N'Clientes'
WHERE [PermisoId] = 54;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver Proveedores', [Modulo] = N'Proveedores'
WHERE [PermisoId] = 55;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Crear', [Descripcion] = N'Crear Proveedores', [Modulo] = N'Proveedores'
WHERE [PermisoId] = 56;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Editar', [Descripcion] = N'Editar Proveedores', [Modulo] = N'Proveedores'
WHERE [PermisoId] = 57;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular Proveedores', [Modulo] = N'Proveedores'
WHERE [PermisoId] = 58;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver Compras', [Modulo] = N'Compras'
WHERE [PermisoId] = 59;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Crear', [Descripcion] = N'Crear Compras', [Modulo] = N'Compras'
WHERE [PermisoId] = 60;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular Compras', [Modulo] = N'Compras'
WHERE [PermisoId] = 61;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'RegistrarPago', [Descripcion] = N'RegistrarPago Compras', [Modulo] = N'Compras'
WHERE [PermisoId] = 62;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'AnularPago', [Descripcion] = N'AnularPago Compras', [Modulo] = N'Compras'
WHERE [PermisoId] = 63;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Descripcion] = N'Ver OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 64;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Registrar', [Descripcion] = N'Registrar OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 65;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Editar', [Descripcion] = N'Editar OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 66;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Aprobar', [Descripcion] = N'Aprobar OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 67;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'RegistrarPago', [Descripcion] = N'RegistrarPago OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 68;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'RegistrarCompra', [Descripcion] = N'RegistrarCompra OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 69;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'AplicarPagos', [Descripcion] = N'AplicarPagos OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 70;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'SolicitarDevolucion', [Descripcion] = N'SolicitarDevolucion OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 71;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Cerrar', [Descripcion] = N'Cerrar OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 72;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular OrdenesCompra', [Modulo] = N'OrdenesCompra'
WHERE [PermisoId] = 73;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Aplicar', [Descripcion] = N'Aplicar PagoProveedorAplicacion', [Modulo] = N'PagoProveedorAplicacion'
WHERE [PermisoId] = 74;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'AnularAplicacion', [Descripcion] = N'AnularAplicacion PagoProveedorAplicacion', [Modulo] = N'PagoProveedorAplicacion'
WHERE [PermisoId] = 75;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver Gastos', [Modulo] = N'Gastos'
WHERE [PermisoId] = 76;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Crear', [Descripcion] = N'Crear Gastos', [Modulo] = N'Gastos'
WHERE [PermisoId] = 77;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Editar', [Descripcion] = N'Editar Gastos', [Modulo] = N'Gastos'
WHERE [PermisoId] = 78;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular Gastos', [Modulo] = N'Gastos'
WHERE [PermisoId] = 79;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver Ingresos', [Modulo] = N'Ingresos'
WHERE [PermisoId] = 80;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Crear', [Descripcion] = N'Crear Ingresos', [Modulo] = N'Ingresos'
WHERE [PermisoId] = 81;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Editar', [Descripcion] = N'Editar Ingresos', [Modulo] = N'Ingresos'
WHERE [PermisoId] = 82;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Anular', [Descripcion] = N'Anular Ingresos', [Modulo] = N'Ingresos'
WHERE [PermisoId] = 83;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[Permiso] SET [Accion] = N'Ver', [Descripcion] = N'Ver Devoluciones', [Modulo] = N'Devoluciones'
WHERE [PermisoId] = 84;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermisoId', N'Accion', N'Descripcion', N'Estado', N'Modulo') AND [object_id] = OBJECT_ID(N'[erp].[Permiso]'))
    SET IDENTITY_INSERT [erp].[Permiso] ON;
INSERT INTO [erp].[Permiso] ([PermisoId], [Accion], [Descripcion], [Estado], [Modulo])
VALUES (85, N'Registrar', N'Registrar Devoluciones', 1, N'Devoluciones'),
(86, N'Ver', N'Ver Caja', 1, N'Caja'),
(87, N'Ver', N'Ver ReporteGeneral', 1, N'ReporteGeneral'),
(88, N'Ver', N'Ver PropuestasComerciales', 1, N'PropuestasComerciales'),
(89, N'Ver', N'Ver ReporteNotasPedido', 1, N'ReporteNotasPedido'),
(90, N'Ver', N'Ver ReporteComprobantes', 1, N'ReporteComprobantes'),
(91, N'Ver', N'Ver CuentasPorPagar', 1, N'CuentasPorPagar'),
(92, N'Ver', N'Ver DevolucionesProveedor', 1, N'DevolucionesProveedor'),
(93, N'Ver', N'Ver ReporteCaja', 1, N'ReporteCaja'),
(94, N'Ver', N'Ver EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(95, N'Ver', N'Ver Empresas', 1, N'Empresas'),
(96, N'Crear', N'Crear Empresas', 1, N'Empresas'),
(97, N'Editar', N'Editar Empresas', 1, N'Empresas'),
(98, N'Anular', N'Anular Empresas', 1, N'Empresas'),
(99, N'Ver', N'Ver Usuarios', 1, N'Usuarios'),
(100, N'Crear', N'Crear Usuarios', 1, N'Usuarios'),
(101, N'Editar', N'Editar Usuarios', 1, N'Usuarios'),
(102, N'RestablecerPassword', N'RestablecerPassword Usuarios', 1, N'Usuarios'),
(103, N'Ver', N'Ver Roles', 1, N'Roles'),
(104, N'Crear', N'Crear Roles', 1, N'Roles'),
(105, N'Editar', N'Editar Roles', 1, N'Roles'),
(106, N'Ver', N'Ver Configuracion', 1, N'Configuracion'),
(107, N'Crear', N'Crear Configuracion', 1, N'Configuracion'),
(108, N'Editar', N'Editar Configuracion', 1, N'Configuracion'),
(109, N'Configurar', N'Configurar Configuracion', 1, N'Configuracion'),
(110, N'Ver', N'Ver NubefactLogs', 1, N'NubefactLogs'),
(111, N'Ver', N'Ver ErroresAplicacion', 1, N'ErroresAplicacion'),
(112, N'Revisar', N'Revisar ErroresAplicacion', 1, N'ErroresAplicacion');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermisoId', N'Accion', N'Descripcion', N'Estado', N'Modulo') AND [object_id] = OBJECT_ID(N'[erp].[Permiso]'))
    SET IDENTITY_INSERT [erp].[Permiso] OFF;
GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 85, [RolId] = 1
WHERE [RolPermisoId] = 85;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 86, [RolId] = 1
WHERE [RolPermisoId] = 86;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 87, [RolId] = 1
WHERE [RolPermisoId] = 87;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 88, [RolId] = 1
WHERE [RolPermisoId] = 88;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 89, [RolId] = 1
WHERE [RolPermisoId] = 89;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 90, [RolId] = 1
WHERE [RolPermisoId] = 90;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 91, [RolId] = 1
WHERE [RolPermisoId] = 91;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 92, [RolId] = 1
WHERE [RolPermisoId] = 92;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 93, [RolId] = 1
WHERE [RolPermisoId] = 93;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 94, [RolId] = 1
WHERE [RolPermisoId] = 94;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 95, [RolId] = 1
WHERE [RolPermisoId] = 95;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 96, [RolId] = 1
WHERE [RolPermisoId] = 96;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 97, [RolId] = 1
WHERE [RolPermisoId] = 97;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 98, [RolId] = 1
WHERE [RolPermisoId] = 98;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 99, [RolId] = 1
WHERE [RolPermisoId] = 99;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 100, [RolId] = 1
WHERE [RolPermisoId] = 100;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 101, [RolId] = 1
WHERE [RolPermisoId] = 101;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 102, [RolId] = 1
WHERE [RolPermisoId] = 102;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 103, [RolId] = 1
WHERE [RolPermisoId] = 103;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 104, [RolId] = 1
WHERE [RolPermisoId] = 104;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 105, [RolId] = 1
WHERE [RolPermisoId] = 105;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 106, [RolId] = 1
WHERE [RolPermisoId] = 106;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 107, [RolId] = 1
WHERE [RolPermisoId] = 107;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 108, [RolId] = 1
WHERE [RolPermisoId] = 108;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 109, [RolId] = 1
WHERE [RolPermisoId] = 109;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 110, [RolId] = 1
WHERE [RolPermisoId] = 110;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 111, [RolId] = 1
WHERE [RolPermisoId] = 111;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 112, [RolId] = 1
WHERE [RolPermisoId] = 112;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 1
WHERE [RolPermisoId] = 113;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 2
WHERE [RolPermisoId] = 114;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 3
WHERE [RolPermisoId] = 115;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 4
WHERE [RolPermisoId] = 116;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 5
WHERE [RolPermisoId] = 117;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 6
WHERE [RolPermisoId] = 118;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 7
WHERE [RolPermisoId] = 119;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 8
WHERE [RolPermisoId] = 120;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 9
WHERE [RolPermisoId] = 121;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 10
WHERE [RolPermisoId] = 122;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 11
WHERE [RolPermisoId] = 123;
SELECT @@ROWCOUNT;

GO

UPDATE [erp].[RolPermiso] SET [PermisoId] = 12
WHERE [RolPermisoId] = 124;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolPermisoId', N'PermisoId', N'RolId') AND [object_id] = OBJECT_ID(N'[erp].[RolPermiso]'))
    SET IDENTITY_INSERT [erp].[RolPermiso] ON;
INSERT INTO [erp].[RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
VALUES (125, 13, 2),
(126, 14, 2),
(127, 15, 2),
(128, 16, 2),
(129, 17, 2),
(130, 18, 2),
(131, 19, 2),
(132, 20, 2),
(133, 22, 2),
(134, 23, 2),
(135, 24, 2),
(136, 25, 2),
(137, 26, 2),
(138, 27, 2),
(139, 28, 2),
(140, 33, 2),
(141, 34, 2),
(142, 35, 2),
(143, 38, 2),
(144, 39, 2),
(145, 40, 2),
(146, 41, 2),
(147, 43, 2),
(148, 44, 2),
(149, 45, 2),
(150, 46, 2),
(151, 47, 2),
(152, 48, 2),
(153, 49, 2),
(154, 50, 2),
(155, 51, 2),
(156, 52, 2),
(157, 53, 2),
(158, 54, 2),
(159, 84, 2),
(160, 85, 2),
(161, 86, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolPermisoId', N'PermisoId', N'RolId') AND [object_id] = OBJECT_ID(N'[erp].[RolPermiso]'))
    SET IDENTITY_INSERT [erp].[RolPermiso] OFF;
GO

CREATE INDEX [IX_PagoProveedor_CuentaFinancieraId] ON [erp].[PagoProveedor] ([CuentaFinancieraId]);
GO

CREATE INDEX [IX_PagoProveedor_EmpresaId_CuentaFinancieraId] ON [erp].[PagoProveedor] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_PagoProveedor_EmpresaId_OrdenCompraId] ON [erp].[PagoProveedor] ([EmpresaId], [OrdenCompraId]);
GO

CREATE INDEX [IX_PagoProveedor_OrdenCompraId] ON [erp].[PagoProveedor] ([OrdenCompraId]);
GO

CREATE INDEX [IX_MovimientoCaja_CuentaFinancieraId] ON [erp].[MovimientoCaja] ([CuentaFinancieraId]);
GO

CREATE INDEX [IX_MovimientoCaja_EmpresaId_CuentaFinancieraId] ON [erp].[MovimientoCaja] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_Ingreso_ClienteId] ON [erp].[Ingreso] ([ClienteId]);
GO

CREATE INDEX [IX_Ingreso_CuentaFinancieraId] ON [erp].[Ingreso] ([CuentaFinancieraId]);
GO

CREATE INDEX [IX_Ingreso_EmpresaId_ClienteId] ON [erp].[Ingreso] ([EmpresaId], [ClienteId]);
GO

CREATE INDEX [IX_Ingreso_EmpresaId_CuentaFinancieraId] ON [erp].[Ingreso] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_Gasto_CuentaFinancieraId] ON [erp].[Gasto] ([CuentaFinancieraId]);
GO

CREATE INDEX [IX_Gasto_EmpresaId_CuentaFinancieraId] ON [erp].[Gasto] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_Gasto_EmpresaId_ProveedorId] ON [erp].[Gasto] ([EmpresaId], [ProveedorId]);
GO

CREATE INDEX [IX_Gasto_ProveedorId] ON [erp].[Gasto] ([ProveedorId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_OrdenCompraId] ON [erp].[Devolucion] ([EmpresaId], [OrdenCompraId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_PagoProveedorId] ON [erp].[Devolucion] ([EmpresaId], [PagoProveedorId]);
GO

CREATE INDEX [IX_Devolucion_OrdenCompraId] ON [erp].[Devolucion] ([OrdenCompraId]);
GO

CREATE INDEX [IX_Devolucion_PagoProveedorId] ON [erp].[Devolucion] ([PagoProveedorId]);
GO

CREATE INDEX [IX_Compra_EmpresaId_OrdenCompraId] ON [erp].[Compra] ([EmpresaId], [OrdenCompraId]);
GO

CREATE INDEX [IX_Compra_OrdenCompraId] ON [erp].[Compra] ([OrdenCompraId]);
GO

CREATE INDEX [IX_CobroCliente_CuentaFinancieraId] ON [erp].[CobroCliente] ([CuentaFinancieraId]);
GO

CREATE INDEX [IX_CobroCliente_EmpresaId_CuentaFinancieraId] ON [erp].[CobroCliente] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_Cliente_EmpresaId] ON [erp].[Cliente] ([EmpresaId]);
GO

CREATE INDEX [IX_CondicionComercialItem_CondicionComercialPlantillaId] ON [erp].[CondicionComercialItem] ([CondicionComercialPlantillaId]);
GO

CREATE INDEX [IX_CondicionComercialPlantilla_EmpresaId_TeamId_TipoDocumento_EsPredeterminada_Activa] ON [erp].[CondicionComercialPlantilla] ([EmpresaId], [TeamId], [TipoDocumento], [EsPredeterminada], [Activa]);
GO

CREATE INDEX [IX_CotizacionCondicionSnapshot_CotizacionId_Orden] ON [erp].[CotizacionCondicionSnapshot] ([CotizacionId], [Orden]);
GO

CREATE UNIQUE INDEX [IX_CuentaFinanciera_EmpresaId_Nombre] ON [erp].[CuentaFinanciera] ([EmpresaId], [Nombre]);
GO

CREATE INDEX [IX_CuentaFinanciera_EmpresaId_Tipo] ON [erp].[CuentaFinanciera] ([EmpresaId], [Tipo]);
GO

CREATE UNIQUE INDEX [IX_FormularioBloqueConfiguracion_FormularioConfiguracionId_Bloque] ON [erp].[FormularioBloqueConfiguracion] ([FormularioConfiguracionId], [Bloque]);
GO

CREATE UNIQUE INDEX [IX_FormularioBloqueProductoConfiguracion_FormularioConfiguracionId] ON [erp].[FormularioBloqueProductoConfiguracion] ([FormularioConfiguracionId]);
GO

CREATE UNIQUE INDEX [IX_FormularioCampoConfiguracion_FormularioConfiguracionId_Bloque_Campo] ON [erp].[FormularioCampoConfiguracion] ([FormularioConfiguracionId], [Bloque], [Campo]);
GO

CREATE INDEX [IX_FormularioConfiguracion_EmpresaId_TeamId_TipoDocumento_Activo] ON [erp].[FormularioConfiguracion] ([EmpresaId], [TeamId], [TipoDocumento], [Activo]);
GO

CREATE INDEX [IX_OrdenCompra_EmpresaId] ON [erp].[OrdenCompra] ([EmpresaId]);
GO

CREATE INDEX [IX_OrdenCompra_EmpresaId_ProveedorId] ON [erp].[OrdenCompra] ([EmpresaId], [ProveedorId]);
GO

CREATE UNIQUE INDEX [IX_OrdenCompra_EmpresaId_Serie_Correlativo] ON [erp].[OrdenCompra] ([EmpresaId], [Serie], [Correlativo]);
GO

CREATE INDEX [IX_OrdenCompra_ProveedorId] ON [erp].[OrdenCompra] ([ProveedorId]);
GO

CREATE INDEX [IX_OrdenCompraDetalle_OrdenCompraId] ON [erp].[OrdenCompraDetalle] ([OrdenCompraId]);
GO

CREATE INDEX [IX_OrdenCompraDetalle_ProductoId] ON [erp].[OrdenCompraDetalle] ([ProductoId]);
GO

CREATE INDEX [IX_PagoProveedorAplicacion_CompraId] ON [erp].[PagoProveedorAplicacion] ([CompraId]);
GO

CREATE INDEX [IX_PagoProveedorAplicacion_EmpresaId] ON [erp].[PagoProveedorAplicacion] ([EmpresaId]);
GO

CREATE INDEX [IX_PagoProveedorAplicacion_EmpresaId_CompraId] ON [erp].[PagoProveedorAplicacion] ([EmpresaId], [CompraId]);
GO

CREATE INDEX [IX_PagoProveedorAplicacion_EmpresaId_Estado] ON [erp].[PagoProveedorAplicacion] ([EmpresaId], [Estado]);
GO

CREATE INDEX [IX_PagoProveedorAplicacion_EmpresaId_PagoProveedorId] ON [erp].[PagoProveedorAplicacion] ([EmpresaId], [PagoProveedorId]);
GO

CREATE INDEX [IX_PagoProveedorAplicacion_PagoProveedorId_CompraId] ON [erp].[PagoProveedorAplicacion] ([PagoProveedorId], [CompraId]);
GO

CREATE INDEX [IX_PlantillaDocumento_EmpresaId_TeamId_TipoDocumento_EsPredeterminada_Activa] ON [erp].[PlantillaDocumento] ([EmpresaId], [TeamId], [TipoDocumento], [EsPredeterminada], [Activa]);
GO

CREATE UNIQUE INDEX [IX_PlantillaDocumentoBloque_PlantillaDocumentoId_Bloque] ON [erp].[PlantillaDocumentoBloque] ([PlantillaDocumentoId], [Bloque]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_CuentaDestinoId] ON [erp].[TransferenciaFinanciera] ([CuentaDestinoId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_CuentaOrigenId] ON [erp].[TransferenciaFinanciera] ([CuentaOrigenId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_CuentaDestinoId] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [CuentaDestinoId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_CuentaOrigenId] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [CuentaOrigenId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_Fecha] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [Fecha]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_MovimientoEgresoId] ON [erp].[TransferenciaFinanciera] ([MovimientoEgresoId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_MovimientoIngresoId] ON [erp].[TransferenciaFinanciera] ([MovimientoIngresoId]);
GO

ALTER TABLE [erp].[Cliente] ADD CONSTRAINT [FK_Cliente_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE;
GO

ALTER TABLE [erp].[CobroCliente] ADD CONSTRAINT [FK_CobroCliente_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[Compra] ADD CONSTRAINT [FK_Compra_OrdenCompra_OrdenCompraId] FOREIGN KEY ([OrdenCompraId]) REFERENCES [erp].[OrdenCompra] ([OrdenCompraId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[Devolucion] ADD CONSTRAINT [FK_Devolucion_OrdenCompra_OrdenCompraId] FOREIGN KEY ([OrdenCompraId]) REFERENCES [erp].[OrdenCompra] ([OrdenCompraId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[Devolucion] ADD CONSTRAINT [FK_Devolucion_PagoProveedor_PagoProveedorId] FOREIGN KEY ([PagoProveedorId]) REFERENCES [erp].[PagoProveedor] ([PagoProveedorId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[Gasto] ADD CONSTRAINT [FK_Gasto_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[Gasto] ADD CONSTRAINT [FK_Gasto_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[Ingreso] ADD CONSTRAINT [FK_Ingreso_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[Ingreso] ADD CONSTRAINT [FK_Ingreso_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[MovimientoCaja] ADD CONSTRAINT [FK_MovimientoCaja_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[PagoProveedor] ADD CONSTRAINT [FK_PagoProveedor_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
GO

ALTER TABLE [erp].[PagoProveedor] ADD CONSTRAINT [FK_PagoProveedor_OrdenCompra_OrdenCompraId] FOREIGN KEY ([OrdenCompraId]) REFERENCES [erp].[OrdenCompra] ([OrdenCompraId]) ON DELETE NO ACTION;
GO

INSERT INTO [erp].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715234537_AddOrdenCompraYPagoProveedorAplicacion', N'8.0.6');
GO

COMMIT;
GO

