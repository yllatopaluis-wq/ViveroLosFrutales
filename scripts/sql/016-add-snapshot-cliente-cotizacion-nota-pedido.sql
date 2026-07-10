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
BEGIN
    THROW 50000, 'No existe el esquema erp.', 1;
END;

IF OBJECT_ID(N'erp.Cotizacion', N'U') IS NULL
BEGIN
    THROW 50001, 'No existe la tabla erp.Cotizacion.', 1;
END;

IF OBJECT_ID(N'erp.NotaPedido', N'U') IS NULL
BEGIN
    THROW 50002, 'No existe la tabla erp.NotaPedido.', 1;
END;

/* =========================
   COTIZACION
   ========================= */
IF COL_LENGTH(N'erp.Cotizacion', N'ClienteTipoDocumento') IS NULL
    ALTER TABLE erp.Cotizacion ADD ClienteTipoDocumento int NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteNumeroDocumento') IS NULL
    ALTER TABLE erp.Cotizacion ADD ClienteNumeroDocumento nvarchar(20) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteNombre') IS NULL
    ALTER TABLE erp.Cotizacion ADD ClienteNombre nvarchar(250) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteNombreComercial') IS NULL
    ALTER TABLE erp.Cotizacion ADD ClienteNombreComercial nvarchar(250) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteDireccion') IS NULL
    ALTER TABLE erp.Cotizacion ADD ClienteDireccion nvarchar(500) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteTelefono') IS NULL
    ALTER TABLE erp.Cotizacion ADD ClienteTelefono nvarchar(40) NULL;

IF COL_LENGTH(N'erp.Cotizacion', N'ClienteEmail') IS NULL
    ALTER TABLE erp.Cotizacion ADD ClienteEmail nvarchar(120) NULL;

IF EXISTS (
    SELECT 1
    FROM sys.columns c
    INNER JOIN sys.types t ON t.user_type_id = c.user_type_id
    WHERE c.object_id = OBJECT_ID(N'erp.Cotizacion')
      AND c.name = N'ClienteTipoDocumento'
      AND t.name <> N'int'
)
BEGIN
    IF COL_LENGTH(N'erp.Cotizacion', N'ClienteTipoDocumento_Int') IS NOT NULL
        ALTER TABLE erp.Cotizacion DROP COLUMN ClienteTipoDocumento_Int;

    ALTER TABLE erp.Cotizacion ADD ClienteTipoDocumento_Int int NULL;

    UPDATE erp.Cotizacion
    SET ClienteTipoDocumento_Int = CASE UPPER(LTRIM(RTRIM(CONVERT(nvarchar(50), ClienteTipoDocumento))))
        WHEN N'DNI' THEN 1
        WHEN N'RUC' THEN 2
        WHEN N'CE' THEN 3
        WHEN N'CARNET EXTRANJERIA' THEN 3
        WHEN N'CARNET DE EXTRANJERIA' THEN 3
        WHEN N'OTRO' THEN 9
        ELSE TRY_CONVERT(int, ClienteTipoDocumento)
    END;

    ALTER TABLE erp.Cotizacion DROP COLUMN ClienteTipoDocumento;
    EXEC sp_rename N'erp.Cotizacion.ClienteTipoDocumento_Int', N'ClienteTipoDocumento', N'COLUMN';
END;

/* =========================
   NOTA PEDIDO
   ========================= */
IF COL_LENGTH(N'erp.NotaPedido', N'ClienteTipoDocumento') IS NULL
    ALTER TABLE erp.NotaPedido ADD ClienteTipoDocumento int NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteNumeroDocumento') IS NULL
    ALTER TABLE erp.NotaPedido ADD ClienteNumeroDocumento nvarchar(20) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteNombre') IS NULL
    ALTER TABLE erp.NotaPedido ADD ClienteNombre nvarchar(250) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteNombreComercial') IS NULL
    ALTER TABLE erp.NotaPedido ADD ClienteNombreComercial nvarchar(250) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteDireccion') IS NULL
    ALTER TABLE erp.NotaPedido ADD ClienteDireccion nvarchar(500) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteTelefono') IS NULL
    ALTER TABLE erp.NotaPedido ADD ClienteTelefono nvarchar(40) NULL;

IF COL_LENGTH(N'erp.NotaPedido', N'ClienteEmail') IS NULL
    ALTER TABLE erp.NotaPedido ADD ClienteEmail nvarchar(120) NULL;

IF EXISTS (
    SELECT 1
    FROM sys.columns c
    INNER JOIN sys.types t ON t.user_type_id = c.user_type_id
    WHERE c.object_id = OBJECT_ID(N'erp.NotaPedido')
      AND c.name = N'ClienteTipoDocumento'
      AND t.name <> N'int'
)
BEGIN
    IF COL_LENGTH(N'erp.NotaPedido', N'ClienteTipoDocumento_Int') IS NOT NULL
        ALTER TABLE erp.NotaPedido DROP COLUMN ClienteTipoDocumento_Int;

    ALTER TABLE erp.NotaPedido ADD ClienteTipoDocumento_Int int NULL;

    UPDATE erp.NotaPedido
    SET ClienteTipoDocumento_Int = CASE UPPER(LTRIM(RTRIM(CONVERT(nvarchar(50), ClienteTipoDocumento))))
        WHEN N'DNI' THEN 1
        WHEN N'RUC' THEN 2
        WHEN N'CE' THEN 3
        WHEN N'CARNET EXTRANJERIA' THEN 3
        WHEN N'CARNET DE EXTRANJERIA' THEN 3
        WHEN N'OTRO' THEN 9
        ELSE TRY_CONVERT(int, ClienteTipoDocumento)
    END;

    ALTER TABLE erp.NotaPedido DROP COLUMN ClienteTipoDocumento;
    EXEC sp_rename N'erp.NotaPedido.ClienteTipoDocumento_Int', N'ClienteTipoDocumento', N'COLUMN';
END;

IF OBJECT_ID(N'erp.__EFMigrationsHistory', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM erp.__EFMigrationsHistory
       WHERE MigrationId = N'20260708173000_AddClienteSnapshotCotizacionNotaPedido'
   )
BEGIN
    INSERT INTO erp.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES (N'20260708173000_AddClienteSnapshotCotizacionNotaPedido', N'9.0.0');
END;

SELECT
    Tabla = s.name + N'.' + o.name,
    Columna = c.name,
    Tipo = t.name,
    Longitud = c.max_length,
    PermiteNull = c.is_nullable
FROM sys.columns c
INNER JOIN sys.objects o ON o.object_id = c.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE s.name = N'erp'
  AND o.name IN (N'Cotizacion', N'NotaPedido')
  AND c.name IN (
      N'ClienteTipoDocumento',
      N'ClienteNumeroDocumento',
      N'ClienteNombre',
      N'ClienteNombreComercial',
      N'ClienteDireccion',
      N'ClienteTelefono',
      N'ClienteEmail'
  )
ORDER BY o.name, c.column_id;