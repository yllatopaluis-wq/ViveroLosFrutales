# Depuracion del esquema SQL

## Resultado

El esquema de instalacion se genero desde `ApplicationDbContext`, por lo que las tablas, columnas, relaciones e indices coinciden con el modelo EF vigente.

Los quince scripts incrementales se redujeron a:

1. `001-create-database.sql`: base, esquema completo, Identity y datos maestros estaticos.
2. `002-cargar-categorias-financieras.sql`: categorias iniciales de gastos e ingresos para las empresas existentes.
3. `003-cargar-productos-catalogo.sql`: catalogo de productos para la empresa con RUC `20615082997`.

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

`001-create-database.sql` esta destinado exclusivamente a instalaciones nuevas. No reemplaza una migracion de una base existente ni elimina automaticamente tablas antiguas de una base ya creada.

