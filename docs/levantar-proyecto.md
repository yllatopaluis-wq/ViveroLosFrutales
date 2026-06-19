# Levantar el proyecto - ViveroLosFrutales

Esta guia deja el proyecto listo para ejecutar localmente en Windows con SQL Server.

## 1. Requisitos

- .NET SDK 8 instalado.
- SQL Server o SQL Server Express.
- Acceso a SQL Server con permisos para crear base de datos y tablas.
- PowerShell.

Verificar .NET:

```powershell
dotnet --version
```

## 2. Configurar SQL Server

Editar:

```text
src/ViveroLosFrutales.Web/appsettings.json
```

Cadena esperada:

```json
"ViveroLosFrutalesConnection": "Server=localhost;Database=ViveroLosFrutalesDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Si la autenticacion integrada falla, usar usuario SQL Server:

```json
"ViveroLosFrutalesConnection": "Server=localhost;Database=ViveroLosFrutalesDB;User Id=sa;Password=TU_PASSWORD;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

## 3. Crear la base de datos

Para una instalacion nueva, ejecutar en SQL Server Management Studio, Azure Data Studio o `sqlcmd`:

```text
scripts/sql/001-create-database.sql
```

El script `001` crea el esquema completo vigente, incluyendo Identity, modulos operativos, claves foraneas, indices y datos maestros estaticos.

Despues del primer arranque, cuando ya exista la empresa, ejecutar las cargas requeridas:

```text
scripts/sql/002-cargar-categorias-financieras.sql
scripts/sql/003-cargar-productos-catalogo.sql
```

El catalogo de productos aplica a la empresa con RUC `20615082997` y puede omitirse en otras instalaciones.

## 4. Restaurar paquetes

Desde la raiz del proyecto:

```powershell
dotnet restore ViveroLosFrutales.sln --configfile NuGet.Config
```

En entornos restringidos:

```powershell
$env:DOTNET_CLI_HOME="$PWD\.dotnet"
$env:NUGET_PACKAGES="$PWD\.nuget\packages"
$env:APPDATA="$PWD\.appdata"
$env:LOCALAPPDATA="$PWD\.localappdata"
dotnet restore ViveroLosFrutales.sln --configfile NuGet.Config
```

## 5. Compilar

```powershell
dotnet build ViveroLosFrutales.sln
```

Si existe una instancia web ejecutandose, el build puede fallar por archivos bloqueados. Opciones:

- Detener la consola donde corre `dotnet run`.
- Finalizar el proceso `ViveroLosFrutales.Web`.
- Compilar a una salida temporal:

```powershell
dotnet build src/ViveroLosFrutales.Web/ViveroLosFrutales.Web.csproj -o ./.build-check
Remove-Item -LiteralPath .\.build-check -Recurse -Force
```

## 6. Ejecutar

```powershell
dotnet run --project src/ViveroLosFrutales.Web
```

Abrir la URL que imprime la consola. Normalmente:

```text
https://localhost:5001
http://localhost:5000
```

## 7. Seed inicial

El proyecto ejecuta seed al arrancar si esta activo:

```json
"Seed": {
  "RunOnStartup": true
}
```

Para desactivarlo temporalmente:

```json
"Seed": {
  "RunOnStartup": false
}
```

## 8. Probar modulos principales

Al iniciar sesion:

1. Seleccionar la empresa en el formulario de login.
2. Verificar que el encabezado muestre empresa activa y usuario.
3. Verificar que Roles muestre permisos por modulo, formulario y accion.
4. Crear categorias.
5. Crear productos asociados a categorias.
6. Crear clientes.
7. Crear proveedores.
8. Registrar compras y validar que el stock del producto aumente.
9. Registrar gastos e ingresos.
10. Crear cotizaciones o comprobantes.
11. Revisar Log Nubefact cuando exista un error o envio.

## 9. Problemas frecuentes

### `Failed to generate SSPI context`

Usar autenticacion SQL Server en la cadena de conexion.

### `The process cannot access the file ... ViveroLosFrutales.Web.dll`

Hay una instancia de la app corriendo. Detenerla o compilar con `-o ./.build-check`.

### Error al consultar vulnerabilidades de NuGet

Si el entorno no tiene acceso a internet, puede aparecer un warning `NU1900`. No impide compilar si los paquetes ya estan restaurados.

### No aparecen tablas nuevas

En una instalacion nueva no se aplican parches incrementales. Verificar que `scripts/sql/001-create-database.sql` se haya ejecutado completamente sobre una base vacia.

### No aparecen todos los permisos en Roles

Reiniciar la aplicacion con el seed activo:

```json
"Seed": {
  "RunOnStartup": true
}
```

Tambien se completan permisos faltantes al entrar al formulario de crear/editar roles.
