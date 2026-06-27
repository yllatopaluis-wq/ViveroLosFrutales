# Arquitectura de las aplicaciones web

```text
ViveroLosFrutales.PublicWeb --\
                              +--> Application --> Domain
ViveroLosFrutales.Web -------/           |
                                          +--> Infrastructure --> SQL Server
```

`PublicWeb` contiene solamente la experiencia corporativa: inicio, nosotros, productos, servicios y contacto. No registra autenticacion ni expone controladores del ERP.

`Web` conserva Identity, sesion, permisos y todos los modulos internos. Su filtro global exige autenticacion y su URL base es `/app`.

Los DTO publicos y el servicio de catalogo viven en Application. La consulta EF Core vive en Infrastructure. No se duplican entidades, `DbContext`, repositorios existentes ni reglas del ERP.

## Ejecutar y publicar

```powershell
dotnet run --project src/ViveroLosFrutales.PublicWeb
dotnet run --project src/ViveroLosFrutales.Web

.\scripts\publish-release.ps1
```

El publicador genera `DeployHosting/PublicWeb` para el sitio publico, `DeployHosting/ERPApp` para el ERP y los ZIP de entrega. Consultar `docs/despliegue-iis-dos-aplicaciones.md` para el montaje bajo un solo dominio y certificado.
