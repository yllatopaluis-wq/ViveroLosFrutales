/*
Agrega el bloque/campo Observaciones a la configuracion de Cotizacion.
No renombra la columna fisica: el texto se sigue guardando en erp.Cotizacion.CaracteristicasTecnicas.
El cambio de nombre es funcional/visual mediante FormularioCampoConfiguracion.Etiqueta = 'Observaciones'.
*/

IF SCHEMA_ID(N'erp') IS NULL
    THROW 50000, 'No existe el esquema erp.', 1;

IF OBJECT_ID(N'erp.FormularioConfiguracion', N'U') IS NULL
    THROW 50001, 'No existe erp.FormularioConfiguracion. Ejecute primero el script 018.', 1;

IF OBJECT_ID(N'erp.FormularioBloqueConfiguracion', N'U') IS NULL
    THROW 50002, 'No existe erp.FormularioBloqueConfiguracion. Ejecute primero el script 018.', 1;

IF OBJECT_ID(N'erp.FormularioCampoConfiguracion', N'U') IS NULL
    THROW 50003, 'No existe erp.FormularioCampoConfiguracion. Ejecute primero el script 018.', 1;

;WITH Formularios AS (
    SELECT FormularioConfiguracionId
    FROM erp.FormularioConfiguracion
    WHERE TipoDocumento = N'COTIZACION'
      AND Activo = 1
)
INSERT INTO erp.FormularioBloqueConfiguracion (FormularioConfiguracionId, Bloque, Titulo, Visible, Orden, Colapsado)
SELECT f.FormularioConfiguracionId, N'OBSERVACIONES', N'Observaciones', 1, 35, 0
FROM Formularios f
WHERE NOT EXISTS (
    SELECT 1
    FROM erp.FormularioBloqueConfiguracion b
    WHERE b.FormularioConfiguracionId = f.FormularioConfiguracionId
      AND b.Bloque = N'OBSERVACIONES'
);

;WITH Formularios AS (
    SELECT FormularioConfiguracionId
    FROM erp.FormularioConfiguracion
    WHERE TipoDocumento = N'COTIZACION'
      AND Activo = 1
)
INSERT INTO erp.FormularioCampoConfiguracion (FormularioConfiguracionId, Bloque, Campo, Etiqueta, Visible, Obligatorio, SoloLectura, Orden, Ancho, ValorDefecto)
SELECT f.FormularioConfiguracionId, N'OBSERVACIONES', N'CaracteristicasTecnicas', N'Observaciones', 1, 0, 0, 10, N'full', NULL
FROM Formularios f
WHERE NOT EXISTS (
    SELECT 1
    FROM erp.FormularioCampoConfiguracion c
    WHERE c.FormularioConfiguracionId = f.FormularioConfiguracionId
      AND c.Bloque = N'OBSERVACIONES'
      AND c.Campo = N'CaracteristicasTecnicas'
);

UPDATE c
SET Etiqueta = N'Observaciones',
    Bloque = N'OBSERVACIONES',
    Visible = 1,
    SoloLectura = 0,
    Ancho = N'full'
FROM erp.FormularioCampoConfiguracion c
INNER JOIN erp.FormularioConfiguracion f
    ON f.FormularioConfiguracionId = c.FormularioConfiguracionId
WHERE f.TipoDocumento = N'COTIZACION'
  AND c.Campo = N'CaracteristicasTecnicas';

SELECT
    f.FormularioConfiguracionId,
    f.EmpresaId,
    b.Bloque,
    b.Visible AS BloqueVisible,
    c.Campo,
    c.Etiqueta,
    c.Visible AS CampoVisible
FROM erp.FormularioConfiguracion f
INNER JOIN erp.FormularioBloqueConfiguracion b
    ON b.FormularioConfiguracionId = f.FormularioConfiguracionId
LEFT JOIN erp.FormularioCampoConfiguracion c
    ON c.FormularioConfiguracionId = f.FormularioConfiguracionId
   AND c.Bloque = b.Bloque
WHERE f.TipoDocumento = N'COTIZACION'
  AND b.Bloque = N'OBSERVACIONES'
ORDER BY f.EmpresaId, f.FormularioConfiguracionId;
