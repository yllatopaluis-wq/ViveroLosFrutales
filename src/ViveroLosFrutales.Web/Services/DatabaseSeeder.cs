using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;
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
IF OBJECT_ID('dbo.Empresa', 'U') IS NOT NULL AND COL_LENGTH('dbo.Empresa', 'Direccion') IS NULL
BEGIN
    ALTER TABLE dbo.Empresa ADD Direccion nvarchar(max) NOT NULL CONSTRAINT DF_Empresa_Direccion DEFAULT N'';
END;
IF OBJECT_ID('dbo.Empresa', 'U') IS NOT NULL AND COL_LENGTH('dbo.Empresa', 'LogoPath') IS NULL
BEGIN
    ALTER TABLE dbo.Empresa ADD LogoPath nvarchar(500) NOT NULL CONSTRAINT DF_Empresa_LogoPath DEFAULT N'';
END;
IF OBJECT_ID('dbo.Empresa', 'U') IS NOT NULL AND COL_LENGTH('dbo.Empresa', 'SerieNotaCredito') IS NULL
BEGIN
    ALTER TABLE dbo.Empresa ADD SerieNotaCredito nvarchar(10) NOT NULL CONSTRAINT DF_Empresa_SerieNotaCredito DEFAULT N'NC001';
END;
IF OBJECT_ID('dbo.Empresa', 'U') IS NOT NULL AND COL_LENGTH('dbo.Empresa', 'SerieNotaCreditoFactura') IS NULL
BEGIN
    ALTER TABLE dbo.Empresa ADD SerieNotaCreditoFactura nvarchar(10) NOT NULL CONSTRAINT DF_Empresa_SerieNotaCreditoFactura DEFAULT N'F101';
END;
IF OBJECT_ID('dbo.Empresa', 'U') IS NOT NULL AND COL_LENGTH('dbo.Empresa', 'SerieNotaCreditoBoleta') IS NULL
BEGIN
    ALTER TABLE dbo.Empresa ADD SerieNotaCreditoBoleta nvarchar(10) NOT NULL CONSTRAINT DF_Empresa_SerieNotaCreditoBoleta DEFAULT N'B101';
END;
IF OBJECT_ID('dbo.Cliente', 'U') IS NOT NULL AND COL_LENGTH('dbo.Cliente', 'Direccion') IS NULL
BEGIN
    ALTER TABLE dbo.Cliente ADD Direccion nvarchar(max) NOT NULL CONSTRAINT DF_Cliente_Direccion DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'Direccion') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD Direccion nvarchar(500) NOT NULL CONSTRAINT DF_Comprobante_Direccion DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'EmpresaRazonSocial') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD EmpresaRazonSocial nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaRazonSocial DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'EmpresaNombreComercial') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD EmpresaNombreComercial nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaNombreComercial DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'EmpresaRuc') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD EmpresaRuc nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaRuc DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'EmpresaDireccion') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD EmpresaDireccion nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaDireccion DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'EmpresaTelefono') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD EmpresaTelefono nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaTelefono DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'EmpresaEmail') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD EmpresaEmail nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_EmpresaEmail DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'CondicionesVenta') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD CondicionesVenta nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_CondicionesVenta DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'CaracteristicasTecnicas') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD CaracteristicasTecnicas nvarchar(max) NOT NULL CONSTRAINT DF_Comprobante_CaracteristicasTecnicas DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'DocumentoImpreso') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD DocumentoImpreso bit NOT NULL CONSTRAINT DF_Comprobante_DocumentoImpreso DEFAULT CAST(0 AS bit);
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'DocumentoImpreso') IS NOT NULL
BEGIN
    UPDATE dbo.Comprobante
    SET DocumentoImpreso = CAST(1 AS bit)
    WHERE DocumentoImpreso = CAST(0 AS bit)
      AND NULLIF(LTRIM(RTRIM(PdfUrl)), N'') IS NOT NULL;
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'ComprobanteReferenciaId') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD ComprobanteReferenciaId int NULL;
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'MotivoNotaCredito') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD MotivoNotaCredito nvarchar(500) NOT NULL CONSTRAINT DF_Comprobante_MotivoNotaCredito DEFAULT N'';
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'MotivoAnulacion') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_Comprobante_MotivoAnulacion DEFAULT N'';
END;
IF OBJECT_ID('dbo.MotivoNotaCredito', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MotivoNotaCredito (
        MotivoNotaCreditoId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_MotivoNotaCredito PRIMARY KEY,
        Nombre nvarchar(150) NOT NULL,
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_MotivoNotaCredito_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_MotivoNotaCredito_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_MotivoNotaCredito_Estado DEFAULT 1
    );
    CREATE UNIQUE INDEX UX_MotivoNotaCredito_Nombre ON dbo.MotivoNotaCredito(Nombre);
END;
IF OBJECT_ID('dbo.MotivoNotaCredito', 'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.MotivoNotaCredito (Nombre, UsuarioRegistro, Estado)
    SELECT v.Nombre, N'system', 1
    FROM (VALUES (N'Anulacion de la operacion'), (N'Error en datos del comprobante'), (N'Devolucion total'), (N'Descuento posterior')) v(Nombre)
    WHERE NOT EXISTS (SELECT 1 FROM dbo.MotivoNotaCredito m WHERE m.Nombre = v.Nombre);
END;
IF OBJECT_ID('dbo.Comprobante', 'U') IS NOT NULL AND COL_LENGTH('dbo.Comprobante', 'MotivoNotaCreditoId') IS NULL
BEGIN
    ALTER TABLE dbo.Comprobante ADD MotivoNotaCreditoId int NULL;
END;
IF OBJECT_ID('dbo.Devolucion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Devolucion (
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
    CREATE INDEX IX_Devolucion_Empresa_Fecha ON dbo.Devolucion(EmpresaId, FechaGeneracion);
    CREATE INDEX IX_Devolucion_Empresa_TipoTercero ON dbo.Devolucion(EmpresaId, TipoTercero);
    CREATE INDEX IX_Devolucion_Empresa_Cliente ON dbo.Devolucion(EmpresaId, ClienteId);
    CREATE INDEX IX_Devolucion_Empresa_Proveedor ON dbo.Devolucion(EmpresaId, ProveedorId);
    CREATE INDEX IX_Devolucion_Empresa_NotaPedido ON dbo.Devolucion(EmpresaId, NotaPedidoId);
    CREATE INDEX IX_Devolucion_Empresa_Comprobante ON dbo.Devolucion(EmpresaId, ComprobanteId);
    CREATE INDEX IX_Devolucion_Empresa_NotaCredito ON dbo.Devolucion(EmpresaId, NotaCreditoId);
    CREATE INDEX IX_Devolucion_Empresa_Compra ON dbo.Devolucion(EmpresaId, CompraId);
END;
IF OBJECT_ID('dbo.NubefactOperacion', 'U') IS NOT NULL AND COL_LENGTH('dbo.NubefactOperacion', 'SolicitudJson') IS NULL
BEGIN
    ALTER TABLE dbo.NubefactOperacion ADD SolicitudJson nvarchar(max) NOT NULL CONSTRAINT DF_NubefactOperacion_SolicitudJson DEFAULT N'';
END;");
        await db.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID('dbo.Compra', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Compra', 'TipoDocumento') IS NULL ALTER TABLE dbo.Compra ADD TipoDocumento int NOT NULL CONSTRAINT DF_Compra_TipoDocumento DEFAULT 1;
    IF COL_LENGTH('dbo.Compra', 'Serie') IS NULL ALTER TABLE dbo.Compra ADD Serie nvarchar(20) NOT NULL CONSTRAINT DF_Compra_Serie DEFAULT N'';
    IF COL_LENGTH('dbo.Compra', 'Numero') IS NULL ALTER TABLE dbo.Compra ADD Numero nvarchar(30) NOT NULL CONSTRAINT DF_Compra_Numero DEFAULT N'';
    IF COL_LENGTH('dbo.Compra', 'FormaPago') IS NULL ALTER TABLE dbo.Compra ADD FormaPago int NOT NULL CONSTRAINT DF_Compra_FormaPago DEFAULT 2;
    IF COL_LENGTH('dbo.Compra', 'Observacion') IS NULL ALTER TABLE dbo.Compra ADD Observacion nvarchar(500) NOT NULL CONSTRAINT DF_Compra_Observacion DEFAULT N'';
    IF COL_LENGTH('dbo.Compra', 'TotalPagado') IS NULL ALTER TABLE dbo.Compra ADD TotalPagado decimal(18,2) NOT NULL CONSTRAINT DF_Compra_TotalPagado DEFAULT 0;
    IF COL_LENGTH('dbo.Compra', 'SaldoPendiente') IS NULL ALTER TABLE dbo.Compra ADD SaldoPendiente decimal(18,2) NOT NULL CONSTRAINT DF_Compra_SaldoPendiente DEFAULT 0;
    IF COL_LENGTH('dbo.Compra', 'EstadoPago') IS NULL ALTER TABLE dbo.Compra ADD EstadoPago int NOT NULL CONSTRAINT DF_Compra_EstadoPago DEFAULT 1;
    IF COL_LENGTH('dbo.Compra', 'EstadoDocumento') IS NULL ALTER TABLE dbo.Compra ADD EstadoDocumento int NOT NULL CONSTRAINT DF_Compra_EstadoDocumento DEFAULT 1;
    IF COL_LENGTH('dbo.Compra', 'FechaAnulacion') IS NULL ALTER TABLE dbo.Compra ADD FechaAnulacion datetime2 NULL;
    IF COL_LENGTH('dbo.Compra', 'MotivoAnulacion') IS NULL ALTER TABLE dbo.Compra ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_Compra_MotivoAnulacion DEFAULT N'';
    IF COL_LENGTH('dbo.Compra', 'UsuarioAnulacion') IS NULL ALTER TABLE dbo.Compra ADD UsuarioAnulacion nvarchar(120) NOT NULL CONSTRAINT DF_Compra_UsuarioAnulacion DEFAULT N'';
    EXEC sp_executesql N'UPDATE dbo.Compra SET SaldoPendiente = CASE WHEN SaldoPendiente = 0 AND TotalPagado = 0 THEN Total ELSE SaldoPendiente END WHERE EstadoPago = 1;';
END;
IF OBJECT_ID('dbo.CompraDetalle', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.CompraDetalle', 'UnidadMedida') IS NULL ALTER TABLE dbo.CompraDetalle ADD UnidadMedida nvarchar(20) NOT NULL CONSTRAINT DF_CompraDetalle_UnidadMedida DEFAULT N'';
    IF COL_LENGTH('dbo.CompraDetalle', 'Igv') IS NULL ALTER TABLE dbo.CompraDetalle ADD Igv decimal(18,2) NOT NULL CONSTRAINT DF_CompraDetalle_Igv DEFAULT 0;
    IF COL_LENGTH('dbo.CompraDetalle', 'TotalLinea') IS NULL ALTER TABLE dbo.CompraDetalle ADD TotalLinea decimal(18,2) NOT NULL CONSTRAINT DF_CompraDetalle_TotalLinea DEFAULT 0;
    EXEC sp_executesql N'UPDATE dbo.CompraDetalle SET TotalLinea = Importe + Igv WHERE TotalLinea = 0;';
END;
IF OBJECT_ID('dbo.PagoProveedor', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PagoProveedor (
        PagoProveedorId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PagoProveedor PRIMARY KEY,
        EmpresaId int NOT NULL,
        ProveedorId int NOT NULL,
        CompraId int NOT NULL,
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
    CREATE INDEX IX_PagoProveedor_Empresa_Proveedor_Fecha ON dbo.PagoProveedor(EmpresaId, ProveedorId, FechaPago);
    CREATE INDEX IX_PagoProveedor_Empresa_Compra ON dbo.PagoProveedor(EmpresaId, CompraId);
END;
IF OBJECT_ID('dbo.MovimientoCaja', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.MovimientoCaja', 'ClienteId') IS NULL ALTER TABLE dbo.MovimientoCaja ADD ClienteId int NULL;
    IF COL_LENGTH('dbo.MovimientoCaja', 'ProveedorId') IS NULL ALTER TABLE dbo.MovimientoCaja ADD ProveedorId int NULL;
END;
IF OBJECT_ID('dbo.CategoriaGasto', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CategoriaGasto (
        CategoriaGastoId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CategoriaGasto PRIMARY KEY,
        EmpresaId int NOT NULL,
        Nombre nvarchar(100) NOT NULL,
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_CategoriaGasto_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_CategoriaGasto_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_CategoriaGasto_Estado DEFAULT 1,
        CONSTRAINT FK_CategoriaGasto_Empresa FOREIGN KEY (EmpresaId) REFERENCES dbo.Empresa(EmpresaId)
    );
    CREATE UNIQUE INDEX UX_CategoriaGasto_Empresa_Nombre ON dbo.CategoriaGasto(EmpresaId, Nombre);
END;
IF OBJECT_ID('dbo.CategoriaIngreso', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CategoriaIngreso (
        CategoriaIngresoId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CategoriaIngreso PRIMARY KEY,
        EmpresaId int NOT NULL,
        Nombre nvarchar(100) NOT NULL,
        FechaRegistro datetime2 NOT NULL CONSTRAINT DF_CategoriaIngreso_FechaRegistro DEFAULT SYSUTCDATETIME(),
        UsuarioRegistro nvarchar(max) NOT NULL CONSTRAINT DF_CategoriaIngreso_UsuarioRegistro DEFAULT N'system',
        Estado int NOT NULL CONSTRAINT DF_CategoriaIngreso_Estado DEFAULT 1,
        CONSTRAINT FK_CategoriaIngreso_Empresa FOREIGN KEY (EmpresaId) REFERENCES dbo.Empresa(EmpresaId)
    );
    CREATE UNIQUE INDEX UX_CategoriaIngreso_Empresa_Nombre ON dbo.CategoriaIngreso(EmpresaId, Nombre);
END;
IF OBJECT_ID('dbo.Gasto', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Gasto', 'CategoriaGastoId') IS NULL ALTER TABLE dbo.Gasto ADD CategoriaGastoId int NULL;
    IF COL_LENGTH('dbo.Gasto', 'MovimientoCajaId') IS NULL ALTER TABLE dbo.Gasto ADD MovimientoCajaId int NULL;
    IF COL_LENGTH('dbo.Gasto', 'MotivoAnulacion') IS NULL ALTER TABLE dbo.Gasto ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_Gasto_MotivoAnulacion DEFAULT N'';
    IF COL_LENGTH('dbo.Gasto', 'FechaAnulacion') IS NULL ALTER TABLE dbo.Gasto ADD FechaAnulacion datetime2 NULL;
END;
IF OBJECT_ID('dbo.Ingreso', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Ingreso', 'CategoriaIngresoId') IS NULL ALTER TABLE dbo.Ingreso ADD CategoriaIngresoId int NULL;
    IF COL_LENGTH('dbo.Ingreso', 'MedioPago') IS NULL ALTER TABLE dbo.Ingreso ADD MedioPago nvarchar(80) NOT NULL CONSTRAINT DF_Ingreso_MedioPago DEFAULT N'EFECTIVO';
    IF COL_LENGTH('dbo.Ingreso', 'MovimientoCajaId') IS NULL ALTER TABLE dbo.Ingreso ADD MovimientoCajaId int NULL;
    IF COL_LENGTH('dbo.Ingreso', 'MotivoAnulacion') IS NULL ALTER TABLE dbo.Ingreso ADD MotivoAnulacion nvarchar(500) NOT NULL CONSTRAINT DF_Ingreso_MotivoAnulacion DEFAULT N'';
    IF COL_LENGTH('dbo.Ingreso', 'FechaAnulacion') IS NULL ALTER TABLE dbo.Ingreso ADD FechaAnulacion datetime2 NULL;
END;");

        await db.Database.ExecuteSqlRawAsync(@"
DECLARE @CategoriasGasto TABLE (Nombre nvarchar(100) NOT NULL);
INSERT INTO @CategoriasGasto (Nombre) VALUES
(N'MOVILIDAD'), (N'COMBUSTIBLE'), (N'LUZ'), (N'ALQUILER'), (N'INTERNET'), (N'MANTENIMIENTO'),
(N'HERRAMIENTAS MENORES'), (N'VIATICOS'), (N'COMISION'), (N'CAMPO PALTA'), (N'PAGO PERSONAL'), (N'PAPELERIA');
INSERT INTO dbo.CategoriaGasto (EmpresaId, Nombre, FechaRegistro, UsuarioRegistro, Estado)
SELECT e.EmpresaId, c.Nombre, SYSUTCDATETIME(), N'system', 1
FROM dbo.Empresa e
CROSS JOIN @CategoriasGasto c
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.CategoriaGasto x WHERE x.EmpresaId = e.EmpresaId AND x.Nombre = c.Nombre
);

DECLARE @CategoriasIngreso TABLE (Nombre nvarchar(100) NOT NULL);
INSERT INTO @CategoriasIngreso (Nombre) VALUES
(N'PRESTAMO RECIBIDO'), (N'APORTE DE SOCIOS'), (N'ALQUILER'), (N'EMBALAJE'), (N'FLETE');
INSERT INTO dbo.CategoriaIngreso (EmpresaId, Nombre, FechaRegistro, UsuarioRegistro, Estado)
SELECT e.EmpresaId, c.Nombre, SYSUTCDATETIME(), N'system', 1
FROM dbo.Empresa e
CROSS JOIN @CategoriasIngreso c
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.CategoriaIngreso x WHERE x.EmpresaId = e.EmpresaId AND x.Nombre = c.Nombre
);

IF OBJECT_ID('dbo.MovimientoCaja', 'U') IS NOT NULL AND OBJECT_ID('dbo.Gasto', 'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.MovimientoCaja (EmpresaId, TipoMovimiento, Origen, OrigenId, Fecha, Monto, MedioPago, Descripcion, Estado, FechaRegistro, UsuarioRegistro)
    SELECT g.EmpresaId, 2, 3, g.GastoId, g.Fecha, g.Importe, g.MedioPago, g.Descripcion, g.Estado, SYSUTCDATETIME(), N'backfill'
    FROM dbo.Gasto g
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.MovimientoCaja m WHERE m.EmpresaId = g.EmpresaId AND m.Origen = 3 AND m.OrigenId = g.GastoId
    );

    UPDATE g
    SET MovimientoCajaId = m.MovimientoCajaId
    FROM dbo.Gasto g
    INNER JOIN dbo.MovimientoCaja m ON m.EmpresaId = g.EmpresaId AND m.Origen = 3 AND m.OrigenId = g.GastoId
    WHERE g.MovimientoCajaId IS NULL;
END;

IF OBJECT_ID('dbo.MovimientoCaja', 'U') IS NOT NULL AND OBJECT_ID('dbo.Ingreso', 'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.MovimientoCaja (EmpresaId, TipoMovimiento, Origen, OrigenId, Fecha, Monto, MedioPago, Descripcion, Estado, FechaRegistro, UsuarioRegistro)
    SELECT i.EmpresaId, 1, 6, i.IngresoId, i.Fecha, i.Importe, i.MedioPago, i.Descripcion, i.Estado, SYSUTCDATETIME(), N'backfill'
    FROM dbo.Ingreso i
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.MovimientoCaja m WHERE m.EmpresaId = i.EmpresaId AND m.Origen = 6 AND m.OrigenId = i.IngresoId
    );

    UPDATE i
    SET MovimientoCajaId = m.MovimientoCajaId
    FROM dbo.Ingreso i
    INNER JOIN dbo.MovimientoCaja m ON m.EmpresaId = i.EmpresaId AND m.Origen = 6 AND m.OrigenId = i.IngresoId
    WHERE i.MovimientoCajaId IS NULL;
END;");
    }

    private static async Task EnsurePermissionsAsync(ApplicationDbContext db)
    {
        var modulos = new[] { "Empresas", "Usuarios", "Roles", "Categorias", "Productos", "Clientes", "Proveedores", "Compras", "Cotizaciones", "Comprobantes", "NotasCredito", "NotasPedido", "CobrosClientes", "Devoluciones", "Caja", "EstadoCuentaClientes", "Gastos", "Ingresos", "Reportes", "Configuracion", "NubefactLogs" };
        var acciones = new[] { "Ver", "Crear", "Editar", "Anular", "Imprimir", "Configurar", "Convertir" };

        var existentes = await db.Permisos
            .Select(x => new { x.Modulo, x.Accion })
            .ToListAsync();
        var existentesSet = existentes.Select(x => $"{x.Modulo}|{x.Accion}").ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var modulo in modulos)
        {
            foreach (var accion in acciones)
            {
                if (existentesSet.Contains($"{modulo}|{accion}")) continue;

                db.Permisos.Add(new Permiso
                {
                    Modulo = modulo,
                    Accion = accion,
                    Descripcion = $"{accion} {modulo}",
                    Estado = EstadoRegistro.Activo
                });
            }
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
            var modulosVendedor = new[] { "Categorias", "Productos", "Clientes", "Cotizaciones", "Comprobantes", "NotasCredito", "NotasPedido", "CobrosClientes", "Devoluciones" };
            var accionesVendedor = new[] { "Ver", "Crear", "Editar", "Anular", "Imprimir", "Convertir" };
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
