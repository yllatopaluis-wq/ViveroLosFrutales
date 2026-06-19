using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Infrastructure.Data;
using ViveroLosFrutales.Infrastructure.Identity;
using ViveroLosFrutales.Infrastructure.Nubefact;
using ViveroLosFrutales.Infrastructure.Pdf;

namespace ViveroLosFrutales.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ViveroLosFrutalesConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IComprobanteRepository, ComprobanteRepository>();
        services.AddScoped<IMotivoNotaCreditoRepository, MotivoNotaCreditoRepository>();
        services.AddScoped<ICotizacionRepository, CotizacionRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<IPagoProveedorRepository, PagoProveedorRepository>();
        services.AddScoped<IGastoRepository, GastoRepository>();
        services.AddScoped<IIngresoRepository, IngresoRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<INotaPedidoRepository, NotaPedidoRepository>();
        services.AddScoped<ICobroClienteRepository, CobroClienteRepository>();
        services.AddScoped<IMovimientoCajaRepository, MovimientoCajaRepository>();
        services.AddScoped<IComprobanteCobroAplicadoRepository, ComprobanteCobroAplicadoRepository>();
        services.AddScoped<IEstadoCuentaClienteRepository, EstadoCuentaClienteRepository>();
        services.AddScoped<IDevolucionRepository, DevolucionRepository>();
        services.AddScoped<INubefactOperacionRepository, NubefactOperacionRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IConfiguracionEmpresaRepository, ConfiguracionEmpresaRepository>();
        services.Configure<PdfOptions>(configuration.GetSection("Pdf"));
        services.Configure<NubefactOptions>(configuration.GetSection("Nubefact"));
        services.AddScoped<IPdfService, PdfService>();
        services.AddHttpClient<INubefactService, NubefactService>();

        return services;
    }
}
