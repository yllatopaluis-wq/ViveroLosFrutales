/*
Agrega forma de pago y observacion a erp.NotaPedido.
Seguro para ejecutar multiples veces.
*/

IF OBJECT_ID(N'erp.NotaPedido', N'U') IS NULL
    THROW 50002, 'No existe la tabla erp.NotaPedido.', 1;

IF COL_LENGTH(N'erp.NotaPedido', N'FormaPago') IS NULL
    ALTER TABLE [erp].[NotaPedido]
        ADD [FormaPago] int NOT NULL CONSTRAINT [DF_NotaPedido_FormaPago] DEFAULT 0;

IF COL_LENGTH(N'erp.NotaPedido', N'Observacion') IS NULL
    ALTER TABLE [erp].[NotaPedido]
        ADD [Observacion] nvarchar(1000) NOT NULL CONSTRAINT [DF_NotaPedido_Observacion] DEFAULT N'';

SELECT
    FormaPago = COL_LENGTH(N'erp.NotaPedido', N'FormaPago'),
    Observacion = COL_LENGTH(N'erp.NotaPedido', N'Observacion');