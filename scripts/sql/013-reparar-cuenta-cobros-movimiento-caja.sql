USE ViveroLosFrutalesDB;
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'erp.CobroCliente', N'U') IS NULL
BEGIN
    THROW 51301, 'No existe la tabla erp.CobroCliente.', 1;
END;

IF OBJECT_ID(N'erp.MovimientoCaja', N'U') IS NULL
BEGIN
    THROW 51302, 'No existe la tabla erp.MovimientoCaja.', 1;
END;

IF COL_LENGTH(N'erp.CobroCliente', N'CuentaFinancieraId') IS NULL
BEGIN
    THROW 51303, 'No existe la columna erp.CobroCliente.CuentaFinancieraId.', 1;
END;

IF COL_LENGTH(N'erp.MovimientoCaja', N'CuentaFinancieraId') IS NULL
BEGIN
    THROW 51304, 'No existe la columna erp.MovimientoCaja.CuentaFinancieraId.', 1;
END;

-- OrigenMovimientoCaja.COBRO_CLIENTE = 1.
UPDATE movimiento
SET movimiento.CuentaFinancieraId = cobro.CuentaFinancieraId
FROM [erp].[MovimientoCaja] movimiento
JOIN [erp].[CobroCliente] cobro
    ON cobro.EmpresaId = movimiento.EmpresaId
   AND cobro.CobroClienteId = movimiento.OrigenId
WHERE movimiento.Origen = 1
  AND cobro.CuentaFinancieraId IS NOT NULL
  AND ISNULL(movimiento.CuentaFinancieraId, -1) <> cobro.CuentaFinancieraId;

PRINT N'Cuentas financieras de cobros reparadas en MovimientoCaja.';
GO
