# Documentacion funcional - Vivero Los Frutales

## 1. Objetivo

Vivero Los Frutales es un sistema web multiempresa para gestionar ventas, compras, productos, clientes, proveedores, caja operativa, comprobantes, cotizaciones, notas de pedido y facturacion electronica con Nubefact.

## 2. Acceso al sistema

El usuario ingresa desde el formulario de login indicando:

- Usuario.
- Contrasena.
- Empresa.

La empresa se selecciona en el login. Luego de validar las credenciales, el sistema verifica que el usuario tenga acceso a la empresa seleccionada. Si no tiene acceso, se cancela el inicio de sesion.

La empresa activa se guarda en la sesion y se muestra en la cabecera.

## 3. Navegacion

El sistema usa una cabecera superior y menu lateral colapsable.

El menu esta organizado por modulos:

- Tablero de Control.
- Ventas.
- Maestros.
- Operacion.
- Administracion.
- Reportes.

Solo un modulo del menu queda abierto a la vez para evitar consumo excesivo de espacio.

## 4. Seguridad, roles y permisos

Los roles permiten seleccionar permisos en 3 niveles:

- Modulo: Tablero, Ventas, Maestros, Operacion, Administracion o Reportes.
- Formularios: Categorias, Productos, Comprobantes, Usuarios, etc.
- Botones o acciones: solo las operaciones reales de cada formulario. Por ejemplo, Compras permite Registrar Pago, Devoluciones permite Registrar y Usuarios permite Restablecer Password.

Reglas de seleccion:

- Si se marca un modulo, se marcan todos sus formularios y botones.
- Si se marca un formulario, se marcan todos sus botones.
- Si solo se marcan algunos botones, el modulo/formulario queda en estado parcial.

Roles base:

- Administrador: acceso total.
- Vendedor: acceso operativo a categorias, productos, clientes, cotizaciones, comprobantes, notas de pedido y cobros de clientes.

El modulo `Administracion > Errores de aplicacion` permite consultar excepciones no controladas, filtrar por fecha, estado o texto, revisar el detalle tecnico y marcar cada incidencia como revisada con una observacion.

## 5. Empresas

El mantenimiento de empresas permite registrar:

- RUC.
- Razon social.
- Nombre comercial.
- Direccion.
- Telefono.
- Email.
- URL y token Nubefact.
- Series de boleta, factura, nota de credito, nota de pedido y cotizacion.
- Moneda predeterminada.
- Logo de empresa.
- Representante legal: nombre, documento y cargo.
- Firma del representante legal en imagen.

El logo se carga desde un archivo de imagen y se guarda en base de datos.

## 6. Usuarios

El mantenimiento de usuarios permite:

- Registrar nombres, apellidos, correo y usuario.
- Asignar rol desde una lista.
- Asignar una o mas empresas desde una lista seleccionable.
- Activar o desactivar usuarios.
- Restablecer contrasena.

## 7. Categorias

El mantenimiento de categorias permite administrar las categorias de productos por empresa.

Categorias iniciales sugeridas:

- Frutales.
- Ornamentales.
- Materiales.
- Abonos.

El formulario de productos obtiene sus categorias desde este mantenimiento.

## 8. Productos

El mantenimiento de productos permite registrar:

- Categoria.
- Nombre.
- Unidad de medida.
- Stock.
- Precio sin IGV.
- Indicador de afectacion a IGV.
- Indicador y porcentaje de detraccion.
- Estado.

El precio con IGV se calcula automaticamente cuando corresponde.

## 9. Clientes y proveedores

Los clientes se administran por empresa. Cada empresa tiene su propio padron de clientes y puede registrar el mismo DNI o RUC sin afectar a otra empresa.

Las cotizaciones, notas de pedido, comprobantes, cobros y estados de cuenta solo muestran clientes de la empresa activa.

Los proveedores tambien se administran por empresa, porque sus operaciones se relacionan con compras, gastos e inventario de una empresa especifica.

Datos principales:

- Tipo y numero de documento.
- Nombre o razon social.
- Nombre comercial.
- Direccion.
- Telefono.
- Email.
- Estado.

## 10. Compras

El formulario de compras permite registrar:

- Proveedor.
- Documento.
- Fecha.
- Detalle de productos.
- Cantidad.
- Costo unitario.
- Importe por linea.
- Subtotal, IGV y total.

El detalle usa el mismo estilo operativo que comprobantes:

- Una fila inicial.
- Boton de agregar con icono `+`.
- Boton de quitar en filas nuevas.
- Calculo automatico de importes y totales.

Al guardar una compra, el stock del producto aumenta y se registra movimiento de inventario.


Tipos de documento admitidos:

- `FACTURA`.
- `BOLETA`.
- `LIQUIDACION COMPRA`.
- `RECIBO`.
- `NOTA VENTA`.
- `PENDIENTE COMPROBANTE`.
- `SIN DOCUMENTO`.

Reglas del documento:

- `FACTURA`, `BOLETA` y `LIQUIDACION COMPRA` requieren serie y numero.
- `RECIBO`, `NOTA VENTA`, `PENDIENTE COMPROBANTE` y `SIN DOCUMENTO` no requieren serie ni numero.
- `PENDIENTE COMPROBANTE` es tipo de documento, no estado de compra.

Reglas de pago y anulacion:

- Los estados de pago de compra son `PENDIENTE`, `PARCIAL` y `PAGADO`.
- El estado del documento de compra es `ACTIVO` o `ANULADO`.
- Una compra al contado queda pagada y genera pago a proveedor con movimiento de caja.
- Una compra a credito queda pendiente hasta registrar pagos desde el detalle.
- Al anular una compra se revierte el stock. Si existen pagos activos, se conserva la trazabilidad historica y se genera una devolucion pendiente del proveedor.
- Los documentos anulados no bloquean volver a registrar el mismo documento para el mismo proveedor.

## 11. Cotizaciones

Las cotizaciones son documentos no SUNAT.

Permiten:

- Registrar cliente, direccion, fecha y detalle.
- Usar detalle con producto, unidad, cantidad, precio y total.
- Calcular subtotal, IGV y total.
- Generar PDF local.
- Convertir manualmente solo a nota de pedido.

Reglas importantes:

- Nunca registran cobros.
- Nunca generan movimientos de caja.
- Nunca generan efectos tributarios.
- No descuentan stock.
- No se consideran deuda del cliente.
- No generan comprobantes directamente; la boleta o factura nace desde la nota de pedido.
- Al convertir, la cotizacion cambia a estado `CONVERTIDA`.
- Al guardar una cotizacion, se conserva una copia historica de los datos principales del cliente: tipo y numero de documento, nombre, nombre comercial, direccion, telefono y email. Si la cotizacion es antigua y no tiene esos campos, las consultas y PDF usan fallback al maestro de clientes mediante `ClienteId`.
- La conversion a nota de pedido traslada el snapshot historico del cliente desde la cotizacion, para que el documento generado no cambie si luego se modifica el maestro de clientes.

## 12. Notas de pedido

Las notas de pedido son documentos comerciales internos y se gestionan en el modulo `NotasPedido`.

No son comprobantes SUNAT y no deben tratarse como boletas ni facturas.

Permiten:

- Registrar cliente, fecha y detalle de productos.
- Calcular subtotal, IGV referencial y total.
- Generar PDF local.
- Registrar cobros parciales o totales cuando aun no tienen comprobante relacionado.
- Convertirse posteriormente a comprobante, siempre a demanda del usuario.

Reglas de cobro:

- Una nota de pedido activa puede recibir cobros mientras no tenga comprobante relacionado.
- Si la nota de pedido ya fue convertida a comprobante, no se registran nuevos cobros directamente sobre la nota.
- Los cobros existentes de la nota se aplican al comprobante generado al momento de convertirla.
- El saldo pendiente nunca debe mostrarse como negativo.

Reglas de conversion:

- La conversion no es automatica al registrar cobros; el usuario debe usar la accion `Convertir a Comprobante`.
- Si el cliente tiene RUC, el sistema genera factura.
- Si el cliente no tiene RUC, el sistema genera boleta.
- Solo se convierte si esta activa, pagada, sin saldo pendiente y sin comprobante relacionado.
- Al convertir, se mantiene la trazabilidad entre la nota de pedido, sus cobros y el comprobante generado.
- La nota de pedido conserva una copia historica de los datos principales del cliente. Si proviene de una cotizacion, hereda el snapshot de la cotizacion; si se registra directamente, toma los datos actuales del maestro al guardar.
- El comprobante generado hereda el snapshot de cliente de la nota de pedido y muestra los cobros aplicados sin duplicarlos.

Reglas de anulacion:

- Si no tiene cobros activos, al anular se solicita motivo, se marca la nota como `ANULADO` y no se modifica caja.
- Si tiene cobros activos, al anular se solicita motivo, se marca la nota como `ANULADO` y no se anulan los cobros ni sus movimientos de caja.
- Cuando una nota anulada tiene cobros activos, el sistema genera una `Devolucion` pendiente para el cliente por el total cobrado activo.
- La devolucion pendiente no impacta caja hasta que el usuario registre la devolucion real al cliente.
- La lista muestra `Estado Devolucion`: `No aplica`, `Pendiente`, `Parcial` o `Devuelto`.

## 13. Comprobantes

Tipos disponibles actualmente en la entidad `Comprobante`:

- Boleta.
- Factura.
- Nota de credito.

El modulo `Comprobantes` lista y crea boletas y facturas. Las notas de credito reutilizan la entidad `Comprobante`, pero se gestionan en el modulo separado `NotasCredito`. Las notas de pedido no se crean desde Comprobantes; se gestionan definitivamente en el modulo `NotasPedido`.

El formulario permite registrar:

- Tipo.
- Numero: tipo + serie + correlativo.
- Cliente.
- Direccion.
- Forma de pago.
- Productos.
- Unidad.
- Cantidad.
- Precio.
- Total.
- Subtotal, IGV y total.

La regla de edicion se evalua con estado del documento, impresion y aceptacion SUNAT.

Reglas de comprobantes:

- Una venta directa al contado genera cobro de cliente y movimiento de caja.
- Una venta a credito queda pendiente de pago y no genera caja hasta que se registre el cobro.
- No se registran pagos sobre comprobantes anulados, pagados o notas de credito.
- Al crear un comprobante, `DocumentoImpreso` queda en `NO`.
- Al imprimir o generar PDF, el sistema envia el comprobante a Nubefact cuando corresponde, guarda el PDF retornado y marca `DocumentoImpreso` en `SI`.
- La grilla muestra `Doc. Impreso` como `SI` o `NO`.
- La grilla muestra `Doc. Aceptado SUNAT` como `SI` solo cuando `EstadoSunat = Aceptado`; cualquier otro estado se visualiza como `NO`.
- El detalle tecnico de Nubefact no se muestra en la grilla; se revisa desde el modulo `Log Nubefact`.
- Solo se puede editar un comprobante si esta activo, `DocumentoImpreso = NO` y `EstadoSunat` no es `Aceptado`.
- La anulacion directa aplica a boletas y facturas activas hasta dos dias despues de la fecha de emision.
- No se puede anular un comprobante electronico si aun no fue enviado a Nubefact. Primero debe imprimirse/generarse el PDF para conservar el correlativo ante Nubefact.
- Para anular se exige registrar motivo. Si el comprobante tiene cobros activos, esos cobros y sus movimientos de caja permanecen registrados y se genera una solicitud de devolucion pendiente por el monto cobrado.
- Al anular un comprobante, el sistema envia primero la anulacion a Nubefact cuando corresponde. Solo si Nubefact responde correctamente se marca el comprobante como `ANULADO`; los cobros activos directos o aplicados desde nota de pedido no se anulan.
- Si el comprobante esta fuera del plazo de anulacion, debe reversarse mediante nota de credito.
- Al guardar boletas, facturas y notas de credito, el comprobante conserva una copia historica del cliente: tipo y numero de documento, nombre, nombre comercial, direccion, telefono y email. Si un comprobante antiguo no tiene esos campos, listados, reportes, PDFs y Nubefact usan fallback al maestro `Cliente` mediante `ClienteId`.
- Cuando existe snapshot historico, siempre se muestran esos datos y no los valores actuales del maestro de clientes.

La lista de comprobantes permite buscar por:

- Serie.
- Numero completo en formato `Serie-Correlativo`, por ejemplo `F002-6`.
- Cliente.
- Documento del cliente.

El saldo pendiente se muestra como cero cuando los pagos igualan o superan el total. Si el comprobante proviene de una nota de pedido con cobros ya registrados, esos cobros aplicados no se duplican en el total cobrado.

Los comprobantes son documentos distintos a las notas de pedido. La nota de pedido puede originar un comprobante, pero mantiene su propio modulo, estado y trazabilidad.

## 14. Notas de credito

La nota de credito esta implementada como comprobante de tipo `NCR`.

Se emite desde una boleta o factura activa y conserva referencia al comprobante original mediante `ComprobanteReferenciaId`.

Permite:

- Buscar y seleccionar un comprobante origen aceptado por SUNAT.
- Registrar motivo desde el catalogo de motivos de nota de credito.
- Generar numeracion propia con serie de nota de credito segun el tipo de comprobante origen.
- Cargar automaticamente cliente, fecha, total y detalle del comprobante origen.
- Copiar el snapshot historico del cliente desde el comprobante origen para mantener la misma identidad comercial en la nota de credito.
- Registrar cantidad de nota de credito por producto, mayor que cero y menor o igual a la cantidad original.
- Calcular subtotal, IGV y total de forma proporcional al detalle original, respetando productos exonerados.
- Enviar la nota de credito a Nubefact y guardar PDF/XML/hash/respuesta en el log tecnico.

Alcance actual:

- No registra cobros de cliente.
- No registra pagos.
- No genera MovimientoCaja por si misma.
- No permite accion Registrar Pago.
- No aparece en la lista principal de comprobantes; se gestiona desde `Ventas > Notas de Credito`.
- No se emite contra otra nota de credito, nota de pedido o cotizacion.
- Solo se puede emitir contra boletas o facturas activas con `EstadoSunat = Aceptado`.
- Actualmente se bloquea cualquier nueva nota de credito activa para un comprobante que ya tenga una nota de credito activa relacionada.
- El formulario de busqueda de origen permite buscar en una sola caja por serie-numero, cliente o documento, mas rango de fechas.
- Al emitir una nota de credito se calcula el impacto financiero contra el comprobante origen.
- Los cobros existentes del comprobante origen no se anulan.
- Si la nota de credito solo reduce deuda pendiente, no se genera devolucion.
- Si el comprobante queda con exceso cobrado, se genera una `Devolucion` pendiente para el cliente por el exceso.

## 15. Estado de pago

El estado de pago indica la situacion financiera de una nota de pedido o comprobante respecto a sus cobros.

Estados en notas de pedido:

- `PENDIENTE`: no tiene cobros activos registrados. El saldo pendiente es igual al total.
- `PARCIAL`: tiene uno o mas cobros activos, pero el total cobrado es menor que el total del documento.
- `PAGADO`: el total cobrado o aplicado es igual o mayor que el total del documento. El saldo pendiente se muestra como `0.00`.

Estados en comprobantes:

- `PENDIENTE`: no tiene cobros activos registrados.
- `PARCIAL`: tiene uno o mas cobros activos o aplicados, pero el total pagado es menor que el total del comprobante.
- `PAGADO`: el total pagado o aplicado es igual o mayor que el total del comprobante. El saldo pendiente se muestra como `0.00`.

Reglas:

- Internamente el valor parcial se conserva como `PAGO_PARCIAL`, pero en la interfaz se muestra como `PARCIAL`.
- Los cobros anulados no cuentan para el estado de pago.
- Los cobros aplicados desde una nota de pedido a un comprobante cuentan una sola vez.
- Una nota de pedido pagada puede convertirse a boleta o factura.
- Un comprobante pagado no permite registrar nuevos cobros desde la accion Registrar pago.
- El estado de pago es independiente del estado SUNAT y del estado del documento.
- El estado del documento se muestra en mayuscula: `ACTIVO` o `ANULADO`.

## 16. Cobros de clientes

El formulario Cobros de Clientes permite revisar los pagos registrados para notas de pedido y comprobantes.

Muestra:

- Fecha.
- Cliente.
- Referencia.
- Medio de pago.
- Monto.
- Estado con distintivo visual verde para `ACTIVO` y rojo para `ANULADO`.
- Accion de anulacion con icono estandar y solicitud de motivo.

La referencia puede mostrarse como:

- `NP002-2`, cuando el cobro pertenece directamente a una nota de pedido.
- `F002-6`, cuando el cobro pertenece directamente a un comprobante.
- `NP002-2 -> F002-6`, cuando un cobro registrado en una nota de pedido fue aplicado al comprobante generado desde esa nota.

La busqueda permite encontrar cobros por:

- Cliente.
- Medio de pago.
- Observacion.
- Numero de nota de pedido, por ejemplo `NP002-2`.
- Numero de comprobante, por ejemplo `F002-6`.

El rango de fechas carga por defecto desde el primer dia del mes actual hasta la fecha actual. El usuario puede cambiarlo para ampliar o reducir la consulta.

Al anular un cobro:

- Se exige motivo.
- El cobro queda `ANULADO`.
- El movimiento de caja asociado queda `ANULADO`.
- Se recalcula el estado de pago de la nota de pedido o comprobante relacionado.
- Si el cobro fue aplicado desde una nota de pedido a un comprobante, deja de contar para el comprobante.

## 17. Devoluciones de clientes

El modulo `Ventas > Devoluciones de Clientes` administra devoluciones pendientes generadas por:

- Anulacion de nota de pedido con cobros historicos.
- Emision de nota de credito con exceso cobrado.

Muestra:

- Fecha.
- Cliente.
- Origen.
- Documento.
- Monto original.
- Monto devuelto.
- Monto pendiente.
- Estado.
- Acciones para ver y registrar devolucion.

Estados:

- `PENDIENTE`: no se ha devuelto dinero.
- `PARCIAL`: se registro una devolucion parcial.
- `DEVUELTO`: se devolvio todo el monto pendiente.
- `ANULADO`: devolucion anulada administrativamente.

Reglas:

- No se permite registrar devolucion con monto menor o igual a cero.
- No se permite devolver mas que el monto pendiente.
- No se permite registrar devolucion si el estado es `DEVUELTO` o `ANULADO`.
- Al confirmar una devolucion se genera `MovimientoCaja` de tipo `EGRESO`, origen `DEVOLUCION_CLIENTE`.
- Solo el registro real de devolucion impacta caja; la generacion de la devolucion pendiente no mueve caja.
- La campana del encabezado muestra un contador con las devoluciones `PENDIENTE` o `PARCIAL` de la empresa activa.
- Al abrir la campana se muestran las cinco alertas mas recientes con tercero, origen, documento y monto pendiente.
- Cada alerta permite abrir directamente el detalle de la devolucion.

## 17.1 Gastos e Ingresos Manuales

Gastos e ingresos manuales son movimientos simples de caja.

Gasto:

- Representa salida operativa de dinero.
- No pide proveedor.
- No registra productos.
- No modifica stock.
- Al guardar crea `MovimientoCaja` de tipo `EGRESO` con origen `GASTO`.
- Al editar actualiza el movimiento de caja relacionado.
- Al anular exige motivo y marca como `ANULADO` el gasto y su movimiento de caja.

Categorias de gasto iniciales:

- `MOVILIDAD`
- `COMBUSTIBLE`
- `LUZ`
- `ALQUILER`
- `INTERNET`
- `MANTENIMIENTO`
- `HERRAMIENTAS MENORES`
- `VIATICOS`
- `COMISION`
- `CAMPO PALTA`
- `PAGO PERSONAL`
- `PAPELERIA`

Ingreso manual:

- Representa entrada de dinero no relacionada a ventas, notas de pedido ni comprobantes.
- No pide cliente.
- No genera deuda ni efectos tributarios.
- Al guardar crea `MovimientoCaja` de tipo `INGRESO` con origen `INGRESO_MANUAL`.
- Al editar actualiza el movimiento de caja relacionado.
- Al anular exige motivo y marca como `ANULADO` el ingreso y su movimiento de caja.

Categorias de ingreso iniciales:

- `PRESTAMO RECIBIDO`
- `APORTE DE SOCIOS`
- `ALQUILER`
- `EMBALAJE`
- `FLETE`

## 18. Caja y MovimientoCaja

El modulo Caja permite revisar movimientos reales de dinero registrados en `MovimientoCaja`.

Muestra:

- Fecha.
- Tipo.
- Cliente / Proveedor.
- Origen funcional.
- Documento asociado.
- Medio de pago.
- Monto.
- Estado.

El resumen superior muestra:

- Ingresos: suma de movimientos `INGRESO` activos.
- Egresos: suma de movimientos `EGRESO` activos.
- Saldo: ingresos menos egresos.
- Movimientos: cantidad de movimientos activos encontrados segun filtros.

Tipos de movimiento:

- `INGRESO`: entrada de dinero.
- `EGRESO`: salida de dinero.

Origenes principales:

- `COBRO_CLIENTE`: se muestra como `Cobro Cliente`.
- `PAGO_PROVEEDOR`: se muestra como `Pago Proveedor`.
- `GASTO`: se muestra como `Gasto`.
- `INGRESO_MANUAL`: se muestra como `Ingreso Manual`.
- `DEVOLUCION_CLIENTE`: se muestra como `Devolucion Cliente`.
- `OTRO`: se muestra como `Otro`.

Reglas:

- Un cobro activo de cliente genera un movimiento de caja de tipo `INGRESO`.
- Al anular un cobro, el movimiento de caja relacionado queda anulado.
- Las cotizaciones nunca generan movimientos de caja.
- Las notas de pedido no generan caja directamente; la caja se origina por cobros registrados.
- Los comprobantes no se usan para calcular caja; la caja se calcula exclusivamente desde `MovimientoCaja`.
- Las notas de credito no generan movimientos de caja en la implementacion actual.
- Los movimientos anulados pueden mostrarse en la grilla, pero no suman ingresos, egresos, saldo ni cantidad de movimientos activos.
- Los filtros de caja permiten consultar por fecha, medio de pago, tipo de movimiento y busqueda por cliente, proveedor, documento, medio o descripcion.
- Para cobros aplicados desde nota de pedido a comprobante, el documento se muestra como `NP002-000002 -> F002-000006`.
- Para devoluciones de clientes, el egreso se muestra con el cliente y documento origen de la devolucion.

## 18.1 TesorerÃ­a

El mÃ³dulo TesorerÃ­a centraliza la informaciÃ³n de dinero real de la empresa.

Opciones del menÃº:

- Caja.
- Caja y Bancos.
- Cuentas financieras.
- Cobros clientes.
- Transferencias.
- Estado de cuenta clientes.
- Estado de cuenta proveedores.

### Cuentas financieras

Las cuentas financieras representan dÃ³nde estÃ¡ fÃ­sicamente o bancariamente el dinero:

- Caja: efectivo interno.
- Banco: cuenta bancaria.
- Billetera: Yape, Plin u otro medio similar.

Datos principales:

- Nombre.
- Tipo.
- Banco.
- NÃºmero de cuenta.
- Moneda.
- Saldo inicial.
- Fecha de saldo inicial.
- Estado activo/anulado.

El sistema crea o usa una cuenta por defecto llamada `Caja principal` cuando una operaciÃ³n no indica cuenta financiera.

### Caja y Bancos

La pantalla responde la pregunta: Â¿cuÃ¡nto dinero real tiene la empresa?

Muestra:

- Dinero disponible de la empresa.
- Total en efectivo.
- Total en bancos.
- Total en billeteras.
- Ingresos del periodo.
- Egresos del periodo.
- Saldo por cuenta financiera.

El saldo se calcula como:

`Saldo inicial + ingresos activos - egresos activos`

### Operaciones con cuenta financiera

Registran cuenta financiera:

- Cobros de clientes.
- Comprobantes al contado.
- Gastos.
- Ingresos manuales.
- Pagos a proveedores.
- Devoluciones.

Esto permite saber a quÃ© caja, banco o billetera ingresÃ³ o de dÃ³nde saliÃ³ el dinero.

### Transferencias

Permiten mover dinero entre cuentas financieras.

Formulario:

- Fecha.
- Cuenta origen.
- Cuenta destino.
- Monto.
- ObservaciÃ³n.

Reglas:

- La cuenta origen debe ser distinta de la cuenta destino.
- Al registrar una transferencia se genera un egreso en la cuenta origen y un ingreso en la cuenta destino.
- El origen del movimiento de caja es `TRANSFERENCIA`.
- Las transferencias no afectan ventas, gastos, ingresos ni estados de cuenta.
- Al anular una transferencia se anulan sus dos movimientos de caja relacionados.

## 19. Facturacion electronica y SUNAT

Facturas:

- Se envian en linea a Nubefact.
- Si Nubefact acepta el documento, el estado SUNAT queda Aceptado.

Boletas:

- Se envian a Nubefact desde la impresion o generacion del comprobante.
- Si Nubefact retorna aceptacion, el estado SUNAT queda Aceptado.

Notas de credito:

- Se envian a Nubefact como tipo `NCR`.
- Incluyen tipo, serie y numero del comprobante original.

Reglas implementadas:

- El envio a Nubefact solicita envio automatico a SUNAT.
- Si Nubefact devuelve PDF pero la respuesta inicial no marca aceptacion, el sistema consulta el estado y actualiza `EstadoSunat`.
- La impresion de un comprobante con PDF existente vuelve a consultar Nubefact si el estado aun no esta aceptado ni anulado.
- La aceptacion SUNAT se interpreta desde campos booleanos de Nubefact, codigo de respuesta `0`, estado aceptado o descripcion SUNAT aceptada.

Notas de pedido y cotizaciones:

- No aplican a SUNAT.
- Generan PDF local.

## 20. Detraccion

Para facturas con productos exonerados de IGV y total mayor a S/ 700:

- Tipo de operacion: 30.
- Detraccion: si.
- Tipo de detraccion: 33.
- Medio de pago: 1.
- Porcentaje de detraccion: sale del producto.
- Total detraccion: total aplicado al porcentaje de detraccion.

## 21. Log Nubefact

El formulario Log Nubefact permite revisar operaciones enviadas o consultadas.

Muestra:

- Fecha.
- Numero de comprobante: serie + correlativo.
- Tipo de operacion.
- Estado.
- Detalle de solicitud y respuesta.

No muestra el campo hash en la lista principal.

## 22. Reportes y filtros

El Reporte General presenta un comparativo anual con meses en filas y aÃ±os en columnas. Permite seleccionar hasta diez aÃ±os y cambiar el indicador entre:

- Resultado neto.
- Ventas netas.
- Ingresos manuales.
- Gastos.
- Compras.

Tambien muestra indicadores acumulados y un estado anual consolidado con variacion respecto al aÃ±o anterior. El resultado se calcula como `Ventas + Ingresos - Gastos - Compras`. Las notas de credito activas reducen las ventas; los registros anulados no participan.


Reportes operativos disponibles:

- `Reporte de notas de pedido`: lista notas de pedido con filtros por cliente/documento, numero de nota, fecha, estado de pago y estado de documento. Incluye cartillas de total de notas, total vendido, total cobrado, saldo pendiente, notas pendientes, parciales, pagadas y anuladas. Permite exportar a Excel/CSV.
- `Reporte de comprobantes`: lista boletas y facturas emitidas con filtros por tipo, serie, numero, fecha, cliente, estado SUNAT, estado del comprobante, medio de pago y vendedor. Incluye cartillas de total de comprobantes, importe, IGV, gravado, exonerado, cancelado y por cobrar. La moneda se muestra como `Soles` y no se muestra la columna canal. Permite exportar a Excel/CSV.

Permisos de reportes:

- `ReporteNotasPedido` controla el acceso al reporte de notas de pedido.
- `ReporteComprobantes` controla el acceso al reporte de comprobantes.
- Estos permisos son independientes de los permisos operativos de `NotasPedido` y `Comprobantes` en ventas.

Los formularios de lista con informacion fechada usan rango de fechas:

- Fecha desde.
- Fecha hasta.
- Texto de busqueda.

Aplica a:

- Comprobantes.
- Cotizaciones.
- Notas de Pedido.
- Cobros de Clientes.
- Caja.
- Devoluciones de Clientes.
- Compras.
- Gastos.
- Ingresos.

## 23. Estados visuales

Los estados se muestran con distintivos de color:

- `ACTIVO`: verde.
- `ANULADO`: rojo.

Esto aplica a listas como comprobantes, cotizaciones, empresas, categorias y otros mantenimientos.

## 24. Datos iniciales de salida

La instalaciÃ³n inicial deja preparadas dos empresas activas:

- `20615082997`: VIVERO LOS FRUTALES LIMA SAC.
- `20615619273`: VIVERO LOS FRUTALES HUARAL SAC.

El usuario inicial para ingreso y configuraciÃ³n es:

```text
Usuario: admin
Password: Admin1234
```

Este usuario tiene rol administrador, permisos completos y acceso a las dos empresas iniciales. Al iniciar sesiÃ³n debe seleccionar una empresa. DespuÃ©s del primer ingreso se debe cambiar la contraseÃ±a desde el mÃ³dulo de usuarios.

Los productos se cargan por empresa desde el catalogo Nubefact. El sistema mantiene stock, precios, unidad, afectacion IGV y detraccion por producto y empresa. Los clientes tambien se cargan por empresa, por lo que cada empresa mantiene su propio padron comercial.

## 25. Sitio pÃºblico

La soluciÃ³n incluye un sitio pÃºblico separado del ERP.

El sitio pÃºblico presenta contenido institucional y comercial:

- Inicio.
- Nosotros.
- Productos.
- Servicios.
- Contacto.

El catÃ¡logo pÃºblico se alimenta desde empresas y productos activos. No permite operaciones internas de venta, caja, compras ni configuraciÃ³n. Es una aplicaciÃ³n web independiente para publicaciÃ³n externa.

