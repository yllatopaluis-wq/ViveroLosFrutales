using Serilog;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Infrastructure;
using ViveroLosFrutales.Web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.PathBase.json", optional: false, reloadOnChange: true);

var configuredPathBase = builder.Configuration["PathBase"]?.TrimEnd('/');
var applicationPathBase = string.IsNullOrWhiteSpace(configuredPathBase)
    ? PathString.Empty
    : new PathString(configuredPathBase);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    var mensajes = options.ModelBindingMessageProvider;
    mensajes.SetAttemptedValueIsInvalidAccessor((value, field) => $"El valor '{value}' no es valido para {field}.");
    mensajes.SetMissingBindRequiredValueAccessor(field => $"El campo {field} es obligatorio.");
    mensajes.SetMissingKeyOrValueAccessor(() => "Debe ingresar un valor.");
    mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(value => $"El valor '{value}' no es valido.");
    mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor ingresado no es valido.");
    mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "El valor debe ser numerico.");
    mensajes.SetUnknownValueIsInvalidAccessor(field => $"El valor ingresado para {field} no es valido.");
    mensajes.SetValueIsInvalidAccessor(value => $"El valor '{value}' no es valido.");
    mensajes.SetValueMustBeANumberAccessor(field => $"El campo {field} debe ser numerico.");
    mensajes.SetValueMustNotBeNullAccessor(field => $"El campo {field} es obligatorio.");
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

var dataProtectionPath = Path.Combine(builder.Environment.ContentRootPath, ".data-protection-keys");
Directory.CreateDirectory(dataProtectionPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("ViveroLosFrutales");

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Path = applicationPathBase.HasValue ? applicationPathBase.Value : "/";
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".ViveroLosFrutales.ERP";
    options.Cookie.Path = applicationPathBase.HasValue ? applicationPathBase.Value : "/";
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/login";
});
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ComprobanteService>();
builder.Services.AddScoped<IFormularioConfiguracionService, FormularioConfiguracionService>();
builder.Services.AddScoped<ICondicionComercialService, CondicionComercialService>();
builder.Services.AddScoped<IPlantillaDocumentoService, PlantillaDocumentoService>();
builder.Services.AddScoped<DocumentoConfiguracionService>();
builder.Services.AddScoped<CotizacionService>();
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<CompraService>();
 builder.Services.AddScoped<OrdenCompraService>();
 builder.Services.AddScoped<PagoProveedorAplicacionService>();
builder.Services.AddScoped<GastoService>();
builder.Services.AddScoped<IngresoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<ConfiguracionEmpresaService>();
builder.Services.AddScoped<NubefactLogService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ReporteGeneralService>();
builder.Services.AddScoped<ErrorAplicacionService>();
builder.Services.AddScoped<NotaPedidoService>();
builder.Services.AddScoped<CobroClienteService>();
builder.Services.AddScoped<MovimientoCajaService>();
builder.Services.AddScoped<CuentaFinancieraService>();
builder.Services.AddScoped<TransferenciaService>();
builder.Services.AddScoped<EstadoCuentaClienteService>();
builder.Services.AddScoped<DevolucionService>();
builder.Services.AddScoped<IEmpresaContext, EmpresaContext>();
builder.Services.AddSingleton<SunatPendingSyncService>();

var app = builder.Build();

if (applicationPathBase.HasValue)
{
    app.UsePathBase(applicationPathBase);
    app.Use(async (context, next) =>
    {
        if (context.Request.PathBase != applicationPathBase)
        {
            var destination = applicationPathBase.Add(context.Request.Path);
            context.Response.Redirect($"{destination}{context.Request.QueryString}");
            return;
        }

        await next();
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseMiddleware<ApplicationErrorMiddleware>();
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var isAccountFlow = path.StartsWithSegments("/Account/Login") ||
        path.StartsWithSegments("/Account/SeleccionarEmpresa") ||
        path.StartsWithSegments("/Account/Logout") ||
        path.StartsWithSegments("/login") ||
        path.StartsWithSegments("/logout");

    if (context.User.Identity?.IsAuthenticated == true &&
        context.Session.GetInt32("EmpresaId") is null &&
        !isAccountFlow)
    {
        context.Response.Redirect($"{context.Request.PathBase}/logout?expired=1");
        return;
    }

    await next();
});
app.UseAuthorization();

app.MapControllerRoute(name: "login", pattern: "login", defaults: new { controller = "Account", action = "Login" });
app.MapControllerRoute(name: "logout", pattern: "logout", defaults: new { controller = "Account", action = "Logout" });
app.MapControllerRoute(name: "dashboard", pattern: "dashboard", defaults: new { controller = "Home", action = "Index" });
app.MapControllerRoute(name: "ventas", pattern: "ventas", defaults: new { controller = "Comprobantes", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (builder.Configuration.GetValue("Seed:RunOnStartup", true))
{
    await app.SeedDatabaseAsync();
}

app.Run();




