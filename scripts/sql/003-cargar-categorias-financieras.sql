IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');
GO

/*
    Datos iniciales de gastos e ingresos.

    Ejecutar despues de 001-create-database.sql y despues de registrar al menos
    una empresa (el primer arranque de la aplicacion crea la empresa configurada
    en appsettings.json). El script es idempotente.
*/

USE ViveroLosFrutalesDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF NOT EXISTS (SELECT 1 FROM erp.Empresa)
BEGIN
    THROW 51000, 'No existen empresas. Inicie la aplicacion o registre una empresa antes de cargar las categorias financieras.', 1;
END;
GO

DECLARE @CategoriasGasto TABLE (Nombre nvarchar(100) NOT NULL);
INSERT INTO @CategoriasGasto (Nombre) VALUES
(N'MOVILIDAD'), (N'COMBUSTIBLE'), (N'LUZ'), (N'ALQUILER'), (N'INTERNET'), (N'MANTENIMIENTO'),
(N'HERRAMIENTAS MENORES'), (N'VIATICOS'), (N'COMISION'), (N'CAMPO PALTA'), (N'PAGO PERSONAL'), (N'PAPELERIA');

INSERT INTO erp.CategoriaGasto (EmpresaId, Nombre, FechaRegistro, UsuarioRegistro, Estado)
SELECT e.EmpresaId, c.Nombre, SYSUTCDATETIME(), N'system', 1
FROM erp.Empresa e
CROSS JOIN @CategoriasGasto c
WHERE NOT EXISTS (
    SELECT 1
    FROM erp.CategoriaGasto x
    WHERE x.EmpresaId = e.EmpresaId AND x.Nombre = c.Nombre
);
GO

DECLARE @CategoriasIngreso TABLE (Nombre nvarchar(100) NOT NULL);
INSERT INTO @CategoriasIngreso (Nombre) VALUES
(N'PRESTAMO RECIBIDO'), (N'APORTE DE SOCIOS'), (N'ALQUILER'), (N'EMBALAJE'), (N'FLETE');

INSERT INTO erp.CategoriaIngreso (EmpresaId, Nombre, FechaRegistro, UsuarioRegistro, Estado)
SELECT e.EmpresaId, c.Nombre, SYSUTCDATETIME(), N'system', 1
FROM erp.Empresa e
CROSS JOIN @CategoriasIngreso c
WHERE NOT EXISTS (
    SELECT 1
    FROM erp.CategoriaIngreso x
    WHERE x.EmpresaId = e.EmpresaId AND x.Nombre = c.Nombre
);
GO

