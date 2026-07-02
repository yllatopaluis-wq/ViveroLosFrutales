USE ViveroLosFrutalesDB;
GO

IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'erp.Cliente', N'U') IS NULL
BEGIN
    THROW 51001, 'No existe la tabla erp.Cliente.', 1;
END;
GO

IF COL_LENGTH(N'erp.Cliente', N'EmpresaId') IS NULL
BEGIN
    ALTER TABLE [erp].[Cliente] ADD [EmpresaId] int NULL;
END;
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Cliente_Tipo_Numero' AND object_id = OBJECT_ID(N'erp.Cliente'))
    DROP INDEX [UX_Cliente_Tipo_Numero] ON [erp].[Cliente];

DECLARE @EmpresaFallbackId int = (SELECT TOP (1) EmpresaId FROM [erp].[Empresa] WHERE Estado = 1 ORDER BY EmpresaId);
IF @EmpresaFallbackId IS NULL
BEGIN
    SET @EmpresaFallbackId = (SELECT TOP (1) EmpresaId FROM [erp].[Empresa] ORDER BY EmpresaId);
END;

IF @EmpresaFallbackId IS NULL
BEGIN
    THROW 51002, 'Debe existir al menos una empresa antes de diferenciar clientes por empresa.', 1;
END;

DECLARE @UsoClienteEmpresa TABLE (
    ClienteId int NOT NULL,
    EmpresaId int NOT NULL,
    PRIMARY KEY (ClienteId, EmpresaId)
);

INSERT INTO @UsoClienteEmpresa (ClienteId, EmpresaId)
SELECT DISTINCT ClienteId, EmpresaId FROM [erp].[Cotizacion] WHERE ClienteId IS NOT NULL
UNION
SELECT DISTINCT ClienteId, EmpresaId FROM [erp].[NotaPedido] WHERE ClienteId IS NOT NULL
UNION
SELECT DISTINCT ClienteId, EmpresaId FROM [erp].[Comprobante] WHERE ClienteId IS NOT NULL
UNION
SELECT DISTINCT ClienteId, EmpresaId FROM [erp].[CobroCliente] WHERE ClienteId IS NOT NULL
UNION
SELECT DISTINCT ClienteId, EmpresaId FROM [erp].[Devolucion] WHERE ClienteId IS NOT NULL
UNION
SELECT DISTINCT ClienteId, EmpresaId FROM [erp].[MovimientoCaja] WHERE ClienteId IS NOT NULL;

INSERT INTO @UsoClienteEmpresa (ClienteId, EmpresaId)
SELECT c.ClienteId, @EmpresaFallbackId
FROM [erp].[Cliente] c
WHERE NOT EXISTS (SELECT 1 FROM @UsoClienteEmpresa u WHERE u.ClienteId = c.ClienteId);

DECLARE @Mapa TABLE (
    ClienteIdOriginal int NOT NULL,
    EmpresaId int NOT NULL,
    ClienteIdEmpresa int NOT NULL,
    PRIMARY KEY (ClienteIdOriginal, EmpresaId)
);

;WITH Usos AS (
    SELECT ClienteId, EmpresaId, ROW_NUMBER() OVER (PARTITION BY ClienteId ORDER BY EmpresaId) AS Orden
    FROM @UsoClienteEmpresa
)
UPDATE c
SET EmpresaId = u.EmpresaId
OUTPUT inserted.ClienteId, inserted.EmpresaId, inserted.ClienteId INTO @Mapa (ClienteIdOriginal, EmpresaId, ClienteIdEmpresa)
FROM [erp].[Cliente] c
JOIN Usos u ON u.ClienteId = c.ClienteId AND u.Orden = 1
WHERE c.EmpresaId IS NULL;

INSERT INTO @Mapa (ClienteIdOriginal, EmpresaId, ClienteIdEmpresa)
SELECT c.ClienteId, c.EmpresaId, c.ClienteId
FROM [erp].[Cliente] c
WHERE c.EmpresaId IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM @Mapa m WHERE m.ClienteIdOriginal = c.ClienteId AND m.EmpresaId = c.EmpresaId
  );

;WITH UsosPendientes AS (
    SELECT u.ClienteId, u.EmpresaId
    FROM @UsoClienteEmpresa u
    WHERE NOT EXISTS (SELECT 1 FROM @Mapa m WHERE m.ClienteIdOriginal = u.ClienteId AND m.EmpresaId = u.EmpresaId)
)
INSERT INTO [erp].[Cliente] ([EmpresaId], [TipoDocumento], [NumeroDocumento], [NombreCompleto], [Email], [Direccion], [Telefono], [FechaModificacion], [UsuarioModificacion], [FechaRegistro], [UsuarioRegistro], [Estado])
SELECT src.EmpresaId, c.TipoDocumento, c.NumeroDocumento, c.NombreCompleto, c.Email, c.Direccion, c.Telefono, SYSUTCDATETIME(), N'migracion-clientes-empresa', c.FechaRegistro, c.UsuarioRegistro, c.Estado
FROM UsosPendientes src
JOIN [erp].[Cliente] c ON c.ClienteId = src.ClienteId
WHERE NOT EXISTS (
    SELECT 1 FROM [erp].[Cliente] actual
    WHERE actual.EmpresaId = src.EmpresaId
      AND actual.TipoDocumento = c.TipoDocumento
      AND actual.NumeroDocumento = c.NumeroDocumento
);

INSERT INTO @Mapa (ClienteIdOriginal, EmpresaId, ClienteIdEmpresa)
SELECT src.ClienteId, src.EmpresaId, actual.ClienteId
FROM @UsoClienteEmpresa src
JOIN [erp].[Cliente] c ON c.ClienteId = src.ClienteId
JOIN [erp].[Cliente] actual ON actual.EmpresaId = src.EmpresaId AND actual.TipoDocumento = c.TipoDocumento AND actual.NumeroDocumento = c.NumeroDocumento
WHERE NOT EXISTS (SELECT 1 FROM @Mapa m WHERE m.ClienteIdOriginal = src.ClienteId AND m.EmpresaId = src.EmpresaId);

UPDATE t SET ClienteId = m.ClienteIdEmpresa FROM [erp].[Cotizacion] t JOIN @Mapa m ON m.ClienteIdOriginal = t.ClienteId AND m.EmpresaId = t.EmpresaId WHERE t.ClienteId <> m.ClienteIdEmpresa;
UPDATE t SET ClienteId = m.ClienteIdEmpresa FROM [erp].[NotaPedido] t JOIN @Mapa m ON m.ClienteIdOriginal = t.ClienteId AND m.EmpresaId = t.EmpresaId WHERE t.ClienteId <> m.ClienteIdEmpresa;
UPDATE t SET ClienteId = m.ClienteIdEmpresa FROM [erp].[Comprobante] t JOIN @Mapa m ON m.ClienteIdOriginal = t.ClienteId AND m.EmpresaId = t.EmpresaId WHERE t.ClienteId <> m.ClienteIdEmpresa;
UPDATE t SET ClienteId = m.ClienteIdEmpresa FROM [erp].[CobroCliente] t JOIN @Mapa m ON m.ClienteIdOriginal = t.ClienteId AND m.EmpresaId = t.EmpresaId WHERE t.ClienteId <> m.ClienteIdEmpresa;
UPDATE t SET ClienteId = m.ClienteIdEmpresa FROM [erp].[Devolucion] t JOIN @Mapa m ON m.ClienteIdOriginal = t.ClienteId AND m.EmpresaId = t.EmpresaId WHERE t.ClienteId <> m.ClienteIdEmpresa;
UPDATE t SET ClienteId = m.ClienteIdEmpresa FROM [erp].[MovimientoCaja] t JOIN @Mapa m ON m.ClienteIdOriginal = t.ClienteId AND m.EmpresaId = t.EmpresaId WHERE t.ClienteId <> m.ClienteIdEmpresa;

UPDATE [erp].[Cliente] SET EmpresaId = @EmpresaFallbackId WHERE EmpresaId IS NULL;

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Cliente_Tipo_Numero' AND object_id = OBJECT_ID(N'erp.Cliente'))
    DROP INDEX [UX_Cliente_Tipo_Numero] ON [erp].[Cliente];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cliente_NombreCompleto' AND object_id = OBJECT_ID(N'erp.Cliente'))
    DROP INDEX [IX_Cliente_NombreCompleto] ON [erp].[Cliente];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cliente_EmpresaId_NombreCompleto' AND object_id = OBJECT_ID(N'erp.Cliente'))
    DROP INDEX [IX_Cliente_EmpresaId_NombreCompleto] ON [erp].[Cliente];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Cliente_Empresa_Tipo_Numero' AND object_id = OBJECT_ID(N'erp.Cliente'))
    DROP INDEX [UX_Cliente_Empresa_Tipo_Numero] ON [erp].[Cliente];

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Cliente_Empresa_EmpresaId')
    ALTER TABLE [erp].[Cliente] DROP CONSTRAINT [FK_Cliente_Empresa_EmpresaId];

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'erp.Cliente') AND name = N'EmpresaId' AND is_nullable = 1)
    ALTER TABLE [erp].[Cliente] ALTER COLUMN [EmpresaId] int NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Cliente_Empresa_EmpresaId')
    ALTER TABLE [erp].[Cliente] ADD CONSTRAINT [FK_Cliente_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cliente_EmpresaId_NombreCompleto' AND object_id = OBJECT_ID(N'erp.Cliente'))
    CREATE INDEX [IX_Cliente_EmpresaId_NombreCompleto] ON [erp].[Cliente] ([EmpresaId], [NombreCompleto]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Cliente_Empresa_Tipo_Numero' AND object_id = OBJECT_ID(N'erp.Cliente'))
    CREATE UNIQUE INDEX [UX_Cliente_Empresa_Tipo_Numero] ON [erp].[Cliente] ([EmpresaId], [TipoDocumento], [NumeroDocumento]);

PRINT N'Clientes diferenciados por empresa correctamente.';
GO
