# ViveroLosFrutales

Sistema web empresarial multiempresa para gestion comercial de viveros, cotizaciones, comprobantes y facturacion electronica con Nubefact.

## Estructura

```text
ViveroLosFrutales.sln
src/
  ViveroLosFrutales.Web
  ViveroLosFrutales.Application
  ViveroLosFrutales.Domain
  ViveroLosFrutales.Infrastructure
tests/
  ViveroLosFrutales.Tests
scripts/
  sql/001-create-database.sql
  sql/002-cargar-categorias-financieras.sql
  sql/003-cargar-productos-catalogo.sql
docs/
  arquitectura-propuesta.md
  documentacion-funcional.md
  documentacion-tecnica.md
  levantar-proyecto.md
```

## Requisitos

- .NET SDK 8.
- SQL Server.
- Acceso para crear/actualizar la base `ViveroLosFrutalesDB`.
- Hosting Windows compatible con ASP.NET Core 8.

## Configuracion

Editar la cadena de conexion en:

```text
src/ViveroLosFrutales.Web/appsettings.json
```

Nombre configurado:

```text
ViveroLosFrutalesConnection
```

Base de datos:

```text
ViveroLosFrutalesDB
```

Para SQL Server con autenticacion integrada:

```json
"ViveroLosFrutalesConnection": "Server=localhost;Database=ViveroLosFrutalesDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Si aparece `Failed to generate SSPI context`, usar autenticacion SQL Server:

```json
"ViveroLosFrutalesConnection": "Server=localhost;Database=ViveroLosFrutalesDB;User Id=sa;Password=TU_PASSWORD;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

El seed inicial se ejecuta al arrancar. Para desactivarlo temporalmente:

```json
"Seed": {
  "RunOnStartup": false
}
```

## Base de datos

Para una instalacion nueva, ejecutar primero:

```text
scripts/sql/001-create-database.sql
```

El script `001` crea el esquema completo vigente, Identity, relaciones, indices y datos maestros estaticos. No contiene parches ni migraciones historicas.

Despues del primer arranque, cuando ya exista la empresa, ejecutar los scripts de datos que correspondan:

```text
scripts/sql/002-cargar-categorias-financieras.sql
scripts/sql/003-cargar-productos-catalogo.sql
```

El script `003` carga el catalogo exclusivamente para la empresa con RUC `20615082997`.

## Levantar el proyecto

Guia completa:

```text
docs/levantar-proyecto.md
```

Pasos rapidos:

```powershell
dotnet restore ViveroLosFrutales.sln --configfile NuGet.Config
dotnet build ViveroLosFrutales.sln
dotnet run --project src/ViveroLosFrutales.Web
```

Abrir la URL que muestre la consola, por ejemplo:

```text
https://localhost:5001
http://localhost:5000
```

Si ya hay una instancia de `ViveroLosFrutales.Web` corriendo, `dotnet build` puede fallar porque el `.dll` o `.exe` esta bloqueado. Cerrar la app en ejecucion o compilar a una carpeta temporal:

```powershell
dotnet build src/ViveroLosFrutales.Web/ViveroLosFrutales.Web.csproj -o ./.build-check
```

Luego eliminar `.build-check`.

Si PowerShell no encuentra `dotnet`, usar:

```powershell
& "C:\Program Files\dotnet\dotnet.exe" restore ViveroLosFrutales.sln --configfile NuGet.Config
```

En entornos restringidos puede ser necesario aislar carpetas temporales dentro del proyecto:

```powershell
$env:DOTNET_CLI_HOME="$PWD\.dotnet"
$env:NUGET_PACKAGES="$PWD\.nuget\packages"
$env:APPDATA="$PWD\.appdata"
$env:LOCALAPPDATA="$PWD\.localappdata"
& "C:\Program Files\dotnet\dotnet.exe" restore ViveroLosFrutales.sln --configfile NuGet.Config
```

## Estado funcional

- Arquitectura por capas con nombres `ViveroLosFrutales`.
- Dominio multiempresa.
- Seleccion de empresa desde el login.
- Menu lateral colapsable con un solo modulo abierto.
- Entidad unica `Comprobante` para COT, BOL, FAC y NPE.
- EF Core con SQL Server, indices y decimal para montos.
- ASP.NET Identity configurado.
- Roles con permisos seleccionables en 3 niveles: modulo, formulario y accion.
- Servicios de empresas, categorias, productos, clientes, proveedores, compras, gastos, ingresos y comprobantes.
- Repositorios con proyecciones y `AsNoTracking` en lectura.
- Compras con actualizacion automatica de stock y movimiento de inventario.
- Formulario de compras con detalle dinamico y boton agregar.
- PDF local para cotizaciones y notas de pedido.
- Servicio Nubefact preparado para emitir, consultar y anular.
- Log Nubefact con solicitud/respuesta y numero de comprobante.
- MVC con vistas Razor compactas para operacion de escritorio.
- Estados visuales para activo/anulado.

## Documentacion

- Funcional: `docs/documentacion-funcional.md`
- Tecnica: `docs/documentacion-tecnica.md`
- Arquitectura: `docs/arquitectura-propuesta.md`
- Puesta en marcha: `docs/levantar-proyecto.md`
- Depuracion del esquema SQL: `docs/depuracion-esquema-sql.md`
