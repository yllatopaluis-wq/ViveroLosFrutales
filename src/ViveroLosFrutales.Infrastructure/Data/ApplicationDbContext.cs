using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
using ViveroLosFrutales.Domain.Security;
using ViveroLosFrutales.Infrastructure.Identity;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Rol> RolesNegocio => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
    public DbSet<UsuarioEmpresa> UsuarioEmpresas => Set<UsuarioEmpresa>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Comprobante> Comprobantes => Set<Comprobante>();
    public DbSet<ComprobanteDetalle> ComprobanteDetalles => Set<ComprobanteDetalle>();
    public DbSet<MotivoNotaCredito> MotivosNotaCredito => Set<MotivoNotaCredito>();
    public DbSet<Moneda> Monedas => Set<Moneda>();
    public DbSet<NubefactOperacion> NubefactOperaciones => Set<NubefactOperacion>();
    public DbSet<ConfiguracionEmpresa> ConfiguracionesEmpresa => Set<ConfiguracionEmpresa>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<CompraDetalle> CompraDetalles => Set<CompraDetalle>();
    public DbSet<PagoProveedor> PagosProveedor => Set<PagoProveedor>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<Ingreso> Ingresos => Set<Ingreso>();
    public DbSet<CategoriaGasto> CategoriasGasto => Set<CategoriaGasto>();
    public DbSet<CategoriaIngreso> CategoriasIngreso => Set<CategoriaIngreso>();
    public DbSet<NotaPedido> NotasPedido => Set<NotaPedido>();
    public DbSet<NotaPedidoDetalle> NotaPedidoDetalles => Set<NotaPedidoDetalle>();
    public DbSet<CobroCliente> CobrosCliente => Set<CobroCliente>();
    public DbSet<MovimientoCaja> MovimientosCaja => Set<MovimientoCaja>();
    public DbSet<CuentaFinanciera> CuentasFinancieras => Set<CuentaFinanciera>();
    public DbSet<TransferenciaFinanciera> TransferenciasFinancieras => Set<TransferenciaFinanciera>();
    public DbSet<ComprobanteCobroAplicado> ComprobanteCobrosAplicados => Set<ComprobanteCobroAplicado>();
    public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();
    public DbSet<CotizacionDetalle> CotizacionDetalles => Set<CotizacionDetalle>();
    public DbSet<Devolucion> Devoluciones => Set<Devolucion>();
    public DbSet<ErrorAplicacion> ErroresAplicacion => Set<ErrorAplicacion>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("erp");

        builder.Entity<ApplicationUser>().ToTable("AspNetUsers", "erp");
        builder.Entity<IdentityRole>().ToTable("AspNetRoles", "erp");
        builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "erp");
        builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "erp");
        builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "erp");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "erp");
        builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "erp");

        builder.Entity<Empresa>(entity =>
        {
            entity.ToTable("Empresa");
            entity.HasKey(x => x.EmpresaId);
            entity.Property(x => x.RUC).HasMaxLength(11).IsRequired();
            entity.Property(x => x.RazonSocial).HasMaxLength(200).IsRequired();
            entity.Property(x => x.NombreComercial).HasMaxLength(200);
            entity.Property(x => x.TokenNubefact).HasMaxLength(1000);
            entity.Property(x => x.LogoPath).HasMaxLength(500);
            entity.Property(x => x.LogoContenido).HasColumnType("varbinary(max)");
            entity.Property(x => x.LogoContentType).HasMaxLength(120);
            entity.Property(x => x.LogoNombre).HasMaxLength(260);
            entity.Property(x => x.RepresentanteLegalNombre).HasMaxLength(200);
            entity.Property(x => x.RepresentanteLegalDocumento).HasMaxLength(20);
            entity.Property(x => x.RepresentanteLegalCargo).HasMaxLength(120);
            entity.Property(x => x.FirmaContenido).HasColumnType("varbinary(max)");
            entity.Property(x => x.FirmaContentType).HasMaxLength(120);
            entity.Property(x => x.FirmaNombre).HasMaxLength(260);
            entity.Property(x => x.SerieNotaCredito).HasMaxLength(10);
            entity.Property(x => x.SerieNotaCreditoFactura).HasMaxLength(10);
            entity.Property(x => x.SerieNotaCreditoBoleta).HasMaxLength(10);
            entity.HasIndex(x => x.RUC).IsUnique();
        });

        builder.Entity<Rol>(entity =>
        {
            entity.ToTable("Rol");
            entity.HasKey(x => x.RolId);
            entity.Property(x => x.Nombre).HasMaxLength(80).IsRequired();
            entity.HasData(
                new Rol { RolId = 1, Nombre = "Administrador", Descripcion = "Acceso total", Estado = EstadoRegistro.Activo },
                new Rol { RolId = 2, Nombre = "Vendedor", Descripcion = "Operacion comercial", Estado = EstadoRegistro.Activo });
        });

        builder.Entity<Permiso>(entity =>
        {
            entity.ToTable("Permiso");
            entity.HasKey(x => x.PermisoId);
            entity.Property(x => x.Modulo).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Accion).HasMaxLength(40).IsRequired();
            entity.HasIndex(x => new { x.Modulo, x.Accion }).IsUnique();
            entity.HasData(BuildPermisos());
        });

        builder.Entity<RolPermiso>(entity =>
        {
            entity.ToTable("RolPermiso");
            entity.HasKey(x => x.RolPermisoId);
            entity.HasOne(x => x.Rol).WithMany(x => x.RolPermisos).HasForeignKey(x => x.RolId);
            entity.HasOne(x => x.Permiso).WithMany(x => x.RolPermisos).HasForeignKey(x => x.PermisoId);
            entity.HasData(BuildRolPermisos());
        });

        builder.Entity<UsuarioEmpresa>(entity =>
        {
            entity.ToTable("UsuarioEmpresa");
            entity.HasKey(x => x.UsuarioEmpresaId);
            entity.Property(x => x.UsuarioId).HasMaxLength(450).IsRequired();
            entity.HasIndex(x => new { x.UsuarioId, x.EmpresaId }).IsUnique();
            entity.HasOne(x => x.Empresa).WithMany(x => x.UsuarioEmpresas).HasForeignKey(x => x.EmpresaId);
        });

        builder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria");
            entity.HasKey(x => x.CategoriaId);
            entity.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(250);
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.Nombre }).IsUnique();
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
        });

        builder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto");
            entity.HasKey(x => x.ProductoId);
            entity.Property(x => x.Categoria).HasMaxLength(100);
            entity.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UnidadMedida).HasMaxLength(20);
            entity.Property(x => x.Stock).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PrecioVentaSinIgv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PrecioVentaConIgv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PorcentajeDetraccion).HasColumnType("decimal(5,2)");
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.Nombre });
            entity.HasOne(x => x.Empresa).WithMany(x => x.Productos).HasForeignKey(x => x.EmpresaId);
        });

        builder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");
            entity.HasKey(x => x.ClienteId);
            entity.Property(x => x.NumeroDocumento).HasMaxLength(20).IsRequired();
            entity.Property(x => x.NombreCompleto).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(120);
            entity.Property(x => x.UsuarioModificacion).HasMaxLength(120);
            entity.HasIndex(x => new { x.TipoDocumento, x.NumeroDocumento })
                .IsUnique()
                .HasDatabaseName("UX_Cliente_Tipo_Numero");
            entity.HasIndex(x => x.NombreCompleto);
        });

        builder.Entity<Comprobante>(entity =>
        {
            entity.ToTable("Comprobante");
            entity.HasKey(x => x.ComprobanteId);
            entity.Property(x => x.Serie).HasMaxLength(10).IsRequired();
            entity.Property(x => x.MotivoNotaCredito).HasMaxLength(500);
            entity.Property(x => x.MotivoAnulacion).HasMaxLength(500);
            entity.Property(x => x.Direccion).HasMaxLength(500);
            entity.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Igv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TotalPagado).HasColumnType("decimal(18,2)");
            entity.Property(x => x.SaldoPendiente).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MontoDetraccion).HasColumnType("decimal(18,2)");
            entity.Property(x => x.DocumentoImpreso).HasDefaultValue(false);
            entity.Property(x => x.NubefactRespuesta).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.TipoComprobante });
            entity.HasIndex(x => new { x.EmpresaId, x.Serie, x.Correlativo }).IsUnique();
            entity.HasIndex(x => new { x.EmpresaId, x.FechaEmision });
            entity.HasIndex(x => new { x.EmpresaId, x.EstadoSunat });
            entity.HasIndex(x => new { x.EmpresaId, x.CotizacionId });
            entity.HasIndex(x => new { x.EmpresaId, x.NotaPedidoId });
            entity.HasIndex(x => new { x.EmpresaId, x.ComprobanteReferenciaId });
            entity.HasOne(x => x.Empresa).WithMany(x => x.Comprobantes).HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Cotizacion).WithMany().HasForeignKey(x => x.CotizacionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.NotaPedido).WithMany(x => x.Comprobantes).HasForeignKey(x => x.NotaPedidoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ComprobanteReferencia).WithMany().HasForeignKey(x => x.ComprobanteReferenciaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.MotivoNotaCreditoCatalogo).WithMany().HasForeignKey(x => x.MotivoNotaCreditoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MotivoNotaCredito>(entity =>
        {
            entity.ToTable("MotivoNotaCredito");
            entity.HasKey(x => x.MotivoNotaCreditoId);
            entity.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
            entity.HasIndex(x => x.Nombre).IsUnique();
            entity.HasData(
                new MotivoNotaCredito { MotivoNotaCreditoId = 1, Nombre = "Anulacion de la operacion", Estado = EstadoRegistro.Activo },
                new MotivoNotaCredito { MotivoNotaCreditoId = 2, Nombre = "Error en datos del comprobante", Estado = EstadoRegistro.Activo },
                new MotivoNotaCredito { MotivoNotaCreditoId = 3, Nombre = "Devolucion total", Estado = EstadoRegistro.Activo },
                new MotivoNotaCredito { MotivoNotaCreditoId = 4, Nombre = "Descuento posterior", Estado = EstadoRegistro.Activo });
        });

        builder.Entity<ComprobanteDetalle>(entity =>
        {
            entity.ToTable("ComprobanteDetalle");
            entity.HasKey(x => x.ComprobanteDetalleId);
            entity.Property(x => x.Cantidad).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Importe).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ImporteIgv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MontoDetraccion).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.Comprobante).WithMany(x => x.Detalles).HasForeignKey(x => x.ComprobanteId);
            entity.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Moneda>(entity =>
        {
            entity.ToTable("Moneda");
            entity.HasKey(x => x.MonedaId);
            entity.Property(x => x.Codigo).HasMaxLength(3).IsRequired();
            entity.HasIndex(x => x.Codigo).IsUnique();
            entity.HasData(new Moneda { MonedaId = 1, Codigo = "PEN", Descripcion = "Soles", Simbolo = "S/", Estado = EstadoRegistro.Activo });
        });

        builder.Entity<NubefactOperacion>(entity =>
        {
            entity.ToTable("NubefactOperacion");
            entity.HasKey(x => x.NubefactOperacionId);
            entity.Property(x => x.SolicitudJson).HasColumnType("nvarchar(max)");
            entity.Property(x => x.RespuestaCompleta).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => x.EmpresaId);
            entity.HasOne(x => x.Comprobante).WithMany().HasForeignKey(x => x.ComprobanteId);
        });

        builder.Entity<ConfiguracionEmpresa>(entity =>
        {
            entity.ToTable("ConfiguracionEmpresa");
            entity.HasKey(x => x.ConfiguracionEmpresaId);
            entity.Property(x => x.Clave).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Valor).HasMaxLength(1000);
            entity.Property(x => x.Descripcion).HasMaxLength(250);
            entity.HasIndex(x => new { x.EmpresaId, x.Clave }).IsUnique();
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
        });

        builder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("Proveedor");
            entity.HasKey(x => x.ProveedorId);
            entity.Property(x => x.NumeroDocumento).HasMaxLength(20).IsRequired();
            entity.Property(x => x.RazonSocial).HasMaxLength(250).IsRequired();
            entity.Property(x => x.NombreComercial).HasMaxLength(250);
            entity.Property(x => x.Direccion).HasMaxLength(500);
            entity.Property(x => x.Telefono).HasMaxLength(40);
            entity.Property(x => x.Email).HasMaxLength(120);
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.NumeroDocumento });
            entity.HasIndex(x => new { x.EmpresaId, x.RazonSocial });
            entity.HasOne(x => x.Empresa).WithMany(x => x.Proveedores).HasForeignKey(x => x.EmpresaId);
        });

        builder.Entity<Compra>(entity =>
        {
            entity.ToTable("Compra");
            entity.HasKey(x => x.CompraId);
            entity.Ignore(x => x.FechaEmision);
            entity.Property(x => x.Documento).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Serie).HasMaxLength(20);
            entity.Property(x => x.Numero).HasMaxLength(30);
            entity.Property(x => x.Observacion).HasMaxLength(500);
            entity.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Igv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TotalPagado).HasColumnType("decimal(18,2)");
            entity.Property(x => x.SaldoPendiente).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MotivoAnulacion).HasMaxLength(500);
            entity.Property(x => x.UsuarioAnulacion).HasMaxLength(120);
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.Fecha });
            entity.HasIndex(x => new { x.EmpresaId, x.ProveedorId });
            entity.HasIndex(x => new { x.EmpresaId, x.ProveedorId, x.TipoDocumento, x.Serie, x.Numero });
            entity.HasOne(x => x.Empresa).WithMany(x => x.Compras).HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CompraDetalle>(entity =>
        {
            entity.ToTable("CompraDetalle");
            entity.HasKey(x => x.CompraDetalleId);
            entity.Property(x => x.UnidadMedida).HasMaxLength(20);
            entity.Property(x => x.Cantidad).HasColumnType("decimal(18,2)");
            entity.Property(x => x.CostoUnitario).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Importe).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Igv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TotalLinea).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.Compra).WithMany(x => x.Detalles).HasForeignKey(x => x.CompraId);
            entity.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PagoProveedor>(entity =>
        {
            entity.ToTable("PagoProveedor");
            entity.HasKey(x => x.PagoProveedorId);
            entity.Property(x => x.Monto).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MedioPago).HasMaxLength(80);
            entity.Property(x => x.Observacion).HasMaxLength(500);
            entity.Property(x => x.MotivoAnulacion).HasMaxLength(500);
            entity.Property(x => x.UsuarioAnulacion).HasMaxLength(120);
            entity.HasIndex(x => new { x.EmpresaId, x.ProveedorId, x.FechaPago });
            entity.HasIndex(x => new { x.EmpresaId, x.CompraId });
            entity.HasIndex(x => new { x.EmpresaId, x.CuentaFinancieraId });
            entity.HasOne(x => x.CuentaFinanciera).WithMany().HasForeignKey(x => x.CuentaFinancieraId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Compra).WithMany(x => x.Pagos).HasForeignKey(x => x.CompraId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MovimientoInventario>(entity =>
        {
            entity.ToTable("MovimientoInventario");
            entity.HasKey(x => x.MovimientoInventarioId);
            entity.Property(x => x.Cantidad).HasColumnType("decimal(18,2)");
            entity.Property(x => x.StockAnterior).HasColumnType("decimal(18,2)");
            entity.Property(x => x.StockNuevo).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Referencia).HasMaxLength(120);
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.ProductoId, x.Fecha });
            entity.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CuentaFinanciera>(entity =>
        {
            entity.ToTable("CuentaFinanciera");
            entity.HasKey(x => x.CuentaFinancieraId);
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Banco).HasMaxLength(120);
            entity.Property(x => x.NumeroCuenta).HasMaxLength(80);
            entity.Property(x => x.Moneda).HasMaxLength(3).IsRequired();
            entity.Property(x => x.SaldoInicial).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Activo).HasDefaultValue(true);
            entity.Property(x => x.UsuarioModificacion).HasMaxLength(120);
            entity.HasIndex(x => new { x.EmpresaId, x.Nombre }).IsUnique();
            entity.HasIndex(x => new { x.EmpresaId, x.Tipo });
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
        });

        builder.Entity<TransferenciaFinanciera>(entity =>
        {
            entity.ToTable("TransferenciaFinanciera");
            entity.HasKey(x => x.TransferenciaFinancieraId);
            entity.Property(x => x.Monto).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Observacion).HasMaxLength(500);
            entity.Property(x => x.MotivoAnulacion).HasMaxLength(500);
            entity.Property(x => x.UsuarioAnulacion).HasMaxLength(120);
            entity.HasIndex(x => new { x.EmpresaId, x.Fecha });
            entity.HasIndex(x => new { x.EmpresaId, x.CuentaOrigenId });
            entity.HasIndex(x => new { x.EmpresaId, x.CuentaDestinoId });
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.CuentaOrigen).WithMany().HasForeignKey(x => x.CuentaOrigenId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CuentaDestino).WithMany().HasForeignKey(x => x.CuentaDestinoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.MovimientoEgreso).WithMany().HasForeignKey(x => x.MovimientoEgresoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.MovimientoIngreso).WithMany().HasForeignKey(x => x.MovimientoIngresoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Gasto>(entity =>
        {
            entity.ToTable("Gasto");
            entity.HasKey(x => x.GastoId);
            entity.Property(x => x.Categoria).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Importe).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MedioPago).HasMaxLength(80);
            entity.Property(x => x.Observacion).HasMaxLength(500);
            entity.Property(x => x.MotivoAnulacion).HasMaxLength(500);
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.Fecha });
            entity.HasIndex(x => new { x.EmpresaId, x.CuentaFinancieraId });
            entity.HasOne(x => x.Empresa).WithMany(x => x.Gastos).HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.CategoriaGasto).WithMany().HasForeignKey(x => x.CategoriaGastoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.MovimientoCaja).WithMany().HasForeignKey(x => x.MovimientoCajaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CuentaFinanciera).WithMany().HasForeignKey(x => x.CuentaFinancieraId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Ingreso>(entity =>
        {
            entity.ToTable("Ingreso");
            entity.HasKey(x => x.IngresoId);
            entity.Property(x => x.TipoIngreso).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Importe).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MedioPago).HasMaxLength(80);
            entity.Property(x => x.Observacion).HasMaxLength(500);
            entity.Property(x => x.MotivoAnulacion).HasMaxLength(500);
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.Fecha });
            entity.HasIndex(x => new { x.EmpresaId, x.CuentaFinancieraId });
            entity.HasOne(x => x.Empresa).WithMany(x => x.Ingresos).HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.CategoriaIngreso).WithMany().HasForeignKey(x => x.CategoriaIngresoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.MovimientoCaja).WithMany().HasForeignKey(x => x.MovimientoCajaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CuentaFinanciera).WithMany().HasForeignKey(x => x.CuentaFinancieraId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CategoriaGasto>(entity =>
        {
            entity.ToTable("CategoriaGasto");
            entity.HasKey(x => x.CategoriaGastoId);
            entity.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => new { x.EmpresaId, x.Nombre }).IsUnique();
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
        });

        builder.Entity<CategoriaIngreso>(entity =>
        {
            entity.ToTable("CategoriaIngreso");
            entity.HasKey(x => x.CategoriaIngresoId);
            entity.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => new { x.EmpresaId, x.Nombre }).IsUnique();
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
        });

        builder.Entity<NotaPedido>(entity =>
        {
            entity.ToTable("NotaPedido");
            entity.HasKey(x => x.NotaPedidoId);
            entity.Property(x => x.Serie).HasMaxLength(10).IsRequired();
            entity.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Igv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TotalCobrado).HasColumnType("decimal(18,2)");
            entity.Property(x => x.SaldoPendiente).HasColumnType("decimal(18,2)");
            entity.Property(x => x.UsuarioModificacion).HasMaxLength(120);
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.Serie, x.Correlativo }).IsUnique();
            entity.HasIndex(x => new { x.EmpresaId, x.ClienteId, x.Fecha });
            entity.HasIndex(x => new { x.EmpresaId, x.CotizacionId });
            entity.HasIndex(x => new { x.EmpresaId, x.ComprobanteId });
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Cotizacion).WithMany().HasForeignKey(x => x.CotizacionId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<NotaPedidoDetalle>(entity =>
        {
            entity.ToTable("NotaPedidoDetalle");
            entity.HasKey(x => x.NotaPedidoDetalleId);
            entity.Property(x => x.Cantidad).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Igv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.NotaPedido).WithMany(x => x.Detalles).HasForeignKey(x => x.NotaPedidoId);
            entity.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CobroCliente>(entity =>
        {
            entity.ToTable("CobroCliente");
            entity.HasKey(x => x.CobroClienteId);
            entity.Property(x => x.Monto).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MedioPago).HasMaxLength(80);
            entity.Property(x => x.Observacion).HasMaxLength(500);
            entity.Property(x => x.UsuarioAnulacion).HasMaxLength(120);
            entity.Property(x => x.MotivoAnulacion).HasMaxLength(500);
            entity.HasIndex(x => new { x.EmpresaId, x.ClienteId, x.FechaCobro });
            entity.HasIndex(x => new { x.EmpresaId, x.NotaPedidoId });
            entity.HasIndex(x => new { x.EmpresaId, x.ComprobanteId });
            entity.HasIndex(x => new { x.EmpresaId, x.CuentaFinancieraId });
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.NotaPedido).WithMany(x => x.Cobros).HasForeignKey(x => x.NotaPedidoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Comprobante).WithMany(x => x.Cobros).HasForeignKey(x => x.ComprobanteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CuentaFinanciera).WithMany().HasForeignKey(x => x.CuentaFinancieraId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MovimientoCaja>(entity =>
        {
            entity.ToTable("MovimientoCaja");
            entity.HasKey(x => x.MovimientoCajaId);
            entity.Property(x => x.Monto).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MedioPago).HasMaxLength(80);
            entity.Property(x => x.Descripcion).HasMaxLength(500);
            entity.HasIndex(x => new { x.EmpresaId, x.Fecha });
            entity.HasIndex(x => new { x.EmpresaId, x.Origen, x.OrigenId });
            entity.HasIndex(x => new { x.EmpresaId, x.ClienteId });
            entity.HasIndex(x => new { x.EmpresaId, x.ProveedorId });
            entity.HasIndex(x => new { x.EmpresaId, x.CuentaFinancieraId });
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.CuentaFinanciera).WithMany(x => x.MovimientosCaja).HasForeignKey(x => x.CuentaFinancieraId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ComprobanteCobroAplicado>(entity =>
        {
            entity.ToTable("ComprobanteCobroAplicado");
            entity.HasKey(x => x.ComprobanteCobroAplicadoId);
            entity.Property(x => x.MontoAplicado).HasColumnType("decimal(18,2)");
            entity.Property(x => x.UsuarioRegistro).HasMaxLength(120);
            entity.HasIndex(x => new { x.EmpresaId, x.ComprobanteId });
            entity.HasIndex(x => new { x.EmpresaId, x.CobroClienteId });
            entity.HasOne(x => x.Comprobante).WithMany(x => x.CobrosAplicados).HasForeignKey(x => x.ComprobanteId);
            entity.HasOne(x => x.CobroCliente).WithMany(x => x.Aplicaciones).HasForeignKey(x => x.CobroClienteId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Devolucion>(entity =>
        {
            entity.ToTable("Devolucion");
            entity.HasKey(x => x.DevolucionId);
            entity.Property(x => x.MontoOriginal).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MontoDevuelto).HasColumnType("decimal(18,2)");
            entity.Property(x => x.MontoPendiente).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Observacion).HasMaxLength(500);
            entity.Property(x => x.MotivoGeneracion).HasMaxLength(500);
            entity.Property(x => x.UsuarioModificacion).HasMaxLength(120);
            entity.HasIndex(x => new { x.EmpresaId, x.FechaGeneracion });
            entity.HasIndex(x => new { x.EmpresaId, x.TipoTercero });
            entity.HasIndex(x => new { x.EmpresaId, x.ClienteId });
            entity.HasIndex(x => new { x.EmpresaId, x.ProveedorId });
            entity.HasIndex(x => new { x.EmpresaId, x.NotaPedidoId });
            entity.HasIndex(x => new { x.EmpresaId, x.ComprobanteId });
            entity.HasIndex(x => new { x.EmpresaId, x.NotaCreditoId });
            entity.HasIndex(x => new { x.EmpresaId, x.CompraId });
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.NotaPedido).WithMany().HasForeignKey(x => x.NotaPedidoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Comprobante).WithMany().HasForeignKey(x => x.ComprobanteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.NotaCredito).WithMany().HasForeignKey(x => x.NotaCreditoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Compra).WithMany().HasForeignKey(x => x.CompraId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ErrorAplicacion>(entity =>
        {
            entity.ToTable("ErrorAplicacion");
            entity.HasKey(x => x.ErrorAplicacionId);
            entity.Property(x => x.Usuario).HasMaxLength(150);
            entity.Property(x => x.Ruta).HasMaxLength(500);
            entity.Property(x => x.MetodoHttp).HasMaxLength(10);
            entity.Property(x => x.TipoExcepcion).HasMaxLength(300);
            entity.Property(x => x.Mensaje).HasMaxLength(2000);
            entity.Property(x => x.Identificador).HasMaxLength(120);
            entity.Property(x => x.UsuarioRevision).HasMaxLength(150);
            entity.Property(x => x.ObservacionRevision).HasMaxLength(1000);
            entity.HasIndex(x => new { x.EmpresaId, x.FechaUtc });
            entity.HasIndex(x => new { x.EmpresaId, x.Estado });
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Cotizacion>(entity =>
        {
            entity.ToTable("Cotizacion");
            entity.HasKey(x => x.CotizacionId);
            entity.Property(x => x.Serie).HasMaxLength(10).IsRequired();
            entity.Property(x => x.Direccion).HasMaxLength(500);
            entity.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Igv).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PdfUrl).HasMaxLength(500);
            entity.Property(x => x.UsuarioModificacion).HasMaxLength(120);
            entity.HasIndex(x => x.EmpresaId);
            entity.HasIndex(x => new { x.EmpresaId, x.Serie, x.Correlativo }).IsUnique();
            entity.HasIndex(x => new { x.EmpresaId, x.ClienteId, x.FechaEmision });
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId);
            entity.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CotizacionDetalle>(entity =>
        {
            entity.ToTable("CotizacionDetalle");
            entity.HasKey(x => x.CotizacionDetalleId);
            entity.Property(x => x.Cantidad).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Importe).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ImporteIgv).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.Cotizacion).WithMany(x => x.Detalles).HasForeignKey(x => x.CotizacionId);
            entity.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static Permiso[] BuildPermisos()
    {
        var permisos = new List<Permiso>();
        var id = 1;

        foreach (var permiso in PermissionCatalog.All())
        {
            permisos.Add(new Permiso
            {
                PermisoId = id++,
                Modulo = permiso.Module,
                Accion = permiso.Action,
                Descripcion = $"{permiso.Action} {permiso.Module}",
                Estado = EstadoRegistro.Activo
            });
        }

        return permisos.ToArray();
    }

    private static RolPermiso[] BuildRolPermisos()
    {
        var permisosBase = BuildPermisos();
        var permisosAdmin = permisosBase.Select((permiso, index) => new RolPermiso
        {
            RolPermisoId = index + 1,
            RolId = 1,
            PermisoId = permiso.PermisoId
        });

        var modulosVendedor = new[] { "Home", "Categorias", "Productos", "Clientes", "Cotizaciones", "Comprobantes", "NotasCredito", "NotasPedido", "CobrosClientes", "Devoluciones", "Caja", "TESORERIA", "TESORERIA_CAJA", "TESORERIA_CAJABANCOS", "TESORERIA_COBROS", "TESORERIA_TRANSFERENCIAS", "TESORERIA_CUENTASCLIENTES" };
        var accionesVendedor = new[] { "Ver", "Crear", "Editar", "Anular", "Imprimir", "Convertir", "Registrar", "RegistrarPago" };
        var permisos = permisosBase
            .Where(x => modulosVendedor.Contains(x.Modulo) && accionesVendedor.Contains(x.Accion))
            .Select((permiso, index) => new RolPermiso
            {
                RolPermisoId = permisosBase.Length + index + 1,
                RolId = 2,
                PermisoId = permiso.PermisoId
            });

        return permisosAdmin.Concat(permisos).ToArray();
    }
}




