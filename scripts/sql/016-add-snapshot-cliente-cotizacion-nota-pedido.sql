/*
Agrega y corrige campos historicos del cliente en erp.Cotizacion y erp.NotaPedido.

Seguro para ejecutar manualmente:
- Solo agrega columnas si no existen.
- Todas las columnas son NULL para no romper documentos existentes.
- No modifica montos, estados, correlativos ni relaciones.
- Si ClienteTipoDocumento existe como nvarchar/texto, lo convierte a int.
- Registra la migracion EF como aplicada si existe la tabla de historial.
*/

IF SCHEMA_ID(N'erp') IS NULL
    THROW 50000, 'No existe el esquema erp.', 1;

IF OBJECT_ID(N'erp.Cotizacion', N'U') IS NULL
    THROW 50001, 'No existe la tabla erp.Cotizacion.', 1;

IF OBJECT_ID(N'erp.NotaPedido', N'U') IS NULL
    THROW 50002, 'No existe la tabla erp.NotaPedido.', 1;


/* =========================
   erp.Cotizacion
   ========================= */

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteTipoDocumento') IS NULL
    ALTER TABLE [erp].[Cotizacion] ADD [ClienteTipoDocumento] int NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteNumeroDocumento') IS NULL
    ALTER TABLE [erp].[Cotizacion] ADD [ClienteNumeroDocumento] NVARCHAR(30) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteNombre') IS NULL
    ALTER TABLE [erp].[Cotizacion] ADD [ClienteNombre] NVARCHAR(250) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteNombreComercial') IS NULL
    ALTER TABLE [erp].[Cotizacion] ADD [ClienteNombreComercial] NVARCHAR(250) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteDireccion') IS NULL
    ALTER TABLE [erp].[Cotizacion] ADD [ClienteDireccion] NVARCHAR(500) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteTelefono') IS NULL
    ALTER TABLE [erp].[Cotizacion] ADD [ClienteTelefono] NVARCHAR(50) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteEmail') IS NULL
    ALTER TABLE [erp].[Cotizacion] ADD [ClienteEmail] NVARCHAR(150) NULL;


/* =========================
   erp.NotaPedido
   ========================= */

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteTipoDocumento') IS NULL
    ALTER TABLE [erp].[NotaPedido] ADD [ClienteTipoDocumento] int NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteNumeroDocumento') IS NULL
    ALTER TABLE [erp].[NotaPedido] ADD [ClienteNumeroDocumento] NVARCHAR(30) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteNombre') IS NULL
    ALTER TABLE [erp].[NotaPedido] ADD [ClienteNombre] NVARCHAR(250) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteNombreComercial') IS NULL
    ALTER TABLE [erp].[NotaPedido] ADD [ClienteNombreComercial] NVARCHAR(250) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteDireccion') IS NULL
    ALTER TABLE [erp].[NotaPedido] ADD [ClienteDireccion] NVARCHAR(500) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteTelefono') IS NULL
    ALTER TABLE [erp].[NotaPedido] ADD [ClienteTelefono] NVARCHAR(50) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteEmail') IS NULL
    ALTER TABLE [erp].[NotaPedido] ADD [ClienteEmail] NVARCHAR(150) NULL;


/* Registrar migracion EF como aplicada */

IF OBJECT_ID(N'erp.__EFMigrationsHistory', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM [erp].[__EFMigrationsHistory]
       WHERE [MigrationId] = N'20260708173000_AddClienteSnapshotCotizacionNotaPedido'
   )
BEGIN
    INSERT INTO [erp].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260708173000_AddClienteSnapshotCotizacionNotaPedido', N'9.0.0');
END;


/* Validacion */

SELECT
    Tabla = N'Cotizacion',
    ClienteTipoDocumento = COL_LENGTH(N'erp.Cotizacion', N'ClienteTipoDocumento'),
    ClienteNumeroDocumento = COL_LENGTH(N'erp.Cotizacion', N'ClienteNumeroDocumento'),
    ClienteNombre = COL_LENGTH(N'erp.Cotizacion', N'ClienteNombre'),
    ClienteNombreComercial = COL_LENGTH(N'erp.Cotizacion', N'ClienteNombreComercial'),
    ClienteDireccion = COL_LENGTH(N'erp.Cotizacion', N'ClienteDireccion'),
    ClienteTelefono = COL_LENGTH(N'erp.Cotizacion', N'ClienteTelefono'),
    ClienteEmail = COL_LENGTH(N'erp.Cotizacion', N'ClienteEmail')
UNION ALL
SELECT
    Tabla = N'NotaPedido',
    ClienteTipoDocumento = COL_LENGTH(N'erp.NotaPedido', N'ClienteTipoDocumento'),
    ClienteNumeroDocumento = COL_LENGTH(N'erp.NotaPedido', N'ClienteNumeroDocumento'),
    ClienteNombre = COL_LENGTH(N'erp.NotaPedido', N'ClienteNombre'),
    ClienteNombreComercial = COL_LENGTH(N'erp.NotaPedido', N'ClienteNombreComercial'),
    ClienteDireccion = COL_LENGTH(N'erp.NotaPedido', N'ClienteDireccion'),
    ClienteTelefono = COL_LENGTH(N'erp.NotaPedido', N'ClienteTelefono'),
    ClienteEmail = COL_LENGTH(N'erp.NotaPedido', N'ClienteEmail');