using System.Text;
using Serilog;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Infrastructure.Data;

namespace ViveroLosFrutales.Web.Services;

public class ApplicationErrorMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (EsCancelacionEsperada(context, exception))
        {
            Log.Debug(exception, "Solicitud cancelada por el cliente {TraceIdentifier} en {Method} {Path}", context.TraceIdentifier, context.Request.Method, context.Request.Path);
        }
        catch (Exception exception)
        {
            await RegistrarAsync(context, exception);
            throw;
        }
    }

    private static bool EsCancelacionEsperada(HttpContext context, Exception exception)
    {
        if (context.RequestAborted.IsCancellationRequested) return true;
        if (exception is OperationCanceledException or TaskCanceledException) return true;

        return exception.InnerException is not null && EsCancelacionEsperada(context, exception.InnerException);
    }

    private async Task RegistrarAsync(HttpContext context, Exception exception)
    {
        Log.Error(exception, "Error no controlado {TraceIdentifier} en {Method} {Path}", context.TraceIdentifier, context.Request.Method, context.Request.Path);

        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.ErroresAplicacion.Add(new ErrorAplicacion
            {
                EmpresaId = context.Session.GetInt32("EmpresaId"),
                FechaUtc = DateTime.UtcNow,
                Usuario = Limitar(context.User.Identity?.Name ?? "anonimo", 150),
                Ruta = Limitar(context.Request.Path.Value ?? "/", 500),
                MetodoHttp = Limitar(context.Request.Method, 10),
                TipoExcepcion = Limitar(exception.GetType().FullName ?? exception.GetType().Name, 300),
                Mensaje = Limitar(exception.Message, 2000),
                Detalle = CrearDetalle(context, exception),
                Identificador = Limitar(context.TraceIdentifier, 120)
            });
            await db.SaveChangesAsync();
        }
        catch (Exception loggingException)
        {
            Log.Error(loggingException, "No se pudo persistir el error {TraceIdentifier}", context.TraceIdentifier);
        }
    }

    private static string CrearDetalle(HttpContext context, Exception exception)
    {
        var detalle = new StringBuilder();
        detalle.AppendLine("=== RESUMEN ===");
        detalle.AppendLine($"Fecha UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        detalle.AppendLine($"TraceIdentifier: {context.TraceIdentifier}");
        detalle.AppendLine($"Usuario: {context.User.Identity?.Name ?? "anonimo"}");
        detalle.AppendLine($"Autenticado: {context.User.Identity?.IsAuthenticated == true}");
        detalle.AppendLine($"EmpresaId sesión: {context.Session.GetInt32("EmpresaId")?.ToString() ?? "sin empresa"}");
        detalle.AppendLine($"EmpresaNombre sesión: {context.Session.GetString("EmpresaNombre") ?? "sin empresa"}");
        detalle.AppendLine();

        detalle.AppendLine("=== REQUEST ===");
        detalle.AppendLine($"Método: {context.Request.Method}");
        detalle.AppendLine($"Scheme: {context.Request.Scheme}");
        detalle.AppendLine($"Host: {context.Request.Host}");
        detalle.AppendLine($"PathBase: {context.Request.PathBase}");
        detalle.AppendLine($"Path: {context.Request.Path}");
        detalle.AppendLine($"QueryString: {context.Request.QueryString}");
        detalle.AppendLine($"ContentType: {context.Request.ContentType ?? "sin content-type"}");
        detalle.AppendLine($"ContentLength: {context.Request.ContentLength?.ToString() ?? "sin content-length"}");
        detalle.AppendLine($"RemoteIp: {context.Connection.RemoteIpAddress}");
        detalle.AppendLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "sin endpoint"}");
        detalle.AppendLine();

        if (context.Request.RouteValues.Count > 0)
        {
            detalle.AppendLine("=== ROUTE VALUES ===");
            foreach (var item in context.Request.RouteValues)
            {
                detalle.AppendLine($"{item.Key}: {item.Value}");
            }
            detalle.AppendLine();
        }

        if (context.Request.Query.Count > 0)
        {
            detalle.AppendLine("=== QUERY ===");
            foreach (var item in context.Request.Query)
            {
                detalle.AppendLine($"{item.Key}: {Limitar(string.Join(",", item.Value.ToArray()), 1000)}");
            }
            detalle.AppendLine();
        }

        detalle.AppendLine("=== HEADERS ===");
        foreach (var header in context.Request.Headers)
        {
            if (EsHeaderSensible(header.Key)) continue;
            detalle.AppendLine($"{header.Key}: {Limitar(string.Join(",", header.Value.ToArray()), 1000)}");
        }
        detalle.AppendLine();

        detalle.AppendLine("=== EXCEPCIÓN ===");
        detalle.AppendLine(exception.ToString());
        if (exception.InnerException is not null)
        {
            detalle.AppendLine();
            detalle.AppendLine("=== INNER EXCEPTION ===");
            detalle.AppendLine(exception.InnerException.ToString());
        }

        return detalle.ToString();
    }

    private static bool EsHeaderSensible(string header) =>
        header.Equals("Cookie", StringComparison.OrdinalIgnoreCase)
        || header.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
        || header.Equals("Proxy-Authorization", StringComparison.OrdinalIgnoreCase)
        || header.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase);

    private static string Limitar(string valor, int maximo) =>
        valor.Length <= maximo ? valor : valor[..maximo];
}