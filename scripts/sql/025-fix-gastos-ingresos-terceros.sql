USE ViveroLosFrutalesDB;
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'erp.Gasto', N'U') IS NULL
    THROW 51025, 'No existe la tabla erp.Gasto.', 1;
IF OBJECT_ID(N'erp.Ingreso', N'U') IS NULL
    THROW 51026, 'No existe la tabla erp.Ingreso.', 1;
GO

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Gasto_Cliente_ClienteId')
    ALTER TABLE [erp].[Gasto] DROP CONSTRAINT [FK_Gasto_Cliente_ClienteId];
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Ingreso_Proveedor_ProveedorId')
    ALTER TABLE [erp].[Ingreso] DROP CONSTRAINT [FK_Ingreso_Proveedor_ProveedorId];
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Gasto_EmpresaId_ClienteId' AND object_id = OBJECT_ID(N'erp.Gasto'))
    DROP INDEX [IX_Gasto_EmpresaId_ClienteId] ON [erp].[Gasto];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Ingreso_EmpresaId_ProveedorId' AND object_id = OBJECT_ID(N'erp.Ingreso'))
    DROP INDEX [IX_Ingreso_EmpresaId_ProveedorId] ON [erp].[Ingreso];
GO

IF COL_LENGTH(N'erp.Gasto', N'ProveedorId') IS NULL
    ALTER TABLE [erp].[Gasto] ADD [ProveedorId] int NULL;
IF COL_LENGTH(N'erp.Ingreso', N'ClienteId') IS NULL
    ALTER TABLE [erp].[Ingreso] ADD [ClienteId] int NULL;
GO

-- No se migra automaticamente ClienteId -> ProveedorId ni ProveedorId -> ClienteId porque son catalogos distintos.
-- Las columnas antiguas se conservan temporalmente si tienen datos historicos para revision operativa.

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Gasto_Proveedor_ProveedorId')
    ALTER TABLE [erp].[Gasto]
        ADD CONSTRAINT [FK_Gasto_Proveedor_ProveedorId]
        FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Ingreso_Cliente_ClienteId')
    ALTER TABLE [erp].[Ingreso]
        ADD CONSTRAINT [FK_Ingreso_Cliente_ClienteId]
        FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Gasto_EmpresaId_ProveedorId' AND object_id = OBJECT_ID(N'erp.Gasto'))
    CREATE INDEX [IX_Gasto_EmpresaId_ProveedorId] ON [erp].[Gasto] ([EmpresaId], [ProveedorId]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Ingreso_EmpresaId_ClienteId' AND object_id = OBJECT_ID(N'erp.Ingreso'))
    CREATE INDEX [IX_Ingreso_EmpresaId_ClienteId] ON [erp].[Ingreso] ([EmpresaId], [ClienteId]);
GO

PRINT N'Gastos e ingresos corregidos: Gasto usa ProveedorId e Ingreso usa ClienteId.';
GO