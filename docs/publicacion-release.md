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

## Archivos de configuraciÃ³n

Antes de subir a hosting revisar:

- `DeployHosting/ERPApp/appsettings.json`: cadena de conexiÃ³n, ruta PDF, token/URL Nubefact por empresa cuando aplique.
- `DeployHosting/PublicWeb/appsettings.json`: configuraciÃ³n del sitio pÃºblico.
- `web.config` generado en cada aplicaciÃ³n.

La aplicaciÃ³n ERP y el sitio pÃºblico son aplicaciones web independientes. En IIS deben publicarse como dos aplicaciones o sitios separados segÃºn la arquitectura de hosting definida.
