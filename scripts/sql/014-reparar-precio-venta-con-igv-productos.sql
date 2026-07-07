USE ViveroLosFrutalesDB;
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'erp.Producto', N'U') IS NULL
BEGIN
    THROW 51401, 'No existe la tabla erp.Producto.', 1;
END;

IF COL_LENGTH(N'erp.Producto', N'PrecioVentaSinIgv') IS NULL
BEGIN
    THROW 51402, 'No existe la columna erp.Producto.PrecioVentaSinIgv.', 1;
END;

IF COL_LENGTH(N'erp.Producto', N'PrecioVentaConIgv') IS NULL
BEGIN
    THROW 51403, 'No existe la columna erp.Producto.PrecioVentaConIgv.', 1;
END;

IF COL_LENGTH(N'erp.Producto', N'AfectoIgv') IS NULL
BEGIN
    THROW 51404, 'No existe la columna erp.Producto.AfectoIgv.', 1;
END;

UPDATE [erp].[Producto]
SET [PrecioVentaConIgv] = CASE
    WHEN [AfectoIgv] = 1 THEN ROUND([PrecioVentaSinIgv] * 1.18, 2)
    ELSE [PrecioVentaSinIgv]
END
WHERE ISNULL([PrecioVentaConIgv], 0) = 0
  AND ISNULL([PrecioVentaSinIgv], 0) > 0;

PRINT N'Precios de venta con IGV reparados para productos existentes.';
GO
