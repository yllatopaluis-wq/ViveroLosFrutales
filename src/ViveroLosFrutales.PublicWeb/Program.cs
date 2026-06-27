using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Infrastructure.Data;
using ViveroLosFrutales.PublicWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddControllersWithViews();

var dataProtectionPath = Path.Combine(builder.Environment.ContentRootPath, ".data-protection-keys");
Directory.CreateDirectory(dataProtectionPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("ViveroLosFrutales.PublicWeb");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ViveroLosFrutalesConnection")));
builder.Services.AddScoped<IPublicCatalogRepository, PublicCatalogRepository>();
builder.Services.AddScoped<PublicCatalogService>();
builder.Services.AddScoped<PublicContentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("nosotros", "nosotros", new { controller = "Nosotros", action = "Index" });
app.MapControllerRoute("productos", "productos", new { controller = "Productos", action = "Index" });
app.MapControllerRoute("servicios", "servicios", new { controller = "Servicios", action = "Index" });
app.MapControllerRoute("contacto", "contacto", new { controller = "Contacto", action = "Index" });
app.MapControllerRoute("error", "error", new { controller = "Home", action = "Error" });
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
