# Documentacion tecnica - Vivero Los Frutales

## 1. Arquitectura

La solucion usa ASP.NET Core 8 MVC con una arquitectura por capas:

```text
src/
  ViveroLosFrutales.Web
  ViveroLosFrutales.Application
  ViveroLosFrutales.Domain
  ViveroLosFrutales.Infrastructure
tests/
  ViveroLosFrutales.Tests
```

Responsabilidades:

- `Web`: controladores MVC, vistas Razor, archivos estaticos, sesion y flujo web.
- `Application`: DTOs, servicios de aplicacion e interfaces.
- `Domain`: entidades, enums y reglas puras.
- `Infrastructure`: EF Core, SQL Server, Identity, repositorios, Nubefact y QuestPDF.

## 2. Tecnologias

- .NET 8.
- ASP.NET Core MVC.
- ASP.NET Identity.
- Entity Framework Core.
- SQL Server.
- QuestPDF.
- Nubefact API.
- Serilog.
- Razor Views con CSS/JS propio.

## 3. Multiempresa

La empresa activa se selecciona en el login.

Flujo:

1. `AccountController.Login` carga empresas activas.
2. El usuario envia usuario, contrasena y `EmpresaId`.
3. Identity valida credenciales.
4. `UsuarioService.ObtenerEmpresasAsync` valida que el usuario tenga acceso a la empresa seleccionada.
5. La empresa activa se guarda en sesion:
   - `EmpresaId`.
   - `EmpresaNombre`.

Las operaciones transaccionales obtienen `EmpresaId` desde `IEmpresaContext`, no desde el formulario.

## 4. Seguridad y permisos

Entidades principales:

- `Rol`.
- `Permiso`.
- `RolPermiso`.
- `UsuarioEmpresa`.
- `ApplicationUser`.

Los permisos se guardan por formulario/modulo tecnico en la tabla `Permiso`:

- `Modulo`: nombre del formulario o recurso, por ejemplo `Comprobantes`, `Productos`, `Usuarios`.
- `Accion`: `Ver`, `Crear`, `Editar`, `Anular`, `Imprimir`, `Configurar`.

La vista de roles agrupa esos permisos en 3 niveles funcionales:

- Grupo funcional: Ventas, Maestros, Operacion, Administracion, Reportes.
- Formulario.
- Accion.

`RolRepository.ObtenerPermisosAsync` ejecuta `EnsurePermisosAsync` para completar permisos faltantes al cargar el formulario de roles.

`DatabaseSeeder.EnsurePermissionsAsync` tambien completa permisos al arrancar la aplicacion cuando `Seed:RunOnStartup` esta activo.

## 5. Modulos y formularios de permisos

Agrupacion funcional usada por la vista:

```text
Ventas
  Cotizaciones
  Comprobantes

Maestros
  Categorias
  Productos
  Clientes
  Proveedores

Operacion
  Compras
  Gastos
  Ingresos

Administracion
  Empresas
  Usuarios
  Roles
  Configuracion
  NubefactLogs

Reportes
  Reportes
```

## 6. Entidades principales

- `Empresa`.
- `Categoria`.
- `Producto`.
- `Cliente`.
- `Proveedor`.
- `Compra`.
- `CompraDetalle`.
- `MovimientoInventario`.
- `Gasto`.
- `Ingreso`.
- `Comprobante`.
- `ComprobanteDetalle`.
- `Comprobante.ComprobanteReferenciaId`.
- `Comprobante.MotivoNotaCredito`.
- `CobroCliente`.
- `ComprobanteCobroAplicado`.
- `NotaPedido`.
- `NotaPedidoDetalle`.
- `NubefactOperacion`.
- `ConfiguracionEmpresa`.
- `Moneda`.
- `Rol`.
- `Permiso`.
- `RolPermiso`.
- `UsuarioEmpresa`.

## 7. Empresa y logo

La entidad `Empresa` incluye soporte para logo en base de datos:

```text
LogoContenido varbinary(max) NULL
LogoContentType nvarchar(120)
LogoNombre nvarchar(260)
```

El formulario recibe `IFormFile logoArchivo`, valida que sea imagen y que no supere 2 MB. El contenido se guarda en `LogoContenido`.

## 8. Categorias

La entidad `Categoria` es multiempresa.

Campos principales:

- `CategoriaId`.
- `EmpresaId`.
- `Nombre`.
- `Descripcion`.
- `Estado`.
- `FechaRegistro`.
- `UsuarioRegistro`.

`CategoriaService.ListarActivasAsync` alimenta el combo de categoria en productos.

## 9. Productos

`ProductoService` normaliza unidad de medida:

- `UNIDAD`, `UND`, `UNIDADES` -> `NIU`.
- `KILO`, `KG`, `KILOGRAMO` -> `KGM`.
- `METRO` -> `MTR`.
- `LITRO` -> `LTR`.

El precio con IGV se recalcula en dominio con `Producto.RecalcularPrecioConIgv`.

## 10. Compras e inventario

`CompraRepository.GuardarAsync` ejecuta una transaccion.

Al guardar:

1. Inserta compra y detalle.
2. Busca cada producto.
3. Actualiza stock.
4. Registra `MovimientoInventario` con tipo `COMPRA`.
5. Confirma transaccion.

La vista `Compras/Create.cshtml` usa `viveroCompraForm` en `site.js` para agregar filas y calcular totales en cliente.

## 11. Cotizaciones, notas de pedido y comprobantes

La implementacion actual usa entidades separadas para el flujo comercial:

- `Cotizacion` y `CotizacionDetalle`: modulo `Cotizaciones`.
- `NotaPedido` y `NotaPedidoDetalle`: modulo `NotasPedido`.
- `Comprobante` y `ComprobanteDetalle`: modulo `Comprobantes`, para BOL, FAC y NCR.

El enum `TipoComprobante` conserva los valores:

- `COT`.
- `BOL`.
- `FAC`.
- `NPE` solo para compatibilidad con PDF/formas antiguas de nota de pedido.
- `NCR`.

En la practica operativa actual:

- Las cotizaciones se crean desde `CotizacionesController` y se guardan en `Cotizacion`.
- Las notas de pedido se crean desde `NotasPedidoController` y se guardan en `NotaPedido`.
- Los comprobantes se crean desde `ComprobantesController` y se guardan en `Comprobante`.
- `ComprobantesController.Create` rechaza `COT`, `NPE` y `NCR`; el formulario directo solo crea BOL/FAC.
- La vista de comprobantes ya no ofrece `NPE`; las notas de pedido se gestionan desde `NotasPedido`.
- Las notas de credito se crean desde `NotasCreditoController`, reutilizando `ComprobanteService`.

Cotizaciones:

- Usan `CotizacionService` y `CotizacionRepository`.
- Pueden convertirse manualmente solo a nota de pedido mediante `CotizacionService.ConvertirANotaPedidoAsync(id)`.
- La conversion directa de cotizacion a comprobante esta bloqueada; boletas y facturas se generan desde notas de pedido o desde el modulo de comprobantes.
- Al convertirse actualizan `EstadoCotizacion` a `CONVERTIDA`.
- No registran cobros, movimientos de caja, stock ni efectos SUNAT.
- La UI muestra estados funcionales `ACTIVA`, `CONVERTIDA` y `ANULADA`.
- La lista muestra acciones en el orden ver, imprimir, editar, anular y convertir a nota de pedido. No hay acciones para convertir a boleta, factura o comprobante.

Notas de pedido:

- Usan `NotaPedidoService` y `NotaPedidoRepository`.
- Pueden registrar cobros mientras no tengan comprobante relacionado.
- Pueden convertirse a comprobante solo cuando el usuario ejecuta `Convertir a Comprobante`.
- `NotaPedidoService.ConvertirAsync(id)` decide FAC si el cliente tiene RUC, caso contrario BOL.
- La conversion exige nota activa, pagada, saldo cero y sin comprobante relacionado.
- Al convertirse, sus cobros activos se registran en `ComprobanteCobroAplicado`.
- La conversion no es automatica al completar el pago.
- Los listados y detalles muestran el estado de pago parcial como `PARCIAL`, aunque el enum tecnico conserva `PAGO_PARCIAL`.
- El estado del documento se muestra como `ACTIVO` o `ANULADO`.
- `NotaPedidoService.AnularAsync` recibe motivo. Si existen cobros activos no llama a `CobroClienteService.AnularAsync`; marca la nota como anulada y delega a `DevolucionService.CrearDevolucionPorAnulacionNotaPedidoAsync`.
- La lista de notas de pedido muestra `EstadoDevolucion` desde `Devolucion` cuando existe una devolucion activa relacionada.

Notas de credito:

- Se implementan como `Comprobante.TipoComprobante = NCR`.
- Se relacionan con la boleta/factura original por `ComprobanteReferenciaId`.
- El motivo se guarda con `MotivoNotaCreditoId` y `MotivoNotaCredito`.
- `MotivoNotaCredito` es catalogo propio y se consulta desde `IMotivoNotaCreditoRepository`.
- `ComprobanteService.PrepararNotaCreditoAsync` carga serie, correlativo, comprobante origen, cliente, motivo inicial y detalle.
- `ComprobanteService.EmitirNotaCreditoAsync` crea la NCR, valida cantidades, calcula importes proporcionalmente al detalle original y la envia a Nubefact.
- La cantidad NC debe ser mayor que cero y no puede superar la cantidad original de la linea.
- La implementacion actual permite cantidades parciales por linea, pero `TieneNotaCreditoActivaAsync` bloquea emitir otra NCR activa para el mismo comprobante origen.
- No crea `CobroCliente` ni `MovimientoCaja`.
- `CobroClienteService` bloquea cobros sobre NCR.
- Despues de emitir la NCR, `ComprobanteService.EmitirNotaCreditoAsync` recalcula el saldo valido del comprobante origen considerando notas de credito activas.
- Si `TotalCobradoActivo > Comprobante.Total - NotaCredito.Total`, genera una devolucion pendiente con `DevolucionService.CrearDevolucionPorNotaCreditoAsync`.
- `NotasCreditoController` y las vistas de `Views/NotasCredito` contienen el modulo separado de lista y creacion.
- La busqueda del comprobante origen en la vista usa una sola caja de texto para serie-numero, cliente o documento, mas rango de fechas. La tabla de resultados no muestra la columna tipo.

Estados SUNAT:

- `NoAplica`.
- `Pendiente`.
- `Aceptado`.
- `Observado`.
- `Rechazado`.
- `Anulado`.

Reglas actuales:

- FAC: envio en linea a Nubefact.
- BOL: se envia a Nubefact desde impresion/generacion igual que FAC.
- NCR: envio a Nubefact con datos del comprobante original.
- NPE: `NoAplica`, solo en compatibilidad/PDF de nota de pedido.
- `DocumentoImpreso` se inicializa en `false` y se marca en `true` cuando se obtiene PDF local o de Nubefact.
- `DocumentoAceptadoSunat` es campo calculado en DTO: `EstadoSunat == Aceptado`.
- `ComprobanteService.ImprimirComprobanteAsync` reutiliza `PdfUrl` existente. Si el comprobante electronico aun no esta aceptado ni anulado, consulta Nubefact y persiste el estado antes de devolver el PDF.
- `PuedeEditar` requiere `Estado = Activo`, `DocumentoImpreso = false` y `EstadoSunat != Aceptado`.
- `PuedeAnular` requiere `Estado = Activo`, tipo distinto de NCR y fecha actual menor o igual a `FechaEmision.Date.AddDays(2)`.
- `AnularComprobanteAsync` bloquea comprobantes electronicos con `DocumentoImpreso = false`; el usuario debe imprimir/generar el PDF para enviar primero el documento a Nubefact y no perder el correlativo.
- `AnularComprobanteAsync` exige motivo, envia anulacion a Nubefact para BOL/FAC/NCR y solo continua con la anulacion local si Nubefact responde correctamente. Si Nubefact falla, guarda la respuesta tecnica y detiene el flujo sin marcar el comprobante como anulado.
- Cuando Nubefact confirma la anulacion, guarda `MotivoAnulacion` y marca el comprobante como `Anulado`. No anula cobros activos directos ni aplicados; si existe monto cobrado activo, crea una devolucion pendiente para sustentar la devolucion real posterior.

La lista muestra el numero como:

```text
Tipo-Serie-Correlativo
```

`ComprobanteRepository.BuscarAsync` permite filtrar por:

- Serie.
- Numero completo `Serie-Correlativo`, por ejemplo `F002-6`.
- Cliente.
- Documento del cliente.

Para listados y DTOs de visualizacion, el total pagado se calcula desde:

- `CobroCliente` activo asociado directamente al comprobante.
- `ComprobanteCobroAplicado` asociado al comprobante.

Si un mismo `CobroCliente` esta asociado directamente al comprobante y tambien existe en `ComprobanteCobroAplicado`, se cuenta una sola vez. El saldo pendiente se limita a cero para evitar saldos negativos por pagos completos o cobros previamente aplicados.

Estados de pago usados en comprobantes y notas de pedido:

- `PENDIENTE`.
- `PAGO_PARCIAL` internamente.
- `PAGADO`.

La UI muestra `PAGO_PARCIAL` como `PARCIAL`.

## 12. Cobros de clientes

Entidades:

- `CobroCliente`: pago registrado para una nota de pedido o comprobante.
- `ComprobanteCobroAplicado`: relaciona un cobro existente con el comprobante generado desde una nota de pedido.

Relaciones principales:

- `CobroCliente.NotaPedidoId` para cobros directos a nota de pedido.
- `CobroCliente.ComprobanteId` para cobros directos a comprobante.
- `ComprobanteCobroAplicado.CobroClienteId` y `ComprobanteCobroAplicado.ComprobanteId` para cobros aplicados.

`CobroClienteRepository.BuscarAsync` aplica filtros de fecha cuando `SearchRequest.FechaDesde` o `SearchRequest.FechaHasta` tienen valor. La vista `CobrosClientes/Index.cshtml` carga por defecto desde el primer dia del mes hasta la fecha actual, siguiendo el patron de otros listados.

La busqueda textual contempla:

- Cliente.
- Medio de pago.
- Observacion.
- Nota directa `NotaPedido.Serie-Correlativo`.
- Comprobante directo `Comprobante.Serie-Correlativo`.
- Comprobante asociado por aplicaciones.
- Nota de pedido relacionada al comprobante directo o aplicado.

La proyeccion `CobroClienteListDto.Referencia` muestra `Nota -> Comprobante` cuando el cobro de una nota fue aplicado a un comprobante, por ejemplo:

```text
NP002-2 -> F002-6
```

Anulacion:

- `CobrosClientesController.Anular` recibe el motivo desde la vista.
- `CobroClienteService.AnularAsync` exige motivo, marca el cobro como `ANULADO`, guarda fecha/usuario/motivo, anula el `MovimientoCaja` relacionado y recalcula la nota de pedido o comprobante afectado.
- En comprobantes originados desde nota de pedido, los calculos de pagado ignoran aplicaciones cuyo `CobroCliente` esta anulado.

## 13. Devoluciones unificadas

La entidad `Devolucion` representa dinero pendiente de devolver a clientes o por recuperar de proveedores sin afectar caja historica hasta el registro real.

Campos principales:

- `EmpresaId`.
- `TipoTercero`: `CLIENTE` o `PROVEEDOR`.
- `ClienteId` nullable.
- `ProveedorId` nullable.
- `NotaPedidoId` nullable.
- `ComprobanteId` nullable.
- `NotaCreditoId` nullable.
- `CompraId` nullable.
- `Origen`: `ANULACION_NOTA_PEDIDO`, `NOTA_CREDITO`, `ANULACION_COMPROBANTE` o `ANULACION_COMPRA`.
- `MontoOriginal`.
- `MontoDevuelto`.
- `MontoPendiente`.
- `EstadoDevolucion`: `PENDIENTE`, `PARCIAL`, `DEVUELTO`, `ANULADO`.
- `MotivoGeneracion`.
- Auditoria de registro y modificacion.

Componentes:

- Entidad: `Domain.Entities.Devolucion`.
- Enums: `TipoTerceroDevolucion`, `OrigenDevolucion`, `EstadoDevolucion`.
- DTOs: `DevolucionListDto`, `DevolucionDetalleDto`, `RegistrarDevolucionDto`.
- Repositorio: `IDevolucionRepository` y `DevolucionRepository`.
- Servicio: `DevolucionService`.
- Controlador: `DevolucionesController`.
- Vistas: `Views/Devoluciones`.

Reglas tecnicas:

- Las devoluciones pendientes no crean `MovimientoCaja`.
- `RegistrarDevolucionAsync` valida monto, saldo y estado.
- Al registrar una devolucion de cliente crea un egreso; una devolucion de proveedor crea un ingreso.
- `MovimientoCaja.OrigenId` apunta a `DevolucionId`.
- El repositorio de caja enriquece los movimientos desde `Devolucion` para mostrar tercero y documento.
- Se evita duplicidad por nota de pedido, nota de credito, comprobante o compra.

## 14. Caja y MovimientoCaja

La consulta de caja esta en `MovimientoCajaRepository.BuscarAsync`.

Reglas tecnicas:

- Filtra siempre por `empresaId` recibido desde `IEmpresaContext` por el servicio; la vista no envia EmpresaId.
- Usa `AsNoTracking` para movimientos y entidades relacionadas.
- Parte de `MovimientoCaja` y no de comprobantes, cotizaciones ni notas de pedido.
- Enriquece el listado con consultas agrupadas por origen para evitar N+1.
- Devuelve `CajaIndexDto` con `Movimientos` y `CajaResumenDto`.
- El DTO de grilla es `MovimientoCajaListDto`.

`MovimientoCajaListDto` contiene:

- `MovimientoCajaId`.
- `Fecha`.
- `TipoMovimiento`.
- `Origen`.
- `OrigenDescripcion`.
- `ClienteProveedor`.
- `Documento`.
- `MedioPago`.
- `Monto`.
- `Estado`.

La grilla muestra:

- Fecha.
- Tipo.
- Cliente / Proveedor.
- Origen.
- Documento.
- Medio.
- Monto.
- Estado.

Origenes amigables:

- `COBRO_CLIENTE`: `Cobro Cliente`.
- `PAGO_PROVEEDOR`: `Pago Proveedor`.
- `GASTO`: `Gasto`.
- `INGRESO_MANUAL`: `Ingreso Manual`.
- `DEVOLUCION_CLIENTE`: `Devolucion Cliente`.
- `OTRO`: `Otro`.

El documento relacionado se arma asi:

- Cobro directo a nota: `SerieNota-Correlativo`.
- Cobro directo a comprobante: `SerieComprobante-Correlativo`.
- Cobro de nota aplicado a comprobante: `Nota -> Comprobante`.
- Gasto: `GASTO-000001`.
- Ingreso manual: `INGRESO-000001`.
- Pago proveedor: documento de compra o `COMPRA-000001`.

El resumen calcula:

- Ingresos: suma de movimientos activos con `TipoMovimiento = INGRESO`.
- Egresos: suma de movimientos activos con `TipoMovimiento = EGRESO`.
- Saldo: ingresos menos egresos.
- Movimientos: cantidad de movimientos activos filtrados.

La busqueda textual se aplica sobre cliente/proveedor, documento, medio de pago y descripcion. Los movimientos anulados pueden aparecer en la lista, pero no suman en el resumen.

## 14.1 Gastos e Ingresos Manuales

- Entidades: `Gasto`, `Ingreso`, `CategoriaGasto`, `CategoriaIngreso`.
- `GastoService.GuardarAsync` crea o actualiza `Gasto` y su `MovimientoCaja` en una transaccion.
- `IngresoService.GuardarAsync` crea o actualiza `Ingreso` y su `MovimientoCaja` en una transaccion.
- `GastoService.AnularAsync` e `IngresoService.AnularAsync` exigen motivo, guardan fecha de anulacion y anulan el movimiento de caja relacionado.
- `Gasto.MovimientoCajaId` e `Ingreso.MovimientoCajaId` guardan la relacion principal; `MovimientoCaja.OrigenId` conserva la referencia funcional al registro origen.
- `scripts/sql/001-create-database.sql` crea las tablas y relaciones finales; `002-cargar-categorias-financieras.sql` carga las categorias iniciales por empresa.

## 15. Nubefact

La integracion esta en `ViveroLosFrutales.Infrastructure.Nubefact`.

Responsabilidades:

- Generar payload.
- Normalizar unidad de medida para Nubefact.
- Enviar comprobantes.
- Consultar estado.
- Anular comprobantes.
- Emitir notas de credito.
- Registrar operacion en `NubefactOperacion`.

`NubefactOperacion` guarda:

- Solicitud JSON.
- Respuesta completa.
- Estado.
- PDF/XML.
- Hash.
- Fecha y usuario.

La vista `NubefactLogs/Index.cshtml` muestra numero de comprobante, operacion, estado y detalle; no muestra hash como columna principal.

Para `NCR`, el payload usa:

- `tipo_de_comprobante = 3`.
- `documento_que_se_modifica_tipo`.
- `documento_que_se_modifica_serie`.
- `documento_que_se_modifica_numero`.
- `tipo_de_nota_de_credito = 1`.
- `observaciones` con el motivo de la nota de credito.

El parseo de respuesta marca `EstadoSunat = Aceptado` cuando Nubefact devuelve aceptacion explicita, codigo SUNAT `0`, estado textual aceptado o descripcion SUNAT aceptada. Esta regla evita que comprobantes con PDF emitido y aceptado queden visualmente como no aceptados.

## 16. PDF local

La generacion PDF esta en `PdfService` con QuestPDF.

Aplica a:

- Cotizaciones.
- Notas de pedido.
- Compatibilidad de comprobantes no electronicos.

Ajustes relevantes:

- Nota de pedido usa recuadro compacto para RUC/tipo/numero.
- Unidad `NIU` se muestra como `UNIDAD` en PDF.
- La columna de producto tiene mayor espacio para evitar saltos de linea.

Los PDF locales se guardan en la ruta configurada por `PdfOptions`.

## 17. JavaScript de UI

Archivo principal:

```text
src/ViveroLosFrutales.Web/wwwroot/js/site.js
```

Funciones relevantes:

- `viveroComprobanteForm`: detalle, precios, totales y filas de comprobantes/cotizaciones.
- `viveroCompraForm`: detalle de compras, filas dinamicas e importes.
- `viveroStartSunatSync`: sincronizacion en segundo plano de pendientes SUNAT.
- `viveroShell`: menu colapsable y acordeon de modulos.
- `viveroPermissionTree`: seleccion de permisos en 3 niveles y estados parciales.
- Funciones inline de comprobantes y cobros solicitan motivo de anulacion antes de enviar el formulario.

## 18. CSS de UI

Archivo principal:

```text
src/ViveroLosFrutales.Web/wwwroot/css/site.css
```

Bloques relevantes:

- Layout principal: cabecera, menu lateral y contenido.
- Login profesional.
- Formularios densos.
- Grillas `data-grid`.
- Detalle de comprobantes/compras `detalle-grid`.
- Totales `form-totals`.
- Estados `status-badge`.
- Permisos `permission-tree`.

## 19. Base de datos y scripts

Scripts principales:

```text
scripts/sql/001-create-database.sql
scripts/sql/002-cargar-categorias-financieras.sql
scripts/sql/003-cargar-productos-catalogo.sql
```

`001-create-database.sql` se genera desde el modelo EF vigente y crea desde cero todas las tablas, relaciones, indices, permisos, roles, moneda y motivos de nota de credito. No contiene migraciones, backfills ni alteraciones historicas.

Los scripts `002` y `003` son cargas idempotentes. El segundo es especifico para la empresa con RUC `20615082997`.

## 20. Build y pruebas

Comandos:

```powershell
dotnet restore ViveroLosFrutales.sln --configfile NuGet.Config
dotnet build ViveroLosFrutales.sln -p:UseAppHost=false
dotnet test ViveroLosFrutales.sln --no-build
```

Si la app web esta corriendo y bloquea DLLs:

```powershell
New-Item -ItemType Directory -Force -Path .codex-build | Out-Null
dotnet build src/ViveroLosFrutales.Web/ViveroLosFrutales.Web.csproj -p:UseAppHost=false -o .codex-build
Remove-Item -LiteralPath .codex-build -Recurse -Force
```

## 21. Consideraciones tecnicas

- No confiar `EmpresaId` desde formularios transaccionales.
- Usar `AsNoTracking` para listados.
- Usar proyecciones DTO en repositorios.
- Evitar consultas N+1.
- Registrar errores de Nubefact con solicitud y respuesta.
- No exponer token Nubefact en vistas.
- Mantener scripts SQL idempotentes con `IF OBJECT_ID` y `COL_LENGTH`.
