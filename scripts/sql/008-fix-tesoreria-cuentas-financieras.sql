IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'erp.Empresa', N'U') IS NULL
BEGIN
    THROW 51080, 'No existe la tabla erp.Empresa. Ejecute primero la base inicial.', 1;
END;
GO

IF OBJECT_ID(N'erp.CuentaFinanciera', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[CuentaFinanciera] (
        [CuentaFinancieraId] int NOT NULL IDENTITY,
        [Nombre] nvarchar(120) NOT NULL,
        [Tipo] int NOT NULL,
        [Banco] nvarchar(120) NOT NULL CONSTRAINT [DF_CuentaFinanciera_Banco] DEFAULT N'',
        [NumeroCuenta] nvarchar(80) NOT NULL CONSTRAINT [DF_CuentaFinanciera_NumeroCuenta] DEFAULT N'',
        [Moneda] nvarchar(3) NOT NULL CONSTRAINT [DF_CuentaFinanciera_Moneda] DEFAULT N'PEN',
        [SaldoInicial] decimal(18,2) NOT NULL CONSTRAINT [DF_CuentaFinanciera_SaldoInicial] DEFAULT 0,
        [FechaSaldoInicial] datetime2 NOT NULL CONSTRAINT [DF_CuentaFinanciera_FechaSaldoInicial] DEFAULT SYSUTCDATETIME(),
        [Activo] bit NOT NULL CONSTRAINT [DF_CuentaFinanciera_Activo] DEFAULT CAST(1 AS bit),
        [FechaModificacion] datetime2 NULL,
        [UsuarioModificacion] nvarchar(120) NOT NULL CONSTRAINT [DF_CuentaFinanciera_UsuarioModificacion] DEFAULT N'',
        [EmpresaId] int NOT NULL,
        [FechaRegistro] datetime2 NOT NULL CONSTRAINT [DF_CuentaFinanciera_FechaRegistro] DEFAULT SYSUTCDATETIME(),
        [UsuarioRegistro] nvarchar(max) NOT NULL CONSTRAINT [DF_CuentaFinanciera_UsuarioRegistro] DEFAULT N'migracion',
        [Estado] int NOT NULL CONSTRAINT [DF_CuentaFinanciera_Estado] DEFAULT 1,
        CONSTRAINT [PK_CuentaFinanciera] PRIMARY KEY ([CuentaFinancieraId])
    );
END;
GO

IF COL_LENGTH(N'erp.CuentaFinanciera', N'Banco') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [Banco] nvarchar(120) NOT NULL CONSTRAINT [DF_CuentaFinanciera_Banco] DEFAULT N'';
IF COL_LENGTH(N'erp.CuentaFinanciera', N'NumeroCuenta') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [NumeroCuenta] nvarchar(80) NOT NULL CONSTRAINT [DF_CuentaFinanciera_NumeroCuenta] DEFAULT N'';
IF COL_LENGTH(N'erp.CuentaFinanciera', N'Moneda') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [Moneda] nvarchar(3) NOT NULL CONSTRAINT [DF_CuentaFinanciera_Moneda] DEFAULT N'PEN';
IF COL_LENGTH(N'erp.CuentaFinanciera', N'SaldoInicial') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [SaldoInicial] decimal(18,2) NOT NULL CONSTRAINT [DF_CuentaFinanciera_SaldoInicial] DEFAULT 0;
IF COL_LENGTH(N'erp.CuentaFinanciera', N'FechaSaldoInicial') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [FechaSaldoInicial] datetime2 NOT NULL CONSTRAINT [DF_CuentaFinanciera_FechaSaldoInicial] DEFAULT SYSUTCDATETIME();
IF COL_LENGTH(N'erp.CuentaFinanciera', N'Activo') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [Activo] bit NOT NULL CONSTRAINT [DF_CuentaFinanciera_Activo] DEFAULT CAST(1 AS bit);
IF COL_LENGTH(N'erp.CuentaFinanciera', N'FechaModificacion') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [FechaModificacion] datetime2 NULL;
IF COL_LENGTH(N'erp.CuentaFinanciera', N'UsuarioModificacion') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [UsuarioModificacion] nvarchar(120) NOT NULL CONSTRAINT [DF_CuentaFinanciera_UsuarioModificacion] DEFAULT N'';
IF COL_LENGTH(N'erp.CuentaFinanciera', N'FechaRegistro') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [FechaRegistro] datetime2 NOT NULL CONSTRAINT [DF_CuentaFinanciera_FechaRegistro] DEFAULT SYSUTCDATETIME();
IF COL_LENGTH(N'erp.CuentaFinanciera', N'UsuarioRegistro') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [UsuarioRegistro] nvarchar(max) NOT NULL CONSTRAINT [DF_CuentaFinanciera_UsuarioRegistro] DEFAULT N'migracion';
IF COL_LENGTH(N'erp.CuentaFinanciera', N'Estado') IS NULL
    ALTER TABLE [erp].[CuentaFinanciera] ADD [Estado] int NOT NULL CONSTRAINT [DF_CuentaFinanciera_Estado] DEFAULT 1;
GO

IF COL_LENGTH(N'erp.MovimientoCaja', N'CuentaFinancieraId') IS NULL
    ALTER TABLE [erp].[MovimientoCaja] ADD [CuentaFinancieraId] int NULL;
IF COL_LENGTH(N'erp.CobroCliente', N'CuentaFinancieraId') IS NULL
    ALTER TABLE [erp].[CobroCliente] ADD [CuentaFinancieraId] int NULL;
IF COL_LENGTH(N'erp.Gasto', N'CuentaFinancieraId') IS NULL
    ALTER TABLE [erp].[Gasto] ADD [CuentaFinancieraId] int NULL;
IF COL_LENGTH(N'erp.Ingreso', N'CuentaFinancieraId') IS NULL
    ALTER TABLE [erp].[Ingreso] ADD [CuentaFinancieraId] int NULL;
IF OBJECT_ID(N'erp.PagoProveedor', N'U') IS NOT NULL AND COL_LENGTH(N'erp.PagoProveedor', N'CuentaFinancieraId') IS NULL
    ALTER TABLE [erp].[PagoProveedor] ADD [CuentaFinancieraId] int NULL;
GO

INSERT INTO [erp].[CuentaFinanciera] ([Nombre], [Tipo], [Banco], [NumeroCuenta], [Moneda], [SaldoInicial], [FechaSaldoInicial], [Activo], [EmpresaId], [FechaRegistro], [UsuarioRegistro], [Estado], [UsuarioModificacion])
SELECT N'Caja principal', 1, N'', N'', N'PEN', 0, CAST(GETDATE() AS date), CAST(1 AS bit), e.EmpresaId, SYSUTCDATETIME(), N'migracion-tesoreria', 1, N''
FROM [erp].[Empresa] e
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[CuentaFinanciera] c WHERE c.EmpresaId = e.EmpresaId AND c.Nombre = N'Caja principal'
);

UPDATE m
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM [erp].[MovimientoCaja] m
INNER JOIN [erp].[CuentaFinanciera] c ON c.EmpresaId = m.EmpresaId AND c.Nombre = N'Caja principal'
WHERE m.CuentaFinancieraId IS NULL;

UPDATE cc
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM [erp].[CobroCliente] cc
INNER JOIN [erp].[CuentaFinanciera] c ON c.EmpresaId = cc.EmpresaId AND c.Nombre = N'Caja principal'
WHERE cc.CuentaFinancieraId IS NULL;

UPDATE g
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM [erp].[Gasto] g
INNER JOIN [erp].[CuentaFinanciera] c ON c.EmpresaId = g.EmpresaId AND c.Nombre = N'Caja principal'
WHERE g.CuentaFinancieraId IS NULL;

UPDATE i
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM [erp].[Ingreso] i
INNER JOIN [erp].[CuentaFinanciera] c ON c.EmpresaId = i.EmpresaId AND c.Nombre = N'Caja principal'
WHERE i.CuentaFinancieraId IS NULL;

IF OBJECT_ID(N'erp.PagoProveedor', N'U') IS NOT NULL
BEGIN
    UPDATE p
    SET CuentaFinancieraId = c.CuentaFinancieraId
    FROM [erp].[PagoProveedor] p
    INNER JOIN [erp].[CuentaFinanciera] c ON c.EmpresaId = p.EmpresaId AND c.Nombre = N'Caja principal'
    WHERE p.CuentaFinancieraId IS NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CuentaFinanciera_EmpresaId_Nombre' AND object_id = OBJECT_ID(N'erp.CuentaFinanciera'))
    CREATE UNIQUE INDEX [IX_CuentaFinanciera_EmpresaId_Nombre] ON [erp].[CuentaFinanciera] ([EmpresaId], [Nombre]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CuentaFinanciera_EmpresaId_Tipo' AND object_id = OBJECT_ID(N'erp.CuentaFinanciera'))
    CREATE INDEX [IX_CuentaFinanciera_EmpresaId_Tipo] ON [erp].[CuentaFinanciera] ([EmpresaId], [Tipo]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MovimientoCaja_EmpresaId_CuentaFinancieraId' AND object_id = OBJECT_ID(N'erp.MovimientoCaja'))
    CREATE INDEX [IX_MovimientoCaja_EmpresaId_CuentaFinancieraId] ON [erp].[MovimientoCaja] ([EmpresaId], [CuentaFinancieraId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CobroCliente_EmpresaId_CuentaFinancieraId' AND object_id = OBJECT_ID(N'erp.CobroCliente'))
    CREATE INDEX [IX_CobroCliente_EmpresaId_CuentaFinancieraId] ON [erp].[CobroCliente] ([EmpresaId], [CuentaFinancieraId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Gasto_EmpresaId_CuentaFinancieraId' AND object_id = OBJECT_ID(N'erp.Gasto'))
    CREATE INDEX [IX_Gasto_EmpresaId_CuentaFinancieraId] ON [erp].[Gasto] ([EmpresaId], [CuentaFinancieraId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Ingreso_EmpresaId_CuentaFinancieraId' AND object_id = OBJECT_ID(N'erp.Ingreso'))
    CREATE INDEX [IX_Ingreso_EmpresaId_CuentaFinancieraId] ON [erp].[Ingreso] ([EmpresaId], [CuentaFinancieraId]);
IF OBJECT_ID(N'erp.PagoProveedor', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PagoProveedor_EmpresaId_CuentaFinancieraId' AND object_id = OBJECT_ID(N'erp.PagoProveedor'))
    CREATE INDEX [IX_PagoProveedor_EmpresaId_CuentaFinancieraId] ON [erp].[PagoProveedor] ([EmpresaId], [CuentaFinancieraId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CuentaFinanciera_Empresa_EmpresaId')
    ALTER TABLE [erp].[CuentaFinanciera] ADD CONSTRAINT [FK_CuentaFinanciera_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MovimientoCaja_CuentaFinanciera_CuentaFinancieraId')
    ALTER TABLE [erp].[MovimientoCaja] ADD CONSTRAINT [FK_MovimientoCaja_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CobroCliente_CuentaFinanciera_CuentaFinancieraId')
    ALTER TABLE [erp].[CobroCliente] ADD CONSTRAINT [FK_CobroCliente_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Gasto_CuentaFinanciera_CuentaFinancieraId')
    ALTER TABLE [erp].[Gasto] ADD CONSTRAINT [FK_Gasto_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Ingreso_CuentaFinanciera_CuentaFinancieraId')
    ALTER TABLE [erp].[Ingreso] ADD CONSTRAINT [FK_Ingreso_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
IF OBJECT_ID(N'erp.PagoProveedor', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PagoProveedor_CuentaFinanciera_CuentaFinancieraId')
    ALTER TABLE [erp].[PagoProveedor] ADD CONSTRAINT [FK_PagoProveedor_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
GO

IF OBJECT_ID(N'erp.TransferenciaFinanciera', N'U') IS NULL
BEGIN
    CREATE TABLE [erp].[TransferenciaFinanciera] (
        [TransferenciaFinancieraId] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [CuentaOrigenId] int NOT NULL,
        [CuentaDestinoId] int NOT NULL,
        [Monto] decimal(18,2) NOT NULL,
        [Observacion] nvarchar(500) NOT NULL CONSTRAINT [DF_TransferenciaFinanciera_Observacion] DEFAULT N'',
        [MovimientoEgresoId] int NULL,
        [MovimientoIngresoId] int NULL,
        [FechaAnulacion] datetime2 NULL,
        [MotivoAnulacion] nvarchar(500) NOT NULL CONSTRAINT [DF_TransferenciaFinanciera_MotivoAnulacion] DEFAULT N'',
        [UsuarioAnulacion] nvarchar(120) NOT NULL CONSTRAINT [DF_TransferenciaFinanciera_UsuarioAnulacion] DEFAULT N'',
        [EmpresaId] int NOT NULL,
        [FechaRegistro] datetime2 NOT NULL CONSTRAINT [DF_TransferenciaFinanciera_FechaRegistro] DEFAULT SYSUTCDATETIME(),
        [UsuarioRegistro] nvarchar(max) NOT NULL CONSTRAINT [DF_TransferenciaFinanciera_UsuarioRegistro] DEFAULT N'migracion',
        [Estado] int NOT NULL CONSTRAINT [DF_TransferenciaFinanciera_Estado] DEFAULT 1,
        CONSTRAINT [PK_TransferenciaFinanciera] PRIMARY KEY ([TransferenciaFinancieraId])
    );
END;
GO

IF COL_LENGTH(N'erp.TransferenciaFinanciera', N'FechaAnulacion') IS NULL
    ALTER TABLE [erp].[TransferenciaFinanciera] ADD [FechaAnulacion] datetime2 NULL;
IF COL_LENGTH(N'erp.TransferenciaFinanciera', N'MotivoAnulacion') IS NULL
    ALTER TABLE [erp].[TransferenciaFinanciera] ADD [MotivoAnulacion] nvarchar(500) NOT NULL CONSTRAINT [DF_TransferenciaFinanciera_MotivoAnulacion] DEFAULT N'';
IF COL_LENGTH(N'erp.TransferenciaFinanciera', N'UsuarioAnulacion') IS NULL
    ALTER TABLE [erp].[TransferenciaFinanciera] ADD [UsuarioAnulacion] nvarchar(120) NOT NULL CONSTRAINT [DF_TransferenciaFinanciera_UsuarioAnulacion] DEFAULT N'';
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferenciaFinanciera_EmpresaId_Fecha' AND object_id = OBJECT_ID(N'erp.TransferenciaFinanciera'))
    CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_Fecha] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [Fecha]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferenciaFinanciera_EmpresaId_CuentaOrigenId' AND object_id = OBJECT_ID(N'erp.TransferenciaFinanciera'))
    CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_CuentaOrigenId] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [CuentaOrigenId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferenciaFinanciera_EmpresaId_CuentaDestinoId' AND object_id = OBJECT_ID(N'erp.TransferenciaFinanciera'))
    CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_CuentaDestinoId] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [CuentaDestinoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferenciaFinanciera_CuentaOrigenId' AND object_id = OBJECT_ID(N'erp.TransferenciaFinanciera'))
    CREATE INDEX [IX_TransferenciaFinanciera_CuentaOrigenId] ON [erp].[TransferenciaFinanciera] ([CuentaOrigenId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferenciaFinanciera_CuentaDestinoId' AND object_id = OBJECT_ID(N'erp.TransferenciaFinanciera'))
    CREATE INDEX [IX_TransferenciaFinanciera_CuentaDestinoId] ON [erp].[TransferenciaFinanciera] ([CuentaDestinoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferenciaFinanciera_MovimientoEgresoId' AND object_id = OBJECT_ID(N'erp.TransferenciaFinanciera'))
    CREATE INDEX [IX_TransferenciaFinanciera_MovimientoEgresoId] ON [erp].[TransferenciaFinanciera] ([MovimientoEgresoId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferenciaFinanciera_MovimientoIngresoId' AND object_id = OBJECT_ID(N'erp.TransferenciaFinanciera'))
    CREATE INDEX [IX_TransferenciaFinanciera_MovimientoIngresoId] ON [erp].[TransferenciaFinanciera] ([MovimientoIngresoId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TransferenciaFinanciera_Empresa_EmpresaId')
    ALTER TABLE [erp].[TransferenciaFinanciera] ADD CONSTRAINT [FK_TransferenciaFinanciera_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TransferenciaFinanciera_CuentaFinanciera_CuentaOrigenId')
    ALTER TABLE [erp].[TransferenciaFinanciera] ADD CONSTRAINT [FK_TransferenciaFinanciera_CuentaFinanciera_CuentaOrigenId] FOREIGN KEY ([CuentaOrigenId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TransferenciaFinanciera_CuentaFinanciera_CuentaDestinoId')
    ALTER TABLE [erp].[TransferenciaFinanciera] ADD CONSTRAINT [FK_TransferenciaFinanciera_CuentaFinanciera_CuentaDestinoId] FOREIGN KEY ([CuentaDestinoId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TransferenciaFinanciera_MovimientoCaja_MovimientoEgresoId')
    ALTER TABLE [erp].[TransferenciaFinanciera] ADD CONSTRAINT [FK_TransferenciaFinanciera_MovimientoCaja_MovimientoEgresoId] FOREIGN KEY ([MovimientoEgresoId]) REFERENCES [erp].[MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TransferenciaFinanciera_MovimientoCaja_MovimientoIngresoId')
    ALTER TABLE [erp].[TransferenciaFinanciera] ADD CONSTRAINT [FK_TransferenciaFinanciera_MovimientoCaja_MovimientoIngresoId] FOREIGN KEY ([MovimientoIngresoId]) REFERENCES [erp].[MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION;
GO

IF OBJECT_ID(N'erp.__EFMigrationsHistory', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [erp].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260627090000_AddCuentaFinancieraCajaBancos')
        INSERT INTO [erp].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260627090000_AddCuentaFinancieraCajaBancos', N'8.0.6');

    IF NOT EXISTS (SELECT 1 FROM [erp].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260627103000_AddTransferenciasFinancieras')
        INSERT INTO [erp].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260627103000_AddTransferenciasFinancieras', N'8.0.6');
END;
GO

PRINT N'Esquema de Tesoreria, cuentas financieras y transferencias verificado correctamente.';
GO

