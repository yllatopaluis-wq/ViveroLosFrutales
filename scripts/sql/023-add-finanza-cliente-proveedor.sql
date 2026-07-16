SET XACT_ABORT ON;

IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Gasto', N'ProveedorId') IS NULL
BEGIN
    ALTER TABLE [erp].[Gasto] ADD [ProveedorId] int NULL;
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Ingreso', N'ClienteId') IS NULL
BEGIN
    ALTER TABLE [erp].[Ingreso] ADD [ClienteId] int NULL;
END;

IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL
   AND COL_LENGTH(N'erp.Gasto', N'ProveedorId') IS NOT NULL
   AND OBJECT_ID(N'erp.Proveedor', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Gasto_Proveedor_ProveedorId')
BEGIN
    ALTER TABLE [erp].[Gasto]
        ADD CONSTRAINT [FK_Gasto_Proveedor_ProveedorId]
        FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION;
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL
   AND COL_LENGTH(N'erp.Ingreso', N'ClienteId') IS NOT NULL
   AND OBJECT_ID(N'erp.Cliente', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Ingreso_Cliente_ClienteId')
BEGIN
    ALTER TABLE [erp].[Ingreso]
        ADD CONSTRAINT [FK_Ingreso_Cliente_ClienteId]
        FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION;
END;

IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL
   AND COL_LENGTH(N'erp.Gasto', N'ProveedorId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Gasto_EmpresaId_ProveedorId' AND object_id = OBJECT_ID(N'erp.Gasto'))
BEGIN
    CREATE INDEX [IX_Gasto_EmpresaId_ProveedorId] ON [erp].[Gasto] ([EmpresaId], [ProveedorId]);
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL
   AND COL_LENGTH(N'erp.Ingreso', N'ClienteId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Ingreso_EmpresaId_ClienteId' AND object_id = OBJECT_ID(N'erp.Ingreso'))
BEGIN
    CREATE INDEX [IX_Ingreso_EmpresaId_ClienteId] ON [erp].[Ingreso] ([EmpresaId], [ClienteId]);
END;

SELECT
    GastoProveedorId = COL_LENGTH(N'erp.Gasto', N'ProveedorId'),
    IngresoClienteId = COL_LENGTH(N'erp.Ingreso', N'ClienteId');