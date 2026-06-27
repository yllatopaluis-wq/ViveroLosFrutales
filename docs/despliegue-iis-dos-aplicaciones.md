# Despliegue IIS: web pública y ERP bajo un dominio

La solución contiene dos aplicaciones ASP.NET Core independientes que comparten las capas de negocio y datos:

| Proyecto | URL final | Autenticación |
|---|---|---|
| `ViveroLosFrutales.PublicWeb` | `https://viverolosfrutales.pe` | No |
| `ViveroLosFrutales.Web` | `https://viverolosfrutales.pe/app` | ASP.NET Core Identity |

El certificado TLS se configura una sola vez en el sitio IIS. La aplicación `/app` hereda el mismo dominio, puerto y certificado.

## Publicación independiente

Ejecutar desde la raíz de la solución:

```powershell
.\scripts\publish-release.ps1
```

El proceso genera `DeployHosting/PublicWeb`, `DeployHosting/ERPApp` y los ZIP de entrega, incluido `ViveroLosFrutales-BaseDatos.zip` con los scripts SQL. Publicar nuevamente uno de los proyectos no obliga a publicar el otro.

## Configurar el sitio raíz

1. Instalar el Hosting Bundle de .NET 8 en el servidor.
2. Crear un Application Pool con `No Managed Code`.
3. Crear un sitio IIS para `viverolosfrutales.pe` cuyo directorio físico sea `DeployHosting/PublicWeb`.
4. Agregar los bindings HTTP y HTTPS del dominio.
5. Seleccionar el certificado SSL de `viverolosfrutales.pe` en el binding HTTPS.
6. Conceder al Application Pool permisos de lectura y ejecución sobre la carpeta publicada.

## Configurar la subaplicación ERP

1. Crear dentro del sitio una carpeta virtual llamada `app` que apunte a `DeployHosting/ERPApp`.
2. En IIS, elegir **Convert to Application**. No dejarla solamente como directorio virtual.
3. Asignarle un Application Pool `No Managed Code`, preferiblemente independiente del sitio público.
4. Conceder permisos sobre la carpeta, además de escritura en las carpetas usadas por logs, PDF y llaves de protección de datos.
5. Mantener `PathBase` con valor `/app` en `appsettings.PathBase.json`.
6. Configurar la cadena `ConnectionStrings__ViveroLosFrutalesConnection` como variable de entorno o mediante configuración segura del servidor.

El `web.config` de cada carpeta se genera automáticamente mediante `dotnet publish`; no debe copiarse el de una aplicación sobre la otra.

## Rutas que deben verificarse

```text
https://viverolosfrutales.pe/
https://viverolosfrutales.pe/productos
https://viverolosfrutales.pe/contacto
https://viverolosfrutales.pe/app/login
https://viverolosfrutales.pe/app/dashboard
https://viverolosfrutales.pe/app/ventas
```

Una solicitud anónima a `/app/dashboard` debe responder con una redirección hacia `/app/login` y conservar el `ReturnUrl`.

## Consideraciones operativas

- El sitio público y el ERP tienen `wwwroot`, layouts y procesos separados.
- La cookie del ERP usa una ruta limitada a `/app`; no se envía innecesariamente al sitio público.
- Los Tag Helpers de MVC y `Url.Action` agregan automáticamente el `PathBase` a enlaces, formularios y recursos estáticos del ERP.
- Evitar rutas manuales que comiencen en `/` dentro del ERP. Usar `asp-controller`, `asp-action`, `Url.Action` o `Request.PathBase`.
- Si IIS ya entrega `/app` como `PathBase`, la configuración no duplica el prefijo.
- El sitio público puede consultar catálogo y empresa mediante Infrastructure; si la base no está disponible, presenta contenido comercial de respaldo.

## Ejecución local

Abrir dos consolas desde la raíz:

```cmd
dotnet run --project src\ViveroLosFrutales.PublicWeb
```

```cmd
dotnet run --project src\ViveroLosFrutales.Web
```

URLs locales configuradas:

```text
http://localhost:5100
http://localhost:5200/app/login
```

