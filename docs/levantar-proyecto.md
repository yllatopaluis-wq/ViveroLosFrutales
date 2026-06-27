# Levantar ViveroLosFrutales

La solución ejecuta dos procesos web independientes. La web pública ocupa la raíz y el ERP funciona bajo `/app`.

## Requisitos

- .NET SDK 8
- SQL Server o SQL Server Express
- PowerShell o CMD

Verificar el SDK:

```cmd
dotnet --version
```

## Configuración de datos

Configurar la conexión `ViveroLosFrutalesConnection` en el proyecto que se ejecutará o mediante una variable de entorno:

```cmd
set ConnectionStrings__ViveroLosFrutalesConnection=Server=localhost;Database=ViveroLosFrutalesDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Para una instalación nueva, ejecutar los scripts de `scripts/sql` del `001` al `007` en orden. El detalle está en `docs/instalacion-base-datos.md`.

## Restaurar y compilar

Desde la raíz:

```cmd
dotnet restore ViveroLosFrutales.sln --configfile NuGet.Config
dotnet build ViveroLosFrutales.sln
dotnet test ViveroLosFrutales.sln
```

## Ejecutar la web pública

```cmd
dotnet run --project src\ViveroLosFrutales.PublicWeb
```

Abrir:

```text
http://localhost:5100
```

Las páginas Inicio, Nosotros, Productos, Servicios y Contacto son públicas. El catálogo usa las capas compartidas y muestra contenido de respaldo si SQL Server no está disponible.

## Ejecutar el ERP

En otra consola:

```cmd
dotnet run --project src\ViveroLosFrutales.Web
```

Abrir:

```text
http://localhost:5200/app/login
```

Rutas de comprobación:

```text
http://localhost:5200/app/dashboard
http://localhost:5200/app/ventas
```

Un usuario anónimo es redirigido a `/app/login`. `PathBase` se configura en `src/ViveroLosFrutales.Web/appsettings.PathBase.json`.

## Publicación

```cmd
dotnet publish src\ViveroLosFrutales.PublicWeb -c Release -o .\DeployHosting\PublicWeb
dotnet publish src\ViveroLosFrutales.Web -c Release -o .\DeployHosting\ERPApp
```

Para release completo usar `scripts\publish-release.ps1`. Consultar `docs/despliegue-iis-dos-aplicaciones.md` para montar ambas salidas con un solo dominio y certificado.

## Problemas frecuentes

- Si una DLL está bloqueada, detener la aplicación antes de volver a compilar o publicar.
- Si falla SQL Server, verificar servidor, credenciales y que el esquema `001` se haya aplicado.
- Para evitar el seed durante una prueba usar `set Seed__RunOnStartup=false` antes de ejecutar el ERP.
- En IIS, `/app` debe convertirse en Application; un directorio virtual no es suficiente.
