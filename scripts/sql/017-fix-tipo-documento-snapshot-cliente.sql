/*
Repara el tipo fisico de ClienteTipoDocumento en snapshots de cliente.

Motivo:
EF espera int nullable para ClienteTipoDocumento. Si la columna fue creada como nvarchar
por un script manual previo, al ver/imprimir documentos puede aparecer:
"Unable to cast object of type 'System.String' to type 'System.Int32'".

Seguro/idempotente:
- Solo actua si la columna existe y NO es int.
- Convierte valores conocidos: DNI=1, RUC=2, CE=3, OTRO=9.
- Tambien convierte textos numericos: '1', '2', '3', '9'.
- No modifica otros campos ni documentos historicos.
*/

IF SCHEMA_ID(N'erp') IS NULL
BEGIN
    THROW 50000, 'No existe el esquema erp.', 1;
END;

DECLARE @tablas TABLE (Nombre sysname);
INSERT INTO @tablas (Nombre)
VALUES (N'Comprobante'), (N'Cotizacion'), (N'NotaPedido');

DECLARE @tabla sysname;
DECLARE tablas_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT Nombre FROM @tablas;
OPEN tablas_cursor;
FETCH NEXT FROM tablas_cursor INTO @tabla;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(N'erp.' + @tabla, N'U') IS NOT NULL
       AND COL_LENGTH(N'erp.' + @tabla, N'ClienteTipoDocumento') IS NOT NULL
       AND EXISTS (
            SELECT 1
            FROM sys.columns c
            INNER JOIN sys.types t ON t.user_type_id = c.user_type_id
            WHERE c.object_id = OBJECT_ID(N'erp.' + @tabla)
              AND c.name = N'ClienteTipoDocumento'
              AND t.name <> N'int'
       )
    BEGIN
        DECLARE @sql nvarchar(max) = N'
ALTER TABLE erp.' + QUOTENAME(@tabla) + N' ADD ClienteTipoDocumento_Int int NULL;

UPDATE erp.' + QUOTENAME(@tabla) + N'
SET ClienteTipoDocumento_Int = CASE UPPER(LTRIM(RTRIM(CONVERT(nvarchar(50), ClienteTipoDocumento))))
    WHEN N''''DNI'''' THEN 1
    WHEN N''''RUC'''' THEN 2
    WHEN N''''CE'''' THEN 3
    WHEN N''''CARNET EXTRANJERIA'''' THEN 3
    WHEN N''''CARNET DE EXTRANJERIA'''' THEN 3
    WHEN N''''OTRO'''' THEN 9
    ELSE TRY_CONVERT(int, ClienteTipoDocumento)
END;

ALTER TABLE erp.' + QUOTENAME(@tabla) + N' DROP COLUMN ClienteTipoDocumento;
EXEC sp_rename N''''erp.' + @tabla + N'.ClienteTipoDocumento_Int'''', N''''ClienteTipoDocumento'''', N''''COLUMN'''';
';
        EXEC sp_executesql @sql;
    END;

    FETCH NEXT FROM tablas_cursor INTO @tabla;
END;

CLOSE tablas_cursor;
DEALLOCATE tablas_cursor;

SELECT
    Tabla = s.name + N'.' + o.name,
    Columna = c.name,
    Tipo = t.name,
    c.max_length,
    c.is_nullable
FROM sys.columns c
INNER JOIN sys.objects o ON o.object_id = c.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE s.name = N'erp'
  AND o.name IN (N'Comprobante', N'Cotizacion', N'NotaPedido')
  AND c.name = N'ClienteTipoDocumento'
ORDER BY o.name;