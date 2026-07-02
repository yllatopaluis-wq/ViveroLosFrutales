USE ViveroLosFrutalesDB;
GO

IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'erp.Empresa', N'U') IS NULL
BEGIN
    THROW 51010, 'No existe la tabla erp.Empresa.', 1;
END;
GO

IF COL_LENGTH(N'erp.Empresa', N'RepresentanteLegalNombre') IS NULL
    ALTER TABLE [erp].[Empresa] ADD [RepresentanteLegalNombre] nvarchar(200) NOT NULL CONSTRAINT [DF_Empresa_RepresentanteLegalNombre] DEFAULT N'';

IF COL_LENGTH(N'erp.Empresa', N'RepresentanteLegalDocumento') IS NULL
    ALTER TABLE [erp].[Empresa] ADD [RepresentanteLegalDocumento] nvarchar(20) NOT NULL CONSTRAINT [DF_Empresa_RepresentanteLegalDocumento] DEFAULT N'';

IF COL_LENGTH(N'erp.Empresa', N'RepresentanteLegalCargo') IS NULL
    ALTER TABLE [erp].[Empresa] ADD [RepresentanteLegalCargo] nvarchar(120) NOT NULL CONSTRAINT [DF_Empresa_RepresentanteLegalCargo] DEFAULT N'';

IF COL_LENGTH(N'erp.Empresa', N'FirmaContenido') IS NULL
    ALTER TABLE [erp].[Empresa] ADD [FirmaContenido] varbinary(max) NULL;

IF COL_LENGTH(N'erp.Empresa', N'FirmaContentType') IS NULL
    ALTER TABLE [erp].[Empresa] ADD [FirmaContentType] nvarchar(120) NOT NULL CONSTRAINT [DF_Empresa_FirmaContentType] DEFAULT N'';

IF COL_LENGTH(N'erp.Empresa', N'FirmaNombre') IS NULL
    ALTER TABLE [erp].[Empresa] ADD [FirmaNombre] nvarchar(260) NOT NULL CONSTRAINT [DF_Empresa_FirmaNombre] DEFAULT N'';

PRINT N'Datos de representante legal y firma agregados a Empresa.';
GO
