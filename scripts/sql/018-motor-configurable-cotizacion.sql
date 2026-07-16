/*
Motor configurable de formularios y plantillas - piloto Cotizacion.
Seguro e idempotente: crea tablas si no existen, agrega semillas globales COTIZACION si faltan.
No modifica cotizaciones ni data historica existente.
*/

IF SCHEMA_ID(N'erp') IS NULL
    THROW 50000, 'No existe el esquema erp.', 1;

IF OBJECT_ID(N'erp.FormularioConfiguracion', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[FormularioConfiguracion] (
        [FormularioConfiguracionId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FormularioConfiguracion] PRIMARY KEY,
        [EmpresaId] int NULL,
        [TeamId] int NULL,
        [TipoDocumento] nvarchar(40) NOT NULL,
        [Nombre] nvarchar(160) NOT NULL,
        [Version] int NOT NULL CONSTRAINT [DF_FormularioConfiguracion_Version] DEFAULT 1,
        [Activo] bit NOT NULL CONSTRAINT [DF_FormularioConfiguracion_Activo] DEFAULT 1,
        [FechaRegistro] datetime2 NOT NULL CONSTRAINT [DF_FormularioConfiguracion_FechaRegistro] DEFAULT SYSDATETIME(),
        [UsuarioRegistro] nvarchar(120) NOT NULL CONSTRAINT [DF_FormularioConfiguracion_UsuarioRegistro] DEFAULT N'SISTEMA'
    );
END;

IF OBJECT_ID(N'erp.FormularioBloqueConfiguracion', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[FormularioBloqueConfiguracion] (
        [FormularioBloqueConfiguracionId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FormularioBloqueConfiguracion] PRIMARY KEY,
        [FormularioConfiguracionId] int NOT NULL,
        [Bloque] nvarchar(60) NOT NULL,
        [Titulo] nvarchar(120) NOT NULL CONSTRAINT [DF_FormularioBloqueConfiguracion_Titulo] DEFAULT N'',
        [Visible] bit NOT NULL CONSTRAINT [DF_FormularioBloqueConfiguracion_Visible] DEFAULT 1,
        [Orden] int NOT NULL,
        [Colapsado] bit NOT NULL CONSTRAINT [DF_FormularioBloqueConfiguracion_Colapsado] DEFAULT 0
    );
END;

IF OBJECT_ID(N'erp.FormularioCampoConfiguracion', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[FormularioCampoConfiguracion] (
        [FormularioCampoConfiguracionId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FormularioCampoConfiguracion] PRIMARY KEY,
        [FormularioConfiguracionId] int NOT NULL,
        [Bloque] nvarchar(60) NOT NULL,
        [Campo] nvarchar(80) NOT NULL,
        [Etiqueta] nvarchar(120) NOT NULL CONSTRAINT [DF_FormularioCampoConfiguracion_Etiqueta] DEFAULT N'',
        [Visible] bit NOT NULL CONSTRAINT [DF_FormularioCampoConfiguracion_Visible] DEFAULT 1,
        [Obligatorio] bit NOT NULL CONSTRAINT [DF_FormularioCampoConfiguracion_Obligatorio] DEFAULT 0,
        [SoloLectura] bit NOT NULL CONSTRAINT [DF_FormularioCampoConfiguracion_SoloLectura] DEFAULT 0,
        [Orden] int NOT NULL,
        [Ancho] nvarchar(40) NOT NULL CONSTRAINT [DF_FormularioCampoConfiguracion_Ancho] DEFAULT N'',
        [ValorDefecto] nvarchar(500) NULL
    );
END;

IF OBJECT_ID(N'erp.CondicionComercialPlantilla', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[CondicionComercialPlantilla] (
        [CondicionComercialPlantillaId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CondicionComercialPlantilla] PRIMARY KEY,
        [EmpresaId] int NULL,
        [TeamId] int NULL,
        [TipoDocumento] nvarchar(40) NOT NULL,
        [Nombre] nvarchar(160) NOT NULL,
        [EsPredeterminada] bit NOT NULL CONSTRAINT [DF_CondicionComercialPlantilla_EsPredeterminada] DEFAULT 0,
        [Activa] bit NOT NULL CONSTRAINT [DF_CondicionComercialPlantilla_Activa] DEFAULT 1,
        [FechaRegistro] datetime2 NOT NULL CONSTRAINT [DF_CondicionComercialPlantilla_FechaRegistro] DEFAULT SYSDATETIME(),
        [UsuarioRegistro] nvarchar(120) NOT NULL CONSTRAINT [DF_CondicionComercialPlantilla_UsuarioRegistro] DEFAULT N'SISTEMA'
    );
END;

IF OBJECT_ID(N'erp.CondicionComercialItem', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[CondicionComercialItem] (
        [CondicionComercialItemId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CondicionComercialItem] PRIMARY KEY,
        [CondicionComercialPlantillaId] int NOT NULL,
        [Etiqueta] nvarchar(120) NOT NULL CONSTRAINT [DF_CondicionComercialItem_Etiqueta] DEFAULT N'',
        [Texto] nvarchar(1000) NOT NULL,
        [Orden] int NOT NULL,
        [Visible] bit NOT NULL CONSTRAINT [DF_CondicionComercialItem_Visible] DEFAULT 1
    );
END;

IF OBJECT_ID(N'erp.CotizacionCondicionSnapshot', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[CotizacionCondicionSnapshot] (
        [CotizacionCondicionSnapshotId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CotizacionCondicionSnapshot] PRIMARY KEY,
        [CotizacionId] int NOT NULL,
        [Etiqueta] nvarchar(120) NOT NULL CONSTRAINT [DF_CotizacionCondicionSnapshot_Etiqueta] DEFAULT N'',
        [Texto] nvarchar(1000) NOT NULL,
        [Orden] int NOT NULL
    );
END;

IF OBJECT_ID(N'erp.PlantillaDocumento', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[PlantillaDocumento] (
        [PlantillaDocumentoId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PlantillaDocumento] PRIMARY KEY,
        [EmpresaId] int NULL,
        [TeamId] int NULL,
        [TipoDocumento] nvarchar(40) NOT NULL,
        [Nombre] nvarchar(160) NOT NULL,
        [Version] int NOT NULL CONSTRAINT [DF_PlantillaDocumento_Version] DEFAULT 1,
        [Activa] bit NOT NULL CONSTRAINT [DF_PlantillaDocumento_Activa] DEFAULT 1,
        [EsPredeterminada] bit NOT NULL CONSTRAINT [DF_PlantillaDocumento_EsPredeterminada] DEFAULT 0,
        [FechaRegistro] datetime2 NOT NULL CONSTRAINT [DF_PlantillaDocumento_FechaRegistro] DEFAULT SYSDATETIME(),
        [UsuarioRegistro] nvarchar(120) NOT NULL CONSTRAINT [DF_PlantillaDocumento_UsuarioRegistro] DEFAULT N'SISTEMA'
    );
END;

IF OBJECT_ID(N'erp.PlantillaDocumentoBloque', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[PlantillaDocumentoBloque] (
        [PlantillaDocumentoBloqueId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PlantillaDocumentoBloque] PRIMARY KEY,
        [PlantillaDocumentoId] int NOT NULL,
        [Bloque] nvarchar(60) NOT NULL,
        [Titulo] nvarchar(120) NOT NULL CONSTRAINT [DF_PlantillaDocumentoBloque_Titulo] DEFAULT N'',
        [Visible] bit NOT NULL CONSTRAINT [DF_PlantillaDocumentoBloque_Visible] DEFAULT 1,
        [Orden] int NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FormularioConfiguracion_Empresa_EmpresaId')
    ALTER TABLE [erp].[FormularioConfiguracion] ADD CONSTRAINT [FK_FormularioConfiguracion_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa]([EmpresaId]);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FormularioBloqueConfiguracion_FormularioConfiguracion_FormularioConfiguracionId')
    ALTER TABLE [erp].[FormularioBloqueConfiguracion] ADD CONSTRAINT [FK_FormularioBloqueConfiguracion_FormularioConfiguracion_FormularioConfiguracionId] FOREIGN KEY ([FormularioConfiguracionId]) REFERENCES [erp].[FormularioConfiguracion]([FormularioConfiguracionId]) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FormularioCampoConfiguracion_FormularioConfiguracion_FormularioConfiguracionId')
    ALTER TABLE [erp].[FormularioCampoConfiguracion] ADD CONSTRAINT [FK_FormularioCampoConfiguracion_FormularioConfiguracion_FormularioConfiguracionId] FOREIGN KEY ([FormularioConfiguracionId]) REFERENCES [erp].[FormularioConfiguracion]([FormularioConfiguracionId]) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CondicionComercialPlantilla_Empresa_EmpresaId')
    ALTER TABLE [erp].[CondicionComercialPlantilla] ADD CONSTRAINT [FK_CondicionComercialPlantilla_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa]([EmpresaId]);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CondicionComercialItem_CondicionComercialPlantilla_CondicionComercialPlantillaId')
    ALTER TABLE [erp].[CondicionComercialItem] ADD CONSTRAINT [FK_CondicionComercialItem_CondicionComercialPlantilla_CondicionComercialPlantillaId] FOREIGN KEY ([CondicionComercialPlantillaId]) REFERENCES [erp].[CondicionComercialPlantilla]([CondicionComercialPlantillaId]) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CotizacionCondicionSnapshot_Cotizacion_CotizacionId')
    ALTER TABLE [erp].[CotizacionCondicionSnapshot] ADD CONSTRAINT [FK_CotizacionCondicionSnapshot_Cotizacion_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [erp].[Cotizacion]([CotizacionId]) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PlantillaDocumento_Empresa_EmpresaId')
    ALTER TABLE [erp].[PlantillaDocumento] ADD CONSTRAINT [FK_PlantillaDocumento_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa]([EmpresaId]);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PlantillaDocumentoBloque_PlantillaDocumento_PlantillaDocumentoId')
    ALTER TABLE [erp].[PlantillaDocumentoBloque] ADD CONSTRAINT [FK_PlantillaDocumentoBloque_PlantillaDocumento_PlantillaDocumentoId] FOREIGN KEY ([PlantillaDocumentoId]) REFERENCES [erp].[PlantillaDocumento]([PlantillaDocumentoId]) ON DELETE CASCADE;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FormularioConfiguracion_Alcance' AND object_id = OBJECT_ID(N'erp.FormularioConfiguracion'))
    CREATE INDEX [IX_FormularioConfiguracion_Alcance] ON [erp].[FormularioConfiguracion] ([EmpresaId], [TeamId], [TipoDocumento], [Activo]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_FormularioBloqueConfiguracion_Bloque' AND object_id = OBJECT_ID(N'erp.FormularioBloqueConfiguracion'))
    CREATE UNIQUE INDEX [UX_FormularioBloqueConfiguracion_Bloque] ON [erp].[FormularioBloqueConfiguracion] ([FormularioConfiguracionId], [Bloque]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_FormularioCampoConfiguracion_Campo' AND object_id = OBJECT_ID(N'erp.FormularioCampoConfiguracion'))
    CREATE UNIQUE INDEX [UX_FormularioCampoConfiguracion_Campo] ON [erp].[FormularioCampoConfiguracion] ([FormularioConfiguracionId], [Bloque], [Campo]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CondicionComercialPlantilla_Alcance' AND object_id = OBJECT_ID(N'erp.CondicionComercialPlantilla'))
    CREATE INDEX [IX_CondicionComercialPlantilla_Alcance] ON [erp].[CondicionComercialPlantilla] ([EmpresaId], [TeamId], [TipoDocumento], [EsPredeterminada], [Activa]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CotizacionCondicionSnapshot_Cotizacion' AND object_id = OBJECT_ID(N'erp.CotizacionCondicionSnapshot'))
    CREATE INDEX [IX_CotizacionCondicionSnapshot_Cotizacion] ON [erp].[CotizacionCondicionSnapshot] ([CotizacionId], [Orden]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PlantillaDocumento_Alcance' AND object_id = OBJECT_ID(N'erp.PlantillaDocumento'))
    CREATE INDEX [IX_PlantillaDocumento_Alcance] ON [erp].[PlantillaDocumento] ([EmpresaId], [TeamId], [TipoDocumento], [EsPredeterminada], [Activa]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PlantillaDocumentoBloque_Bloque' AND object_id = OBJECT_ID(N'erp.PlantillaDocumentoBloque'))
    CREATE UNIQUE INDEX [UX_PlantillaDocumentoBloque_Bloque] ON [erp].[PlantillaDocumentoBloque] ([PlantillaDocumentoId], [Bloque]);

DECLARE @FormularioId int;
SELECT @FormularioId = [FormularioConfiguracionId]
FROM [erp].[FormularioConfiguracion]
WHERE [EmpresaId] IS NULL AND [TeamId] IS NULL AND [TipoDocumento] = N'COTIZACION' AND [Activo] = 1;

IF @FormularioId IS NULL
BEGIN
    INSERT INTO [erp].[FormularioConfiguracion] ([EmpresaId], [TeamId], [TipoDocumento], [Nombre], [Version], [Activo], [UsuarioRegistro])
    VALUES (NULL, NULL, N'COTIZACION', N'Cotizacion estandar SaaS', 1, 1, N'SISTEMA');
    SET @FormularioId = SCOPE_IDENTITY();
END;

DECLARE @Bloques TABLE (Bloque nvarchar(60), Titulo nvarchar(120), Orden int);
INSERT INTO @Bloques VALUES
(N'GENERAL', N'Informacion general', 10),
(N'CLIENTE', N'Cliente', 20),
(N'PRODUCTOS', N'Detalle de productos', 30),
(N'CONDICIONES', N'Condiciones comerciales', 40),
(N'TOTALES', N'Totales', 50),
(N'ACCIONES', N'Acciones', 60);

INSERT INTO [erp].[FormularioBloqueConfiguracion] ([FormularioConfiguracionId], [Bloque], [Titulo], [Visible], [Orden], [Colapsado])
SELECT @FormularioId, b.Bloque, b.Titulo, 1, b.Orden, 0
FROM @Bloques b
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[FormularioBloqueConfiguracion] x WHERE x.FormularioConfiguracionId = @FormularioId AND x.Bloque = b.Bloque
);

DECLARE @Campos TABLE (Bloque nvarchar(60), Campo nvarchar(80), Etiqueta nvarchar(120), Orden int, Obligatorio bit, SoloLectura bit, Ancho nvarchar(40), ValorDefecto nvarchar(500));
INSERT INTO @Campos VALUES
(N'GENERAL', N'FechaEmision', N'Fecha', 10, 1, 0, N'col', NULL),
(N'GENERAL', N'ValidezDias', N'Validez', 20, 0, 1, N'col', N'15'),
(N'GENERAL', N'Moneda', N'Moneda', 30, 0, 1, N'col', N'Soles'),
(N'GENERAL', N'FormaPago', N'Forma de pago', 40, 1, 0, N'col', NULL),
(N'CLIENTE', N'ClienteId', N'Cliente', 10, 1, 0, N'full', NULL),
(N'PRODUCTOS', N'ProductoBuscador', N'Buscar producto', 10, 0, 0, N'full', NULL),
(N'PRODUCTOS', N'DetalleProductos', N'Detalle de productos', 20, 1, 0, N'full', NULL),
(N'CONDICIONES', N'CondicionesVenta', N'Condiciones comerciales', 10, 0, 1, N'full', NULL),
(N'TOTALES', N'SubtotalExonerado', N'Subtotal exonerado', 10, 0, 1, N'col', NULL),
(N'TOTALES', N'SubtotalGravado', N'Subtotal gravado', 20, 0, 1, N'col', NULL),
(N'TOTALES', N'Descuento', N'Descuento', 30, 0, 1, N'col', NULL),
(N'TOTALES', N'Igv', N'IGV', 40, 0, 1, N'col', NULL),
(N'TOTALES', N'Total', N'Total', 50, 0, 1, N'col', NULL);

INSERT INTO [erp].[FormularioCampoConfiguracion] ([FormularioConfiguracionId], [Bloque], [Campo], [Etiqueta], [Visible], [Obligatorio], [SoloLectura], [Orden], [Ancho], [ValorDefecto])
SELECT @FormularioId, c.Bloque, c.Campo, c.Etiqueta, 1, c.Obligatorio, c.SoloLectura, c.Orden, c.Ancho, c.ValorDefecto
FROM @Campos c
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[FormularioCampoConfiguracion] x WHERE x.FormularioConfiguracionId = @FormularioId AND x.Bloque = c.Bloque AND x.Campo = c.Campo
);

DECLARE @CondicionId int;
SELECT @CondicionId = [CondicionComercialPlantillaId]
FROM [erp].[CondicionComercialPlantilla]
WHERE [EmpresaId] IS NULL AND [TeamId] IS NULL AND [TipoDocumento] = N'COTIZACION' AND [Activa] = 1 AND [EsPredeterminada] = 1;

IF @CondicionId IS NULL
BEGIN
    INSERT INTO [erp].[CondicionComercialPlantilla] ([EmpresaId], [TeamId], [TipoDocumento], [Nombre], [EsPredeterminada], [Activa], [UsuarioRegistro])
    VALUES (NULL, NULL, N'COTIZACION', N'Condiciones estandar SaaS', 1, 1, N'SISTEMA');
    SET @CondicionId = SCOPE_IDENTITY();
END;

DECLARE @Condiciones TABLE (Etiqueta nvarchar(120), Texto nvarchar(1000), Orden int);
INSERT INTO @Condiciones VALUES
(N'Validez', N'Cotizacion valida por: {ValidezDias} dias calendario.', 10),
(N'Precios', N'Precios sujetos a disponibilidad de stock.', 20),
(N'Entrega', N'Plazo de entrega: a coordinar.', 30),
(N'Lugar', N'Lugar de entrega: segun acuerdo.', 40),
(N'Pago', N'Forma de pago: contado o segun acuerdo.', 50),
(N'Garantia', N'Garantia segun condiciones del producto.', 60);

INSERT INTO [erp].[CondicionComercialItem] ([CondicionComercialPlantillaId], [Etiqueta], [Texto], [Orden], [Visible])
SELECT @CondicionId, c.Etiqueta, c.Texto, c.Orden, 1
FROM @Condiciones c
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[CondicionComercialItem] x WHERE x.CondicionComercialPlantillaId = @CondicionId AND x.Orden = c.Orden
);

DECLARE @PlantillaId int;
SELECT @PlantillaId = [PlantillaDocumentoId]
FROM [erp].[PlantillaDocumento]
WHERE [EmpresaId] IS NULL AND [TeamId] IS NULL AND [TipoDocumento] = N'COTIZACION' AND [Activa] = 1 AND [EsPredeterminada] = 1;

IF @PlantillaId IS NULL
BEGIN
    INSERT INTO [erp].[PlantillaDocumento] ([EmpresaId], [TeamId], [TipoDocumento], [Nombre], [Version], [Activa], [EsPredeterminada], [UsuarioRegistro])
    VALUES (NULL, NULL, N'COTIZACION', N'PDF Cotizacion estandar SaaS', 1, 1, 1, N'SISTEMA');
    SET @PlantillaId = SCOPE_IDENTITY();
END;

DECLARE @PdfBloques TABLE (Bloque nvarchar(60), Titulo nvarchar(120), Orden int);
INSERT INTO @PdfBloques VALUES
(N'ENCABEZADO', N'Encabezado', 10),
(N'CLIENTE', N'Cliente', 20),
(N'DETALLE', N'Detalle', 30),
(N'TOTALES', N'Totales', 40),
(N'CONDICIONES', N'Condiciones', 50),
(N'FIRMA', N'Firma', 60),
(N'PIE', N'Pie', 70);

INSERT INTO [erp].[PlantillaDocumentoBloque] ([PlantillaDocumentoId], [Bloque], [Titulo], [Visible], [Orden])
SELECT @PlantillaId, b.Bloque, b.Titulo, 1, b.Orden
FROM @PdfBloques b
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[PlantillaDocumentoBloque] x WHERE x.PlantillaDocumentoId = @PlantillaId AND x.Bloque = b.Bloque
);

IF OBJECT_ID(N'erp.__EFMigrationsHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM [erp].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260710120000_AddDocumentoConfiguracionCotizacion')
BEGIN
    INSERT INTO [erp].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260710120000_AddDocumentoConfiguracionCotizacion', N'9.0.0');
END;

SELECT Tabla = N'FormularioConfiguracion', Registros = COUNT(*) FROM [erp].[FormularioConfiguracion] WHERE [TipoDocumento] = N'COTIZACION'
UNION ALL SELECT N'FormularioBloqueConfiguracion', COUNT(*) FROM [erp].[FormularioBloqueConfiguracion] WHERE [FormularioConfiguracionId] = @FormularioId
UNION ALL SELECT N'FormularioCampoConfiguracion', COUNT(*) FROM [erp].[FormularioCampoConfiguracion] WHERE [FormularioConfiguracionId] = @FormularioId
UNION ALL SELECT N'CondicionComercialItem', COUNT(*) FROM [erp].[CondicionComercialItem] WHERE [CondicionComercialPlantillaId] = @CondicionId
UNION ALL SELECT N'PlantillaDocumentoBloque', COUNT(*) FROM [erp].[PlantillaDocumentoBloque] WHERE [PlantillaDocumentoId] = @PlantillaId;
