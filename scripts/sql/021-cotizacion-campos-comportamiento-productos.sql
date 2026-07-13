/*
Completa la configuracion configurable por campo de Cotizacion y agrega
comportamiento tipado para el bloque Productos.
Idempotente: no elimina configuraciones existentes de empresas.
*/

IF OBJECT_ID(N'erp.FormularioConfiguracion', N'U') IS NULL
    THROW 50001, 'No existe erp.FormularioConfiguracion. Ejecute primero el script 018.', 1;

IF OBJECT_ID(N'erp.FormularioCampoConfiguracion', N'U') IS NULL
    THROW 50002, 'No existe erp.FormularioCampoConfiguracion. Ejecute primero el script 018.', 1;

IF OBJECT_ID(N'erp.FormularioBloqueProductoConfiguracion', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[FormularioBloqueProductoConfiguracion] (
        [FormularioBloqueProductoConfiguracionId] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FormularioBloqueProductoConfiguracion] PRIMARY KEY,
        [FormularioConfiguracionId] int NOT NULL,
        [UnirProductosDuplicados] bit NOT NULL CONSTRAINT [DF_FormularioBloqueProductoConfiguracion_UnirProductosDuplicados] DEFAULT 1,
        [CantidadInicial] decimal(18,2) NOT NULL CONSTRAINT [DF_FormularioBloqueProductoConfiguracion_CantidadInicial] DEFAULT 1,
        [PermitirEditarPrecio] bit NOT NULL CONSTRAINT [DF_FormularioBloqueProductoConfiguracion_PermitirEditarPrecio] DEFAULT 1,
        [PermitirDescuento] bit NOT NULL CONSTRAINT [DF_FormularioBloqueProductoConfiguracion_PermitirDescuento] DEFAULT 1,
        [MostrarStock] bit NOT NULL CONSTRAINT [DF_FormularioBloqueProductoConfiguracion_MostrarStock] DEFAULT 1,
        [BloquearSinStock] bit NOT NULL CONSTRAINT [DF_FormularioBloqueProductoConfiguracion_BloquearSinStock] DEFAULT 0
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_FormularioBloqueProductoConfiguracion_Formulario' AND object_id = OBJECT_ID(N'erp.FormularioBloqueProductoConfiguracion'))
    CREATE UNIQUE INDEX [UX_FormularioBloqueProductoConfiguracion_Formulario] ON [erp].[FormularioBloqueProductoConfiguracion] ([FormularioConfiguracionId]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_FormularioBloqueProductoConfiguracion_FormularioConfiguracion_FormularioConfiguracionId')
    ALTER TABLE [erp].[FormularioBloqueProductoConfiguracion] ADD CONSTRAINT [FK_FormularioBloqueProductoConfiguracion_FormularioConfiguracion_FormularioConfiguracionId]
    FOREIGN KEY ([FormularioConfiguracionId]) REFERENCES [erp].[FormularioConfiguracion]([FormularioConfiguracionId]) ON DELETE CASCADE;

DECLARE @FormularioId int;
SELECT @FormularioId = FormularioConfiguracionId
FROM erp.FormularioConfiguracion
WHERE EmpresaId IS NULL AND TeamId IS NULL AND TipoDocumento = N'COTIZACION' AND Activo = 1;

IF @FormularioId IS NULL
BEGIN
    INSERT INTO erp.FormularioConfiguracion (EmpresaId, TeamId, TipoDocumento, Nombre, Version, Activo, UsuarioRegistro)
    VALUES (NULL, NULL, N'COTIZACION', N'Cotizacion estandar SaaS', 1, 1, N'SISTEMA');
    SET @FormularioId = SCOPE_IDENTITY();
END;

DECLARE @Bloques TABLE (Bloque nvarchar(60), Titulo nvarchar(120), Orden int);
INSERT INTO @Bloques VALUES
(N'GENERAL', N'Informacion general', 10),
(N'CLIENTE', N'Cliente', 20),
(N'PRODUCTOS', N'Detalle de productos', 30),
(N'OBSERVACIONES', N'Observaciones', 35),
(N'CONDICIONES', N'Condiciones comerciales', 40),
(N'TOTALES', N'Totales', 50),
(N'ACCIONES', N'Acciones', 60);

INSERT INTO erp.FormularioBloqueConfiguracion (FormularioConfiguracionId, Bloque, Titulo, Visible, Orden, Colapsado)
SELECT @FormularioId, b.Bloque, b.Titulo, 1, b.Orden, 0
FROM @Bloques b
WHERE NOT EXISTS (
    SELECT 1 FROM erp.FormularioBloqueConfiguracion x
    WHERE x.FormularioConfiguracionId = @FormularioId AND x.Bloque = b.Bloque
);

DECLARE @Campos TABLE (Bloque nvarchar(60), Campo nvarchar(80), Etiqueta nvarchar(120), Visible bit, Obligatorio bit, SoloLectura bit, Orden int, Ancho nvarchar(40));
INSERT INTO @Campos VALUES
(N'GENERAL', N'Serie', N'Serie', 1, 0, 1, 10, N'4'),
(N'GENERAL', N'Numero', N'Numero', 1, 0, 1, 20, N'4'),
(N'GENERAL', N'Fecha', N'Fecha', 1, 1, 0, 30, N'3'),
(N'GENERAL', N'ValidezDias', N'Validez', 1, 0, 1, 40, N'3'),
(N'GENERAL', N'Moneda', N'Moneda', 1, 0, 1, 50, N'3'),
(N'GENERAL', N'FormaPago', N'Forma de pago', 1, 0, 0, 60, N'3'),
(N'GENERAL', N'Vendedor', N'Vendedor', 0, 0, 1, 70, N'3'),
(N'CLIENTE', N'ClienteBuscador', N'Buscar cliente', 1, 1, 0, 10, N'12'),
(N'CLIENTE', N'ClienteDocumento', N'Documento', 1, 0, 1, 20, N'2'),
(N'CLIENTE', N'ClienteTelefono', N'Telefono', 1, 0, 1, 30, N'2'),
(N'CLIENTE', N'ClienteEmail', N'Email', 1, 0, 1, 40, N'3'),
(N'CLIENTE', N'ClienteDireccion', N'Direccion', 1, 0, 1, 50, N'5'),
(N'PRODUCTOS', N'Codigo', N'Codigo', 0, 0, 1, 10, N'2'),
(N'PRODUCTOS', N'Producto', N'Producto', 1, 1, 1, 20, N'4'),
(N'PRODUCTOS', N'Unidad', N'Unidad', 1, 0, 1, 30, N'1'),
(N'PRODUCTOS', N'Stock', N'Stock', 1, 0, 1, 40, N'1'),
(N'PRODUCTOS', N'Cantidad', N'Cantidad', 1, 1, 0, 50, N'1'),
(N'PRODUCTOS', N'PrecioUnitario', N'Precio unitario', 1, 1, 0, 60, N'2'),
(N'PRODUCTOS', N'DescuentoPorcentaje', N'Descuento %', 1, 0, 0, 70, N'1'),
(N'PRODUCTOS', N'TotalLinea', N'Total', 1, 0, 1, 80, N'2'),
(N'OBSERVACIONES', N'CaracteristicasTecnicas', N'Observaciones', 1, 0, 0, 10, N'12'),
(N'CONDICIONES', N'CondicionesComerciales', N'Condiciones comerciales', 1, 0, 0, 10, N'12'),
(N'TOTALES', N'SubtotalExonerado', N'Subtotal exonerado', 1, 0, 1, 10, N'2'),
(N'TOTALES', N'SubtotalGravado', N'Subtotal gravado', 1, 0, 1, 20, N'2'),
(N'TOTALES', N'Descuento', N'Descuento', 1, 0, 1, 30, N'2'),
(N'TOTALES', N'Igv', N'IGV (18%)', 1, 0, 1, 40, N'2'),
(N'TOTALES', N'Total', N'Total', 1, 0, 1, 50, N'4'),
(N'ACCIONES', N'Guardar', N'Guardar cotizacion', 1, 0, 0, 10, N'4'),
(N'ACCIONES', N'GuardarPdf', N'Guardar PDF', 0, 0, 0, 20, N'4'),
(N'ACCIONES', N'Volver', N'Volver', 1, 0, 0, 30, N'4');

INSERT INTO erp.FormularioCampoConfiguracion (FormularioConfiguracionId, Bloque, Campo, Etiqueta, Visible, Obligatorio, SoloLectura, Orden, Ancho, ValorDefecto)
SELECT @FormularioId, c.Bloque, c.Campo, c.Etiqueta, c.Visible, c.Obligatorio, c.SoloLectura, c.Orden, c.Ancho, NULL
FROM @Campos c
WHERE NOT EXISTS (
    SELECT 1 FROM erp.FormularioCampoConfiguracion x
    WHERE x.FormularioConfiguracionId = @FormularioId AND x.Bloque = c.Bloque AND x.Campo = c.Campo
);

IF NOT EXISTS (SELECT 1 FROM erp.FormularioBloqueProductoConfiguracion WHERE FormularioConfiguracionId = @FormularioId)
BEGIN
    INSERT INTO erp.FormularioBloqueProductoConfiguracion (FormularioConfiguracionId)
    VALUES (@FormularioId);
END;

SELECT Tabla = N'FormularioCampoConfiguracion', Registros = COUNT(*)
FROM erp.FormularioCampoConfiguracion
WHERE FormularioConfiguracionId = @FormularioId
UNION ALL
SELECT N'FormularioBloqueProductoConfiguracion', COUNT(*)
FROM erp.FormularioBloqueProductoConfiguracion
WHERE FormularioConfiguracionId = @FormularioId;
