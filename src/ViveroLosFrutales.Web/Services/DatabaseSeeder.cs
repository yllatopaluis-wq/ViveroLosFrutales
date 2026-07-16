using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
using ViveroLosFrutales.Domain.Security;
using ViveroLosFrutales.Infrastructure.Data;
using ViveroLosFrutales.Infrastructure.Identity;

namespace ViveroLosFrutales.Web.Services;

public static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var migrations = db.Database.GetMigrations();
        if (migrations.Any())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }

        await EnsureDatabaseSchemaAsync(db);
        await db.EnsureIdentitySchemaAsync();
        await EnsurePermissionsAsync(db);

        var empresa = await db.Empresas.FirstOrDefaultAsync();
        if (empresa is null)
        {
            empresa = new Empresa
            {
                RUC = configuration["Seed:Empresa:RUC"] ?? "00000000000",
                RazonSocial = configuration["Seed:Empresa:RazonSocial"] ?? "Vivero Los Frutales",
                NombreComercial = configuration["Seed:Empresa:NombreComercial"] ?? "ViveroLosFrutales",
                Direccion = string.Empty,
                Telefono = string.Empty,
                Email = configuration["Seed:Admin:Email"] ?? "admin@viverolosfrutales.local",
                MonedaPredeterminada = "PEN",
                Estado = EstadoRegistro.Activo
            };
            db.Empresas.Add(empresa);
            await db.SaveChangesAsync();
        }

        var adminUser = configuration["Seed:Admin:User"] ?? "admin";
        var adminEmail = configuration["Seed:Admin:Email"] ?? "admin@viverolosfrutales.local";
        var adminPassword = configuration["Seed:Admin:Password"] ?? "Admin1234";

        var user = await userManager.FindByNameAsync(adminUser);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = adminUser,
                Email = adminEmail,
                Nombres = "Administrador",
                Apellidos = "Sistema",
                RolId = 1,
                Activo = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, adminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(" ", result.Errors.Select(x => x.Description)));
            }
        }

        var tieneEmpresa = await db.UsuarioEmpresas.AnyAsync(x => x.UsuarioId == user.Id && x.EmpresaId == empresa.EmpresaId);
        if (!tieneEmpresa)
        {
            db.UsuarioEmpresas.Add(new UsuarioEmpresa { UsuarioId = user.Id, EmpresaId = empresa.EmpresaId });
            await db.SaveChangesAsync();
        }
    }

    private static async Task EnsureDatabaseSchemaAsync(ApplicationDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');
IF OBJECT_ID('erp.Producto', 'U') IS NOT NULL AND COL_LENGTH('erp.Producto', 'PrecioCompra') IS NULL
BEGIN
    ALTER TABLE erp.Producto ADD PrecioCompra decimal(18,2) NOT NULL CONSTRAINT DF_Producto_PrecioCompra DEFAULT 0;
END;
IF OBJECT_ID('erp.CuentaFinanciera', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.CuentaFinanciera', 'FechaModificacion') IS NULL
        ALTER TABLE erp.CuentaFinanciera ADD FechaModificacion datetime2 NULL;
    IF COL_LENGTH('erp.CuentaFinanciera', 'UsuarioModificacion') IS NULL
        ALTER TABLE erp.CuentaFinanciera ADD UsuarioModificacion nvarchar(120) NOT NULL CONSTRAINT DF_CuentaFinanciera_UsuarioModificacion DEFAULT N'';
END;
IF OBJECT_ID('erp.TransferenciaFinanciera', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.TransferenciaFinanciera', 'FechaAnulacion') IS NULL
        ALTER TABLE erp.TransferenciaFinanciera ADD FechaAnulacion datetime2 NULL;
    IF COL_LENGTH('erp.TransferenciaFinanciera', 'MotivoAnulacion') IS NULL
        ALTER TABLE erp.TransferenciaFinanciera ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_TransferenciaFinanciera_MotivoAnulacion DEFAULT N'';
    IF COL_LENGTH('erp.TransferenciaFinanciera', 'UsuarioAnulacion') IS NULL
        ALTER TABLE erp.TransferenciaFinanciera ADD UsuarioAnulacion nvarchar(120) NOT NULL CONSTRAINT DF_TransferenciaFinanciera_UsuarioAnulacion DEFAULT N'';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'Direccion') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD Direccion nvarchar(max) NOT NULL CONSTRAINT DF_Empresa_Direccion DEFAULT N'';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'LogoPath') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD LogoPath nvarchar(500) NOT NULL CONSTRAINT DF_Empresa_LogoPath DEFAULT N'';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'RepresentanteLegalNombre') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD RepresentanteLegalNombre nvarchar(200) NOT NULL CONSTRAINT DF_Empresa_RepresentanteLegalNombre DEFAULT N'';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'RepresentanteLegalDocumento') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD RepresentanteLegalDocumento nvarchar(20) NOT NULL CONSTRAINT DF_Empresa_RepresentanteLegalDocumento DEFAULT N'';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'RepresentanteLegalCargo') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD RepresentanteLegalCargo nvarchar(120) NOT NULL CONSTRAINT DF_Empresa_RepresentanteLegalCargo DEFAULT N'';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'FirmaContenido') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD FirmaContenido varbinary(max) NULL;
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'FirmaContentType') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD FirmaContentType nvarchar(120) NOT NULL CONSTRAINT DF_Empresa_FirmaContentType DEFAULT N'';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'FirmaNombre') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD FirmaNombre nvarchar(260) NOT NULL CONSTRAINT DF_Empresa_FirmaNombre DEFAULT N'';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'SerieNotaCredito') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD SerieNotaCredito nvarchar(10) NOT NULL CONSTRAINT DF_Empresa_SerieNotaCredito DEFAULT N'NC001';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'SerieNotaCreditoFactura') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD SerieNotaCreditoFactura nvarchar(10) NOT NULL CONSTRAINT DF_Empresa_SerieNotaCreditoFactura DEFAULT N'F101';
END;
IF OBJECT_ID('erp.Empresa', 'U') IS NOT NULL AND COL_LENGTH('erp.Empresa', 'SerieNotaCreditoBoleta') IS NULL
BEGIN
    ALTER TABLE erp.Empresa ADD SerieNotaCreditoBoleta nvarchar(10) NOT NULL CONSTRAINT DF_Empresa_SerieNotaCreditoBoleta DEFAULT N'B101';
END;
IF OBJECT_ID('erp.Cliente', 'U') IS NOT NULL AND COL_LENGTH('erp.Cliente', 'Direccion') IS NULL
BEGIN
    ALTER TABLE erp.Cliente ADD Direccion nvarchar(max) NOT NULL CONSTRAINT DF_Cliente_Direccion DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'Direccion') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD Direccion nvarchar(500) NOT NULL CONSTRAINT DF_Comprobante_Direccion DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'EmpresaRazonSocial') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD EmpresaRazonSocial nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaRazonSocial DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'EmpresaNombreComercial') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD EmpresaNombreComercial nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaNombreComercial DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'EmpresaRuc') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD EmpresaRuc nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaRuc DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'EmpresaDireccion') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD EmpresaDireccion nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaDireccion DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'EmpresaTelefono') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD EmpresaTelefono nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaTelefono DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'EmpresaEmail') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD EmpresaEmail nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaEmail DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'CondicionesVenta') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD CondicionesVenta nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_CondicionesVenta DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'CaracteristicasTecnicas') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD CaracteristicasTecnicas nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_CaracteristicasTecnicas DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'DocumentoImpreso') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD DocumentoImpreso bit NOT NULL CONSTRAINT DF_Comprobante_DocumentoImpreso DEFAULT CAST(0 AS bit);
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'DocumentoImpreso') IS NOT NULL
BEGIN
    UPDATE erp.Comprobante
    SET DocumentoImpreso = CAST(1 AS bit)
    WHERE DocumentoImpreso = CAST(0 AS bit)
      AND NULLIF(LTRIM(RTRIM(PdfUrl)), N'') IS NOT NULL;
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'ComprobanteReferenciaId') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD ComprobanteReferenciaId int NULL;
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'MotivoNotaCredito') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD MotivoNotaCredito nvarchar(500) NOT NULL CONSTRAINT DF_Comprobante_MotivoNotaCredito DEFAULT N'';
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'MotivoAnulacion') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_Comprobante_MotivoAnulacion DEFAULT N'';
END;
IF OBJECT_ID('erp.MotivoNotaCredito', 'U') IS NULL
BEGIN
    CREATE TABLE erp.MotivoNotaCredito (
        MotivoNotaCreditoId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_MotivoNotaCredito PRIMARY KEY,
        Nombre nvarchar(150) NOT NULL,
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_MotivoNotaCredito_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_MotivoNotaCredito_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_MotivoNotaCredito_Estado DEFAULT 1
    );
    CREATE UNIQUE INDEX UX_MotivoNotaCredito_Nombre ON erp.MotivoNotaCredito(Nombre);
END;
IF OBJECT_ID('erp.MotivoNotaCredito', 'U') IS NOT NULL
BEGIN
    INSERT INTO erp.MotivoNotaCredito (Nombre, UsuarioRegistro, Estado)
    SELECT v.Nombre, N'system', 1
    FROM (VALUES (N'Anulacion de la operacion'), (N'Error en datos del comprobante'), (N'Devolucion total'), (N'Descuento posterior')) v(Nombre)
    WHERE NOT EXISTS (SELECT 1 FROM erp.MotivoNotaCredito m WHERE m.Nombre = v.Nombre);
END;
IF OBJECT_ID('erp.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('erp.Comprobante', 'MotivoNotaCreditoId') IS NULL
BEGIN
    ALTER TABLE erp.Comprobante ADD MotivoNotaCreditoId int NULL;
END;
IF OBJECT_ID('erp.Devolucion', 'U') IS NULL
BEGIN
    CREATE TABLE erp.Devolucion (
        DevolucionId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Devolucion PRIMARY KEY,
        EmpresaId int NOT NULL,
        TipoTercero int NOT NULL,
        ClienteId int NULL,
        ProveedorId int NULL,
        Origen int NOT NULL,
        NotaPedidoId int NULL,
        ComprobanteId int NULL,
        NotaCreditoId int NULL,
        CompraId int NULL,
        FechaGeneracion datetime2 NOT NULL,
        MontoOriginal decimal(18,2) NOT NULL,
        MontoDevuelto decimal(18,2) NOT NULL CONSTRAINT DF_Devolucion_MontoDevuelto DEFAULT 0,
        MontoPendiente decimal(18,2) NOT NULL,
        EstadoDevolucion int NOT NULL,
        Observacion nvarchar(500) NOT NULL CONSTRAINT DF_Devolucion_Observacion DEFAULT N'',
        MotivoGeneracion nvarchar(500) NOT NULL CONSTRAINT DF_Devolucion_MotivoGeneracion DEFAULT N'',
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_Devolucion_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_Devolucion_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_Devolucion_Estado DEFAULT 1,
        FechaModificacion datetime2 NULL,
        UsuarioModificacion nvarchar(120) NOT NULL CONSTRAINT DF_Devolucion_UsuarioModificacion DEFAULT N''
    );
    CREATE INDEX IX_Devolucion_Empresa_Fecha ON erp.Devolucion(EmpresaId, FechaGeneracion);
    CREATE INDEX IX_Devolucion_Empresa_TipoTercero ON erp.Devolucion(EmpresaId, TipoTercero);
    CREATE INDEX IX_Devolucion_Empresa_Cliente ON erp.Devolucion(EmpresaId, ClienteId);
    CREATE INDEX IX_Devolucion_Empresa_Proveedor ON erp.Devolucion(EmpresaId, ProveedorId);
    CREATE INDEX IX_Devolucion_Empresa_NotaPedido ON erp.Devolucion(EmpresaId, NotaPedidoId);
    CREATE INDEX IX_Devolucion_Empresa_Comprobante ON erp.Devolucion(EmpresaId, ComprobanteId);
    CREATE INDEX IX_Devolucion_Empresa_NotaCredito ON erp.Devolucion(EmpresaId, NotaCreditoId);
    CREATE INDEX IX_Devolucion_Empresa_Compra ON erp.Devolucion(EmpresaId, CompraId);
END;
IF OBJECT_ID('erp.NubefactOperacion', 'U') IS NOT NULL AND COL_LENGTH('erp.NubefactOperacion', 'SolicitudJson') IS NULL
BEGIN
    ALTER TABLE erp.NubefactOperacion ADD SolicitudJson nvarchar(max) NOT NULL CONSTRAINT DF_NubefactOperacion_SolicitudJson DEFAULT N'';
END;");
        await db.Database.ExecuteSqlRawAsync(@"
IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');

IF OBJECT_ID('erp.Compra', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.Compra', 'TipoDocumento') IS NULL ALTER TABLE erp.Compra ADD TipoDocumento int NOT NULL CONSTRAINT DF_Compra_TipoDocumento DEFAULT 1;
    IF COL_LENGTH('erp.Compra', 'Serie') IS NULL ALTER TABLE erp.Compra ADD Serie nvarchar(20) NOT NULL CONSTRAINT DF_Compra_Serie DEFAULT N'';
    IF COL_LENGTH('erp.Compra', 'Numero') IS NULL ALTER TABLE erp.Compra ADD Numero nvarchar(30) NOT NULL CONSTRAINT DF_Compra_Numero DEFAULT N'';
    IF COL_LENGTH('erp.Compra', 'FormaPago') IS NULL ALTER TABLE erp.Compra ADD FormaPago int NOT NULL CONSTRAINT DF_Compra_FormaPago DEFAULT 2;
    IF COL_LENGTH('erp.Compra', 'FechaVencimiento') IS NULL ALTER TABLE erp.Compra ADD FechaVencimiento datetime2 NULL;
    IF COL_LENGTH('erp.Compra', 'Moneda') IS NULL ALTER TABLE erp.Compra ADD Moneda nvarchar(20) NOT NULL CONSTRAINT DF_Compra_Moneda DEFAULT N'Soles';
    IF COL_LENGTH('erp.Compra', 'TipoCambio') IS NULL ALTER TABLE erp.Compra ADD TipoCambio decimal(18,4) NOT NULL CONSTRAINT DF_Compra_TipoCambio DEFAULT 1;
    IF COL_LENGTH('erp.Compra', 'DiasCredito') IS NULL ALTER TABLE erp.Compra ADD DiasCredito int NOT NULL CONSTRAINT DF_Compra_DiasCredito DEFAULT 0;
    IF COL_LENGTH('erp.Compra', 'EstadoEntrega') IS NULL ALTER TABLE erp.Compra ADD EstadoEntrega int NOT NULL CONSTRAINT DF_Compra_EstadoEntrega DEFAULT 3;
    IF COL_LENGTH('erp.Compra', 'Observacion') IS NULL ALTER TABLE erp.Compra ADD Observacion nvarchar(500) NOT NULL CONSTRAINT DF_Compra_Observacion DEFAULT N'';
    IF COL_LENGTH('erp.Compra', 'TotalPagado') IS NULL ALTER TABLE erp.Compra ADD TotalPagado decimal(18,2) NOT NULL CONSTRAINT DF_Compra_TotalPagado DEFAULT 0;
    IF COL_LENGTH('erp.Compra', 'SaldoPendiente') IS NULL ALTER TABLE erp.Compra ADD SaldoPendiente decimal(18,2) NOT NULL CONSTRAINT DF_Compra_SaldoPendiente DEFAULT 0;
    IF COL_LENGTH('erp.Compra', 'EstadoPago') IS NULL ALTER TABLE erp.Compra ADD EstadoPago int NOT NULL CONSTRAINT DF_Compra_EstadoPago DEFAULT 1;
    IF COL_LENGTH('erp.Compra', 'EstadoDocumento') IS NULL ALTER TABLE erp.Compra ADD EstadoDocumento int NOT NULL CONSTRAINT DF_Compra_EstadoDocumento DEFAULT 1;
    IF COL_LENGTH('erp.Compra', 'FechaAnulacion') IS NULL ALTER TABLE erp.Compra ADD FechaAnulacion datetime2 NULL;
    IF COL_LENGTH('erp.Compra', 'MotivoAnulacion') IS NULL ALTER TABLE erp.Compra ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_Compra_MotivoAnulacion DEFAULT N'';
    IF COL_LENGTH('erp.Compra', 'UsuarioAnulacion') IS NULL ALTER TABLE erp.Compra ADD UsuarioAnulacion nvarchar(120) NOT NULL CONSTRAINT DF_Compra_UsuarioAnulacion DEFAULT N'';
    EXEC sp_executesql N'UPDATE erp.Compra SET SaldoPendiente = CASE WHEN SaldoPendiente = 0 AND TotalPagado = 0 THEN Total ELSE SaldoPendiente END WHERE EstadoPago = 1;';
END;
IF OBJECT_ID('erp.CompraDetalle', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.CompraDetalle', 'UnidadMedida') IS NULL ALTER TABLE erp.CompraDetalle ADD UnidadMedida nvarchar(20) NOT NULL CONSTRAINT DF_CompraDetalle_UnidadMedida DEFAULT N'';
    IF COL_LENGTH('erp.CompraDetalle', 'CantidadRecibida') IS NULL
    BEGIN
        ALTER TABLE erp.CompraDetalle ADD CantidadRecibida decimal(18,2) NOT NULL CONSTRAINT DF_CompraDetalle_CantidadRecibida DEFAULT 0;
        EXEC sp_executesql N'UPDATE erp.CompraDetalle SET CantidadRecibida = Cantidad;';
    END;
    IF COL_LENGTH('erp.CompraDetalle', 'Igv') IS NULL ALTER TABLE erp.CompraDetalle ADD Igv decimal(18,2) NOT NULL CONSTRAINT DF_CompraDetalle_Igv DEFAULT 0;
    IF COL_LENGTH('erp.CompraDetalle', 'TotalLinea') IS NULL ALTER TABLE erp.CompraDetalle ADD TotalLinea decimal(18,2) NOT NULL CONSTRAINT DF_CompraDetalle_TotalLinea DEFAULT 0;
    EXEC sp_executesql N'UPDATE erp.CompraDetalle SET TotalLinea = Importe + Igv WHERE TotalLinea = 0;';
END;
IF OBJECT_ID('erp.PagoProveedor', 'U') IS NULL
BEGIN
    CREATE TABLE erp.PagoProveedor (
        PagoProveedorId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PagoProveedor PRIMARY KEY,
        EmpresaId int NOT NULL,
        ProveedorId int NOT NULL,
        CompraId int NULL,
        FechaPago datetime2 NOT NULL,
        Monto decimal(18,2) NOT NULL,
        MedioPago nvarchar(80) NOT NULL CONSTRAINT DF_PagoProveedor_MedioPago DEFAULT N'EFECTIVO',
        Observacion nvarchar(500) NOT NULL CONSTRAINT DF_PagoProveedor_Observacion DEFAULT N'',
        EstadoPago int NOT NULL CONSTRAINT DF_PagoProveedor_EstadoPago DEFAULT 1,
        FechaAnulacion datetime2 NULL,
        MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_PagoProveedor_MotivoAnulacion DEFAULT N'',
        UsuarioAnulacion nvarchar(120) NOT NULL CONSTRAINT DF_PagoProveedor_UsuarioAnulacion DEFAULT N'',
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_PagoProveedor_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_PagoProveedor_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_PagoProveedor_Estado DEFAULT 1
    );
    CREATE INDEX IX_PagoProveedor_Empresa_Proveedor_Fecha ON erp.PagoProveedor(EmpresaId, ProveedorId, FechaPago);
    CREATE INDEX IX_PagoProveedor_Empresa_Compra ON erp.PagoProveedor(EmpresaId, CompraId);
END;
IF OBJECT_ID('erp.MovimientoCaja', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.MovimientoCaja', 'ClienteId') IS NULL ALTER TABLE erp.MovimientoCaja ADD ClienteId int NULL;
    IF COL_LENGTH('erp.MovimientoCaja', 'ProveedorId') IS NULL ALTER TABLE erp.MovimientoCaja ADD ProveedorId int NULL;
END;
IF OBJECT_ID('erp.Gasto', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.Gasto', 'DocumentoReferencia') IS NULL ALTER TABLE erp.Gasto ADD DocumentoReferencia nvarchar(120) NOT NULL CONSTRAINT DF_Gasto_DocumentoReferencia DEFAULT N'';
    IF COL_LENGTH('erp.Gasto', 'ProveedorId') IS NULL ALTER TABLE erp.Gasto ADD ProveedorId int NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Gasto_EmpresaId_ProveedorId' AND object_id = OBJECT_ID('erp.Gasto')) CREATE INDEX IX_Gasto_EmpresaId_ProveedorId ON erp.Gasto(EmpresaId, ProveedorId);
END;
IF OBJECT_ID('erp.Ingreso', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.Ingreso', 'DocumentoReferencia') IS NULL ALTER TABLE erp.Ingreso ADD DocumentoReferencia nvarchar(120) NOT NULL CONSTRAINT DF_Ingreso_DocumentoReferencia DEFAULT N'';
    IF COL_LENGTH('erp.Ingreso', 'ClienteId') IS NULL ALTER TABLE erp.Ingreso ADD ClienteId int NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ingreso_EmpresaId_ClienteId' AND object_id = OBJECT_ID('erp.Ingreso')) CREATE INDEX IX_Ingreso_EmpresaId_ClienteId ON erp.Ingreso(EmpresaId, ClienteId);
END;
IF OBJECT_ID('erp.CategoriaGasto', 'U') IS NULL
BEGIN
    CREATE TABLE erp.CategoriaGasto (
        CategoriaGastoId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CategoriaGasto PRIMARY KEY,
        EmpresaId int NOT NULL,
        Nombre nvarchar(100) NOT NULL,
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_CategoriaGasto_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_CategoriaGasto_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_CategoriaGasto_Estado DEFAULT 1,
        CONSTRAINT FK_CategoriaGasto_Empresa FOREIGN KEY (EmpresaId) REFERENCES erp.Empresa(EmpresaId)
    );
    CREATE UNIQUE INDEX UX_CategoriaGasto_Empresa_Nombre ON erp.CategoriaGasto(EmpresaId, Nombre);
END;
IF OBJECT_ID('erp.CategoriaIngreso', 'U') IS NULL
BEGIN
    CREATE TABLE erp.CategoriaIngreso (
        CategoriaIngresoId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CategoriaIngreso PRIMARY KEY,
        EmpresaId int NOT NULL,
        Nombre nvarchar(100) NOT NULL,
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_CategoriaIngreso_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_CategoriaIngreso_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_CategoriaIngreso_Estado DEFAULT 1,
        CONSTRAINT FK_CategoriaIngreso_Empresa FOREIGN KEY (EmpresaId) REFERENCES erp.Empresa(EmpresaId)
    );
    CREATE UNIQUE INDEX UX_CategoriaIngreso_Empresa_Nombre ON erp.CategoriaIngreso(EmpresaId, Nombre);
END;
IF OBJECT_ID('erp.Gasto', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.Gasto', 'DocumentoReferencia') IS NULL ALTER TABLE erp.Gasto ADD DocumentoReferencia nvarchar(120) NOT NULL CONSTRAINT DF_Gasto_DocumentoReferencia DEFAULT N'';
    IF COL_LENGTH('erp.Gasto', 'CategoriaGastoId') IS NULL ALTER TABLE erp.Gasto ADD CategoriaGastoId int NULL;
    IF COL_LENGTH('erp.Gasto', 'MovimientoCajaId') IS NULL ALTER TABLE erp.Gasto ADD MovimientoCajaId int NULL;
    IF COL_LENGTH('erp.Gasto', 'MotivoAnulacion') IS NULL ALTER TABLE erp.Gasto ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_Gasto_MotivoAnulacion DEFAULT N'';
    IF COL_LENGTH('erp.Gasto', 'FechaAnulacion') IS NULL ALTER TABLE erp.Gasto ADD FechaAnulacion datetime2 NULL;
END;
IF OBJECT_ID('erp.Ingreso', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.Ingreso', 'DocumentoReferencia') IS NULL ALTER TABLE erp.Ingreso ADD DocumentoReferencia nvarchar(120) NOT NULL CONSTRAINT DF_Ingreso_DocumentoReferencia DEFAULT N'';
    IF COL_LENGTH('erp.Ingreso', 'CategoriaIngresoId') IS NULL ALTER TABLE erp.Ingreso ADD CategoriaIngresoId int NULL;
    IF COL_LENGTH('erp.Ingreso', 'MedioPago') IS NULL ALTER TABLE erp.Ingreso ADD MedioPago nvarchar(80) NOT NULL CONSTRAINT DF_Ingreso_MedioPago DEFAULT N'EFECTIVO';
    IF COL_LENGTH('erp.Ingreso', 'MovimientoCajaId') IS NULL ALTER TABLE erp.Ingreso ADD MovimientoCajaId int NULL;
    IF COL_LENGTH('erp.Ingreso', 'MotivoAnulacion') IS NULL ALTER TABLE erp.Ingreso ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_Ingreso_MotivoAnulacion DEFAULT N'';
    IF COL_LENGTH('erp.Ingreso', 'FechaAnulacion') IS NULL ALTER TABLE erp.Ingreso ADD FechaAnulacion datetime2 NULL;
END;");

        await db.Database.ExecuteSqlRawAsync(@"
IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');

DECLARE @CategoriasGasto TABLE (Nombre nvarchar(100) NOT NULL);
INSERT INTO @CategoriasGasto (Nombre) VALUES
(N'MOVILIDAD'), (N'COMBUSTIBLE'), (N'LUZ'), (N'ALQUILER'), (N'INTERNET'), (N'MANTENIMIENTO'),
(N'HERRAMIENTAS MENORES'), (N'VIATICOS'), (N'COMISION'), (N'CAMPO PALTA'), (N'PAGO PERSONAL'), (N'PAPELERIA');
INSERT INTO erp.CategoriaGasto (EmpresaId, Nombre, FechaRegistro, UsuarioRegistro, Estado)
SELECT e.EmpresaId, c.Nombre, SYSUTCDATETIME(), N'system', 1
FROM erp.Empresa e
CROSS JOIN @CategoriasGasto c
WHERE NOT EXISTS (
    SELECT 1 FROM erp.CategoriaGasto x WHERE x.EmpresaId = e.EmpresaId AND x.Nombre = c.Nombre
);

DECLARE @CategoriasIngreso TABLE (Nombre nvarchar(100) NOT NULL);
INSERT INTO @CategoriasIngreso (Nombre) VALUES
(N'PRESTAMO RECIBIDO'), (N'APORTE DE SOCIOS'), (N'ALQUILER'), (N'EMBALAJE'), (N'FLETE');
INSERT INTO erp.CategoriaIngreso (EmpresaId, Nombre, FechaRegistro, UsuarioRegistro, Estado)
SELECT e.EmpresaId, c.Nombre, SYSUTCDATETIME(), N'system', 1
FROM erp.Empresa e
CROSS JOIN @CategoriasIngreso c
WHERE NOT EXISTS (
    SELECT 1 FROM erp.CategoriaIngreso x WHERE x.EmpresaId = e.EmpresaId AND x.Nombre = c.Nombre
);

IF OBJECT_ID('erp.MovimientoCaja', 'U') IS NOT NULL AND OBJECT_ID('erp.Gasto', 'U') IS NOT NULL
BEGIN
    INSERT INTO erp.MovimientoCaja (EmpresaId, TipoMovimiento, Origen, OrigenId, Fecha, Monto, MedioPago, Descripcion, Estado, FechaRegistro, UsuarioRegistro)
    SELECT g.EmpresaId, 2, 3, g.GastoId, g.Fecha, g.Importe, g.MedioPago, g.Descripcion, g.Estado, SYSUTCDATETIME(), N'backfill'
    FROM erp.Gasto g
    WHERE NOT EXISTS (
        SELECT 1 FROM erp.MovimientoCaja m WHERE m.EmpresaId = g.EmpresaId AND m.Origen = 3 AND m.OrigenId = g.GastoId
    );

    UPDATE g
    SET MovimientoCajaId = m.MovimientoCajaId
    FROM erp.Gasto g
    INNER JOIN erp.MovimientoCaja m ON m.EmpresaId = g.EmpresaId AND m.Origen = 3 AND m.OrigenId = g.GastoId
    WHERE g.MovimientoCajaId IS NULL;
END;

IF OBJECT_ID('erp.MovimientoCaja', 'U') IS NOT NULL AND OBJECT_ID('erp.Ingreso', 'U') IS NOT NULL
BEGIN
    INSERT INTO erp.MovimientoCaja (EmpresaId, TipoMovimiento, Origen, OrigenId, Fecha, Monto, MedioPago, Descripcion, Estado, FechaRegistro, UsuarioRegistro)
    SELECT i.EmpresaId, 1, 6, i.IngresoId, i.Fecha, i.Importe, i.MedioPago, i.Descripcion, i.Estado, SYSUTCDATETIME(), N'backfill'
    FROM erp.Ingreso i
    WHERE NOT EXISTS (
        SELECT 1 FROM erp.MovimientoCaja m WHERE m.EmpresaId = i.EmpresaId AND m.Origen = 6 AND m.OrigenId = i.IngresoId
    );

    UPDATE i
    SET MovimientoCajaId = m.MovimientoCajaId
    FROM erp.Ingreso i
    INNER JOIN erp.MovimientoCaja m ON m.EmpresaId = i.EmpresaId AND m.Origen = 6 AND m.OrigenId = i.IngresoId
    WHERE i.MovimientoCajaId IS NULL;
END;");

        await db.Database.ExecuteSqlRawAsync(@"
IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');

IF OBJECT_ID('erp.ErrorAplicacion', 'U') IS NULL
BEGIN
    CREATE TABLE erp.ErrorAplicacion (
        ErrorAplicacionId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ErrorAplicacion PRIMARY KEY,
        EmpresaId int NULL,
        FechaUtc datetime2 NOT NULL,
        Usuario nvarchar(150) NOT NULL CONSTRAINT DF_ErrorAplicacion_Usuario DEFAULT N'',
        Ruta nvarchar(500) NOT NULL CONSTRAINT DF_ErrorAplicacion_Ruta DEFAULT N'',
        MetodoHttp nvarchar(10) NOT NULL CONSTRAINT DF_ErrorAplicacion_MetodoHttp DEFAULT N'',
        TipoExcepcion nvarchar(300) NOT NULL CONSTRAINT DF_ErrorAplicacion_TipoExcepcion DEFAULT N'',
        Mensaje nvarchar(2000) NOT NULL CONSTRAINT DF_ErrorAplicacion_Mensaje DEFAULT N'',
        Detalle nvarchar(max) NOT NULL CONSTRAINT DF_ErrorAplicacion_Detalle DEFAULT N'',
        Identificador nvarchar(120) NOT NULL CONSTRAINT DF_ErrorAplicacion_Identificador DEFAULT N'',
        Estado int NOT NULL CONSTRAINT DF_ErrorAplicacion_Estado DEFAULT 1,
        FechaRevisionUtc datetime2 NULL,
        UsuarioRevision nvarchar(150) NOT NULL CONSTRAINT DF_ErrorAplicacion_UsuarioRevision DEFAULT N'',
        ObservacionRevision nvarchar(1000) NOT NULL CONSTRAINT DF_ErrorAplicacion_ObservacionRevision DEFAULT N'',
        CONSTRAINT FK_ErrorAplicacion_Empresa FOREIGN KEY (EmpresaId) REFERENCES erp.Empresa(EmpresaId)
    );
    CREATE INDEX IX_ErrorAplicacion_EmpresaId_FechaUtc ON erp.ErrorAplicacion(EmpresaId, FechaUtc);
    CREATE INDEX IX_ErrorAplicacion_EmpresaId_Estado ON erp.ErrorAplicacion(EmpresaId, Estado);
END;");
        await db.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID('erp.OrdenCompra', 'U') IS NULL
BEGIN
    CREATE TABLE erp.OrdenCompra (
        OrdenCompraId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrdenCompra PRIMARY KEY,
        EmpresaId int NOT NULL,
        ProveedorId int NOT NULL,
        Serie nvarchar(20) NOT NULL,
        Correlativo int NOT NULL,
        Fecha datetime2 NOT NULL,
        Moneda nvarchar(20) NOT NULL CONSTRAINT DF_OrdenCompra_Moneda DEFAULT N'Soles',
        FechaEntregaEsperada datetime2 NULL,
        LugarEntrega nvarchar(250) NOT NULL CONSTRAINT DF_OrdenCompra_LugarEntrega DEFAULT N'',
        FormaPago int NOT NULL CONSTRAINT DF_OrdenCompra_FormaPago DEFAULT 2,
        CondicionPago nvarchar(120) NOT NULL CONSTRAINT DF_OrdenCompra_CondicionPago DEFAULT N'',
        Observacion nvarchar(1000) NOT NULL CONSTRAINT DF_OrdenCompra_Observacion DEFAULT N'',
        CondicionEntrega nvarchar(250) NOT NULL CONSTRAINT DF_OrdenCompra_CondicionEntrega DEFAULT N'',
        Garantia nvarchar(250) NOT NULL CONSTRAINT DF_OrdenCompra_Garantia DEFAULT N'',
        PorcentajeAdelanto decimal(5,2) NOT NULL CONSTRAINT DF_OrdenCompra_PorcentajeAdelanto DEFAULT 0,
        PlazoDias int NOT NULL CONSTRAINT DF_OrdenCompra_PlazoDias DEFAULT 0,
        SubTotal decimal(18,2) NOT NULL,
        Igv decimal(18,2) NOT NULL,
        Total decimal(18,2) NOT NULL,
        TotalFacturado decimal(18,2) NOT NULL CONSTRAINT DF_OrdenCompra_TotalFacturado DEFAULT 0,
        TotalPagado decimal(18,2) NOT NULL CONSTRAINT DF_OrdenCompra_TotalPagado DEFAULT 0,
        TotalAplicado decimal(18,2) NOT NULL CONSTRAINT DF_OrdenCompra_TotalAplicado DEFAULT 0,
        TotalDevuelto decimal(18,2) NOT NULL CONSTRAINT DF_OrdenCompra_TotalDevuelto DEFAULT 0,
        SaldoDisponible decimal(18,2) NOT NULL CONSTRAINT DF_OrdenCompra_SaldoDisponible DEFAULT 0,
        PendienteFacturar decimal(18,2) NOT NULL CONSTRAINT DF_OrdenCompra_PendienteFacturar DEFAULT 0,
        EstadoDocumento int NOT NULL CONSTRAINT DF_OrdenCompra_EstadoDocumento DEFAULT 1,
        EstadoAprobacion int NOT NULL CONSTRAINT DF_OrdenCompra_EstadoAprobacion DEFAULT 3,
        EstadoFacturacion int NOT NULL CONSTRAINT DF_OrdenCompra_EstadoFacturacion DEFAULT 1,
        EstadoRecepcion int NOT NULL CONSTRAINT DF_OrdenCompra_EstadoRecepcion DEFAULT 1,
        EstadoFinanciero int NOT NULL CONSTRAINT DF_OrdenCompra_EstadoFinanciero DEFAULT 1,
        FechaAnulacion datetime2 NULL,
        MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_OrdenCompra_MotivoAnulacion DEFAULT N'',
        UsuarioAnulacion nvarchar(120) NOT NULL CONSTRAINT DF_OrdenCompra_UsuarioAnulacion DEFAULT N'',
        ProveedorTipoDocumento nvarchar(30) NOT NULL CONSTRAINT DF_OrdenCompra_ProveedorTipoDocumento DEFAULT N'',
        ProveedorNumeroDocumento nvarchar(20) NOT NULL CONSTRAINT DF_OrdenCompra_ProveedorNumeroDocumento DEFAULT N'',
        ProveedorRazonSocial nvarchar(250) NOT NULL CONSTRAINT DF_OrdenCompra_ProveedorRazonSocial DEFAULT N'',
        ProveedorNombreComercial nvarchar(250) NOT NULL CONSTRAINT DF_OrdenCompra_ProveedorNombreComercial DEFAULT N'',
        ProveedorDireccion nvarchar(500) NOT NULL CONSTRAINT DF_OrdenCompra_ProveedorDireccion DEFAULT N'',
        ProveedorTelefono nvarchar(40) NOT NULL CONSTRAINT DF_OrdenCompra_ProveedorTelefono DEFAULT N'',
        ProveedorEmail nvarchar(120) NOT NULL CONSTRAINT DF_OrdenCompra_ProveedorEmail DEFAULT N'',
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_OrdenCompra_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_OrdenCompra_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_OrdenCompra_Estado DEFAULT 1,
        RowVersion rowversion NOT NULL
    );
    CREATE UNIQUE INDEX UX_OrdenCompra_Empresa_Serie_Correlativo ON erp.OrdenCompra(EmpresaId, Serie, Correlativo);
    CREATE INDEX IX_OrdenCompra_Empresa_Proveedor ON erp.OrdenCompra(EmpresaId, ProveedorId);
END;
IF OBJECT_ID('erp.OrdenCompra', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.OrdenCompra', 'CondicionPago') IS NULL ALTER TABLE erp.OrdenCompra ADD CondicionPago nvarchar(120) NOT NULL CONSTRAINT DF_OrdenCompra_CondicionPago DEFAULT N'';
    IF COL_LENGTH('erp.OrdenCompra', 'Garantia') IS NULL ALTER TABLE erp.OrdenCompra ADD Garantia nvarchar(250) NOT NULL CONSTRAINT DF_OrdenCompra_Garantia DEFAULT N'';
END;
IF OBJECT_ID('erp.OrdenCompraDetalle', 'U') IS NULL
BEGIN
    CREATE TABLE erp.OrdenCompraDetalle (
        OrdenCompraDetalleId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrdenCompraDetalle PRIMARY KEY,
        OrdenCompraId int NOT NULL,
        ProductoId int NOT NULL,
        UnidadMedida nvarchar(20) NOT NULL CONSTRAINT DF_OrdenCompraDetalle_Unidad DEFAULT N'',
        Cantidad decimal(18,2) NOT NULL,
        CostoUnitario decimal(18,2) NOT NULL,
        Subtotal decimal(18,2) NOT NULL,
        Igv decimal(18,2) NOT NULL,
        Total decimal(18,2) NOT NULL,
        Orden int NOT NULL CONSTRAINT DF_OrdenCompraDetalle_Orden DEFAULT 0,
        CantidadFacturada decimal(18,2) NOT NULL CONSTRAINT DF_OrdenCompraDetalle_CantidadFacturada DEFAULT 0,
        CantidadRecibida decimal(18,2) NOT NULL CONSTRAINT DF_OrdenCompraDetalle_CantidadRecibida DEFAULT 0
    );
    CREATE INDEX IX_OrdenCompraDetalle_OrdenCompra ON erp.OrdenCompraDetalle(OrdenCompraId);
END;
IF OBJECT_ID('erp.Compra', 'U') IS NOT NULL AND COL_LENGTH('erp.Compra', 'OrdenCompraId') IS NULL
    ALTER TABLE erp.Compra ADD OrdenCompraId int NULL;
IF OBJECT_ID('erp.PagoProveedor', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.PagoProveedor', 'OrdenCompraId') IS NULL ALTER TABLE erp.PagoProveedor ADD OrdenCompraId int NULL;
    IF EXISTS (
        SELECT 1
        FROM sys.columns c
        INNER JOIN sys.objects o ON o.object_id = c.object_id
        INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
        WHERE s.name = 'erp' AND o.name = 'PagoProveedor' AND c.name = 'CompraId' AND c.is_nullable = 0
    )
    BEGIN
        ALTER TABLE erp.PagoProveedor ALTER COLUMN CompraId int NULL;
    END
END;
IF OBJECT_ID('erp.PagoProveedorAplicacion', 'U') IS NULL
BEGIN
    CREATE TABLE erp.PagoProveedorAplicacion (
        PagoProveedorAplicacionId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PagoProveedorAplicacion PRIMARY KEY,
        EmpresaId int NOT NULL,
        PagoProveedorId int NOT NULL,
        CompraId int NULL,
        MontoAplicado decimal(18,2) NOT NULL,
        FechaAplicacion datetime2 NOT NULL,
        Estado int NOT NULL CONSTRAINT DF_PagoProveedorAplicacion_EstadoAplicacion DEFAULT 1,
        MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_PagoProveedorAplicacion_Motivo DEFAULT N'',
        FechaAnulacion datetime2 NULL,
        UsuarioRegistro nvarchar(120) NOT NULL CONSTRAINT DF_PagoProveedorAplicacion_UsuarioRegistro DEFAULT N'',
        UsuarioAnulacion nvarchar(120) NOT NULL CONSTRAINT DF_PagoProveedorAplicacion_UsuarioAnulacion DEFAULT N'',
        RowVersion rowversion NOT NULL
    );
    CREATE INDEX IX_PagoProveedorAplicacion_Empresa_Pago ON erp.PagoProveedorAplicacion(EmpresaId, PagoProveedorId);
    CREATE INDEX IX_PagoProveedorAplicacion_Empresa_Compra ON erp.PagoProveedorAplicacion(EmpresaId, CompraId);
END;
IF OBJECT_ID('erp.Devolucion', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('erp.Devolucion', 'OrdenCompraId') IS NULL ALTER TABLE erp.Devolucion ADD OrdenCompraId int NULL;
    IF COL_LENGTH('erp.Devolucion', 'PagoProveedorId') IS NULL ALTER TABLE erp.Devolucion ADD PagoProveedorId int NULL;
END;");


    }
    private static async Task EnsurePermissionsAsync(ApplicationDbContext db)
    {
        var catalogo = PermissionCatalog.All().ToArray();
        var catalogoSet = catalogo
            .Select(x => $"{x.Module}|{x.Action}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existentes = await db.Permisos
            .Include(x => x.RolPermisos)
            .ToListAsync();

        var obsoletos = existentes
            .Where(x => !catalogoSet.Contains($"{x.Modulo}|{x.Accion}"))
            .ToList();
        if (obsoletos.Count > 0)
        {
            db.RolPermisos.RemoveRange(obsoletos.SelectMany(x => x.RolPermisos));
            db.Permisos.RemoveRange(obsoletos);
        }

        var existentesSet = existentes
            .Except(obsoletos)
            .Select(x => $"{x.Modulo}|{x.Accion}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var permiso in catalogo.Where(x => !existentesSet.Contains($"{x.Module}|{x.Action}")))
        {
            db.Permisos.Add(new Permiso
            {
                Modulo = permiso.Module,
                Accion = permiso.Action,
                Descripcion = $"{permiso.Action} {permiso.Module}",
                Estado = EstadoRegistro.Activo
            });
        }

        await db.SaveChangesAsync();

        var admin = await db.RolesNegocio.FirstOrDefaultAsync(x => x.RolId == 1);
        if (admin is not null)
        {
            var permisos = await db.Permisos.Select(x => x.PermisoId).ToListAsync();
            var asignados = await db.RolPermisos.Where(x => x.RolId == admin.RolId).Select(x => x.PermisoId).ToListAsync();
            var asignadosSet = asignados.ToHashSet();

            foreach (var permisoId in permisos.Where(x => !asignadosSet.Contains(x)))
            {
                db.RolPermisos.Add(new RolPermiso { RolId = admin.RolId, PermisoId = permisoId });
            }
        }

        var vendedor = await db.RolesNegocio.FirstOrDefaultAsync(x => x.RolId == 2);
        if (vendedor is not null)
        {
            var modulosVendedor = new[] { "Home", "Categorias", "Productos", "Clientes", "Cotizaciones", "Comprobantes", "NotasCredito", "NotasPedido", "CobrosClientes", "Devoluciones", "Caja", "TESORERIA", "TESORERIA_CAJA", "TESORERIA_CAJABANCOS", "TESORERIA_COBROS", "TESORERIA_PAGOSPROVEEDORES", "TESORERIA_TRANSFERENCIAS", "TESORERIA_CUENTASCLIENTES" };
            var accionesVendedor = new[] { "Ver", "Crear", "Editar", "Anular", "Imprimir", "Convertir", "Registrar", "RegistrarPago" };
            var permisos = await db.Permisos
                .Where(x => modulosVendedor.Contains(x.Modulo) && accionesVendedor.Contains(x.Accion))
                .Select(x => x.PermisoId)
                .ToListAsync();
            var asignados = await db.RolPermisos.Where(x => x.RolId == vendedor.RolId).Select(x => x.PermisoId).ToListAsync();
            var asignadosSet = asignados.ToHashSet();

            foreach (var permisoId in permisos.Where(x => !asignadosSet.Contains(x)))
            {
                db.RolPermisos.Add(new RolPermiso { RolId = vendedor.RolId, PermisoId = permisoId });
            }
        }

        await db.SaveChangesAsync();
    }
}






