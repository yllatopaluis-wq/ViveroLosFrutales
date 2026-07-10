/*
Agrega campos historicos del cliente en erp.Comprobante.

Seguro para ejecutar manualmente:
- Solo agrega columnas si no existen.
- Todas las columnas son NULL para no romper comprobantes existentes.
- No modifica datos historicos ni hace backfill.
- Registra la migracion EF como aplicada si existe la tabla de historial.
*/

IF SCHEMA_ID(N'erp') IS NULL
BEGIN
    THROW 50000, 'No existe el esquema erp.', 1;
END;

IF OBJECT_ID(N'erp.Comprobante', N'U') IS NULL
BEGIN
    THROW 50001, 'No existe la tabla erp.Comprobante.', 1;
END;

IF COL_LENGTH(N'erp.Comprobante', N'ClienteTipoDocumento') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD ClienteTipoDocumento int NULL;
END;

IF COL_LENGTH(N'erp.Comprobante', N'ClienteNumeroDocumento') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD ClienteNumeroDocumento nvarchar(20) NULL;
END;

IF COL_LENGTH(N'erp.Comprobante', N'ClienteNombre') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD ClienteNombre nvarchar(250) NULL;
END;

IF COL_LENGTH(N'erp.Comprobante', N'ClienteNombreComercial') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD ClienteNombreComercial nvarchar(250) NULL;
END;

IF COL_LENGTH(N'erp.Comprobante', N'ClienteDireccion') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD ClienteDireccion nvarchar(500) NULL;
END;

IF COL_LENGTH(N'erp.Comprobante', N'ClienteTelefono') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD ClienteTelefono nvarchar(40) NULL;
END;

IF COL_LENGTH(N'erp.Comprobante', N'ClienteEmail') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD ClienteEmail nvarchar(120) NULL;
END;

IF OBJECT_ID(N'erp.__EFMigrationsHistory', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM erp.__EFMigrationsHistory
       WHERE MigrationId = N'20260708170000_AddComprobanteClienteSnapshot'
   )
BEGIN
    INSERT INTO erp.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES (N'20260708170000_AddComprobanteClienteSnapshot', N'9.0.0');
END;

SELECT
    ClienteTipoDocumento = COL_LENGTH(N'erp.Comprobante', N'ClienteTipoDocumento'),
    ClienteNumeroDocumento = COL_LENGTH(N'erp.Comprobante', N'ClienteNumeroDocumento'),
    ClienteNombre = COL_LENGTH(N'erp.Comprobante', N'ClienteNombre'),
    ClienteNombreComercial = COL_LENGTH(N'erp.Comprobante', N'ClienteNombreComercial'),
    ClienteDireccion = COL_LENGTH(N'erp.Comprobante', N'ClienteDireccion'),
    ClienteTelefono = COL_LENGTH(N'erp.Comprobante', N'ClienteTelefono'),
    ClienteEmail = COL_LENGTH(N'erp.Comprobante', N'ClienteEmail');