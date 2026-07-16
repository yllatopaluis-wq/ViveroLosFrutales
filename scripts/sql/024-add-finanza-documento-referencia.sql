SET XACT_ABORT ON;

IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Gasto', N'DocumentoReferencia') IS NULL
BEGIN
    ALTER TABLE [erp].[Gasto]
        ADD [DocumentoReferencia] nvarchar(120) NOT NULL
            CONSTRAINT [DF_Gasto_DocumentoReferencia] DEFAULT N'';
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Ingreso', N'DocumentoReferencia') IS NULL
BEGIN
    ALTER TABLE [erp].[Ingreso]
        ADD [DocumentoReferencia] nvarchar(120) NOT NULL
            CONSTRAINT [DF_Ingreso_DocumentoReferencia] DEFAULT N'';
END;

SELECT
    GastoDocumentoReferencia = COL_LENGTH(N'erp.Gasto', N'DocumentoReferencia'),
    IngresoDocumentoReferencia = COL_LENGTH(N'erp.Ingreso', N'DocumentoReferencia');
