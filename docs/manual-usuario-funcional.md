# Manual de usuario funcional - Vivero Los Frutales

## 1. Acceso al sistema

1. Abrir la URL del ERP.
2. Ingresar usuario y contraseña.
3. Seleccionar la empresa con la que se trabajará.
4. Presionar `Ingresar`.

Usuario inicial de salida:

```text
Usuario: admin
Password: Admin1234
```

Después del primer ingreso, cambiar la contraseña del usuario administrador.

Si el usuario no tiene acceso a la empresa seleccionada, el sistema no permitirá iniciar sesión.

## 2. Empresa activa

La empresa seleccionada en el login queda activa para toda la sesión. Todas las operaciones de productos, comprobantes, notas de pedido, compras, caja, gastos, ingresos y reportes se registran para esa empresa.

Cuando el usuario tiene más de una empresa asignada, puede cambiar de empresa desde la opción de selección de empresa sin crear un nuevo usuario.

## 3. Tablero de control

El tablero muestra un resumen operativo de la empresa activa. Sirve como punto de entrada para revisar indicadores principales y acceder rápidamente a módulos de trabajo.

## 4. Maestros

### 4.1 Categorías

Ruta: `Maestros > Categorías`.

Permite crear y mantener categorías de productos por empresa.

Uso básico:

1. Entrar a Categorías.
2. Presionar `Nuevo` o editar una categoría existente.
3. Completar nombre y descripción.
4. Guardar.

Las categorías activas aparecen luego en el formulario de productos.

### 4.2 Productos

Ruta: `Maestros > Productos`.

Permite registrar productos o servicios de venta.

Campos principales:

- Categoría.
- Nombre.
- Unidad de medida.
- Stock.
- Precio de venta sin IGV.
- Precio de venta con IGV.
- Afecto a IGV.
- Detracción y porcentaje.
- Estado.

Reglas de uso:

- El precio con IGV se calcula cuando el producto está afecto.
- El stock aumenta al registrar compras.
- El stock se descuenta en ventas según la lógica del documento registrado.
- Los productos son por empresa; cada empresa tiene su propio catálogo y stock.

### 4.3 Clientes

Ruta: `Maestros > Clientes`.

Los clientes son globales para el sistema y pueden usarse en cualquier empresa activa.

Campos principales:

- Tipo de documento.
- Número de documento.
- Nombre o razón social.
- Email.
- Dirección.
- Teléfono.
- Estado.

Los clientes se usan en cotizaciones, notas de pedido, comprobantes, cobros y estados de cuenta.

### 4.4 Proveedores

Ruta: `Maestros > Proveedores`.

Los proveedores se administran por empresa y se usan en compras y pagos a proveedores.

Campos principales:

- Tipo y número de documento.
- Razón social o nombre.
- Nombre comercial.
- Dirección.
- Teléfono.
- Email.
- Estado.

## 5. Ventas

### 5.1 Cotizaciones

Ruta: `Ventas > Cotizaciones`.

Una cotización es una propuesta comercial. No afecta caja, stock, deuda ni SUNAT.

Flujo recomendado:

1. Crear cotización.
2. Seleccionar cliente.
3. Agregar productos, cantidades y precios.
4. Revisar subtotal, IGV y total.
5. Guardar.
6. Imprimir o generar PDF cuando sea necesario.
7. Convertir a nota de pedido si el cliente acepta la propuesta.

Reglas importantes:

- Una cotización no registra cobros.
- Una cotización no se convierte directamente en boleta o factura.
- La conversión válida es a nota de pedido.
- La cotizacion conserva los datos del cliente usados al guardarla. Cambios posteriores en el maestro no modifican la cotizacion ni su PDF.

### 5.2 Notas de pedido

Ruta: `Ventas > Notas de Pedido`.

La nota de pedido es un documento interno de venta. Puede recibir cobros y luego convertirse a comprobante.

Flujo recomendado:

1. Crear nota de pedido.
2. Seleccionar cliente.
3. Agregar productos y cantidades.
4. Guardar.
5. Registrar cobros parciales o totales.
6. Convertir a comprobante cuando esté pagada.

Reglas de cobro:

- Puede registrar cobros mientras no tenga comprobante relacionado.
- Si ya fue convertida a comprobante, los nuevos cobros se gestionan desde el comprobante.
- El estado de pago puede ser `PENDIENTE`, `PARCIAL` o `PAGADO`.

Conversión a comprobante:

- Si el cliente tiene RUC, se genera factura.
- Si el cliente no tiene RUC, se genera boleta.
- La nota debe estar activa, pagada y sin saldo pendiente.
- Los cobros registrados en la nota se aplican al comprobante generado.
- La nota de pedido conserva los datos del cliente de ese momento. Si proviene de una cotizacion, mantiene los datos que venian de esa cotizacion.

Anulación:

- Requiere motivo.
- Si tiene cobros activos, no se borra la caja histórica.
- Si corresponde devolver dinero, se genera una devolución pendiente.

### 5.3 Comprobantes

Ruta: `Ventas > Comprobantes`.

Permite crear boletas y facturas.

Flujo recomendado para venta directa:

1. Crear comprobante.
2. Elegir tipo: boleta o factura.
3. Seleccionar cliente.
4. Agregar productos.
5. Elegir forma de pago.
6. Guardar.
7. Imprimir o generar PDF para enviar a Nubefact.

Reglas principales:

- Una venta al contado genera cobro y movimiento de caja.
- Una venta a crédito queda pendiente hasta registrar cobro.
- No se puede cobrar un comprobante anulado, pagado o de tipo nota de crédito.
- Solo se puede editar si no fue impreso y no fue aceptado por SUNAT.
- Para anular se exige motivo.
- Si tiene cobros activos y se anula, se genera devolución pendiente.
- El comprobante conserva los datos del cliente usados al emitirlo. Cambios posteriores en el maestro no modifican comprobantes ya emitidos ni notas de credito relacionadas.

### 5.4 Notas de crédito

Ruta: `Ventas > Notas de Crédito`.

Permite emitir una nota de crédito contra una boleta o factura aceptada por SUNAT.

Flujo recomendado:

1. Buscar el comprobante origen por serie, número, cliente o documento.
2. Seleccionar el comprobante origen.
3. Elegir motivo de nota de crédito.
4. Revisar detalle cargado automáticamente.
5. Indicar cantidades a afectar.
6. Emitir.
7. Imprimir o generar PDF.

Reglas principales:

- No se emite contra cotizaciones ni notas de pedido.
- No registra cobros ni movimientos de caja por sí misma.
- Si genera exceso cobrado, el sistema crea una devolución pendiente.

### 5.5 Cobros de clientes

Ruta: `Ventas > Cobros de Clientes`.

Permite consultar y anular cobros registrados a notas de pedido o comprobantes.

La lista muestra:

- Fecha.
- Cliente.
- Referencia.
- Medio de pago.
- Monto.
- Estado.

Para anular un cobro:

1. Presionar anular.
2. Registrar motivo.
3. Confirmar.

Al anular, el cobro y su movimiento de caja quedan anulados, y se recalcula el estado de pago del documento relacionado.

### 5.6 Devoluciones de clientes

Ruta: `Ventas > Devoluciones de Clientes`.

Administra montos pendientes por devolver al cliente.

Se generan por:

- Anulación de nota de pedido con cobros.
- Anulación de comprobante con cobros.
- Nota de crédito con exceso cobrado.

Para registrar devolución:

1. Abrir la devolución pendiente.
2. Presionar registrar devolución.
3. Ingresar monto, medio de pago y observación.
4. Guardar.

Al registrar devolución se genera un egreso en caja.

## 6. Compras y proveedores

### 6.1 Compras

Ruta: `Operación > Compras`.

Permite registrar compras a proveedores.

Flujo recomendado:

1. Crear compra.
2. Seleccionar proveedor.
3. Registrar documento, fecha y forma de pago.
4. Agregar productos, cantidades y costos.
5. Guardar.

Al guardar una compra:

- Aumenta el stock de los productos.
- Se registra movimiento de inventario.
- Si queda pendiente de pago, aparece en cuentas por pagar.


Tipos de documento de compra disponibles:

- `FACTURA`.
- `BOLETA`.
- `LIQUIDACION COMPRA`.
- `RECIBO`.
- `NOTA VENTA`.
- `PENDIENTE COMPROBANTE`.
- `SIN DOCUMENTO`.

Reglas:

- `FACTURA`, `BOLETA` y `LIQUIDACION COMPRA` piden serie y numero.
- `RECIBO`, `NOTA VENTA`, `PENDIENTE COMPROBANTE` y `SIN DOCUMENTO` no piden serie ni numero.
- `PENDIENTE COMPROBANTE` es un tipo de documento, no un estado.
- El costo unitario se ingresa con IGV. Si el producto esta afecto a IGV, el sistema separa base e IGV con factor `1.18`; si no esta afecto, no calcula IGV.

### 6.2 Pagos a proveedores

Desde el detalle de compra se puede registrar pago.

Reglas:

- El pago genera movimiento de caja de tipo egreso.
- Un pago anulado deja de afectar los saldos.
- Para anular pago se solicita motivo.

## 7. Gastos e ingresos manuales

### 7.1 Gastos

Ruta: `Operación > Gastos`.

Registra salidas de dinero no relacionadas a compras de inventario.

Ejemplos:

- Movilidad.
- Combustible.
- Luz.
- Alquiler.
- Internet.
- Herramientas menores.

Al guardar un gasto se genera un egreso en caja.

### 7.2 Ingresos

Ruta: `Operación > Ingresos`.

Registra entradas de dinero no relacionadas a ventas.

Ejemplos:

- Préstamo recibido.
- Aporte de socios.
- Embalaje.
- Flete.

Al guardar un ingreso se genera un ingreso en caja.

## 8. Caja

Ruta: `Operación > Caja`.

Caja muestra movimientos reales de dinero.

Incluye:

- Cobros de clientes.
- Pagos a proveedores.
- Gastos.
- Ingresos manuales.
- Devoluciones de clientes.

El resumen muestra:

- Ingresos.
- Egresos.
- Saldo.
- Cantidad de movimientos.

Los movimientos anulados pueden verse, pero no suman en los totales.

## 9. Reportes

### 9.1 Reporte general

Ruta: `Reportes > Reporte General`.

Presenta indicadores mensuales y anuales:

- Resultado neto.
- Ventas netas.
- Ingresos manuales.
- Gastos.
- Compras.

El resultado se calcula como ventas más ingresos, menos gastos y compras.

### 9.2 Estado de cuenta de clientes

Permite revisar documentos, cobros y saldos relacionados a clientes.

### 9.3 Cuentas por pagar

Permite revisar compras pendientes de pago a proveedores.

### 9.4 Devoluciones proveedor

Permite consultar devoluciones o saldos relacionados con proveedores cuando aplique.


### 9.5 Reporte de notas de pedido

Ruta: `Reportes > Notas de pedido`.

Permite consultar y exportar notas de pedido filtrando por cliente/documento, numero, fechas, estado de pago y estado del documento. Muestra cartillas de total de notas, total vendido, total cobrado, saldo pendiente, pendientes, parciales, pagadas y anuladas.

### 9.6 Reporte de comprobantes

Ruta: `Reportes > Comprobantes`.

Permite consultar y exportar boletas y facturas filtrando por tipo, serie, numero, fechas, cliente, estado SUNAT, estado del comprobante, medio de pago y vendedor. Muestra cartillas de total de comprobantes, importe, IGV, gravado, exonerado, cancelado y por cobrar. La moneda se visualiza como `Soles`.

## 10. Administración

### 10.1 Empresas

Ruta: `Administración > Empresas`.

Permite configurar datos fiscales y operativos:

- RUC.
- Razón social.
- Nombre comercial.
- Dirección.
- Teléfono.
- Email.
- Series.
- Token y URL Nubefact.
- Logo.

### 10.2 Usuarios

Ruta: `Administración > Usuarios`.

Permite crear usuarios, asignar rol, asociar empresas y restablecer contraseña.

Todo usuario debe tener al menos una empresa asignada para poder iniciar sesión.

### 10.3 Roles

Ruta: `Administración > Roles`.

Permite definir permisos por módulos, formularios y acciones.

Reglas de selección:

- Marcar un módulo selecciona todos sus formularios.
- Marcar un formulario selecciona todas sus acciones.
- Se pueden seleccionar acciones específicas.

### 10.4 Configuración

Ruta: `Administración > Configuración`.

Permite mantener parámetros por empresa, incluyendo correlativos cuando corresponda.

### 10.5 Log Nubefact

Ruta: `Administración > Log Nubefact`.

Permite revisar solicitudes, respuestas, PDF/XML y estado de operaciones enviadas o consultadas en Nubefact.

### 10.6 Errores de aplicación

Ruta: `Administración > Errores de aplicación`.

Permite revisar errores técnicos registrados por el sistema, filtrar por estado o fecha y marcarlos como revisados con observación.

## 11. Sitio público

El sistema incluye una web pública separada del ERP.

Páginas principales:

- Inicio.
- Nosotros.
- Productos.
- Servicios.
- Contacto.

El sitio público no permite modificar información interna ni registrar operaciones. Su objetivo es mostrar información comercial y de contacto.

## 12. Buenas prácticas operativas

- Cambiar la contraseña inicial del administrador.
- Trabajar siempre en la empresa correcta.
- Revisar datos de cliente antes de emitir comprobantes.
- Generar PDF o imprimir comprobantes para enviarlos a Nubefact.
- Registrar cobros y pagos con medio de pago correcto.
- No anular documentos sin motivo claro.
- Revisar periódicamente devoluciones pendientes.
- Usar reportes y caja para conciliación diaria.
- Mantener token Nubefact y series actualizadas por empresa.
