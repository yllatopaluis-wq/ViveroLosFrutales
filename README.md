# ViveroLosFrutales

Solución ASP.NET Core MVC .NET 8 para Vivero Los Frutales. Contiene una web pública corporativa y un ERP autenticado publicables de forma independiente.

## Estructura

```text
src/
  ViveroLosFrutales.PublicWeb     Web pública, raíz del dominio
  ViveroLosFrutales.Web           ERP, bajo /app
  ViveroLosFrutales.Application  Casos de uso, servicios y DTOs
  ViveroLosFrutales.Domain       Entidades y reglas de dominio
  ViveroLosFrutales.Infrastructure EF Core, Identity y adaptadores
tests/
  ViveroLosFrutales.Tests
```

Ambos proyectos web reutilizan Application, Domain e Infrastructure. Cada uno conserva sus propios controladores, vistas, layout, configuración, proceso y `wwwroot`.

## Requisitos

- .NET SDK 8
- SQL Server
- Hosting Bundle de ASP.NET Core 8 para IIS

## Base de datos

Configurar `ConnectionStrings:ViveroLosFrutalesConnection`. Para una instalación nueva ejecutar:

```text
scripts/sql/001-create-database.sql
```

Luego, si corresponden, cargar:

```text
scripts/sql/002-cargar-categorias-financieras.sql
scripts/sql/003-cargar-productos-catalogo.sql
```

El script `001` contiene el esquema completo vigente. Los scripts `002` y `003` son cargas de datos.

## Restaurar y compilar

```powershell
dotnet restore ViveroLosFrutales.sln --configfile NuGet.Config
dotnet build ViveroLosFrutales.sln
dotnet test ViveroLosFrutales.sln
```

## Ejecutar localmente

Abrir dos consolas:

```powershell
dotnet run --project src/ViveroLosFrutales.PublicWeb
```

```powershell
dotnet run --project src/ViveroLosFrutales.Web
```

URLs configuradas:

```text
http://localhost:5100
http://localhost:5200/app/login
```

La web pública no requiere autenticación. El ERP usa Identity, un filtro global de autorización y `PathBase=/app`.

## Publicar

```powershell
dotnet publish src/ViveroLosFrutales.PublicWeb -c Release -o ./publish-public
dotnet publish src/ViveroLosFrutales.Web -c Release -o ./publish-app
```

En IIS, `publish-public` se monta en la raíz de `viverolosfrutales.pe`; `publish-app` se convierte en una Application bajo `/app`. Ambos usan el binding y certificado SSL del mismo sitio.

## Documentación

- [Ejecución local](docs/levantar-proyecto.md)
- [Arquitectura de dos aplicaciones](docs/arquitectura-dos-aplicaciones-web.md)
- [Despliegue IIS](docs/despliegue-iis-dos-aplicaciones.md)
- [Documentación funcional](docs/documentacion-funcional.md)
- [Documentación técnica](docs/documentacion-tecnica.md)
- [Esquema SQL](docs/depuracion-esquema-sql.md)
