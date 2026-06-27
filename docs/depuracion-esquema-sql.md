# Depuracion del esquema SQL

## Resultado

El esquema de instalacion se genero desde `ApplicationDbContext`, por lo que las tablas, columnas, relaciones e indices coinciden con el modelo EF vigente.

Los scripts vigentes de instalacion y carga inicial son:

1. `001-create-database.sql`: base, esquema completo, Identity, tablas ERP y datos maestros estaticos.
2. `002-cargar-empresa-inicial.sql`: empresas Vivero Los Frutales Lima y Huaral.
3. `003-cargar-categorias-financieras.sql`: categorias iniciales de gastos e ingresos por empresa.
4. `004-cargar-productos-catalogo.sql`: catalogo base del Excel Nubefact para ambas empresas.
5. `005-cargar-clientes-entidades.sql`: clientes y entidades historicas en `erp.Cliente`.
6. `006-cargar-productos-por-empresa.sql`: version alternativa/idempotente de carga de productos por empresa.
7. `007-cargar-usuario-admin.sql`: rol Identity, usuario administrador y relacion con `erp.UsuarioEmpresa`.

## Elementos retirados

- Tabla, entidad, repositorio, servicio, controlador y vistas del modelo legado `DevolucionCliente`.
- Scripts duplicados de Identity y modulos operativos.
- Parches `ALTER TABLE`, backfills y normalizaciones usados durante el desarrollo.
- Script de reinicio de documentos transaccionales.
- Columnas historicas que no pertenecen al modelo actual, como `Cliente.EmpresaId` y `CobroCliente.TipoCobro`.

## Campos conservados deliberadamente

- Auditoria: `FechaRegistro`, `UsuarioRegistro`, `FechaModificacion`, `UsuarioModificacion` y `Estado`.
- Trazabilidad financiera: totales cobrados/pagados, saldos, estados, referencias y motivos de anulacion.
- Datos de emision: snapshots de empresa, URLs, XML, hash y respuestas Nubefact.
- Relaciones de caja, categorias, documentos de origen y devoluciones.

Aunque algunos de estos campos no se editan directamente en formularios, participan en servicios, reportes, PDFs, integraciones o reconstruccion historica.

## Alcance

`001-create-database.sql` esta destinado exclusivamente a instalaciones nuevas. No reemplaza una migracion de una base existente ni elimina automaticamente tablas antiguas de una base ya creada. Para una salida productiva nueva, ejecutar los scripts del `001` al `007` en orden.
