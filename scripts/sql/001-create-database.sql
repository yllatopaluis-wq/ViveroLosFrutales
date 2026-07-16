IF OBJECT_ID(N'[erp].[__EFMigrationsHistory]') IS NULL
BEGIN
    IF SCHEMA_ID(N'erp') IS NULL EXEC(N'CREATE SCHEMA [erp];');
    CREATE TABLE [erp].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF SCHEMA_ID(N'erp') IS NULL EXEC(N'CREATE SCHEMA [erp];');
GO

CREATE TABLE [erp].[AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [erp].[AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [Nombres] nvarchar(max) NOT NULL,
    [Apellidos] nvarchar(max) NOT NULL,
    [RolId] int NOT NULL,
    [Activo] bit NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [erp].[Cliente] (
    [ClienteId] int NOT NULL IDENTITY,
    [EmpresaId] int NOT NULL,
    [TipoDocumento] int NOT NULL,
    [NumeroDocumento] nvarchar(20) NOT NULL,
    [NombreCompleto] nvarchar(250) NOT NULL,
    [Email] nvarchar(120) NOT NULL,
    [Direccion] nvarchar(max) NOT NULL,
    [Telefono] nvarchar(max) NOT NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Cliente] PRIMARY KEY ([ClienteId])
);
GO

CREATE TABLE [erp].[Empresa] (
    [EmpresaId] int NOT NULL IDENTITY,
    [RUC] nvarchar(11) NOT NULL,
    [RazonSocial] nvarchar(200) NOT NULL,
    [NombreComercial] nvarchar(200) NOT NULL,
    [Direccion] nvarchar(max) NOT NULL,
    [Telefono] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [MonedaPredeterminada] nvarchar(max) NOT NULL,
    [UrlNubefact] nvarchar(max) NOT NULL,
    [TokenNubefact] nvarchar(1000) NOT NULL,
    [LogoPath] nvarchar(500) NOT NULL,
    [LogoContenido] varbinary(max) NULL,
    [LogoContentType] nvarchar(120) NOT NULL,
    [LogoNombre] nvarchar(260) NOT NULL,
    [RepresentanteLegalNombre] nvarchar(200) NOT NULL DEFAULT N'',
    [RepresentanteLegalDocumento] nvarchar(20) NOT NULL DEFAULT N'',
    [RepresentanteLegalCargo] nvarchar(120) NOT NULL DEFAULT N'',
    [FirmaContenido] varbinary(max) NULL,
    [FirmaContentType] nvarchar(120) NOT NULL DEFAULT N'',
    [FirmaNombre] nvarchar(260) NOT NULL DEFAULT N'',
    [SerieBoleta] nvarchar(max) NOT NULL,
    [SerieFactura] nvarchar(max) NOT NULL,
    [SerieNotaCredito] nvarchar(10) NOT NULL,
    [SerieNotaCreditoFactura] nvarchar(10) NOT NULL,
    [SerieNotaCreditoBoleta] nvarchar(10) NOT NULL,
    [SerieNotaPedido] nvarchar(max) NOT NULL,
    [SerieCotizacion] nvarchar(max) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Empresa] PRIMARY KEY ([EmpresaId])
);
GO


CREATE TABLE [erp].[CuentaFinanciera] (
    [CuentaFinancieraId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(120) NOT NULL,
    [Tipo] int NOT NULL,
    [Banco] nvarchar(120) NOT NULL,
    [NumeroCuenta] nvarchar(80) NOT NULL,
    [Moneda] nvarchar(3) NOT NULL,
    [SaldoInicial] decimal(18,2) NOT NULL,
    [FechaSaldoInicial] datetime2 NOT NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_CuentaFinanciera] PRIMARY KEY ([CuentaFinancieraId]),
    CONSTRAINT [FK_CuentaFinanciera_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[Moneda] (
    [MonedaId] int NOT NULL IDENTITY,
    [Codigo] nvarchar(3) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Simbolo] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Moneda] PRIMARY KEY ([MonedaId])
);
GO

CREATE TABLE [erp].[MotivoNotaCredito] (
    [MotivoNotaCreditoId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(150) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_MotivoNotaCredito] PRIMARY KEY ([MotivoNotaCreditoId])
);
GO

CREATE TABLE [erp].[Permiso] (
    [PermisoId] int NOT NULL IDENTITY,
    [Modulo] nvarchar(80) NOT NULL,
    [Accion] nvarchar(40) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Permiso] PRIMARY KEY ([PermisoId])
);
GO

CREATE TABLE [erp].[Rol] (
    [RolId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(80) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Rol] PRIMARY KEY ([RolId])
);
GO

CREATE TABLE [erp].[AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [erp].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [erp].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [erp].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [erp].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [erp].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [erp].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[Categoria] (
    [CategoriaId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(250) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY ([CategoriaId]),
    CONSTRAINT [FK_Categoria_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[CategoriaGasto] (
    [CategoriaGastoId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_CategoriaGasto] PRIMARY KEY ([CategoriaGastoId]),
    CONSTRAINT [FK_CategoriaGasto_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[CategoriaIngreso] (
    [CategoriaIngresoId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_CategoriaIngreso] PRIMARY KEY ([CategoriaIngresoId]),
    CONSTRAINT [FK_CategoriaIngreso_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[ConfiguracionEmpresa] (
    [ConfiguracionEmpresaId] int NOT NULL IDENTITY,
    [Clave] nvarchar(100) NOT NULL,
    [Valor] nvarchar(1000) NOT NULL,
    [Descripcion] nvarchar(250) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_ConfiguracionEmpresa] PRIMARY KEY ([ConfiguracionEmpresaId]),
    CONSTRAINT [FK_ConfiguracionEmpresa_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[Cotizacion] (
    [CotizacionId] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [Serie] nvarchar(10) NOT NULL,
    [Correlativo] int NOT NULL,
    [FechaEmision] datetime2 NOT NULL,
    [Direccion] nvarchar(500) NOT NULL,
    [FormaPago] int NOT NULL,
    [EmpresaRazonSocial] nvarchar(max) NOT NULL,
    [EmpresaNombreComercial] nvarchar(max) NOT NULL,
    [EmpresaRuc] nvarchar(max) NOT NULL,
    [EmpresaDireccion] nvarchar(max) NOT NULL,
    [EmpresaTelefono] nvarchar(max) NOT NULL,
    [EmpresaEmail] nvarchar(max) NOT NULL,
    [CondicionesVenta] nvarchar(max) NOT NULL,
    [CaracteristicasTecnicas] nvarchar(max) NOT NULL,
    [SubTotal] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [EstadoCotizacion] int NOT NULL,
    [PdfUrl] nvarchar(500) NOT NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Cotizacion] PRIMARY KEY ([CotizacionId]),
    CONSTRAINT [FK_Cotizacion_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Cotizacion_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[ErrorAplicacion] (
    [ErrorAplicacionId] int NOT NULL IDENTITY,
    [EmpresaId] int NULL,
    [FechaUtc] datetime2 NOT NULL,
    [Usuario] nvarchar(150) NOT NULL,
    [Ruta] nvarchar(500) NOT NULL,
    [MetodoHttp] nvarchar(10) NOT NULL,
    [TipoExcepcion] nvarchar(300) NOT NULL,
    [Mensaje] nvarchar(2000) NOT NULL,
    [Detalle] nvarchar(max) NOT NULL,
    [Identificador] nvarchar(120) NOT NULL,
    [Estado] int NOT NULL,
    [FechaRevisionUtc] datetime2 NULL,
    [UsuarioRevision] nvarchar(150) NOT NULL,
    [ObservacionRevision] nvarchar(1000) NOT NULL,
    CONSTRAINT [PK_ErrorAplicacion] PRIMARY KEY ([ErrorAplicacionId]),
    CONSTRAINT [FK_ErrorAplicacion_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[MovimientoCaja] (
    [MovimientoCajaId] int NOT NULL IDENTITY,
    [CuentaFinancieraId] int NULL,
    [ClienteId] int NULL,
    [ProveedorId] int NULL,
    [TipoMovimiento] int NOT NULL,
    [Origen] int NOT NULL,
    [OrigenId] int NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [Monto] decimal(18,2) NOT NULL,
    [MedioPago] nvarchar(80) NOT NULL,
    [Descripcion] nvarchar(500) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_MovimientoCaja] PRIMARY KEY ([MovimientoCajaId]),
    CONSTRAINT [FK_MovimientoCaja_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MovimientoCaja_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO


CREATE TABLE [erp].[TransferenciaFinanciera] (
    [TransferenciaFinancieraId] int NOT NULL IDENTITY,
    [Fecha] datetime2 NOT NULL,
    [CuentaOrigenId] int NOT NULL,
    [CuentaDestinoId] int NOT NULL,
    [Monto] decimal(18,2) NOT NULL,
    [Observacion] nvarchar(500) NOT NULL,
    [MovimientoEgresoId] int NULL,
    [MovimientoIngresoId] int NULL,
    [FechaAnulacion] datetime2 NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [UsuarioAnulacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_TransferenciaFinanciera] PRIMARY KEY ([TransferenciaFinancieraId]),
    CONSTRAINT [FK_TransferenciaFinanciera_CuentaFinanciera_CuentaDestinoId] FOREIGN KEY ([CuentaDestinoId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TransferenciaFinanciera_CuentaFinanciera_CuentaOrigenId] FOREIGN KEY ([CuentaOrigenId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TransferenciaFinanciera_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransferenciaFinanciera_MovimientoCaja_MovimientoEgresoId] FOREIGN KEY ([MovimientoEgresoId]) REFERENCES [erp].[MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TransferenciaFinanciera_MovimientoCaja_MovimientoIngresoId] FOREIGN KEY ([MovimientoIngresoId]) REFERENCES [erp].[MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[Producto] (
    [ProductoId] int NOT NULL IDENTITY,
    [Categoria] nvarchar(100) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [UnidadMedida] nvarchar(20) NOT NULL,
    [Stock] decimal(18,2) NOT NULL,
    [AfectoIgv] bit NOT NULL,
    [PrecioVentaSinIgv] decimal(18,2) NOT NULL,
    [PrecioVentaConIgv] decimal(18,2) NOT NULL,
    [PrecioCompra] decimal(18,2) NOT NULL DEFAULT 0,
    [TieneDetraccion] bit NOT NULL,
    [PorcentajeDetraccion] decimal(5,2) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Producto] PRIMARY KEY ([ProductoId]),
    CONSTRAINT [FK_Producto_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[Proveedor] (
    [ProveedorId] int NOT NULL IDENTITY,
    [TipoDocumento] int NOT NULL,
    [NumeroDocumento] nvarchar(20) NOT NULL,
    [RazonSocial] nvarchar(250) NOT NULL,
    [NombreComercial] nvarchar(250) NOT NULL,
    [Direccion] nvarchar(500) NOT NULL,
    [Telefono] nvarchar(40) NOT NULL,
    [Email] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Proveedor] PRIMARY KEY ([ProveedorId]),
    CONSTRAINT [FK_Proveedor_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

ALTER TABLE [erp].[Cliente] ADD CONSTRAINT [FK_Cliente_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE NO ACTION;
GO

CREATE TABLE [erp].[UsuarioEmpresa] (
    [UsuarioEmpresaId] int NOT NULL IDENTITY,
    [UsuarioId] nvarchar(450) NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_UsuarioEmpresa] PRIMARY KEY ([UsuarioEmpresaId]),
    CONSTRAINT [FK_UsuarioEmpresa_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[RolPermiso] (
    [RolPermisoId] int NOT NULL IDENTITY,
    [RolId] int NOT NULL,
    [PermisoId] int NOT NULL,
    CONSTRAINT [PK_RolPermiso] PRIMARY KEY ([RolPermisoId]),
    CONSTRAINT [FK_RolPermiso_Permiso_PermisoId] FOREIGN KEY ([PermisoId]) REFERENCES [erp].[Permiso] ([PermisoId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolPermiso_Rol_RolId] FOREIGN KEY ([RolId]) REFERENCES [erp].[Rol] ([RolId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[NotaPedido] (
    [NotaPedidoId] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [CotizacionId] int NULL,
    [ComprobanteId] int NULL,
    [Serie] nvarchar(10) NOT NULL,
    [Correlativo] int NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [FormaPago] int NOT NULL,
    [Observacion] nvarchar(1000) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [TotalCobrado] decimal(18,2) NOT NULL,
    [SaldoPendiente] decimal(18,2) NOT NULL,
    [EstadoDocumento] int NOT NULL,
    [EstadoPago] int NOT NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_NotaPedido] PRIMARY KEY ([NotaPedidoId]),
    CONSTRAINT [FK_NotaPedido_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_NotaPedido_Cotizacion_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [erp].[Cotizacion] ([CotizacionId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_NotaPedido_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[Gasto] (
    [GastoId] int NOT NULL IDENTITY,
    [CuentaFinancieraId] int NULL,
    [ProveedorId] int NULL,
    [Fecha] datetime2 NOT NULL,
    [CategoriaGastoId] int NULL,
    [Categoria] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(300) NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    [MedioPago] nvarchar(80) NOT NULL,
    [Observacion] nvarchar(500) NOT NULL,
    [MovimientoCajaId] int NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [FechaAnulacion] datetime2 NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Gasto] PRIMARY KEY ([GastoId]),
    CONSTRAINT [FK_Gasto_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Gasto_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Gasto_CategoriaGasto_CategoriaGastoId] FOREIGN KEY ([CategoriaGastoId]) REFERENCES [erp].[CategoriaGasto] ([CategoriaGastoId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Gasto_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Gasto_MovimientoCaja_MovimientoCajaId] FOREIGN KEY ([MovimientoCajaId]) REFERENCES [erp].[MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[Ingreso] (
    [IngresoId] int NOT NULL IDENTITY,
    [CuentaFinancieraId] int NULL,
    [ClienteId] int NULL,
    [Fecha] datetime2 NOT NULL,
    [CategoriaIngresoId] int NULL,
    [TipoIngreso] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(300) NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    [MedioPago] nvarchar(80) NOT NULL,
    [Observacion] nvarchar(500) NOT NULL,
    [MovimientoCajaId] int NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [FechaAnulacion] datetime2 NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Ingreso] PRIMARY KEY ([IngresoId]),
    CONSTRAINT [FK_Ingreso_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ingreso_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ingreso_CategoriaIngreso_CategoriaIngresoId] FOREIGN KEY ([CategoriaIngresoId]) REFERENCES [erp].[CategoriaIngreso] ([CategoriaIngresoId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ingreso_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Ingreso_MovimientoCaja_MovimientoCajaId] FOREIGN KEY ([MovimientoCajaId]) REFERENCES [erp].[MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[CotizacionDetalle] (
    [CotizacionDetalleId] int NOT NULL IDENTITY,
    [CotizacionId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    [ImporteIgv] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_CotizacionDetalle] PRIMARY KEY ([CotizacionDetalleId]),
    CONSTRAINT [FK_CotizacionDetalle_Cotizacion_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [erp].[Cotizacion] ([CotizacionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CotizacionDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [erp].[Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[MovimientoInventario] (
    [MovimientoInventarioId] int NOT NULL IDENTITY,
    [ProductoId] int NOT NULL,
    [Tipo] int NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [StockAnterior] decimal(18,2) NOT NULL,
    [StockNuevo] decimal(18,2) NOT NULL,
    [Referencia] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_MovimientoInventario] PRIMARY KEY ([MovimientoInventarioId]),
    CONSTRAINT [FK_MovimientoInventario_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [erp].[Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[Compra] (
    [CompraId] int NOT NULL IDENTITY,
    [ProveedorId] int NOT NULL,
    [TipoDocumento] int NOT NULL,
    [Serie] nvarchar(20) NOT NULL,
    [Numero] nvarchar(30) NOT NULL,
    [Documento] nvarchar(40) NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [FormaPago] int NOT NULL,
    [Observacion] nvarchar(500) NOT NULL,
    [SubTotal] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [TotalPagado] decimal(18,2) NOT NULL,
    [SaldoPendiente] decimal(18,2) NOT NULL,
    [EstadoPago] int NOT NULL,
    [EstadoDocumento] int NOT NULL,
    [FechaAnulacion] datetime2 NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [UsuarioAnulacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Compra] PRIMARY KEY ([CompraId]),
    CONSTRAINT [FK_Compra_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Compra_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[Comprobante] (
    [ComprobanteId] int NOT NULL IDENTITY,
    [TipoComprobante] int NOT NULL,
    [Serie] nvarchar(10) NOT NULL,
    [Correlativo] int NOT NULL,
    [ClienteId] int NOT NULL,
    [CotizacionId] int NULL,
    [NotaPedidoId] int NULL,
    [ComprobanteReferenciaId] int NULL,
    [MotivoNotaCreditoId] int NULL,
    [MotivoNotaCredito] nvarchar(500) NOT NULL,
    [Direccion] nvarchar(500) NOT NULL,
    [FechaEmision] datetime2 NOT NULL,
    [FormaPago] int NOT NULL,
    [EmpresaRazonSocial] nvarchar(max) NOT NULL,
    [EmpresaNombreComercial] nvarchar(max) NOT NULL,
    [EmpresaRuc] nvarchar(max) NOT NULL,
    [EmpresaDireccion] nvarchar(max) NOT NULL,
    [EmpresaTelefono] nvarchar(max) NOT NULL,
    [EmpresaEmail] nvarchar(max) NOT NULL,
    [CondicionesVenta] nvarchar(max) NOT NULL,
    [CaracteristicasTecnicas] nvarchar(max) NOT NULL,
    [SubTotal] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [TotalPagado] decimal(18,2) NOT NULL,
    [SaldoPendiente] decimal(18,2) NOT NULL,
    [MontoDetraccion] decimal(18,2) NOT NULL,
    [EstadoSunat] int NOT NULL,
    [EstadoPago] int NOT NULL,
    [DocumentoImpreso] bit NOT NULL DEFAULT CAST(0 AS bit),
    [PdfUrl] nvarchar(max) NOT NULL,
    [XmlUrl] nvarchar(max) NOT NULL,
    [NubefactHash] nvarchar(max) NOT NULL,
    [NubefactRespuesta] nvarchar(max) NOT NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Comprobante] PRIMARY KEY ([ComprobanteId]),
    CONSTRAINT [FK_Comprobante_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comprobante_Comprobante_ComprobanteReferenciaId] FOREIGN KEY ([ComprobanteReferenciaId]) REFERENCES [erp].[Comprobante] ([ComprobanteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comprobante_Cotizacion_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [erp].[Cotizacion] ([CotizacionId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comprobante_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comprobante_MotivoNotaCredito_MotivoNotaCreditoId] FOREIGN KEY ([MotivoNotaCreditoId]) REFERENCES [erp].[MotivoNotaCredito] ([MotivoNotaCreditoId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comprobante_NotaPedido_NotaPedidoId] FOREIGN KEY ([NotaPedidoId]) REFERENCES [erp].[NotaPedido] ([NotaPedidoId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[NotaPedidoDetalle] (
    [NotaPedidoDetalleId] int NOT NULL IDENTITY,
    [NotaPedidoId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_NotaPedidoDetalle] PRIMARY KEY ([NotaPedidoDetalleId]),
    CONSTRAINT [FK_NotaPedidoDetalle_NotaPedido_NotaPedidoId] FOREIGN KEY ([NotaPedidoId]) REFERENCES [erp].[NotaPedido] ([NotaPedidoId]) ON DELETE CASCADE,
    CONSTRAINT [FK_NotaPedidoDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [erp].[Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[CompraDetalle] (
    [CompraDetalleId] int NOT NULL IDENTITY,
    [CompraId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [UnidadMedida] nvarchar(20) NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [CostoUnitario] decimal(18,2) NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [TotalLinea] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_CompraDetalle] PRIMARY KEY ([CompraDetalleId]),
    CONSTRAINT [FK_CompraDetalle_Compra_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [erp].[Compra] ([CompraId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompraDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [erp].[Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[PagoProveedor] (
    [PagoProveedorId] int NOT NULL IDENTITY,
    [CuentaFinancieraId] int NULL,
    [ProveedorId] int NOT NULL,
    [CompraId] int NOT NULL,
    [FechaPago] datetime2 NOT NULL,
    [Monto] decimal(18,2) NOT NULL,
    [MedioPago] nvarchar(80) NOT NULL,
    [Observacion] nvarchar(500) NOT NULL,
    [EstadoPago] int NOT NULL,
    [FechaAnulacion] datetime2 NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [UsuarioAnulacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_PagoProveedor] PRIMARY KEY ([PagoProveedorId]),
    CONSTRAINT [FK_PagoProveedor_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PagoProveedor_Compra_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [erp].[Compra] ([CompraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PagoProveedor_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[CobroCliente] (
    [CobroClienteId] int NOT NULL IDENTITY,
    [CuentaFinancieraId] int NULL,
    [ClienteId] int NOT NULL,
    [NotaPedidoId] int NULL,
    [ComprobanteId] int NULL,
    [FechaCobro] datetime2 NOT NULL,
    [Monto] decimal(18,2) NOT NULL,
    [MedioPago] nvarchar(80) NOT NULL,
    [Observacion] nvarchar(500) NOT NULL,
    [Estado] int NOT NULL,
    [FechaAnulacion] datetime2 NULL,
    [UsuarioAnulacion] nvarchar(120) NOT NULL,
    [MotivoAnulacion] nvarchar(500) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_CobroCliente] PRIMARY KEY ([CobroClienteId]),
    CONSTRAINT [FK_CobroCliente_CuentaFinanciera_CuentaFinancieraId] FOREIGN KEY ([CuentaFinancieraId]) REFERENCES [erp].[CuentaFinanciera] ([CuentaFinancieraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CobroCliente_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CobroCliente_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [erp].[Comprobante] ([ComprobanteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CobroCliente_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CobroCliente_NotaPedido_NotaPedidoId] FOREIGN KEY ([NotaPedidoId]) REFERENCES [erp].[NotaPedido] ([NotaPedidoId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[ComprobanteDetalle] (
    [ComprobanteDetalleId] int NOT NULL IDENTITY,
    [ComprobanteId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    [ImporteIgv] decimal(18,2) NOT NULL,
    [MontoDetraccion] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ComprobanteDetalle] PRIMARY KEY ([ComprobanteDetalleId]),
    CONSTRAINT [FK_ComprobanteDetalle_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [erp].[Comprobante] ([ComprobanteId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ComprobanteDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [erp].[Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[Devolucion] (
    [DevolucionId] int NOT NULL IDENTITY,
    [TipoTercero] int NOT NULL,
    [ClienteId] int NULL,
    [ProveedorId] int NULL,
    [Origen] int NOT NULL,
    [NotaPedidoId] int NULL,
    [ComprobanteId] int NULL,
    [NotaCreditoId] int NULL,
    [CompraId] int NULL,
    [FechaGeneracion] datetime2 NOT NULL,
    [MontoOriginal] decimal(18,2) NOT NULL,
    [MontoDevuelto] decimal(18,2) NOT NULL,
    [MontoPendiente] decimal(18,2) NOT NULL,
    [EstadoDevolucion] int NOT NULL,
    [Observacion] nvarchar(500) NOT NULL,
    [MotivoGeneracion] nvarchar(500) NOT NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] nvarchar(120) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Devolucion] PRIMARY KEY ([DevolucionId]),
    CONSTRAINT [FK_Devolucion_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Compra_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [erp].[Compra] ([CompraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [erp].[Comprobante] ([ComprobanteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Comprobante_NotaCreditoId] FOREIGN KEY ([NotaCreditoId]) REFERENCES [erp].[Comprobante] ([ComprobanteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [erp].[Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Devolucion_NotaPedido_NotaPedidoId] FOREIGN KEY ([NotaPedidoId]) REFERENCES [erp].[NotaPedido] ([NotaPedidoId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [erp].[NubefactOperacion] (
    [NubefactOperacionId] int NOT NULL IDENTITY,
    [ComprobanteId] int NOT NULL,
    [TipoOperacion] nvarchar(max) NOT NULL,
    [EstadoSunat] int NOT NULL,
    [PdfUrl] nvarchar(max) NOT NULL,
    [XmlUrl] nvarchar(max) NOT NULL,
    [Hash] nvarchar(max) NOT NULL,
    [SolicitudJson] nvarchar(max) NOT NULL,
    [RespuestaCompleta] nvarchar(max) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_NubefactOperacion] PRIMARY KEY ([NubefactOperacionId]),
    CONSTRAINT [FK_NubefactOperacion_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [erp].[Comprobante] ([ComprobanteId]) ON DELETE CASCADE
);
GO

CREATE TABLE [erp].[ComprobanteCobroAplicado] (
    [ComprobanteCobroAplicadoId] int NOT NULL IDENTITY,
    [EmpresaId] int NOT NULL,
    [ComprobanteId] int NOT NULL,
    [CobroClienteId] int NOT NULL,
    [MontoAplicado] decimal(18,2) NOT NULL,
    [FechaAplicacion] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(120) NOT NULL,
    CONSTRAINT [PK_ComprobanteCobroAplicado] PRIMARY KEY ([ComprobanteCobroAplicadoId]),
    CONSTRAINT [FK_ComprobanteCobroAplicado_CobroCliente_CobroClienteId] FOREIGN KEY ([CobroClienteId]) REFERENCES [erp].[CobroCliente] ([CobroClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ComprobanteCobroAplicado_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [erp].[Comprobante] ([ComprobanteId]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MonedaId', N'Codigo', N'Descripcion', N'Estado', N'Simbolo') AND [object_id] = OBJECT_ID(N'[erp].[Moneda]'))
    SET IDENTITY_INSERT [erp].[Moneda] ON;
INSERT INTO [erp].[Moneda] ([MonedaId], [Codigo], [Descripcion], [Estado], [Simbolo])
VALUES (1, N'PEN', N'Soles', 1, N'S/');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MonedaId', N'Codigo', N'Descripcion', N'Estado', N'Simbolo') AND [object_id] = OBJECT_ID(N'[erp].[Moneda]'))
    SET IDENTITY_INSERT [erp].[Moneda] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MotivoNotaCreditoId', N'Estado', N'FechaRegistro', N'Nombre', N'UsuarioRegistro') AND [object_id] = OBJECT_ID(N'[erp].[MotivoNotaCredito]'))
    SET IDENTITY_INSERT [erp].[MotivoNotaCredito] ON;
INSERT INTO [erp].[MotivoNotaCredito] ([MotivoNotaCreditoId], [Estado], [FechaRegistro], [Nombre], [UsuarioRegistro])
VALUES (1, 1, '2026-06-24T17:55:42.3757293Z', N'Anulacion de la operacion', N''),
(2, 1, '2026-06-24T17:55:42.3757299Z', N'Error en datos del comprobante', N''),
(3, 1, '2026-06-24T17:55:42.3757300Z', N'Devolucion total', N''),
(4, 1, '2026-06-24T17:55:42.3757302Z', N'Descuento posterior', N'');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MotivoNotaCreditoId', N'Estado', N'FechaRegistro', N'Nombre', N'UsuarioRegistro') AND [object_id] = OBJECT_ID(N'[erp].[MotivoNotaCredito]'))
    SET IDENTITY_INSERT [erp].[MotivoNotaCredito] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermisoId', N'Accion', N'Descripcion', N'Estado', N'Modulo') AND [object_id] = OBJECT_ID(N'[erp].[Permiso]'))
    SET IDENTITY_INSERT [erp].[Permiso] ON;
INSERT INTO [erp].[Permiso] ([PermisoId], [Accion], [Descripcion], [Estado], [Modulo])
VALUES (1, N'Ver', N'Ver Home', 1, N'Home'),
(2, N'Ver', N'Ver Cotizaciones', 1, N'Cotizaciones'),
(3, N'Crear', N'Crear Cotizaciones', 1, N'Cotizaciones'),
(4, N'Editar', N'Editar Cotizaciones', 1, N'Cotizaciones'),
(5, N'Anular', N'Anular Cotizaciones', 1, N'Cotizaciones'),
(6, N'Imprimir', N'Imprimir Cotizaciones', 1, N'Cotizaciones'),
(7, N'Convertir', N'Convertir Cotizaciones', 1, N'Cotizaciones'),
(8, N'Ver', N'Ver NotasPedido', 1, N'NotasPedido'),
(9, N'Crear', N'Crear NotasPedido', 1, N'NotasPedido'),
(10, N'Editar', N'Editar NotasPedido', 1, N'NotasPedido'),
(11, N'Anular', N'Anular NotasPedido', 1, N'NotasPedido'),
(12, N'Imprimir', N'Imprimir NotasPedido', 1, N'NotasPedido'),
(13, N'Convertir', N'Convertir NotasPedido', 1, N'NotasPedido'),
(14, N'RegistrarPago', N'RegistrarPago NotasPedido', 1, N'NotasPedido'),
(15, N'Ver', N'Ver Comprobantes', 1, N'Comprobantes'),
(16, N'Crear', N'Crear Comprobantes', 1, N'Comprobantes'),
(17, N'Editar', N'Editar Comprobantes', 1, N'Comprobantes'),
(18, N'Anular', N'Anular Comprobantes', 1, N'Comprobantes'),
(19, N'Imprimir', N'Imprimir Comprobantes', 1, N'Comprobantes'),
(20, N'RegistrarPago', N'RegistrarPago Comprobantes', 1, N'Comprobantes'),
(21, N'ConsultarSunat', N'ConsultarSunat Comprobantes', 1, N'Comprobantes'),
(22, N'Ver', N'Ver NotasCredito', 1, N'NotasCredito'),
(23, N'Crear', N'Crear NotasCredito', 1, N'NotasCredito'),
(24, N'Anular', N'Anular NotasCredito', 1, N'NotasCredito'),
(25, N'Imprimir', N'Imprimir NotasCredito', 1, N'NotasCredito'),
(26, N'Ver', N'Ver CobrosClientes', 1, N'CobrosClientes'),
(27, N'Crear', N'Crear CobrosClientes', 1, N'CobrosClientes'),
(28, N'Anular', N'Anular CobrosClientes', 1, N'CobrosClientes'),
(85, N'Ver', N'Ver TESORERIA', 1, N'TESORERIA'),
(86, N'Ver', N'Ver TESORERIA_CAJA', 1, N'TESORERIA_CAJA'),
(87, N'Ver', N'Ver TESORERIA_CAJABANCOS', 1, N'TESORERIA_CAJABANCOS'),
(88, N'Ver', N'Ver TESORERIA_CUENTASFINANCIERAS', 1, N'TESORERIA_CUENTASFINANCIERAS'),
(89, N'Crear', N'Crear TESORERIA_CUENTASFINANCIERAS', 1, N'TESORERIA_CUENTASFINANCIERAS'),
(90, N'Editar', N'Editar TESORERIA_CUENTASFINANCIERAS', 1, N'TESORERIA_CUENTASFINANCIERAS'),
(91, N'Anular', N'Anular TESORERIA_CUENTASFINANCIERAS', 1, N'TESORERIA_CUENTASFINANCIERAS'),
(92, N'Ver', N'Ver TESORERIA_COBROS', 1, N'TESORERIA_COBROS'),
(93, N'Ver', N'Ver TESORERIA_TRANSFERENCIAS', 1, N'TESORERIA_TRANSFERENCIAS'),
(94, N'Ver', N'Ver TESORERIA_CUENTASCLIENTES', 1, N'TESORERIA_CUENTASCLIENTES'),
(95, N'Ver', N'Ver TESORERIA_CUENTASPROVEEDORES', 1, N'TESORERIA_CUENTASPROVEEDORES'),
(29, N'Ver', N'Ver Productos', 1, N'Productos'),
(30, N'Crear', N'Crear Productos', 1, N'Productos'),
(31, N'Editar', N'Editar Productos', 1, N'Productos'),
(32, N'Anular', N'Anular Productos', 1, N'Productos'),
(33, N'Ver', N'Ver Clientes', 1, N'Clientes'),
(34, N'Crear', N'Crear Clientes', 1, N'Clientes'),
(35, N'Editar', N'Editar Clientes', 1, N'Clientes'),
(36, N'Anular', N'Anular Clientes', 1, N'Clientes'),
(37, N'Ver', N'Ver Proveedores', 1, N'Proveedores'),
(38, N'Crear', N'Crear Proveedores', 1, N'Proveedores'),
(39, N'Editar', N'Editar Proveedores', 1, N'Proveedores'),
(40, N'Anular', N'Anular Proveedores', 1, N'Proveedores'),
(41, N'Ver', N'Ver Categorias', 1, N'Categorias'),
(42, N'Crear', N'Crear Categorias', 1, N'Categorias');
INSERT INTO [erp].[Permiso] ([PermisoId], [Accion], [Descripcion], [Estado], [Modulo])
VALUES (43, N'Editar', N'Editar Categorias', 1, N'Categorias'),
(44, N'Anular', N'Anular Categorias', 1, N'Categorias'),
(45, N'Ver', N'Ver Compras', 1, N'Compras'),
(46, N'Crear', N'Crear Compras', 1, N'Compras'),
(47, N'Anular', N'Anular Compras', 1, N'Compras'),
(48, N'RegistrarPago', N'RegistrarPago Compras', 1, N'Compras'),
(49, N'AnularPago', N'AnularPago Compras', 1, N'Compras'),
(50, N'Ver', N'Ver Gastos', 1, N'Gastos'),
(51, N'Crear', N'Crear Gastos', 1, N'Gastos'),
(52, N'Editar', N'Editar Gastos', 1, N'Gastos'),
(53, N'Anular', N'Anular Gastos', 1, N'Gastos'),
(54, N'Ver', N'Ver Ingresos', 1, N'Ingresos'),
(55, N'Crear', N'Crear Ingresos', 1, N'Ingresos'),
(56, N'Editar', N'Editar Ingresos', 1, N'Ingresos'),
(57, N'Anular', N'Anular Ingresos', 1, N'Ingresos'),
(58, N'Ver', N'Ver Devoluciones', 1, N'Devoluciones'),
(59, N'Registrar', N'Registrar Devoluciones', 1, N'Devoluciones'),
(60, N'Ver', N'Ver Caja', 1, N'Caja'),
(61, N'Ver', N'Ver ReporteGeneral', 1, N'ReporteGeneral'),
(62, N'Ver', N'Ver PropuestasComerciales', 1, N'PropuestasComerciales'),
(63, N'Ver', N'Ver CuentasPorPagar', 1, N'CuentasPorPagar'),
(64, N'Ver', N'Ver DevolucionesProveedor', 1, N'DevolucionesProveedor'),
(65, N'Ver', N'Ver ReporteCaja', 1, N'ReporteCaja'),
(66, N'Ver', N'Ver EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(67, N'Ver', N'Ver Empresas', 1, N'Empresas'),
(68, N'Crear', N'Crear Empresas', 1, N'Empresas'),
(69, N'Editar', N'Editar Empresas', 1, N'Empresas'),
(70, N'Anular', N'Anular Empresas', 1, N'Empresas'),
(71, N'Ver', N'Ver Usuarios', 1, N'Usuarios'),
(72, N'Crear', N'Crear Usuarios', 1, N'Usuarios'),
(73, N'Editar', N'Editar Usuarios', 1, N'Usuarios'),
(74, N'RestablecerPassword', N'RestablecerPassword Usuarios', 1, N'Usuarios'),
(75, N'Ver', N'Ver Roles', 1, N'Roles'),
(76, N'Crear', N'Crear Roles', 1, N'Roles'),
(77, N'Editar', N'Editar Roles', 1, N'Roles'),
(78, N'Ver', N'Ver Configuracion', 1, N'Configuracion'),
(79, N'Crear', N'Crear Configuracion', 1, N'Configuracion'),
(80, N'Editar', N'Editar Configuracion', 1, N'Configuracion'),
(81, N'Configurar', N'Configurar Configuracion', 1, N'Configuracion'),
(82, N'Ver', N'Ver NubefactLogs', 1, N'NubefactLogs'),
(83, N'Ver', N'Ver ErroresAplicacion', 1, N'ErroresAplicacion'),
(84, N'Revisar', N'Revisar ErroresAplicacion', 1, N'ErroresAplicacion');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermisoId', N'Accion', N'Descripcion', N'Estado', N'Modulo') AND [object_id] = OBJECT_ID(N'[erp].[Permiso]'))
    SET IDENTITY_INSERT [erp].[Permiso] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolId', N'Descripcion', N'Estado', N'Nombre') AND [object_id] = OBJECT_ID(N'[erp].[Rol]'))
    SET IDENTITY_INSERT [erp].[Rol] ON;
INSERT INTO [erp].[Rol] ([RolId], [Descripcion], [Estado], [Nombre])
VALUES (1, N'Acceso total', 1, N'Administrador'),
(2, N'Operacion comercial', 1, N'Vendedor');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolId', N'Descripcion', N'Estado', N'Nombre') AND [object_id] = OBJECT_ID(N'[erp].[Rol]'))
    SET IDENTITY_INSERT [erp].[Rol] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolPermisoId', N'PermisoId', N'RolId') AND [object_id] = OBJECT_ID(N'[erp].[RolPermiso]'))
    SET IDENTITY_INSERT [erp].[RolPermiso] ON;
INSERT INTO [erp].[RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
VALUES (1, 1, 1),
(2, 2, 1),
(3, 3, 1),
(4, 4, 1),
(5, 5, 1),
(6, 6, 1),
(7, 7, 1),
(8, 8, 1),
(9, 9, 1),
(10, 10, 1),
(11, 11, 1),
(12, 12, 1),
(13, 13, 1),
(14, 14, 1),
(15, 15, 1),
(16, 16, 1),
(17, 17, 1),
(18, 18, 1),
(19, 19, 1),
(20, 20, 1),
(21, 21, 1),
(22, 22, 1),
(23, 23, 1),
(24, 24, 1),
(25, 25, 1),
(26, 26, 1),
(27, 27, 1),
(28, 28, 1),
(29, 29, 1),
(30, 30, 1),
(31, 31, 1),
(32, 32, 1),
(33, 33, 1),
(34, 34, 1),
(35, 35, 1),
(36, 36, 1),
(37, 37, 1),
(38, 38, 1),
(39, 39, 1),
(40, 40, 1),
(41, 41, 1),
(42, 42, 1);
INSERT INTO [erp].[RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
VALUES (43, 43, 1),
(44, 44, 1),
(45, 45, 1),
(46, 46, 1),
(47, 47, 1),
(48, 48, 1),
(49, 49, 1),
(50, 50, 1),
(51, 51, 1),
(52, 52, 1),
(53, 53, 1),
(54, 54, 1),
(55, 55, 1),
(56, 56, 1),
(57, 57, 1),
(58, 58, 1),
(59, 59, 1),
(60, 60, 1),
(61, 61, 1),
(62, 62, 1),
(63, 63, 1),
(64, 64, 1),
(65, 65, 1),
(66, 66, 1),
(67, 67, 1),
(68, 68, 1),
(69, 69, 1),
(70, 70, 1),
(71, 71, 1),
(72, 72, 1),
(73, 73, 1),
(74, 74, 1),
(75, 75, 1),
(76, 76, 1),
(77, 77, 1),
(78, 78, 1),
(79, 79, 1),
(80, 80, 1),
(81, 81, 1),
(82, 82, 1),
(83, 83, 1),
(84, 84, 1),
(125, 85, 1),
(126, 86, 1),
(127, 87, 1),
(128, 88, 1),
(129, 89, 1),
(130, 90, 1),
(131, 91, 1),
(132, 92, 1),
(133, 93, 1),
(134, 94, 1),
(135, 95, 1);
INSERT INTO [erp].[RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
VALUES (85, 1, 2),
(86, 2, 2),
(87, 3, 2),
(88, 4, 2),
(89, 5, 2),
(90, 6, 2),
(91, 7, 2),
(92, 8, 2),
(93, 9, 2),
(94, 10, 2),
(95, 11, 2),
(96, 12, 2),
(97, 13, 2),
(98, 14, 2),
(99, 15, 2),
(100, 16, 2),
(101, 17, 2),
(102, 18, 2),
(103, 19, 2),
(104, 20, 2),
(105, 22, 2),
(106, 23, 2),
(107, 24, 2),
(108, 25, 2),
(109, 26, 2),
(110, 27, 2),
(111, 28, 2),
(112, 29, 2),
(113, 30, 2),
(114, 31, 2),
(115, 32, 2),
(116, 33, 2),
(117, 34, 2),
(118, 35, 2),
(119, 36, 2),
(120, 41, 2),
(121, 42, 2),
(122, 43, 2),
(123, 44, 2),
(124, 58, 2),
(136, 85, 2),
(137, 86, 2),
(138, 92, 2),
(139, 93, 2),
(140, 94, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolPermisoId', N'PermisoId', N'RolId') AND [object_id] = OBJECT_ID(N'[erp].[RolPermiso]'))
    SET IDENTITY_INSERT [erp].[RolPermiso] OFF;
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [erp].[AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [erp].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [erp].[AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [erp].[AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [erp].[AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [erp].[AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [erp].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Categoria_EmpresaId] ON [erp].[Categoria] ([EmpresaId]);
GO

CREATE UNIQUE INDEX [IX_Categoria_EmpresaId_Nombre] ON [erp].[Categoria] ([EmpresaId], [Nombre]);
GO

CREATE UNIQUE INDEX [IX_CategoriaGasto_EmpresaId_Nombre] ON [erp].[CategoriaGasto] ([EmpresaId], [Nombre]);
GO

CREATE UNIQUE INDEX [IX_CategoriaIngreso_EmpresaId_Nombre] ON [erp].[CategoriaIngreso] ([EmpresaId], [Nombre]);
GO

CREATE INDEX [IX_Cliente_EmpresaId_NombreCompleto] ON [erp].[Cliente] ([EmpresaId], [NombreCompleto]);
GO

CREATE UNIQUE INDEX [UX_Cliente_Empresa_Tipo_Numero] ON [erp].[Cliente] ([EmpresaId], [TipoDocumento], [NumeroDocumento]);
GO

CREATE INDEX [IX_CobroCliente_ClienteId] ON [erp].[CobroCliente] ([ClienteId]);
GO

CREATE INDEX [IX_CobroCliente_ComprobanteId] ON [erp].[CobroCliente] ([ComprobanteId]);
GO

CREATE INDEX [IX_CobroCliente_EmpresaId_ClienteId_FechaCobro] ON [erp].[CobroCliente] ([EmpresaId], [ClienteId], [FechaCobro]);
GO

CREATE INDEX [IX_CobroCliente_EmpresaId_ComprobanteId] ON [erp].[CobroCliente] ([EmpresaId], [ComprobanteId]);
GO

CREATE INDEX [IX_CobroCliente_EmpresaId_CuentaFinancieraId] ON [erp].[CobroCliente] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_CobroCliente_EmpresaId_NotaPedidoId] ON [erp].[CobroCliente] ([EmpresaId], [NotaPedidoId]);
GO

CREATE INDEX [IX_CobroCliente_NotaPedidoId] ON [erp].[CobroCliente] ([NotaPedidoId]);
GO

CREATE INDEX [IX_Compra_EmpresaId] ON [erp].[Compra] ([EmpresaId]);
GO

CREATE INDEX [IX_Compra_EmpresaId_Fecha] ON [erp].[Compra] ([EmpresaId], [Fecha]);
GO

CREATE INDEX [IX_Compra_EmpresaId_ProveedorId] ON [erp].[Compra] ([EmpresaId], [ProveedorId]);
GO

CREATE INDEX [IX_Compra_EmpresaId_ProveedorId_TipoDocumento_Serie_Numero] ON [erp].[Compra] ([EmpresaId], [ProveedorId], [TipoDocumento], [Serie], [Numero]);
GO

CREATE INDEX [IX_Compra_ProveedorId] ON [erp].[Compra] ([ProveedorId]);
GO

CREATE INDEX [IX_CompraDetalle_CompraId] ON [erp].[CompraDetalle] ([CompraId]);
GO

CREATE INDEX [IX_CompraDetalle_ProductoId] ON [erp].[CompraDetalle] ([ProductoId]);
GO

CREATE INDEX [IX_Comprobante_ClienteId] ON [erp].[Comprobante] ([ClienteId]);
GO

CREATE INDEX [IX_Comprobante_ComprobanteReferenciaId] ON [erp].[Comprobante] ([ComprobanteReferenciaId]);
GO

CREATE INDEX [IX_Comprobante_CotizacionId] ON [erp].[Comprobante] ([CotizacionId]);
GO

CREATE INDEX [IX_Comprobante_EmpresaId] ON [erp].[Comprobante] ([EmpresaId]);
GO

CREATE INDEX [IX_Comprobante_EmpresaId_ComprobanteReferenciaId] ON [erp].[Comprobante] ([EmpresaId], [ComprobanteReferenciaId]);
GO

CREATE INDEX [IX_Comprobante_EmpresaId_CotizacionId] ON [erp].[Comprobante] ([EmpresaId], [CotizacionId]);
GO

CREATE INDEX [IX_Comprobante_EmpresaId_EstadoSunat] ON [erp].[Comprobante] ([EmpresaId], [EstadoSunat]);
GO

CREATE INDEX [IX_Comprobante_EmpresaId_FechaEmision] ON [erp].[Comprobante] ([EmpresaId], [FechaEmision]);
GO

CREATE INDEX [IX_Comprobante_EmpresaId_NotaPedidoId] ON [erp].[Comprobante] ([EmpresaId], [NotaPedidoId]);
GO

CREATE UNIQUE INDEX [IX_Comprobante_EmpresaId_Serie_Correlativo] ON [erp].[Comprobante] ([EmpresaId], [Serie], [Correlativo]);
GO

CREATE INDEX [IX_Comprobante_EmpresaId_TipoComprobante] ON [erp].[Comprobante] ([EmpresaId], [TipoComprobante]);
GO

CREATE INDEX [IX_Comprobante_MotivoNotaCreditoId] ON [erp].[Comprobante] ([MotivoNotaCreditoId]);
GO

CREATE INDEX [IX_Comprobante_NotaPedidoId] ON [erp].[Comprobante] ([NotaPedidoId]);
GO

CREATE INDEX [IX_ComprobanteCobroAplicado_CobroClienteId] ON [erp].[ComprobanteCobroAplicado] ([CobroClienteId]);
GO

CREATE INDEX [IX_ComprobanteCobroAplicado_ComprobanteId] ON [erp].[ComprobanteCobroAplicado] ([ComprobanteId]);
GO

CREATE INDEX [IX_ComprobanteCobroAplicado_EmpresaId_CobroClienteId] ON [erp].[ComprobanteCobroAplicado] ([EmpresaId], [CobroClienteId]);
GO

CREATE INDEX [IX_ComprobanteCobroAplicado_EmpresaId_ComprobanteId] ON [erp].[ComprobanteCobroAplicado] ([EmpresaId], [ComprobanteId]);
GO

CREATE INDEX [IX_ComprobanteDetalle_ComprobanteId] ON [erp].[ComprobanteDetalle] ([ComprobanteId]);
GO

CREATE INDEX [IX_ComprobanteDetalle_ProductoId] ON [erp].[ComprobanteDetalle] ([ProductoId]);
GO

CREATE UNIQUE INDEX [IX_ConfiguracionEmpresa_EmpresaId_Clave] ON [erp].[ConfiguracionEmpresa] ([EmpresaId], [Clave]);
GO

CREATE INDEX [IX_Cotizacion_ClienteId] ON [erp].[Cotizacion] ([ClienteId]);
GO

CREATE INDEX [IX_Cotizacion_EmpresaId] ON [erp].[Cotizacion] ([EmpresaId]);
GO

CREATE INDEX [IX_Cotizacion_EmpresaId_ClienteId_FechaEmision] ON [erp].[Cotizacion] ([EmpresaId], [ClienteId], [FechaEmision]);
GO

CREATE UNIQUE INDEX [IX_Cotizacion_EmpresaId_Serie_Correlativo] ON [erp].[Cotizacion] ([EmpresaId], [Serie], [Correlativo]);
GO

CREATE INDEX [IX_CotizacionDetalle_CotizacionId] ON [erp].[CotizacionDetalle] ([CotizacionId]);
GO

CREATE INDEX [IX_CotizacionDetalle_ProductoId] ON [erp].[CotizacionDetalle] ([ProductoId]);
GO

CREATE INDEX [IX_Devolucion_ClienteId] ON [erp].[Devolucion] ([ClienteId]);
GO

CREATE INDEX [IX_Devolucion_CompraId] ON [erp].[Devolucion] ([CompraId]);
GO

CREATE INDEX [IX_Devolucion_ComprobanteId] ON [erp].[Devolucion] ([ComprobanteId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_ClienteId] ON [erp].[Devolucion] ([EmpresaId], [ClienteId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_CompraId] ON [erp].[Devolucion] ([EmpresaId], [CompraId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_ComprobanteId] ON [erp].[Devolucion] ([EmpresaId], [ComprobanteId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_FechaGeneracion] ON [erp].[Devolucion] ([EmpresaId], [FechaGeneracion]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_NotaCreditoId] ON [erp].[Devolucion] ([EmpresaId], [NotaCreditoId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_NotaPedidoId] ON [erp].[Devolucion] ([EmpresaId], [NotaPedidoId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_ProveedorId] ON [erp].[Devolucion] ([EmpresaId], [ProveedorId]);
GO

CREATE INDEX [IX_Devolucion_EmpresaId_TipoTercero] ON [erp].[Devolucion] ([EmpresaId], [TipoTercero]);
GO

CREATE INDEX [IX_Devolucion_NotaCreditoId] ON [erp].[Devolucion] ([NotaCreditoId]);
GO

CREATE INDEX [IX_Devolucion_NotaPedidoId] ON [erp].[Devolucion] ([NotaPedidoId]);
GO

CREATE INDEX [IX_Devolucion_ProveedorId] ON [erp].[Devolucion] ([ProveedorId]);
GO


CREATE INDEX [IX_CuentaFinanciera_EmpresaId_Nombre] ON [erp].[CuentaFinanciera] ([EmpresaId], [Nombre]);
GO

CREATE INDEX [IX_CuentaFinanciera_EmpresaId_Tipo] ON [erp].[CuentaFinanciera] ([EmpresaId], [Tipo]);
GO

CREATE UNIQUE INDEX [IX_Empresa_RUC] ON [erp].[Empresa] ([RUC]);
GO

CREATE INDEX [IX_ErrorAplicacion_EmpresaId_Estado] ON [erp].[ErrorAplicacion] ([EmpresaId], [Estado]);
GO

CREATE INDEX [IX_ErrorAplicacion_EmpresaId_FechaUtc] ON [erp].[ErrorAplicacion] ([EmpresaId], [FechaUtc]);
GO

CREATE INDEX [IX_Gasto_CategoriaGastoId] ON [erp].[Gasto] ([CategoriaGastoId]);
GO

CREATE INDEX [IX_Gasto_EmpresaId] ON [erp].[Gasto] ([EmpresaId]);
GO

CREATE INDEX [IX_Gasto_EmpresaId_CuentaFinancieraId] ON [erp].[Gasto] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_Gasto_EmpresaId_Fecha] ON [erp].[Gasto] ([EmpresaId], [Fecha]);
GO

CREATE INDEX [IX_Gasto_EmpresaId_ProveedorId] ON [erp].[Gasto] ([EmpresaId], [ProveedorId]);
GO

CREATE INDEX [IX_Gasto_MovimientoCajaId] ON [erp].[Gasto] ([MovimientoCajaId]);
GO

CREATE INDEX [IX_Ingreso_CategoriaIngresoId] ON [erp].[Ingreso] ([CategoriaIngresoId]);
GO

CREATE INDEX [IX_Ingreso_EmpresaId] ON [erp].[Ingreso] ([EmpresaId]);
GO

CREATE INDEX [IX_Ingreso_EmpresaId_CuentaFinancieraId] ON [erp].[Ingreso] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_Ingreso_EmpresaId_Fecha] ON [erp].[Ingreso] ([EmpresaId], [Fecha]);
GO

CREATE INDEX [IX_Ingreso_EmpresaId_ClienteId] ON [erp].[Ingreso] ([EmpresaId], [ClienteId]);
GO

CREATE INDEX [IX_Ingreso_MovimientoCajaId] ON [erp].[Ingreso] ([MovimientoCajaId]);
GO

CREATE UNIQUE INDEX [IX_Moneda_Codigo] ON [erp].[Moneda] ([Codigo]);
GO

CREATE UNIQUE INDEX [IX_MotivoNotaCredito_Nombre] ON [erp].[MotivoNotaCredito] ([Nombre]);
GO

CREATE INDEX [IX_MovimientoCaja_EmpresaId_ClienteId] ON [erp].[MovimientoCaja] ([EmpresaId], [ClienteId]);
GO

CREATE INDEX [IX_MovimientoCaja_EmpresaId_CuentaFinancieraId] ON [erp].[MovimientoCaja] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_MovimientoCaja_EmpresaId_Fecha] ON [erp].[MovimientoCaja] ([EmpresaId], [Fecha]);
GO

CREATE INDEX [IX_MovimientoCaja_EmpresaId_Origen_OrigenId] ON [erp].[MovimientoCaja] ([EmpresaId], [Origen], [OrigenId]);
GO

CREATE INDEX [IX_MovimientoCaja_EmpresaId_ProveedorId] ON [erp].[MovimientoCaja] ([EmpresaId], [ProveedorId]);
GO

CREATE INDEX [IX_MovimientoInventario_EmpresaId] ON [erp].[MovimientoInventario] ([EmpresaId]);
GO

CREATE INDEX [IX_MovimientoInventario_EmpresaId_ProductoId_Fecha] ON [erp].[MovimientoInventario] ([EmpresaId], [ProductoId], [Fecha]);
GO

CREATE INDEX [IX_MovimientoInventario_ProductoId] ON [erp].[MovimientoInventario] ([ProductoId]);
GO

CREATE INDEX [IX_NotaPedido_ClienteId] ON [erp].[NotaPedido] ([ClienteId]);
GO

CREATE INDEX [IX_NotaPedido_CotizacionId] ON [erp].[NotaPedido] ([CotizacionId]);
GO

CREATE INDEX [IX_NotaPedido_EmpresaId] ON [erp].[NotaPedido] ([EmpresaId]);
GO

CREATE INDEX [IX_NotaPedido_EmpresaId_ClienteId_Fecha] ON [erp].[NotaPedido] ([EmpresaId], [ClienteId], [Fecha]);
GO

CREATE INDEX [IX_NotaPedido_EmpresaId_ComprobanteId] ON [erp].[NotaPedido] ([EmpresaId], [ComprobanteId]);
GO

CREATE INDEX [IX_NotaPedido_EmpresaId_CotizacionId] ON [erp].[NotaPedido] ([EmpresaId], [CotizacionId]);
GO

CREATE UNIQUE INDEX [IX_NotaPedido_EmpresaId_Serie_Correlativo] ON [erp].[NotaPedido] ([EmpresaId], [Serie], [Correlativo]);
GO

CREATE INDEX [IX_NotaPedidoDetalle_NotaPedidoId] ON [erp].[NotaPedidoDetalle] ([NotaPedidoId]);
GO

CREATE INDEX [IX_NotaPedidoDetalle_ProductoId] ON [erp].[NotaPedidoDetalle] ([ProductoId]);
GO

CREATE INDEX [IX_NubefactOperacion_ComprobanteId] ON [erp].[NubefactOperacion] ([ComprobanteId]);
GO

CREATE INDEX [IX_NubefactOperacion_EmpresaId] ON [erp].[NubefactOperacion] ([EmpresaId]);
GO

CREATE INDEX [IX_PagoProveedor_CompraId] ON [erp].[PagoProveedor] ([CompraId]);
GO

CREATE INDEX [IX_PagoProveedor_EmpresaId_CompraId] ON [erp].[PagoProveedor] ([EmpresaId], [CompraId]);
GO

CREATE INDEX [IX_PagoProveedor_EmpresaId_CuentaFinancieraId] ON [erp].[PagoProveedor] ([EmpresaId], [CuentaFinancieraId]);
GO

CREATE INDEX [IX_PagoProveedor_EmpresaId_ProveedorId_FechaPago] ON [erp].[PagoProveedor] ([EmpresaId], [ProveedorId], [FechaPago]);
GO

CREATE INDEX [IX_PagoProveedor_ProveedorId] ON [erp].[PagoProveedor] ([ProveedorId]);
GO

CREATE UNIQUE INDEX [IX_Permiso_Modulo_Accion] ON [erp].[Permiso] ([Modulo], [Accion]);
GO

CREATE INDEX [IX_Producto_EmpresaId] ON [erp].[Producto] ([EmpresaId]);
GO

CREATE INDEX [IX_Producto_EmpresaId_Nombre] ON [erp].[Producto] ([EmpresaId], [Nombre]);
GO

CREATE INDEX [IX_Proveedor_EmpresaId] ON [erp].[Proveedor] ([EmpresaId]);
GO

CREATE INDEX [IX_Proveedor_EmpresaId_NumeroDocumento] ON [erp].[Proveedor] ([EmpresaId], [NumeroDocumento]);
GO

CREATE INDEX [IX_Proveedor_EmpresaId_RazonSocial] ON [erp].[Proveedor] ([EmpresaId], [RazonSocial]);
GO

CREATE INDEX [IX_RolPermiso_PermisoId] ON [erp].[RolPermiso] ([PermisoId]);
GO

CREATE INDEX [IX_RolPermiso_RolId] ON [erp].[RolPermiso] ([RolId]);
GO


CREATE INDEX [IX_TransferenciaFinanciera_CuentaDestinoId] ON [erp].[TransferenciaFinanciera] ([CuentaDestinoId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_CuentaOrigenId] ON [erp].[TransferenciaFinanciera] ([CuentaOrigenId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_CuentaDestinoId] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [CuentaDestinoId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_CuentaOrigenId] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [CuentaOrigenId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_EmpresaId_Fecha] ON [erp].[TransferenciaFinanciera] ([EmpresaId], [Fecha]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_MovimientoEgresoId] ON [erp].[TransferenciaFinanciera] ([MovimientoEgresoId]);
GO

CREATE INDEX [IX_TransferenciaFinanciera_MovimientoIngresoId] ON [erp].[TransferenciaFinanciera] ([MovimientoIngresoId]);
GO

CREATE INDEX [IX_UsuarioEmpresa_EmpresaId] ON [erp].[UsuarioEmpresa] ([EmpresaId]);
GO

CREATE UNIQUE INDEX [IX_UsuarioEmpresa_UsuarioId_EmpresaId] ON [erp].[UsuarioEmpresa] ([UsuarioId], [EmpresaId]);
GO

INSERT INTO [erp].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260624175549_InitialCreateErpSchema', N'8.0.6'),
(N'20260627090000_AddCuentaFinancieraCajaBancos', N'8.0.6'),
(N'20260627103000_AddTransferenciasFinancieras', N'8.0.6');
GO

COMMIT;
GO


