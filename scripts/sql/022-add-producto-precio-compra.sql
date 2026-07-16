/*
Agrega el precio de compra opcional a productos existentes.
Idempotente para bases ya creadas.
*/

IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA [erp]');

IF OBJECT_ID(N'erp.Producto', N'U') IS NULL
    THROW 52201, 'No existe la tabla erp.Producto.', 1;

IF COL_LENGTH(N'erp.Producto', N'PrecioCompra') IS NULL
BEGIN
    ALTER TABLE [erp].[Producto]
        ADD [PrecioCompra] decimal(18,2) NOT NULL
            CONSTRAINT [DF_Producto_PrecioCompra] DEFAULT 0;
END;

PRINT N'Columna erp.Producto.PrecioCompra verificada.';
