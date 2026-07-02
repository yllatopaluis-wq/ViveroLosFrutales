# PublicaciÃ³n Release

Todas las salidas de hosting se generan Ãºnicamente en `DeployHosting`:

```text
DeployHosting/
  PublicWeb/
  ERPApp/
  ViveroLosFrutales-PublicWeb.zip
  ViveroLosFrutales-ERPApp.zip
  ViveroLosFrutales-BaseDatos.zip
```

Desde la terminal PowerShell de Visual Studio Code ejecutar:

```powershell
.\scripts\publish-release.ps1
```

El script limpia y reemplaza `PublicWeb` y `ERPApp` antes de publicar. No genera carpetas `publish`, `publish-app`, `publish-public` ni `DeployViveroLosFrutales`.

Para publicar manualmente deben usarse siempre estas rutas:

```powershell
dotnet publish src/ViveroLosFrutales.PublicWeb -c Release -o ./DeployHosting/PublicWeb
dotnet publish src/ViveroLosFrutales.Web -c Release -o ./DeployHosting/ERPApp
```

## Hosting Windows/Plesk

- En Plesk, desmarcar `Microsoft ASP.NET support` para que el Application Pool quede en `No Managed Code`.
- El ERP publicado queda con `Seed:RunOnStartup=false`; la base se crea y carga con los scripts SQL de esta carpeta.
- Si aparece `HTTP Error 502.5 - ANCM Out-Of-Process Startup Failure`, revisar primero `DeployHosting/ERPApp/logs/stdout*.log` en el servidor.
- El `web.config` del ERP debe apuntar a `ViveroLosFrutales.Web.dll` con `AspNetCoreModuleV2`.

## Base de datos inicial

Ejecutar los scripts SQL en este orden:

```text
001-create-database.sql
002-cargar-empresa-inicial.sql
003-cargar-categorias-financieras.sql
004-cargar-productos-catalogo.sql
005-cargar-clientes-entidades.sql
006-cargar-productos-por-empresa.sql
007-cargar-usuario-admin.sql
```

El script `007-cargar-usuario-admin.sql` crea/verifica el usuario inicial:

```text
Usuario: admin
Password: Admin1234
```

Cambiar la contraseÃ±a despuÃ©s del primer ingreso.

El script `001-create-database.sql` ya incluye las tablas de TesorerÃ­a: `erp.CuentaFinanciera`, `erp.TransferenciaFinanciera`, columnas `CuentaFinancieraId`, Ã­ndices, relaciones y permisos.

## ActualizaciÃ³n de una base existente

Cuando se publica una versiÃ³n nueva sobre una base ya existente, ejecutar los parches idempotentes en este orden antes de probar los formularios del ERP:

```text
008-fix-tesoreria-cuentas-financieras.sql
009-diferenciar-clientes-por-empresa.sql
010-add-representante-legal-empresa.sql
011-validar-esquema-publicado.sql
012-sync-roles-permisos.sql
013-reparar-cuenta-cobros-movimiento-caja.sql
```

El `008` crea o completa el esquema de TesorerÃ­a, cuentas financieras, columnas `CuentaFinancieraId`, cuenta `Caja principal`, Ã­ndices, relaciones y transferencias. El `009` diferencia clientes por empresa. El `010` agrega representante legal y firma de empresa. El `011` valida que PRE/producciÃ³n tenga las tablas y columnas que el cÃ³digo publicado espera. El `012` sincroniza permisos y asignaciones para que el formulario de roles muestre todos los formularios vigentes. El `013` repara los movimientos de caja de cobros existentes para que usen la cuenta financiera guardada en el cobro y no queden imputados a Caja principal.

Ejemplo con autenticaciÃ³n Windows:

```cmd
sqlcmd -S SERVIDOR_SQL -d ViveroLosFrutalesDB -E -i scripts\sql\008-fix-tesoreria-cuentas-financieras.sql
sqlcmd -S SERVIDOR_SQL -d ViveroLosFrutalesDB -E -i scripts\sql\009-diferenciar-clientes-por-empresa.sql
sqlcmd -S SERVIDOR_SQL -d ViveroLosFrutalesDB -E -i scripts\sql\010-add-representante-legal-empresa.sql
sqlcmd -S SERVIDOR_SQL -d ViveroLosFrutalesDB -E -i scripts\sql\011-validar-esquema-publicado.sql
sqlcmd -S SERVIDOR_SQL -d ViveroLosFrutalesDB -E -i scripts\sql\012-sync-roles-permisos.sql
sqlcmd -S SERVIDOR_SQL -d ViveroLosFrutalesDB -E -i scripts\sql\013-reparar-cuenta-cobros-movimiento-caja.sql
```

Con usuario SQL reemplazar `-E` por `-U usuario -P password`.

Si en la web publicada aparece el mensaje `No se pudo completar la solicitud` en formularios como Comprobantes, Compras, Gastos o Caja, revisar primero:

1. Ejecutar `011-validar-esquema-publicado.sql` contra la base del hosting.
2. Si el validador indica faltantes, ejecutar el script sugerido y volver a validar.
3. Si el validador pasa, revisar `AdministraciÃ³n > Errores de aplicaciÃ³n` o la tabla `erp.ErrorAplicacion` para ver la excepciÃ³n tÃ©cnica.
## Archivos de configuraciÃ³n

Antes de subir a hosting revisar:

- `DeployHosting/ERPApp/appsettings.json`: cadena de conexiÃ³n, ruta PDF, token/URL Nubefact por empresa cuando aplique.
- `DeployHosting/PublicWeb/appsettings.json`: configuraciÃ³n del sitio pÃºblico.
- `web.config` generado en cada aplicaciÃ³n.

La aplicaciÃ³n ERP y el sitio pÃºblico son aplicaciones web independientes. En IIS deben publicarse como dos aplicaciones o sitios separados segÃºn la arquitectura de hosting definida.
