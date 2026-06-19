# Devoluciones unificadas

## Modelo implementado

La tabla `Devolucion` centraliza las devoluciones de dinero de clientes y proveedores.

Estados:

- `PENDIENTE`
- `PARCIAL`
- `DEVUELTO`
- `ANULADO`

Tipos de tercero:

- `CLIENTE`
- `PROVEEDOR`

Origenes:

- `ANULACION_NOTA_PEDIDO`
- `NOTA_CREDITO`
- `ANULACION_COMPRA`

## Reglas implementadas

- Caja se calcula exclusivamente desde `MovimientoCaja`.
- Los cobros historicos no se anulan al anular una nota de pedido.
- Los cobros historicos no se anulan al emitir una nota de credito.
- La devolucion real recien genera movimiento de caja cuando el usuario registra la devolucion.
- Devolucion a cliente genera `MovimientoCaja` de tipo `EGRESO` con origen `DEVOLUCION_CLIENTE`.
- Devolucion de proveedor genera `MovimientoCaja` de tipo `INGRESO` con origen `DEVOLUCION_PROVEEDOR`.
- No se permite devolver un monto mayor al pendiente.
- No se permite registrar devoluciones sobre estados `DEVUELTO` o `ANULADO`.
- No se duplica una devolucion activa para el mismo origen/documento.

## Flujos implementados

### Anulacion de Nota de Pedido con cobros

Al anular una nota de pedido con cobros activos:

- La nota queda `ANULADO`.
- Los cobros quedan historicos.
- Los movimientos de caja de cobro quedan historicos.
- Se crea una `Devolucion` pendiente al cliente por el total cobrado activo.

### Nota de Credito con comprobante pagado

Al emitir una nota de credito:

- No se anulan cobros.
- No se anulan movimientos de caja.
- Si lo cobrado supera el nuevo total valido, se crea una `Devolucion` pendiente por el exceso.

### Anulacion de Comprobante con cobros

Al anular una boleta o factura con cobros activos:

- El comprobante queda `ANULADO`.
- Los cobros quedan historicos.
- Los movimientos de caja de cobro quedan historicos.
- Se crea una `Devolucion` pendiente al cliente por el total cobrado activo.

### Registro real de devolucion

Desde `Operacion -> Devoluciones`:

- Se puede ver el detalle.
- Se puede registrar devolucion total o parcial.
- No se puede anular una solicitud de devolucion pendiente. La devolucion debe registrarse para mantener el sustento del dinero pendiente.

## Preparado tecnicamente

El origen `ANULACION_COMPRA` y el tipo `PROVEEDOR` estan soportados en dominio, servicio, repositorio, caja y UI. La generacion automatica desde anulacion de compra queda preparada porque actualmente el proyecto no tiene estado de compra ni modulo de pagos proveedor implementado.
