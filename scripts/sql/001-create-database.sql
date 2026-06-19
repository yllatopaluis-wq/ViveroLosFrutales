IF DB_ID(N'ViveroLosFrutalesDB') IS NULL
BEGIN
    CREATE DATABASE ViveroLosFrutalesDB;
END;
GO

USE ViveroLosFrutalesDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO



CREATE TABLE [dbo].[AspNetUsers] (
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



CREATE TABLE [Cliente] (
    [ClienteId] int NOT NULL IDENTITY,
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



CREATE TABLE [Empresa] (
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
    [SerieBoleta] nvarchar(max) NOT NULL,
    [SerieFactura] nvarchar(max) NOT NULL,
    [SerieNotaCredito] nvarchar(10) NOT NULL,
    [SerieNotaCreditoFactura] nvarchar(10) NOT NULL,
    [SerieNotaCreditoBoleta] nvarchar(10) NOT NULL,
    [SerieNotaPedido] nvarchar(max) NOT NULL,
    [SerieCotizacion] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Empresa] PRIMARY KEY ([EmpresaId])
);
GO



CREATE TABLE [Moneda] (
    [MonedaId] int NOT NULL IDENTITY,
    [Codigo] nvarchar(3) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Simbolo] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Moneda] PRIMARY KEY ([MonedaId])
);
GO



CREATE TABLE [MotivoNotaCredito] (
    [MotivoNotaCreditoId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(150) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_MotivoNotaCredito] PRIMARY KEY ([MotivoNotaCreditoId])
);
GO



CREATE TABLE [Permiso] (
    [PermisoId] int NOT NULL IDENTITY,
    [Modulo] nvarchar(80) NOT NULL,
    [Accion] nvarchar(40) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Permiso] PRIMARY KEY ([PermisoId])
);
GO



CREATE TABLE [Rol] (
    [RolId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(80) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    CONSTRAINT [PK_Rol] PRIMARY KEY ([RolId])
);
GO



CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO



CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO



CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO



CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO



CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO



CREATE TABLE [Categoria] (
    [CategoriaId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(250) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY ([CategoriaId]),
    CONSTRAINT [FK_Categoria_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [CategoriaGasto] (
    [CategoriaGastoId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_CategoriaGasto] PRIMARY KEY ([CategoriaGastoId]),
    CONSTRAINT [FK_CategoriaGasto_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [CategoriaIngreso] (
    [CategoriaIngresoId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_CategoriaIngreso] PRIMARY KEY ([CategoriaIngresoId]),
    CONSTRAINT [FK_CategoriaIngreso_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [ConfiguracionEmpresa] (
    [ConfiguracionEmpresaId] int NOT NULL IDENTITY,
    [Clave] nvarchar(100) NOT NULL,
    [Valor] nvarchar(1000) NOT NULL,
    [Descripcion] nvarchar(250) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_ConfiguracionEmpresa] PRIMARY KEY ([ConfiguracionEmpresaId]),
    CONSTRAINT [FK_ConfiguracionEmpresa_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [Cotizacion] (
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
    CONSTRAINT [FK_Cotizacion_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Cotizacion_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [MovimientoCaja] (
    [MovimientoCajaId] int NOT NULL IDENTITY,
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
    CONSTRAINT [FK_MovimientoCaja_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [Producto] (
    [ProductoId] int NOT NULL IDENTITY,
    [Categoria] nvarchar(100) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [UnidadMedida] nvarchar(20) NOT NULL,
    [Stock] decimal(18,2) NOT NULL,
    [AfectoIgv] bit NOT NULL,
    [PrecioVentaSinIgv] decimal(18,2) NOT NULL,
    [PrecioVentaConIgv] decimal(18,2) NOT NULL,
    [TieneDetraccion] bit NOT NULL,
    [PorcentajeDetraccion] decimal(5,2) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_Producto] PRIMARY KEY ([ProductoId]),
    CONSTRAINT [FK_Producto_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [Proveedor] (
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
    CONSTRAINT [FK_Proveedor_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [UsuarioEmpresa] (
    [UsuarioEmpresaId] int NOT NULL IDENTITY,
    [UsuarioId] nvarchar(450) NOT NULL,
    [EmpresaId] int NOT NULL,
    CONSTRAINT [PK_UsuarioEmpresa] PRIMARY KEY ([UsuarioEmpresaId]),
    CONSTRAINT [FK_UsuarioEmpresa_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [RolPermiso] (
    [RolPermisoId] int NOT NULL IDENTITY,
    [RolId] int NOT NULL,
    [PermisoId] int NOT NULL,
    CONSTRAINT [PK_RolPermiso] PRIMARY KEY ([RolPermisoId]),
    CONSTRAINT [FK_RolPermiso_Permiso_PermisoId] FOREIGN KEY ([PermisoId]) REFERENCES [Permiso] ([PermisoId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolPermiso_Rol_RolId] FOREIGN KEY ([RolId]) REFERENCES [Rol] ([RolId]) ON DELETE CASCADE
);
GO



CREATE TABLE [NotaPedido] (
    [NotaPedidoId] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [CotizacionId] int NULL,
    [ComprobanteId] int NULL,
    [Serie] nvarchar(10) NOT NULL,
    [Correlativo] int NOT NULL,
    [Fecha] datetime2 NOT NULL,
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
    CONSTRAINT [FK_NotaPedido_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_NotaPedido_Cotizacion_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [Cotizacion] ([CotizacionId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_NotaPedido_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE
);
GO



CREATE TABLE [Gasto] (
    [GastoId] int NOT NULL IDENTITY,
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
    CONSTRAINT [FK_Gasto_CategoriaGasto_CategoriaGastoId] FOREIGN KEY ([CategoriaGastoId]) REFERENCES [CategoriaGasto] ([CategoriaGastoId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Gasto_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Gasto_MovimientoCaja_MovimientoCajaId] FOREIGN KEY ([MovimientoCajaId]) REFERENCES [MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [Ingreso] (
    [IngresoId] int NOT NULL IDENTITY,
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
    CONSTRAINT [FK_Ingreso_CategoriaIngreso_CategoriaIngresoId] FOREIGN KEY ([CategoriaIngresoId]) REFERENCES [CategoriaIngreso] ([CategoriaIngresoId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ingreso_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Ingreso_MovimientoCaja_MovimientoCajaId] FOREIGN KEY ([MovimientoCajaId]) REFERENCES [MovimientoCaja] ([MovimientoCajaId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [CotizacionDetalle] (
    [CotizacionDetalleId] int NOT NULL IDENTITY,
    [CotizacionId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    [ImporteIgv] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_CotizacionDetalle] PRIMARY KEY ([CotizacionDetalleId]),
    CONSTRAINT [FK_CotizacionDetalle_Cotizacion_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [Cotizacion] ([CotizacionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CotizacionDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [MovimientoInventario] (
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
    CONSTRAINT [FK_MovimientoInventario_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [Compra] (
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
    CONSTRAINT [FK_Compra_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Compra_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor] ([ProveedorId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [Comprobante] (
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
    CONSTRAINT [FK_Comprobante_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comprobante_Comprobante_ComprobanteReferenciaId] FOREIGN KEY ([ComprobanteReferenciaId]) REFERENCES [Comprobante] ([ComprobanteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comprobante_Cotizacion_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [Cotizacion] ([CotizacionId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comprobante_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comprobante_MotivoNotaCredito_MotivoNotaCreditoId] FOREIGN KEY ([MotivoNotaCreditoId]) REFERENCES [MotivoNotaCredito] ([MotivoNotaCreditoId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Comprobante_NotaPedido_NotaPedidoId] FOREIGN KEY ([NotaPedidoId]) REFERENCES [NotaPedido] ([NotaPedidoId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [NotaPedidoDetalle] (
    [NotaPedidoDetalleId] int NOT NULL IDENTITY,
    [NotaPedidoId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [Igv] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_NotaPedidoDetalle] PRIMARY KEY ([NotaPedidoDetalleId]),
    CONSTRAINT [FK_NotaPedidoDetalle_NotaPedido_NotaPedidoId] FOREIGN KEY ([NotaPedidoId]) REFERENCES [NotaPedido] ([NotaPedidoId]) ON DELETE CASCADE,
    CONSTRAINT [FK_NotaPedidoDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [CompraDetalle] (
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
    CONSTRAINT [FK_CompraDetalle_Compra_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [Compra] ([CompraId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompraDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [PagoProveedor] (
    [PagoProveedorId] int NOT NULL IDENTITY,
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
    CONSTRAINT [FK_PagoProveedor_Compra_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [Compra] ([CompraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PagoProveedor_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor] ([ProveedorId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [CobroCliente] (
    [CobroClienteId] int NOT NULL IDENTITY,
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
    CONSTRAINT [FK_CobroCliente_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CobroCliente_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [Comprobante] ([ComprobanteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CobroCliente_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CobroCliente_NotaPedido_NotaPedidoId] FOREIGN KEY ([NotaPedidoId]) REFERENCES [NotaPedido] ([NotaPedidoId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [ComprobanteDetalle] (
    [ComprobanteDetalleId] int NOT NULL IDENTITY,
    [ComprobanteId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [Cantidad] decimal(18,2) NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    [ImporteIgv] decimal(18,2) NOT NULL,
    [MontoDetraccion] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ComprobanteDetalle] PRIMARY KEY ([ComprobanteDetalleId]),
    CONSTRAINT [FK_ComprobanteDetalle_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [Comprobante] ([ComprobanteId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ComprobanteDetalle_Producto_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([ProductoId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [Devolucion] (
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
    CONSTRAINT [FK_Devolucion_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([ClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Compra_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [Compra] ([CompraId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [Comprobante] ([ComprobanteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Comprobante_NotaCreditoId] FOREIGN KEY ([NotaCreditoId]) REFERENCES [Comprobante] ([ComprobanteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Empresa_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresa] ([EmpresaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Devolucion_NotaPedido_NotaPedidoId] FOREIGN KEY ([NotaPedidoId]) REFERENCES [NotaPedido] ([NotaPedidoId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Devolucion_Proveedor_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor] ([ProveedorId]) ON DELETE NO ACTION
);
GO



CREATE TABLE [NubefactOperacion] (
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
    CONSTRAINT [FK_NubefactOperacion_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [Comprobante] ([ComprobanteId]) ON DELETE CASCADE
);
GO



CREATE TABLE [ComprobanteCobroAplicado] (
    [ComprobanteCobroAplicadoId] int NOT NULL IDENTITY,
    [EmpresaId] int NOT NULL,
    [ComprobanteId] int NOT NULL,
    [CobroClienteId] int NOT NULL,
    [MontoAplicado] decimal(18,2) NOT NULL,
    [FechaAplicacion] datetime2 NOT NULL,
    [UsuarioRegistro] nvarchar(120) NOT NULL,
    CONSTRAINT [PK_ComprobanteCobroAplicado] PRIMARY KEY ([ComprobanteCobroAplicadoId]),
    CONSTRAINT [FK_ComprobanteCobroAplicado_CobroCliente_CobroClienteId] FOREIGN KEY ([CobroClienteId]) REFERENCES [CobroCliente] ([CobroClienteId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ComprobanteCobroAplicado_Comprobante_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [Comprobante] ([ComprobanteId]) ON DELETE CASCADE
);
GO



IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MonedaId', N'Codigo', N'Descripcion', N'Estado', N'Simbolo') AND [object_id] = OBJECT_ID(N'[Moneda]'))
    SET IDENTITY_INSERT [Moneda] ON;
INSERT INTO [Moneda] ([MonedaId], [Codigo], [Descripcion], [Estado], [Simbolo])
VALUES (1, N'PEN', N'Soles', 1, N'S/');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MonedaId', N'Codigo', N'Descripcion', N'Estado', N'Simbolo') AND [object_id] = OBJECT_ID(N'[Moneda]'))
    SET IDENTITY_INSERT [Moneda] OFF;
GO



IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MotivoNotaCreditoId', N'Estado', N'FechaRegistro', N'Nombre', N'UsuarioRegistro') AND [object_id] = OBJECT_ID(N'[MotivoNotaCredito]'))
    SET IDENTITY_INSERT [MotivoNotaCredito] ON;
INSERT INTO [MotivoNotaCredito] ([MotivoNotaCreditoId], [Estado], [FechaRegistro], [Nombre], [UsuarioRegistro])
VALUES (1, 1, '2026-06-18T22:01:05.7840530Z', N'Anulacion de la operacion', N''),
(2, 1, '2026-06-18T22:01:05.7840534Z', N'Error en datos del comprobante', N''),
(3, 1, '2026-06-18T22:01:05.7840536Z', N'Devolucion total', N''),
(4, 1, '2026-06-18T22:01:05.7840537Z', N'Descuento posterior', N'');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MotivoNotaCreditoId', N'Estado', N'FechaRegistro', N'Nombre', N'UsuarioRegistro') AND [object_id] = OBJECT_ID(N'[MotivoNotaCredito]'))
    SET IDENTITY_INSERT [MotivoNotaCredito] OFF;
GO



IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermisoId', N'Accion', N'Descripcion', N'Estado', N'Modulo') AND [object_id] = OBJECT_ID(N'[Permiso]'))
    SET IDENTITY_INSERT [Permiso] ON;
INSERT INTO [Permiso] ([PermisoId], [Accion], [Descripcion], [Estado], [Modulo])
VALUES (1, N'Ver', N'Ver Empresas', 1, N'Empresas'),
(2, N'Crear', N'Crear Empresas', 1, N'Empresas'),
(3, N'Editar', N'Editar Empresas', 1, N'Empresas'),
(4, N'Anular', N'Anular Empresas', 1, N'Empresas'),
(5, N'Imprimir', N'Imprimir Empresas', 1, N'Empresas'),
(6, N'Configurar', N'Configurar Empresas', 1, N'Empresas'),
(7, N'Convertir', N'Convertir Empresas', 1, N'Empresas'),
(8, N'RegistrarPago', N'RegistrarPago Empresas', 1, N'Empresas'),
(9, N'Ver', N'Ver Usuarios', 1, N'Usuarios'),
(10, N'Crear', N'Crear Usuarios', 1, N'Usuarios'),
(11, N'Editar', N'Editar Usuarios', 1, N'Usuarios'),
(12, N'Anular', N'Anular Usuarios', 1, N'Usuarios'),
(13, N'Imprimir', N'Imprimir Usuarios', 1, N'Usuarios'),
(14, N'Configurar', N'Configurar Usuarios', 1, N'Usuarios'),
(15, N'Convertir', N'Convertir Usuarios', 1, N'Usuarios'),
(16, N'RegistrarPago', N'RegistrarPago Usuarios', 1, N'Usuarios'),
(17, N'Ver', N'Ver Roles', 1, N'Roles'),
(18, N'Crear', N'Crear Roles', 1, N'Roles'),
(19, N'Editar', N'Editar Roles', 1, N'Roles'),
(20, N'Anular', N'Anular Roles', 1, N'Roles'),
(21, N'Imprimir', N'Imprimir Roles', 1, N'Roles'),
(22, N'Configurar', N'Configurar Roles', 1, N'Roles'),
(23, N'Convertir', N'Convertir Roles', 1, N'Roles'),
(24, N'RegistrarPago', N'RegistrarPago Roles', 1, N'Roles'),
(25, N'Ver', N'Ver Categorias', 1, N'Categorias'),
(26, N'Crear', N'Crear Categorias', 1, N'Categorias'),
(27, N'Editar', N'Editar Categorias', 1, N'Categorias'),
(28, N'Anular', N'Anular Categorias', 1, N'Categorias'),
(29, N'Imprimir', N'Imprimir Categorias', 1, N'Categorias'),
(30, N'Configurar', N'Configurar Categorias', 1, N'Categorias'),
(31, N'Convertir', N'Convertir Categorias', 1, N'Categorias'),
(32, N'RegistrarPago', N'RegistrarPago Categorias', 1, N'Categorias'),
(33, N'Ver', N'Ver Productos', 1, N'Productos'),
(34, N'Crear', N'Crear Productos', 1, N'Productos'),
(35, N'Editar', N'Editar Productos', 1, N'Productos'),
(36, N'Anular', N'Anular Productos', 1, N'Productos'),
(37, N'Imprimir', N'Imprimir Productos', 1, N'Productos'),
(38, N'Configurar', N'Configurar Productos', 1, N'Productos'),
(39, N'Convertir', N'Convertir Productos', 1, N'Productos'),
(40, N'RegistrarPago', N'RegistrarPago Productos', 1, N'Productos'),
(41, N'Ver', N'Ver Clientes', 1, N'Clientes'),
(42, N'Crear', N'Crear Clientes', 1, N'Clientes');
INSERT INTO [Permiso] ([PermisoId], [Accion], [Descripcion], [Estado], [Modulo])
VALUES (43, N'Editar', N'Editar Clientes', 1, N'Clientes'),
(44, N'Anular', N'Anular Clientes', 1, N'Clientes'),
(45, N'Imprimir', N'Imprimir Clientes', 1, N'Clientes'),
(46, N'Configurar', N'Configurar Clientes', 1, N'Clientes'),
(47, N'Convertir', N'Convertir Clientes', 1, N'Clientes'),
(48, N'RegistrarPago', N'RegistrarPago Clientes', 1, N'Clientes'),
(49, N'Ver', N'Ver Proveedores', 1, N'Proveedores'),
(50, N'Crear', N'Crear Proveedores', 1, N'Proveedores'),
(51, N'Editar', N'Editar Proveedores', 1, N'Proveedores'),
(52, N'Anular', N'Anular Proveedores', 1, N'Proveedores'),
(53, N'Imprimir', N'Imprimir Proveedores', 1, N'Proveedores'),
(54, N'Configurar', N'Configurar Proveedores', 1, N'Proveedores'),
(55, N'Convertir', N'Convertir Proveedores', 1, N'Proveedores'),
(56, N'RegistrarPago', N'RegistrarPago Proveedores', 1, N'Proveedores'),
(57, N'Ver', N'Ver Compras', 1, N'Compras'),
(58, N'Crear', N'Crear Compras', 1, N'Compras'),
(59, N'Editar', N'Editar Compras', 1, N'Compras'),
(60, N'Anular', N'Anular Compras', 1, N'Compras'),
(61, N'Imprimir', N'Imprimir Compras', 1, N'Compras'),
(62, N'Configurar', N'Configurar Compras', 1, N'Compras'),
(63, N'Convertir', N'Convertir Compras', 1, N'Compras'),
(64, N'RegistrarPago', N'RegistrarPago Compras', 1, N'Compras'),
(65, N'Ver', N'Ver Cotizaciones', 1, N'Cotizaciones'),
(66, N'Crear', N'Crear Cotizaciones', 1, N'Cotizaciones'),
(67, N'Editar', N'Editar Cotizaciones', 1, N'Cotizaciones'),
(68, N'Anular', N'Anular Cotizaciones', 1, N'Cotizaciones'),
(69, N'Imprimir', N'Imprimir Cotizaciones', 1, N'Cotizaciones'),
(70, N'Configurar', N'Configurar Cotizaciones', 1, N'Cotizaciones'),
(71, N'Convertir', N'Convertir Cotizaciones', 1, N'Cotizaciones'),
(72, N'RegistrarPago', N'RegistrarPago Cotizaciones', 1, N'Cotizaciones'),
(73, N'Ver', N'Ver Comprobantes', 1, N'Comprobantes'),
(74, N'Crear', N'Crear Comprobantes', 1, N'Comprobantes'),
(75, N'Editar', N'Editar Comprobantes', 1, N'Comprobantes'),
(76, N'Anular', N'Anular Comprobantes', 1, N'Comprobantes'),
(77, N'Imprimir', N'Imprimir Comprobantes', 1, N'Comprobantes'),
(78, N'Configurar', N'Configurar Comprobantes', 1, N'Comprobantes'),
(79, N'Convertir', N'Convertir Comprobantes', 1, N'Comprobantes'),
(80, N'RegistrarPago', N'RegistrarPago Comprobantes', 1, N'Comprobantes'),
(81, N'Ver', N'Ver NotasCredito', 1, N'NotasCredito'),
(82, N'Crear', N'Crear NotasCredito', 1, N'NotasCredito'),
(83, N'Editar', N'Editar NotasCredito', 1, N'NotasCredito'),
(84, N'Anular', N'Anular NotasCredito', 1, N'NotasCredito');
INSERT INTO [Permiso] ([PermisoId], [Accion], [Descripcion], [Estado], [Modulo])
VALUES (85, N'Imprimir', N'Imprimir NotasCredito', 1, N'NotasCredito'),
(86, N'Configurar', N'Configurar NotasCredito', 1, N'NotasCredito'),
(87, N'Convertir', N'Convertir NotasCredito', 1, N'NotasCredito'),
(88, N'RegistrarPago', N'RegistrarPago NotasCredito', 1, N'NotasCredito'),
(89, N'Ver', N'Ver NotasPedido', 1, N'NotasPedido'),
(90, N'Crear', N'Crear NotasPedido', 1, N'NotasPedido'),
(91, N'Editar', N'Editar NotasPedido', 1, N'NotasPedido'),
(92, N'Anular', N'Anular NotasPedido', 1, N'NotasPedido'),
(93, N'Imprimir', N'Imprimir NotasPedido', 1, N'NotasPedido'),
(94, N'Configurar', N'Configurar NotasPedido', 1, N'NotasPedido'),
(95, N'Convertir', N'Convertir NotasPedido', 1, N'NotasPedido'),
(96, N'RegistrarPago', N'RegistrarPago NotasPedido', 1, N'NotasPedido'),
(97, N'Ver', N'Ver CobrosClientes', 1, N'CobrosClientes'),
(98, N'Crear', N'Crear CobrosClientes', 1, N'CobrosClientes'),
(99, N'Editar', N'Editar CobrosClientes', 1, N'CobrosClientes'),
(100, N'Anular', N'Anular CobrosClientes', 1, N'CobrosClientes'),
(101, N'Imprimir', N'Imprimir CobrosClientes', 1, N'CobrosClientes'),
(102, N'Configurar', N'Configurar CobrosClientes', 1, N'CobrosClientes'),
(103, N'Convertir', N'Convertir CobrosClientes', 1, N'CobrosClientes'),
(104, N'RegistrarPago', N'RegistrarPago CobrosClientes', 1, N'CobrosClientes'),
(105, N'Ver', N'Ver Devoluciones', 1, N'Devoluciones'),
(106, N'Crear', N'Crear Devoluciones', 1, N'Devoluciones'),
(107, N'Editar', N'Editar Devoluciones', 1, N'Devoluciones'),
(108, N'Anular', N'Anular Devoluciones', 1, N'Devoluciones'),
(109, N'Imprimir', N'Imprimir Devoluciones', 1, N'Devoluciones'),
(110, N'Configurar', N'Configurar Devoluciones', 1, N'Devoluciones'),
(111, N'Convertir', N'Convertir Devoluciones', 1, N'Devoluciones'),
(112, N'RegistrarPago', N'RegistrarPago Devoluciones', 1, N'Devoluciones'),
(113, N'Ver', N'Ver Caja', 1, N'Caja'),
(114, N'Crear', N'Crear Caja', 1, N'Caja'),
(115, N'Editar', N'Editar Caja', 1, N'Caja'),
(116, N'Anular', N'Anular Caja', 1, N'Caja'),
(117, N'Imprimir', N'Imprimir Caja', 1, N'Caja'),
(118, N'Configurar', N'Configurar Caja', 1, N'Caja'),
(119, N'Convertir', N'Convertir Caja', 1, N'Caja'),
(120, N'RegistrarPago', N'RegistrarPago Caja', 1, N'Caja'),
(121, N'Ver', N'Ver EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(122, N'Crear', N'Crear EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(123, N'Editar', N'Editar EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(124, N'Anular', N'Anular EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(125, N'Imprimir', N'Imprimir EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(126, N'Configurar', N'Configurar EstadoCuentaClientes', 1, N'EstadoCuentaClientes');
INSERT INTO [Permiso] ([PermisoId], [Accion], [Descripcion], [Estado], [Modulo])
VALUES (127, N'Convertir', N'Convertir EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(128, N'RegistrarPago', N'RegistrarPago EstadoCuentaClientes', 1, N'EstadoCuentaClientes'),
(129, N'Ver', N'Ver Gastos', 1, N'Gastos'),
(130, N'Crear', N'Crear Gastos', 1, N'Gastos'),
(131, N'Editar', N'Editar Gastos', 1, N'Gastos'),
(132, N'Anular', N'Anular Gastos', 1, N'Gastos'),
(133, N'Imprimir', N'Imprimir Gastos', 1, N'Gastos'),
(134, N'Configurar', N'Configurar Gastos', 1, N'Gastos'),
(135, N'Convertir', N'Convertir Gastos', 1, N'Gastos'),
(136, N'RegistrarPago', N'RegistrarPago Gastos', 1, N'Gastos'),
(137, N'Ver', N'Ver Ingresos', 1, N'Ingresos'),
(138, N'Crear', N'Crear Ingresos', 1, N'Ingresos'),
(139, N'Editar', N'Editar Ingresos', 1, N'Ingresos'),
(140, N'Anular', N'Anular Ingresos', 1, N'Ingresos'),
(141, N'Imprimir', N'Imprimir Ingresos', 1, N'Ingresos'),
(142, N'Configurar', N'Configurar Ingresos', 1, N'Ingresos'),
(143, N'Convertir', N'Convertir Ingresos', 1, N'Ingresos'),
(144, N'RegistrarPago', N'RegistrarPago Ingresos', 1, N'Ingresos'),
(145, N'Ver', N'Ver Reportes', 1, N'Reportes'),
(146, N'Crear', N'Crear Reportes', 1, N'Reportes'),
(147, N'Editar', N'Editar Reportes', 1, N'Reportes'),
(148, N'Anular', N'Anular Reportes', 1, N'Reportes'),
(149, N'Imprimir', N'Imprimir Reportes', 1, N'Reportes'),
(150, N'Configurar', N'Configurar Reportes', 1, N'Reportes'),
(151, N'Convertir', N'Convertir Reportes', 1, N'Reportes'),
(152, N'RegistrarPago', N'RegistrarPago Reportes', 1, N'Reportes'),
(153, N'Ver', N'Ver Configuracion', 1, N'Configuracion'),
(154, N'Crear', N'Crear Configuracion', 1, N'Configuracion'),
(155, N'Editar', N'Editar Configuracion', 1, N'Configuracion'),
(156, N'Anular', N'Anular Configuracion', 1, N'Configuracion'),
(157, N'Imprimir', N'Imprimir Configuracion', 1, N'Configuracion'),
(158, N'Configurar', N'Configurar Configuracion', 1, N'Configuracion'),
(159, N'Convertir', N'Convertir Configuracion', 1, N'Configuracion'),
(160, N'RegistrarPago', N'RegistrarPago Configuracion', 1, N'Configuracion'),
(161, N'Ver', N'Ver NubefactLogs', 1, N'NubefactLogs'),
(162, N'Crear', N'Crear NubefactLogs', 1, N'NubefactLogs'),
(163, N'Editar', N'Editar NubefactLogs', 1, N'NubefactLogs'),
(164, N'Anular', N'Anular NubefactLogs', 1, N'NubefactLogs'),
(165, N'Imprimir', N'Imprimir NubefactLogs', 1, N'NubefactLogs'),
(166, N'Configurar', N'Configurar NubefactLogs', 1, N'NubefactLogs'),
(167, N'Convertir', N'Convertir NubefactLogs', 1, N'NubefactLogs'),
(168, N'RegistrarPago', N'RegistrarPago NubefactLogs', 1, N'NubefactLogs');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermisoId', N'Accion', N'Descripcion', N'Estado', N'Modulo') AND [object_id] = OBJECT_ID(N'[Permiso]'))
    SET IDENTITY_INSERT [Permiso] OFF;
GO



IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolId', N'Descripcion', N'Estado', N'Nombre') AND [object_id] = OBJECT_ID(N'[Rol]'))
    SET IDENTITY_INSERT [Rol] ON;
INSERT INTO [Rol] ([RolId], [Descripcion], [Estado], [Nombre])
VALUES (1, N'Acceso total', 1, N'Administrador'),
(2, N'Operacion comercial', 1, N'Vendedor');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolId', N'Descripcion', N'Estado', N'Nombre') AND [object_id] = OBJECT_ID(N'[Rol]'))
    SET IDENTITY_INSERT [Rol] OFF;
GO



IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolPermisoId', N'PermisoId', N'RolId') AND [object_id] = OBJECT_ID(N'[RolPermiso]'))
    SET IDENTITY_INSERT [RolPermiso] ON;
INSERT INTO [RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
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
INSERT INTO [RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
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
(84, 84, 1);
INSERT INTO [RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
VALUES (85, 85, 1),
(86, 86, 1),
(87, 87, 1),
(88, 88, 1),
(89, 89, 1),
(90, 90, 1),
(91, 91, 1),
(92, 92, 1),
(93, 93, 1),
(94, 94, 1),
(95, 95, 1),
(96, 96, 1),
(97, 97, 1),
(98, 98, 1),
(99, 99, 1),
(100, 100, 1),
(101, 101, 1),
(102, 102, 1),
(103, 103, 1),
(104, 104, 1),
(105, 105, 1),
(106, 106, 1),
(107, 107, 1),
(108, 108, 1),
(109, 109, 1),
(110, 110, 1),
(111, 111, 1),
(112, 112, 1),
(113, 113, 1),
(114, 114, 1),
(115, 115, 1),
(116, 116, 1),
(117, 117, 1),
(118, 118, 1),
(119, 119, 1),
(120, 120, 1),
(121, 121, 1),
(122, 122, 1),
(123, 123, 1),
(124, 124, 1),
(125, 125, 1),
(126, 126, 1);
INSERT INTO [RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
VALUES (127, 127, 1),
(128, 128, 1),
(129, 129, 1),
(130, 130, 1),
(131, 131, 1),
(132, 132, 1),
(133, 133, 1),
(134, 134, 1),
(135, 135, 1),
(136, 136, 1),
(137, 137, 1),
(138, 138, 1),
(139, 139, 1),
(140, 140, 1),
(141, 141, 1),
(142, 142, 1),
(143, 143, 1),
(144, 144, 1),
(145, 145, 1),
(146, 146, 1),
(147, 147, 1),
(148, 148, 1),
(149, 149, 1),
(150, 150, 1),
(151, 151, 1),
(152, 152, 1),
(153, 153, 1),
(154, 154, 1),
(155, 155, 1),
(156, 156, 1),
(157, 157, 1),
(158, 158, 1),
(159, 159, 1),
(160, 160, 1),
(161, 161, 1),
(162, 162, 1),
(163, 163, 1),
(164, 164, 1),
(165, 165, 1),
(166, 166, 1),
(167, 167, 1),
(168, 168, 1);
INSERT INTO [RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
VALUES (169, 25, 2),
(170, 26, 2),
(171, 27, 2),
(172, 28, 2),
(173, 29, 2),
(174, 31, 2),
(175, 32, 2),
(176, 33, 2),
(177, 34, 2),
(178, 35, 2),
(179, 36, 2),
(180, 37, 2),
(181, 39, 2),
(182, 40, 2),
(183, 41, 2),
(184, 42, 2),
(185, 43, 2),
(186, 44, 2),
(187, 45, 2),
(188, 47, 2),
(189, 48, 2),
(190, 65, 2),
(191, 66, 2),
(192, 67, 2),
(193, 68, 2),
(194, 69, 2),
(195, 71, 2),
(196, 72, 2),
(197, 73, 2),
(198, 74, 2),
(199, 75, 2),
(200, 76, 2),
(201, 77, 2),
(202, 79, 2),
(203, 80, 2),
(204, 81, 2),
(205, 82, 2),
(206, 83, 2),
(207, 84, 2),
(208, 85, 2),
(209, 87, 2),
(210, 88, 2);
INSERT INTO [RolPermiso] ([RolPermisoId], [PermisoId], [RolId])
VALUES (211, 89, 2),
(212, 90, 2),
(213, 91, 2),
(214, 92, 2),
(215, 93, 2),
(216, 95, 2),
(217, 96, 2),
(218, 97, 2),
(219, 98, 2),
(220, 99, 2),
(221, 100, 2),
(222, 101, 2),
(223, 103, 2),
(224, 104, 2),
(225, 105, 2),
(226, 106, 2),
(227, 107, 2),
(228, 108, 2),
(229, 109, 2),
(230, 111, 2),
(231, 112, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolPermisoId', N'PermisoId', N'RolId') AND [object_id] = OBJECT_ID(N'[RolPermiso]'))
    SET IDENTITY_INSERT [RolPermiso] OFF;
GO



CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims] ([RoleId]);
GO



CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO



CREATE INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims] ([UserId]);
GO



CREATE INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins] ([UserId]);
GO



CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles] ([RoleId]);
GO



CREATE INDEX [EmailIndex] ON [dbo].[AspNetUsers] ([NormalizedEmail]);
GO



CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO



CREATE INDEX [IX_Categoria_EmpresaId] ON [Categoria] ([EmpresaId]);
GO



CREATE UNIQUE INDEX [IX_Categoria_EmpresaId_Nombre] ON [Categoria] ([EmpresaId], [Nombre]);
GO



CREATE UNIQUE INDEX [IX_CategoriaGasto_EmpresaId_Nombre] ON [CategoriaGasto] ([EmpresaId], [Nombre]);
GO



CREATE UNIQUE INDEX [IX_CategoriaIngreso_EmpresaId_Nombre] ON [CategoriaIngreso] ([EmpresaId], [Nombre]);
GO



CREATE INDEX [IX_Cliente_NombreCompleto] ON [Cliente] ([NombreCompleto]);
GO



CREATE UNIQUE INDEX [UX_Cliente_Tipo_Numero] ON [Cliente] ([TipoDocumento], [NumeroDocumento]);
GO



CREATE INDEX [IX_CobroCliente_ClienteId] ON [CobroCliente] ([ClienteId]);
GO



CREATE INDEX [IX_CobroCliente_ComprobanteId] ON [CobroCliente] ([ComprobanteId]);
GO



CREATE INDEX [IX_CobroCliente_EmpresaId_ClienteId_FechaCobro] ON [CobroCliente] ([EmpresaId], [ClienteId], [FechaCobro]);
GO



CREATE INDEX [IX_CobroCliente_EmpresaId_ComprobanteId] ON [CobroCliente] ([EmpresaId], [ComprobanteId]);
GO



CREATE INDEX [IX_CobroCliente_EmpresaId_NotaPedidoId] ON [CobroCliente] ([EmpresaId], [NotaPedidoId]);
GO



CREATE INDEX [IX_CobroCliente_NotaPedidoId] ON [CobroCliente] ([NotaPedidoId]);
GO



CREATE INDEX [IX_Compra_EmpresaId] ON [Compra] ([EmpresaId]);
GO



CREATE INDEX [IX_Compra_EmpresaId_Fecha] ON [Compra] ([EmpresaId], [Fecha]);
GO



CREATE INDEX [IX_Compra_EmpresaId_ProveedorId] ON [Compra] ([EmpresaId], [ProveedorId]);
GO



CREATE INDEX [IX_Compra_EmpresaId_ProveedorId_TipoDocumento_Serie_Numero] ON [Compra] ([EmpresaId], [ProveedorId], [TipoDocumento], [Serie], [Numero]);
GO



CREATE INDEX [IX_Compra_ProveedorId] ON [Compra] ([ProveedorId]);
GO



CREATE INDEX [IX_CompraDetalle_CompraId] ON [CompraDetalle] ([CompraId]);
GO



CREATE INDEX [IX_CompraDetalle_ProductoId] ON [CompraDetalle] ([ProductoId]);
GO



CREATE INDEX [IX_Comprobante_ClienteId] ON [Comprobante] ([ClienteId]);
GO



CREATE INDEX [IX_Comprobante_ComprobanteReferenciaId] ON [Comprobante] ([ComprobanteReferenciaId]);
GO



CREATE INDEX [IX_Comprobante_CotizacionId] ON [Comprobante] ([CotizacionId]);
GO



CREATE INDEX [IX_Comprobante_EmpresaId] ON [Comprobante] ([EmpresaId]);
GO



CREATE INDEX [IX_Comprobante_EmpresaId_ComprobanteReferenciaId] ON [Comprobante] ([EmpresaId], [ComprobanteReferenciaId]);
GO



CREATE INDEX [IX_Comprobante_EmpresaId_CotizacionId] ON [Comprobante] ([EmpresaId], [CotizacionId]);
GO



CREATE INDEX [IX_Comprobante_EmpresaId_EstadoSunat] ON [Comprobante] ([EmpresaId], [EstadoSunat]);
GO



CREATE INDEX [IX_Comprobante_EmpresaId_FechaEmision] ON [Comprobante] ([EmpresaId], [FechaEmision]);
GO



CREATE INDEX [IX_Comprobante_EmpresaId_NotaPedidoId] ON [Comprobante] ([EmpresaId], [NotaPedidoId]);
GO



CREATE UNIQUE INDEX [IX_Comprobante_EmpresaId_Serie_Correlativo] ON [Comprobante] ([EmpresaId], [Serie], [Correlativo]);
GO



CREATE INDEX [IX_Comprobante_EmpresaId_TipoComprobante] ON [Comprobante] ([EmpresaId], [TipoComprobante]);
GO



CREATE INDEX [IX_Comprobante_MotivoNotaCreditoId] ON [Comprobante] ([MotivoNotaCreditoId]);
GO



CREATE INDEX [IX_Comprobante_NotaPedidoId] ON [Comprobante] ([NotaPedidoId]);
GO



CREATE INDEX [IX_ComprobanteCobroAplicado_CobroClienteId] ON [ComprobanteCobroAplicado] ([CobroClienteId]);
GO



CREATE INDEX [IX_ComprobanteCobroAplicado_ComprobanteId] ON [ComprobanteCobroAplicado] ([ComprobanteId]);
GO



CREATE INDEX [IX_ComprobanteCobroAplicado_EmpresaId_CobroClienteId] ON [ComprobanteCobroAplicado] ([EmpresaId], [CobroClienteId]);
GO



CREATE INDEX [IX_ComprobanteCobroAplicado_EmpresaId_ComprobanteId] ON [ComprobanteCobroAplicado] ([EmpresaId], [ComprobanteId]);
GO



CREATE INDEX [IX_ComprobanteDetalle_ComprobanteId] ON [ComprobanteDetalle] ([ComprobanteId]);
GO



CREATE INDEX [IX_ComprobanteDetalle_ProductoId] ON [ComprobanteDetalle] ([ProductoId]);
GO



CREATE UNIQUE INDEX [IX_ConfiguracionEmpresa_EmpresaId_Clave] ON [ConfiguracionEmpresa] ([EmpresaId], [Clave]);
GO



CREATE INDEX [IX_Cotizacion_ClienteId] ON [Cotizacion] ([ClienteId]);
GO



CREATE INDEX [IX_Cotizacion_EmpresaId] ON [Cotizacion] ([EmpresaId]);
GO



CREATE INDEX [IX_Cotizacion_EmpresaId_ClienteId_FechaEmision] ON [Cotizacion] ([EmpresaId], [ClienteId], [FechaEmision]);
GO



CREATE UNIQUE INDEX [IX_Cotizacion_EmpresaId_Serie_Correlativo] ON [Cotizacion] ([EmpresaId], [Serie], [Correlativo]);
GO



CREATE INDEX [IX_CotizacionDetalle_CotizacionId] ON [CotizacionDetalle] ([CotizacionId]);
GO



CREATE INDEX [IX_CotizacionDetalle_ProductoId] ON [CotizacionDetalle] ([ProductoId]);
GO



CREATE INDEX [IX_Devolucion_ClienteId] ON [Devolucion] ([ClienteId]);
GO



CREATE INDEX [IX_Devolucion_CompraId] ON [Devolucion] ([CompraId]);
GO



CREATE INDEX [IX_Devolucion_ComprobanteId] ON [Devolucion] ([ComprobanteId]);
GO



CREATE INDEX [IX_Devolucion_EmpresaId_ClienteId] ON [Devolucion] ([EmpresaId], [ClienteId]);
GO



CREATE INDEX [IX_Devolucion_EmpresaId_CompraId] ON [Devolucion] ([EmpresaId], [CompraId]);
GO



CREATE INDEX [IX_Devolucion_EmpresaId_ComprobanteId] ON [Devolucion] ([EmpresaId], [ComprobanteId]);
GO



CREATE INDEX [IX_Devolucion_EmpresaId_FechaGeneracion] ON [Devolucion] ([EmpresaId], [FechaGeneracion]);
GO



CREATE INDEX [IX_Devolucion_EmpresaId_NotaCreditoId] ON [Devolucion] ([EmpresaId], [NotaCreditoId]);
GO



CREATE INDEX [IX_Devolucion_EmpresaId_NotaPedidoId] ON [Devolucion] ([EmpresaId], [NotaPedidoId]);
GO



CREATE INDEX [IX_Devolucion_EmpresaId_ProveedorId] ON [Devolucion] ([EmpresaId], [ProveedorId]);
GO



CREATE INDEX [IX_Devolucion_EmpresaId_TipoTercero] ON [Devolucion] ([EmpresaId], [TipoTercero]);
GO



CREATE INDEX [IX_Devolucion_NotaCreditoId] ON [Devolucion] ([NotaCreditoId]);
GO



CREATE INDEX [IX_Devolucion_NotaPedidoId] ON [Devolucion] ([NotaPedidoId]);
GO



CREATE INDEX [IX_Devolucion_ProveedorId] ON [Devolucion] ([ProveedorId]);
GO



CREATE UNIQUE INDEX [IX_Empresa_RUC] ON [Empresa] ([RUC]);
GO



CREATE INDEX [IX_Gasto_CategoriaGastoId] ON [Gasto] ([CategoriaGastoId]);
GO



CREATE INDEX [IX_Gasto_EmpresaId] ON [Gasto] ([EmpresaId]);
GO



CREATE INDEX [IX_Gasto_EmpresaId_Fecha] ON [Gasto] ([EmpresaId], [Fecha]);
GO



CREATE INDEX [IX_Gasto_MovimientoCajaId] ON [Gasto] ([MovimientoCajaId]);
GO



CREATE INDEX [IX_Ingreso_CategoriaIngresoId] ON [Ingreso] ([CategoriaIngresoId]);
GO



CREATE INDEX [IX_Ingreso_EmpresaId] ON [Ingreso] ([EmpresaId]);
GO



CREATE INDEX [IX_Ingreso_EmpresaId_Fecha] ON [Ingreso] ([EmpresaId], [Fecha]);
GO



CREATE INDEX [IX_Ingreso_MovimientoCajaId] ON [Ingreso] ([MovimientoCajaId]);
GO



CREATE UNIQUE INDEX [IX_Moneda_Codigo] ON [Moneda] ([Codigo]);
GO



CREATE UNIQUE INDEX [IX_MotivoNotaCredito_Nombre] ON [MotivoNotaCredito] ([Nombre]);
GO



CREATE INDEX [IX_MovimientoCaja_EmpresaId_ClienteId] ON [MovimientoCaja] ([EmpresaId], [ClienteId]);
GO



CREATE INDEX [IX_MovimientoCaja_EmpresaId_Fecha] ON [MovimientoCaja] ([EmpresaId], [Fecha]);
GO



CREATE INDEX [IX_MovimientoCaja_EmpresaId_Origen_OrigenId] ON [MovimientoCaja] ([EmpresaId], [Origen], [OrigenId]);
GO



CREATE INDEX [IX_MovimientoCaja_EmpresaId_ProveedorId] ON [MovimientoCaja] ([EmpresaId], [ProveedorId]);
GO



CREATE INDEX [IX_MovimientoInventario_EmpresaId] ON [MovimientoInventario] ([EmpresaId]);
GO



CREATE INDEX [IX_MovimientoInventario_EmpresaId_ProductoId_Fecha] ON [MovimientoInventario] ([EmpresaId], [ProductoId], [Fecha]);
GO



CREATE INDEX [IX_MovimientoInventario_ProductoId] ON [MovimientoInventario] ([ProductoId]);
GO



CREATE INDEX [IX_NotaPedido_ClienteId] ON [NotaPedido] ([ClienteId]);
GO



CREATE INDEX [IX_NotaPedido_CotizacionId] ON [NotaPedido] ([CotizacionId]);
GO



CREATE INDEX [IX_NotaPedido_EmpresaId] ON [NotaPedido] ([EmpresaId]);
GO



CREATE INDEX [IX_NotaPedido_EmpresaId_ClienteId_Fecha] ON [NotaPedido] ([EmpresaId], [ClienteId], [Fecha]);
GO



CREATE INDEX [IX_NotaPedido_EmpresaId_ComprobanteId] ON [NotaPedido] ([EmpresaId], [ComprobanteId]);
GO



CREATE INDEX [IX_NotaPedido_EmpresaId_CotizacionId] ON [NotaPedido] ([EmpresaId], [CotizacionId]);
GO



CREATE UNIQUE INDEX [IX_NotaPedido_EmpresaId_Serie_Correlativo] ON [NotaPedido] ([EmpresaId], [Serie], [Correlativo]);
GO



CREATE INDEX [IX_NotaPedidoDetalle_NotaPedidoId] ON [NotaPedidoDetalle] ([NotaPedidoId]);
GO



CREATE INDEX [IX_NotaPedidoDetalle_ProductoId] ON [NotaPedidoDetalle] ([ProductoId]);
GO



CREATE INDEX [IX_NubefactOperacion_ComprobanteId] ON [NubefactOperacion] ([ComprobanteId]);
GO



CREATE INDEX [IX_NubefactOperacion_EmpresaId] ON [NubefactOperacion] ([EmpresaId]);
GO



CREATE INDEX [IX_PagoProveedor_CompraId] ON [PagoProveedor] ([CompraId]);
GO



CREATE INDEX [IX_PagoProveedor_EmpresaId_CompraId] ON [PagoProveedor] ([EmpresaId], [CompraId]);
GO



CREATE INDEX [IX_PagoProveedor_EmpresaId_ProveedorId_FechaPago] ON [PagoProveedor] ([EmpresaId], [ProveedorId], [FechaPago]);
GO



CREATE INDEX [IX_PagoProveedor_ProveedorId] ON [PagoProveedor] ([ProveedorId]);
GO



CREATE UNIQUE INDEX [IX_Permiso_Modulo_Accion] ON [Permiso] ([Modulo], [Accion]);
GO



CREATE INDEX [IX_Producto_EmpresaId] ON [Producto] ([EmpresaId]);
GO



CREATE INDEX [IX_Producto_EmpresaId_Nombre] ON [Producto] ([EmpresaId], [Nombre]);
GO



CREATE INDEX [IX_Proveedor_EmpresaId] ON [Proveedor] ([EmpresaId]);
GO



CREATE INDEX [IX_Proveedor_EmpresaId_NumeroDocumento] ON [Proveedor] ([EmpresaId], [NumeroDocumento]);
GO



CREATE INDEX [IX_Proveedor_EmpresaId_RazonSocial] ON [Proveedor] ([EmpresaId], [RazonSocial]);
GO



CREATE INDEX [IX_RolPermiso_PermisoId] ON [RolPermiso] ([PermisoId]);
GO



CREATE INDEX [IX_RolPermiso_RolId] ON [RolPermiso] ([RolId]);
GO



CREATE INDEX [IX_UsuarioEmpresa_EmpresaId] ON [UsuarioEmpresa] ([EmpresaId]);
GO



CREATE UNIQUE INDEX [IX_UsuarioEmpresa_UsuarioId_EmpresaId] ON [UsuarioEmpresa] ([UsuarioId], [EmpresaId]);
GO



