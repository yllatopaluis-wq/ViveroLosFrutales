SET NOCOUNT ON;

DECLARE @Faltantes TABLE (
    Orden int IDENTITY(1,1) NOT NULL,
    Objeto nvarchar(200) NOT NULL,
    Instruccion nvarchar(300) NOT NULL
);

IF SCHEMA_ID(N'erp') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp schema', N'Ejecutar 001-create-database.sql o verificar la base seleccionada.');

IF OBJECT_ID(N'erp.CuentaFinanciera', N'U') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.CuentaFinanciera', N'Ejecutar 008-fix-tesoreria-cuentas-financieras.sql.');

IF OBJECT_ID(N'erp.TransferenciaFinanciera', N'U') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.TransferenciaFinanciera', N'Ejecutar 008-fix-tesoreria-cuentas-financieras.sql.');

IF COL_LENGTH(N'erp.MovimientoCaja', N'CuentaFinancieraId') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.MovimientoCaja.CuentaFinancieraId', N'Ejecutar 008-fix-tesoreria-cuentas-financieras.sql.');
IF COL_LENGTH(N'erp.CobroCliente', N'CuentaFinancieraId') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.CobroCliente.CuentaFinancieraId', N'Ejecutar 008-fix-tesoreria-cuentas-financieras.sql.');
IF COL_LENGTH(N'erp.Gasto', N'CuentaFinancieraId') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Gasto.CuentaFinancieraId', N'Ejecutar 008-fix-tesoreria-cuentas-financieras.sql.');
IF COL_LENGTH(N'erp.Ingreso', N'CuentaFinancieraId') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Ingreso.CuentaFinancieraId', N'Ejecutar 008-fix-tesoreria-cuentas-financieras.sql.');
IF OBJECT_ID(N'erp.PagoProveedor', N'U') IS NOT NULL AND COL_LENGTH(N'erp.PagoProveedor', N'CuentaFinancieraId') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.PagoProveedor.CuentaFinancieraId', N'Ejecutar 008-fix-tesoreria-cuentas-financieras.sql.');

IF COL_LENGTH(N'erp.Cliente', N'EmpresaId') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Cliente.EmpresaId', N'Ejecutar 009-diferenciar-clientes-por-empresa.sql.');

IF COL_LENGTH(N'erp.Empresa', N'RepresentanteLegalNombre') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Empresa.RepresentanteLegalNombre', N'Ejecutar 010-add-representante-legal-empresa.sql.');
IF COL_LENGTH(N'erp.Empresa', N'RepresentanteLegalDocumento') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Empresa.RepresentanteLegalDocumento', N'Ejecutar 010-add-representante-legal-empresa.sql.');
IF COL_LENGTH(N'erp.Empresa', N'RepresentanteLegalCargo') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Empresa.RepresentanteLegalCargo', N'Ejecutar 010-add-representante-legal-empresa.sql.');
IF COL_LENGTH(N'erp.Empresa', N'FirmaContenido') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Empresa.FirmaContenido', N'Ejecutar 010-add-representante-legal-empresa.sql.');
IF COL_LENGTH(N'erp.Empresa', N'FirmaContentType') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Empresa.FirmaContentType', N'Ejecutar 010-add-representante-legal-empresa.sql.');
IF COL_LENGTH(N'erp.Empresa', N'FirmaNombre') IS NULL
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'erp.Empresa.FirmaNombre', N'Ejecutar 010-add-representante-legal-empresa.sql.');

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Cliente_Empresa_Tipo_Numero' AND object_id = OBJECT_ID(N'erp.Cliente'))
    INSERT INTO @Faltantes (Objeto, Instruccion) VALUES (N'UX_Cliente_Empresa_Tipo_Numero', N'Ejecutar 009-diferenciar-clientes-por-empresa.sql.');

IF EXISTS (SELECT 1 FROM @Faltantes)
BEGIN
    SELECT Objeto, Instruccion FROM @Faltantes ORDER BY Orden;
    THROW 51011, 'El esquema publicado no esta actualizado. Ejecute los scripts indicados y vuelva a validar.', 1;
END;

PRINT N'Esquema publicado validado correctamente.';
