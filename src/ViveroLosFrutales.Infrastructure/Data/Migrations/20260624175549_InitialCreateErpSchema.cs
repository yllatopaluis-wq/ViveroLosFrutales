using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateErpSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "erp");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "erp",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "erp",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                schema: "erp",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Empresa",
                schema: "erp",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RUC = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NombreComercial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonedaPredeterminada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlNubefact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenNubefact = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LogoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LogoContenido = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LogoContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    LogoNombre = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    SerieBoleta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerieFactura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerieNotaCredito = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SerieNotaCreditoFactura = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SerieNotaCreditoBoleta = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SerieNotaPedido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerieCotizacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.EmpresaId);
                });

            migrationBuilder.CreateTable(
                name: "Moneda",
                schema: "erp",
                columns: table => new
                {
                    MonedaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Simbolo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moneda", x => x.MonedaId);
                });

            migrationBuilder.CreateTable(
                name: "MotivoNotaCredito",
                schema: "erp",
                columns: table => new
                {
                    MotivoNotaCreditoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivoNotaCredito", x => x.MotivoNotaCreditoId);
                });

            migrationBuilder.CreateTable(
                name: "Permiso",
                schema: "erp",
                columns: table => new
                {
                    PermisoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Modulo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permiso", x => x.PermisoId);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                schema: "erp",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "erp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "erp",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "erp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "erp",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "erp",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "erp",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "erp",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "erp",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "erp",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "erp",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "erp",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                schema: "erp",
                columns: table => new
                {
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.CategoriaId);
                    table.ForeignKey(
                        name: "FK_Categoria_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaGasto",
                schema: "erp",
                columns: table => new
                {
                    CategoriaGastoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaGasto", x => x.CategoriaGastoId);
                    table.ForeignKey(
                        name: "FK_CategoriaGasto_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaIngreso",
                schema: "erp",
                columns: table => new
                {
                    CategoriaIngresoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaIngreso", x => x.CategoriaIngresoId);
                    table.ForeignKey(
                        name: "FK_CategoriaIngreso_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionEmpresa",
                schema: "erp",
                columns: table => new
                {
                    ConfiguracionEmpresaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionEmpresa", x => x.ConfiguracionEmpresaId);
                    table.ForeignKey(
                        name: "FK_ConfiguracionEmpresa_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cotizacion",
                schema: "erp",
                columns: table => new
                {
                    CotizacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Correlativo = table.Column<int>(type: "int", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FormaPago = table.Column<int>(type: "int", nullable: false),
                    EmpresaRazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaNombreComercial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaRuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaDireccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaTelefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CondicionesVenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaracteristicasTecnicas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoCotizacion = table.Column<int>(type: "int", nullable: false),
                    PdfUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizacion", x => x.CotizacionId);
                    table.ForeignKey(
                        name: "FK_Cotizacion_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "erp",
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizacion_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ErrorAplicacion",
                schema: "erp",
                columns: table => new
                {
                    ErrorAplicacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: true),
                    FechaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MetodoHttp = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoExcepcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Identificador = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaRevisionUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioRevision = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ObservacionRevision = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorAplicacion", x => x.ErrorAplicacionId);
                    table.ForeignKey(
                        name: "FK_ErrorAplicacion_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientoCaja",
                schema: "erp",
                columns: table => new
                {
                    MovimientoCajaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    ProveedorId = table.Column<int>(type: "int", nullable: true),
                    TipoMovimiento = table.Column<int>(type: "int", nullable: false),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    OrigenId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedioPago = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoCaja", x => x.MovimientoCajaId);
                    table.ForeignKey(
                        name: "FK_MovimientoCaja_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                schema: "erp",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Stock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AfectoIgv = table.Column<bool>(type: "bit", nullable: false),
                    PrecioVentaSinIgv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioVentaConIgv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TieneDetraccion = table.Column<bool>(type: "bit", nullable: false),
                    PorcentajeDetraccion = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.ProductoId);
                    table.ForeignKey(
                        name: "FK_Producto_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                schema: "erp",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NombreComercial = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.ProveedorId);
                    table.ForeignKey(
                        name: "FK_Proveedor_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioEmpresa",
                schema: "erp",
                columns: table => new
                {
                    UsuarioEmpresaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEmpresa", x => x.UsuarioEmpresaId);
                    table.ForeignKey(
                        name: "FK_UsuarioEmpresa_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolPermiso",
                schema: "erp",
                columns: table => new
                {
                    RolPermisoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    PermisoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermiso", x => x.RolPermisoId);
                    table.ForeignKey(
                        name: "FK_RolPermiso_Permiso_PermisoId",
                        column: x => x.PermisoId,
                        principalSchema: "erp",
                        principalTable: "Permiso",
                        principalColumn: "PermisoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolPermiso_Rol_RolId",
                        column: x => x.RolId,
                        principalSchema: "erp",
                        principalTable: "Rol",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotaPedido",
                schema: "erp",
                columns: table => new
                {
                    NotaPedidoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    CotizacionId = table.Column<int>(type: "int", nullable: true),
                    ComprobanteId = table.Column<int>(type: "int", nullable: true),
                    Serie = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Correlativo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCobrado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoDocumento = table.Column<int>(type: "int", nullable: false),
                    EstadoPago = table.Column<int>(type: "int", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaPedido", x => x.NotaPedidoId);
                    table.ForeignKey(
                        name: "FK_NotaPedido_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "erp",
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaPedido_Cotizacion_CotizacionId",
                        column: x => x.CotizacionId,
                        principalSchema: "erp",
                        principalTable: "Cotizacion",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotaPedido_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gasto",
                schema: "erp",
                columns: table => new
                {
                    GastoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoriaGastoId = table.Column<int>(type: "int", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedioPago = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MovimientoCajaId = table.Column<int>(type: "int", nullable: true),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gasto", x => x.GastoId);
                    table.ForeignKey(
                        name: "FK_Gasto_CategoriaGasto_CategoriaGastoId",
                        column: x => x.CategoriaGastoId,
                        principalSchema: "erp",
                        principalTable: "CategoriaGasto",
                        principalColumn: "CategoriaGastoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gasto_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gasto_MovimientoCaja_MovimientoCajaId",
                        column: x => x.MovimientoCajaId,
                        principalSchema: "erp",
                        principalTable: "MovimientoCaja",
                        principalColumn: "MovimientoCajaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingreso",
                schema: "erp",
                columns: table => new
                {
                    IngresoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoriaIngresoId = table.Column<int>(type: "int", nullable: true),
                    TipoIngreso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedioPago = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MovimientoCajaId = table.Column<int>(type: "int", nullable: true),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingreso", x => x.IngresoId);
                    table.ForeignKey(
                        name: "FK_Ingreso_CategoriaIngreso_CategoriaIngresoId",
                        column: x => x.CategoriaIngresoId,
                        principalSchema: "erp",
                        principalTable: "CategoriaIngreso",
                        principalColumn: "CategoriaIngresoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ingreso_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingreso_MovimientoCaja_MovimientoCajaId",
                        column: x => x.MovimientoCajaId,
                        principalSchema: "erp",
                        principalTable: "MovimientoCaja",
                        principalColumn: "MovimientoCajaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionDetalle",
                schema: "erp",
                columns: table => new
                {
                    CotizacionDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteIgv = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionDetalle", x => x.CotizacionDetalleId);
                    table.ForeignKey(
                        name: "FK_CotizacionDetalle_Cotizacion_CotizacionId",
                        column: x => x.CotizacionId,
                        principalSchema: "erp",
                        principalTable: "Cotizacion",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotizacionDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "erp",
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientoInventario",
                schema: "erp",
                columns: table => new
                {
                    MovimientoInventarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockAnterior = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockNuevo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoInventario", x => x.MovimientoInventarioId);
                    table.ForeignKey(
                        name: "FK_MovimientoInventario_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "erp",
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compra",
                schema: "erp",
                columns: table => new
                {
                    CompraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormaPago = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPagado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoPago = table.Column<int>(type: "int", nullable: false),
                    EstadoDocumento = table.Column<int>(type: "int", nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UsuarioAnulacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compra", x => x.CompraId);
                    table.ForeignKey(
                        name: "FK_Compra_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Compra_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalSchema: "erp",
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comprobante",
                schema: "erp",
                columns: table => new
                {
                    ComprobanteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoComprobante = table.Column<int>(type: "int", nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Correlativo = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    CotizacionId = table.Column<int>(type: "int", nullable: true),
                    NotaPedidoId = table.Column<int>(type: "int", nullable: true),
                    ComprobanteReferenciaId = table.Column<int>(type: "int", nullable: true),
                    MotivoNotaCreditoId = table.Column<int>(type: "int", nullable: true),
                    MotivoNotaCredito = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormaPago = table.Column<int>(type: "int", nullable: false),
                    EmpresaRazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaNombreComercial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaRuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaDireccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaTelefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CondicionesVenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaracteristicasTecnicas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPagado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoDetraccion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoSunat = table.Column<int>(type: "int", nullable: false),
                    EstadoPago = table.Column<int>(type: "int", nullable: false),
                    DocumentoImpreso = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PdfUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XmlUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NubefactHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NubefactRespuesta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprobante", x => x.ComprobanteId);
                    table.ForeignKey(
                        name: "FK_Comprobante_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "erp",
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comprobante_Comprobante_ComprobanteReferenciaId",
                        column: x => x.ComprobanteReferenciaId,
                        principalSchema: "erp",
                        principalTable: "Comprobante",
                        principalColumn: "ComprobanteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comprobante_Cotizacion_CotizacionId",
                        column: x => x.CotizacionId,
                        principalSchema: "erp",
                        principalTable: "Cotizacion",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comprobante_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comprobante_MotivoNotaCredito_MotivoNotaCreditoId",
                        column: x => x.MotivoNotaCreditoId,
                        principalSchema: "erp",
                        principalTable: "MotivoNotaCredito",
                        principalColumn: "MotivoNotaCreditoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comprobante_NotaPedido_NotaPedidoId",
                        column: x => x.NotaPedidoId,
                        principalSchema: "erp",
                        principalTable: "NotaPedido",
                        principalColumn: "NotaPedidoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotaPedidoDetalle",
                schema: "erp",
                columns: table => new
                {
                    NotaPedidoDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotaPedidoId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaPedidoDetalle", x => x.NotaPedidoDetalleId);
                    table.ForeignKey(
                        name: "FK_NotaPedidoDetalle_NotaPedido_NotaPedidoId",
                        column: x => x.NotaPedidoId,
                        principalSchema: "erp",
                        principalTable: "NotaPedido",
                        principalColumn: "NotaPedidoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaPedidoDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "erp",
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompraDetalle",
                schema: "erp",
                columns: table => new
                {
                    CompraDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLinea = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraDetalle", x => x.CompraDetalleId);
                    table.ForeignKey(
                        name: "FK_CompraDetalle_Compra_CompraId",
                        column: x => x.CompraId,
                        principalSchema: "erp",
                        principalTable: "Compra",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompraDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "erp",
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagoProveedor",
                schema: "erp",
                columns: table => new
                {
                    PagoProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedioPago = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstadoPago = table.Column<int>(type: "int", nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UsuarioAnulacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoProveedor", x => x.PagoProveedorId);
                    table.ForeignKey(
                        name: "FK_PagoProveedor_Compra_CompraId",
                        column: x => x.CompraId,
                        principalSchema: "erp",
                        principalTable: "Compra",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagoProveedor_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalSchema: "erp",
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CobroCliente",
                schema: "erp",
                columns: table => new
                {
                    CobroClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    NotaPedidoId = table.Column<int>(type: "int", nullable: true),
                    ComprobanteId = table.Column<int>(type: "int", nullable: true),
                    FechaCobro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedioPago = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioAnulacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CobroCliente", x => x.CobroClienteId);
                    table.ForeignKey(
                        name: "FK_CobroCliente_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "erp",
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CobroCliente_Comprobante_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalSchema: "erp",
                        principalTable: "Comprobante",
                        principalColumn: "ComprobanteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CobroCliente_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CobroCliente_NotaPedido_NotaPedidoId",
                        column: x => x.NotaPedidoId,
                        principalSchema: "erp",
                        principalTable: "NotaPedido",
                        principalColumn: "NotaPedidoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComprobanteDetalle",
                schema: "erp",
                columns: table => new
                {
                    ComprobanteDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComprobanteId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteIgv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoDetraccion = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobanteDetalle", x => x.ComprobanteDetalleId);
                    table.ForeignKey(
                        name: "FK_ComprobanteDetalle_Comprobante_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalSchema: "erp",
                        principalTable: "Comprobante",
                        principalColumn: "ComprobanteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComprobanteDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "erp",
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Devolucion",
                schema: "erp",
                columns: table => new
                {
                    DevolucionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoTercero = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    ProveedorId = table.Column<int>(type: "int", nullable: true),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    NotaPedidoId = table.Column<int>(type: "int", nullable: true),
                    ComprobanteId = table.Column<int>(type: "int", nullable: true),
                    NotaCreditoId = table.Column<int>(type: "int", nullable: true),
                    CompraId = table.Column<int>(type: "int", nullable: true),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoOriginal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoDevuelto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoDevolucion = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MotivoGeneracion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devolucion", x => x.DevolucionId);
                    table.ForeignKey(
                        name: "FK_Devolucion_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "erp",
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devolucion_Compra_CompraId",
                        column: x => x.CompraId,
                        principalSchema: "erp",
                        principalTable: "Compra",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devolucion_Comprobante_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalSchema: "erp",
                        principalTable: "Comprobante",
                        principalColumn: "ComprobanteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devolucion_Comprobante_NotaCreditoId",
                        column: x => x.NotaCreditoId,
                        principalSchema: "erp",
                        principalTable: "Comprobante",
                        principalColumn: "ComprobanteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devolucion_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devolucion_NotaPedido_NotaPedidoId",
                        column: x => x.NotaPedidoId,
                        principalSchema: "erp",
                        principalTable: "NotaPedido",
                        principalColumn: "NotaPedidoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devolucion_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalSchema: "erp",
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NubefactOperacion",
                schema: "erp",
                columns: table => new
                {
                    NubefactOperacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComprobanteId = table.Column<int>(type: "int", nullable: false),
                    TipoOperacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoSunat = table.Column<int>(type: "int", nullable: false),
                    PdfUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XmlUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolicitudJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RespuestaCompleta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NubefactOperacion", x => x.NubefactOperacionId);
                    table.ForeignKey(
                        name: "FK_NubefactOperacion_Comprobante_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalSchema: "erp",
                        principalTable: "Comprobante",
                        principalColumn: "ComprobanteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComprobanteCobroAplicado",
                schema: "erp",
                columns: table => new
                {
                    ComprobanteCobroAplicadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    ComprobanteId = table.Column<int>(type: "int", nullable: false),
                    CobroClienteId = table.Column<int>(type: "int", nullable: false),
                    MontoAplicado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobanteCobroAplicado", x => x.ComprobanteCobroAplicadoId);
                    table.ForeignKey(
                        name: "FK_ComprobanteCobroAplicado_CobroCliente_CobroClienteId",
                        column: x => x.CobroClienteId,
                        principalSchema: "erp",
                        principalTable: "CobroCliente",
                        principalColumn: "CobroClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprobanteCobroAplicado_Comprobante_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalSchema: "erp",
                        principalTable: "Comprobante",
                        principalColumn: "ComprobanteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "erp",
                table: "Moneda",
                columns: new[] { "MonedaId", "Codigo", "Descripcion", "Estado", "Simbolo" },
                values: new object[] { 1, "PEN", "Soles", 1, "S/" });

            migrationBuilder.InsertData(
                schema: "erp",
                table: "MotivoNotaCredito",
                columns: new[] { "MotivoNotaCreditoId", "Estado", "FechaRegistro", "Nombre", "UsuarioRegistro" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 6, 24, 17, 55, 42, 375, DateTimeKind.Utc).AddTicks(7293), "Anulacion de la operacion", "" },
                    { 2, 1, new DateTime(2026, 6, 24, 17, 55, 42, 375, DateTimeKind.Utc).AddTicks(7299), "Error en datos del comprobante", "" },
                    { 3, 1, new DateTime(2026, 6, 24, 17, 55, 42, 375, DateTimeKind.Utc).AddTicks(7300), "Devolucion total", "" },
                    { 4, 1, new DateTime(2026, 6, 24, 17, 55, 42, 375, DateTimeKind.Utc).AddTicks(7302), "Descuento posterior", "" }
                });

            migrationBuilder.InsertData(
                schema: "erp",
                table: "Permiso",
                columns: new[] { "PermisoId", "Accion", "Descripcion", "Estado", "Modulo" },
                values: new object[,]
                {
                    { 1, "Ver", "Ver Home", 1, "Home" },
                    { 2, "Ver", "Ver Cotizaciones", 1, "Cotizaciones" },
                    { 3, "Crear", "Crear Cotizaciones", 1, "Cotizaciones" },
                    { 4, "Editar", "Editar Cotizaciones", 1, "Cotizaciones" },
                    { 5, "Anular", "Anular Cotizaciones", 1, "Cotizaciones" },
                    { 6, "Imprimir", "Imprimir Cotizaciones", 1, "Cotizaciones" },
                    { 7, "Convertir", "Convertir Cotizaciones", 1, "Cotizaciones" },
                    { 8, "Ver", "Ver NotasPedido", 1, "NotasPedido" },
                    { 9, "Crear", "Crear NotasPedido", 1, "NotasPedido" },
                    { 10, "Editar", "Editar NotasPedido", 1, "NotasPedido" },
                    { 11, "Anular", "Anular NotasPedido", 1, "NotasPedido" },
                    { 12, "Imprimir", "Imprimir NotasPedido", 1, "NotasPedido" },
                    { 13, "Convertir", "Convertir NotasPedido", 1, "NotasPedido" },
                    { 14, "RegistrarPago", "RegistrarPago NotasPedido", 1, "NotasPedido" },
                    { 15, "Ver", "Ver Comprobantes", 1, "Comprobantes" },
                    { 16, "Crear", "Crear Comprobantes", 1, "Comprobantes" },
                    { 17, "Editar", "Editar Comprobantes", 1, "Comprobantes" },
                    { 18, "Anular", "Anular Comprobantes", 1, "Comprobantes" },
                    { 19, "Imprimir", "Imprimir Comprobantes", 1, "Comprobantes" },
                    { 20, "RegistrarPago", "RegistrarPago Comprobantes", 1, "Comprobantes" },
                    { 21, "ConsultarSunat", "ConsultarSunat Comprobantes", 1, "Comprobantes" },
                    { 22, "Ver", "Ver NotasCredito", 1, "NotasCredito" },
                    { 23, "Crear", "Crear NotasCredito", 1, "NotasCredito" },
                    { 24, "Anular", "Anular NotasCredito", 1, "NotasCredito" },
                    { 25, "Imprimir", "Imprimir NotasCredito", 1, "NotasCredito" },
                    { 26, "Ver", "Ver CobrosClientes", 1, "CobrosClientes" },
                    { 27, "Crear", "Crear CobrosClientes", 1, "CobrosClientes" },
                    { 28, "Anular", "Anular CobrosClientes", 1, "CobrosClientes" },
                    { 29, "Ver", "Ver Productos", 1, "Productos" },
                    { 30, "Crear", "Crear Productos", 1, "Productos" },
                    { 31, "Editar", "Editar Productos", 1, "Productos" },
                    { 32, "Anular", "Anular Productos", 1, "Productos" },
                    { 33, "Ver", "Ver Clientes", 1, "Clientes" },
                    { 34, "Crear", "Crear Clientes", 1, "Clientes" },
                    { 35, "Editar", "Editar Clientes", 1, "Clientes" },
                    { 36, "Anular", "Anular Clientes", 1, "Clientes" },
                    { 37, "Ver", "Ver Proveedores", 1, "Proveedores" },
                    { 38, "Crear", "Crear Proveedores", 1, "Proveedores" },
                    { 39, "Editar", "Editar Proveedores", 1, "Proveedores" },
                    { 40, "Anular", "Anular Proveedores", 1, "Proveedores" },
                    { 41, "Ver", "Ver Categorias", 1, "Categorias" },
                    { 42, "Crear", "Crear Categorias", 1, "Categorias" },
                    { 43, "Editar", "Editar Categorias", 1, "Categorias" },
                    { 44, "Anular", "Anular Categorias", 1, "Categorias" },
                    { 45, "Ver", "Ver Compras", 1, "Compras" },
                    { 46, "Crear", "Crear Compras", 1, "Compras" },
                    { 47, "Anular", "Anular Compras", 1, "Compras" },
                    { 48, "RegistrarPago", "RegistrarPago Compras", 1, "Compras" },
                    { 49, "AnularPago", "AnularPago Compras", 1, "Compras" },
                    { 50, "Ver", "Ver Gastos", 1, "Gastos" },
                    { 51, "Crear", "Crear Gastos", 1, "Gastos" },
                    { 52, "Editar", "Editar Gastos", 1, "Gastos" },
                    { 53, "Anular", "Anular Gastos", 1, "Gastos" },
                    { 54, "Ver", "Ver Ingresos", 1, "Ingresos" },
                    { 55, "Crear", "Crear Ingresos", 1, "Ingresos" },
                    { 56, "Editar", "Editar Ingresos", 1, "Ingresos" },
                    { 57, "Anular", "Anular Ingresos", 1, "Ingresos" },
                    { 58, "Ver", "Ver Devoluciones", 1, "Devoluciones" },
                    { 59, "Registrar", "Registrar Devoluciones", 1, "Devoluciones" },
                    { 60, "Ver", "Ver Caja", 1, "Caja" },
                    { 61, "Ver", "Ver ReporteGeneral", 1, "ReporteGeneral" },
                    { 62, "Ver", "Ver PropuestasComerciales", 1, "PropuestasComerciales" },
                    { 63, "Ver", "Ver CuentasPorPagar", 1, "CuentasPorPagar" },
                    { 64, "Ver", "Ver DevolucionesProveedor", 1, "DevolucionesProveedor" },
                    { 65, "Ver", "Ver ReporteCaja", 1, "ReporteCaja" },
                    { 66, "Ver", "Ver EstadoCuentaClientes", 1, "EstadoCuentaClientes" },
                    { 67, "Ver", "Ver Empresas", 1, "Empresas" },
                    { 68, "Crear", "Crear Empresas", 1, "Empresas" },
                    { 69, "Editar", "Editar Empresas", 1, "Empresas" },
                    { 70, "Anular", "Anular Empresas", 1, "Empresas" },
                    { 71, "Ver", "Ver Usuarios", 1, "Usuarios" },
                    { 72, "Crear", "Crear Usuarios", 1, "Usuarios" },
                    { 73, "Editar", "Editar Usuarios", 1, "Usuarios" },
                    { 74, "RestablecerPassword", "RestablecerPassword Usuarios", 1, "Usuarios" },
                    { 75, "Ver", "Ver Roles", 1, "Roles" },
                    { 76, "Crear", "Crear Roles", 1, "Roles" },
                    { 77, "Editar", "Editar Roles", 1, "Roles" },
                    { 78, "Ver", "Ver Configuracion", 1, "Configuracion" },
                    { 79, "Crear", "Crear Configuracion", 1, "Configuracion" },
                    { 80, "Editar", "Editar Configuracion", 1, "Configuracion" },
                    { 81, "Configurar", "Configurar Configuracion", 1, "Configuracion" },
                    { 82, "Ver", "Ver NubefactLogs", 1, "NubefactLogs" },
                    { 83, "Ver", "Ver ErroresAplicacion", 1, "ErroresAplicacion" },
                    { 84, "Revisar", "Revisar ErroresAplicacion", 1, "ErroresAplicacion" }
                });

            migrationBuilder.InsertData(
                schema: "erp",
                table: "Rol",
                columns: new[] { "RolId", "Descripcion", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1, "Acceso total", 1, "Administrador" },
                    { 2, "Operacion comercial", 1, "Vendedor" }
                });

            migrationBuilder.InsertData(
                schema: "erp",
                table: "RolPermiso",
                columns: new[] { "RolPermisoId", "PermisoId", "RolId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 4, 1 },
                    { 5, 5, 1 },
                    { 6, 6, 1 },
                    { 7, 7, 1 },
                    { 8, 8, 1 },
                    { 9, 9, 1 },
                    { 10, 10, 1 },
                    { 11, 11, 1 },
                    { 12, 12, 1 },
                    { 13, 13, 1 },
                    { 14, 14, 1 },
                    { 15, 15, 1 },
                    { 16, 16, 1 },
                    { 17, 17, 1 },
                    { 18, 18, 1 },
                    { 19, 19, 1 },
                    { 20, 20, 1 },
                    { 21, 21, 1 },
                    { 22, 22, 1 },
                    { 23, 23, 1 },
                    { 24, 24, 1 },
                    { 25, 25, 1 },
                    { 26, 26, 1 },
                    { 27, 27, 1 },
                    { 28, 28, 1 },
                    { 29, 29, 1 },
                    { 30, 30, 1 },
                    { 31, 31, 1 },
                    { 32, 32, 1 },
                    { 33, 33, 1 },
                    { 34, 34, 1 },
                    { 35, 35, 1 },
                    { 36, 36, 1 },
                    { 37, 37, 1 },
                    { 38, 38, 1 },
                    { 39, 39, 1 },
                    { 40, 40, 1 },
                    { 41, 41, 1 },
                    { 42, 42, 1 },
                    { 43, 43, 1 },
                    { 44, 44, 1 },
                    { 45, 45, 1 },
                    { 46, 46, 1 },
                    { 47, 47, 1 },
                    { 48, 48, 1 },
                    { 49, 49, 1 },
                    { 50, 50, 1 },
                    { 51, 51, 1 },
                    { 52, 52, 1 },
                    { 53, 53, 1 },
                    { 54, 54, 1 },
                    { 55, 55, 1 },
                    { 56, 56, 1 },
                    { 57, 57, 1 },
                    { 58, 58, 1 },
                    { 59, 59, 1 },
                    { 60, 60, 1 },
                    { 61, 61, 1 },
                    { 62, 62, 1 },
                    { 63, 63, 1 },
                    { 64, 64, 1 },
                    { 65, 65, 1 },
                    { 66, 66, 1 },
                    { 67, 67, 1 },
                    { 68, 68, 1 },
                    { 69, 69, 1 },
                    { 70, 70, 1 },
                    { 71, 71, 1 },
                    { 72, 72, 1 },
                    { 73, 73, 1 },
                    { 74, 74, 1 },
                    { 75, 75, 1 },
                    { 76, 76, 1 },
                    { 77, 77, 1 },
                    { 78, 78, 1 },
                    { 79, 79, 1 },
                    { 80, 80, 1 },
                    { 81, 81, 1 },
                    { 82, 82, 1 },
                    { 83, 83, 1 },
                    { 84, 84, 1 },
                    { 85, 1, 2 },
                    { 86, 2, 2 },
                    { 87, 3, 2 },
                    { 88, 4, 2 },
                    { 89, 5, 2 },
                    { 90, 6, 2 },
                    { 91, 7, 2 },
                    { 92, 8, 2 },
                    { 93, 9, 2 },
                    { 94, 10, 2 },
                    { 95, 11, 2 },
                    { 96, 12, 2 },
                    { 97, 13, 2 },
                    { 98, 14, 2 },
                    { 99, 15, 2 },
                    { 100, 16, 2 },
                    { 101, 17, 2 },
                    { 102, 18, 2 },
                    { 103, 19, 2 },
                    { 104, 20, 2 },
                    { 105, 22, 2 },
                    { 106, 23, 2 },
                    { 107, 24, 2 },
                    { 108, 25, 2 },
                    { 109, 26, 2 },
                    { 110, 27, 2 },
                    { 111, 28, 2 },
                    { 112, 29, 2 },
                    { 113, 30, 2 },
                    { 114, 31, 2 },
                    { 115, 32, 2 },
                    { 116, 33, 2 },
                    { 117, 34, 2 },
                    { 118, 35, 2 },
                    { 119, 36, 2 },
                    { 120, 41, 2 },
                    { 121, 42, 2 },
                    { 122, 43, 2 },
                    { 123, 44, 2 },
                    { 124, 58, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "erp",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "erp",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "erp",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "erp",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "erp",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "erp",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "erp",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_EmpresaId",
                schema: "erp",
                table: "Categoria",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_EmpresaId_Nombre",
                schema: "erp",
                table: "Categoria",
                columns: new[] { "EmpresaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaGasto_EmpresaId_Nombre",
                schema: "erp",
                table: "CategoriaGasto",
                columns: new[] { "EmpresaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaIngreso_EmpresaId_Nombre",
                schema: "erp",
                table: "CategoriaIngreso",
                columns: new[] { "EmpresaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_NombreCompleto",
                schema: "erp",
                table: "Cliente",
                column: "NombreCompleto");

            migrationBuilder.CreateIndex(
                name: "UX_Cliente_Tipo_Numero",
                schema: "erp",
                table: "Cliente",
                columns: new[] { "TipoDocumento", "NumeroDocumento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_ClienteId",
                schema: "erp",
                table: "CobroCliente",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_ComprobanteId",
                schema: "erp",
                table: "CobroCliente",
                column: "ComprobanteId");

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_EmpresaId_ClienteId_FechaCobro",
                schema: "erp",
                table: "CobroCliente",
                columns: new[] { "EmpresaId", "ClienteId", "FechaCobro" });

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_EmpresaId_ComprobanteId",
                schema: "erp",
                table: "CobroCliente",
                columns: new[] { "EmpresaId", "ComprobanteId" });

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_EmpresaId_NotaPedidoId",
                schema: "erp",
                table: "CobroCliente",
                columns: new[] { "EmpresaId", "NotaPedidoId" });

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_NotaPedidoId",
                schema: "erp",
                table: "CobroCliente",
                column: "NotaPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Compra_EmpresaId",
                schema: "erp",
                table: "Compra",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Compra_EmpresaId_Fecha",
                schema: "erp",
                table: "Compra",
                columns: new[] { "EmpresaId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Compra_EmpresaId_ProveedorId",
                schema: "erp",
                table: "Compra",
                columns: new[] { "EmpresaId", "ProveedorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Compra_EmpresaId_ProveedorId_TipoDocumento_Serie_Numero",
                schema: "erp",
                table: "Compra",
                columns: new[] { "EmpresaId", "ProveedorId", "TipoDocumento", "Serie", "Numero" });

            migrationBuilder.CreateIndex(
                name: "IX_Compra_ProveedorId",
                schema: "erp",
                table: "Compra",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalle_CompraId",
                schema: "erp",
                table: "CompraDetalle",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalle_ProductoId",
                schema: "erp",
                table: "CompraDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_ClienteId",
                schema: "erp",
                table: "Comprobante",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_ComprobanteReferenciaId",
                schema: "erp",
                table: "Comprobante",
                column: "ComprobanteReferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_CotizacionId",
                schema: "erp",
                table: "Comprobante",
                column: "CotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_EmpresaId",
                schema: "erp",
                table: "Comprobante",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_EmpresaId_ComprobanteReferenciaId",
                schema: "erp",
                table: "Comprobante",
                columns: new[] { "EmpresaId", "ComprobanteReferenciaId" });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_EmpresaId_CotizacionId",
                schema: "erp",
                table: "Comprobante",
                columns: new[] { "EmpresaId", "CotizacionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_EmpresaId_EstadoSunat",
                schema: "erp",
                table: "Comprobante",
                columns: new[] { "EmpresaId", "EstadoSunat" });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_EmpresaId_FechaEmision",
                schema: "erp",
                table: "Comprobante",
                columns: new[] { "EmpresaId", "FechaEmision" });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_EmpresaId_NotaPedidoId",
                schema: "erp",
                table: "Comprobante",
                columns: new[] { "EmpresaId", "NotaPedidoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_EmpresaId_Serie_Correlativo",
                schema: "erp",
                table: "Comprobante",
                columns: new[] { "EmpresaId", "Serie", "Correlativo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_EmpresaId_TipoComprobante",
                schema: "erp",
                table: "Comprobante",
                columns: new[] { "EmpresaId", "TipoComprobante" });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_MotivoNotaCreditoId",
                schema: "erp",
                table: "Comprobante",
                column: "MotivoNotaCreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_NotaPedidoId",
                schema: "erp",
                table: "Comprobante",
                column: "NotaPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobanteCobroAplicado_CobroClienteId",
                schema: "erp",
                table: "ComprobanteCobroAplicado",
                column: "CobroClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobanteCobroAplicado_ComprobanteId",
                schema: "erp",
                table: "ComprobanteCobroAplicado",
                column: "ComprobanteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobanteCobroAplicado_EmpresaId_CobroClienteId",
                schema: "erp",
                table: "ComprobanteCobroAplicado",
                columns: new[] { "EmpresaId", "CobroClienteId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComprobanteCobroAplicado_EmpresaId_ComprobanteId",
                schema: "erp",
                table: "ComprobanteCobroAplicado",
                columns: new[] { "EmpresaId", "ComprobanteId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComprobanteDetalle_ComprobanteId",
                schema: "erp",
                table: "ComprobanteDetalle",
                column: "ComprobanteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobanteDetalle_ProductoId",
                schema: "erp",
                table: "ComprobanteDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionEmpresa_EmpresaId_Clave",
                schema: "erp",
                table: "ConfiguracionEmpresa",
                columns: new[] { "EmpresaId", "Clave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_ClienteId",
                schema: "erp",
                table: "Cotizacion",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_EmpresaId",
                schema: "erp",
                table: "Cotizacion",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_EmpresaId_ClienteId_FechaEmision",
                schema: "erp",
                table: "Cotizacion",
                columns: new[] { "EmpresaId", "ClienteId", "FechaEmision" });

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_EmpresaId_Serie_Correlativo",
                schema: "erp",
                table: "Cotizacion",
                columns: new[] { "EmpresaId", "Serie", "Correlativo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionDetalle_CotizacionId",
                schema: "erp",
                table: "CotizacionDetalle",
                column: "CotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionDetalle_ProductoId",
                schema: "erp",
                table: "CotizacionDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_ClienteId",
                schema: "erp",
                table: "Devolucion",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_CompraId",
                schema: "erp",
                table: "Devolucion",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_ComprobanteId",
                schema: "erp",
                table: "Devolucion",
                column: "ComprobanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_ClienteId",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "ClienteId" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_CompraId",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "CompraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_ComprobanteId",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "ComprobanteId" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_FechaGeneracion",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "FechaGeneracion" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_NotaCreditoId",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "NotaCreditoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_NotaPedidoId",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "NotaPedidoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_ProveedorId",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "ProveedorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_TipoTercero",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "TipoTercero" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_NotaCreditoId",
                schema: "erp",
                table: "Devolucion",
                column: "NotaCreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_NotaPedidoId",
                schema: "erp",
                table: "Devolucion",
                column: "NotaPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_ProveedorId",
                schema: "erp",
                table: "Devolucion",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_RUC",
                schema: "erp",
                table: "Empresa",
                column: "RUC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErrorAplicacion_EmpresaId_Estado",
                schema: "erp",
                table: "ErrorAplicacion",
                columns: new[] { "EmpresaId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorAplicacion_EmpresaId_FechaUtc",
                schema: "erp",
                table: "ErrorAplicacion",
                columns: new[] { "EmpresaId", "FechaUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_CategoriaGastoId",
                schema: "erp",
                table: "Gasto",
                column: "CategoriaGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_EmpresaId",
                schema: "erp",
                table: "Gasto",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_EmpresaId_Fecha",
                schema: "erp",
                table: "Gasto",
                columns: new[] { "EmpresaId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_MovimientoCajaId",
                schema: "erp",
                table: "Gasto",
                column: "MovimientoCajaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_CategoriaIngresoId",
                schema: "erp",
                table: "Ingreso",
                column: "CategoriaIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_EmpresaId",
                schema: "erp",
                table: "Ingreso",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_EmpresaId_Fecha",
                schema: "erp",
                table: "Ingreso",
                columns: new[] { "EmpresaId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_MovimientoCajaId",
                schema: "erp",
                table: "Ingreso",
                column: "MovimientoCajaId");

            migrationBuilder.CreateIndex(
                name: "IX_Moneda_Codigo",
                schema: "erp",
                table: "Moneda",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotivoNotaCredito_Nombre",
                schema: "erp",
                table: "MotivoNotaCredito",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_EmpresaId_ClienteId",
                schema: "erp",
                table: "MovimientoCaja",
                columns: new[] { "EmpresaId", "ClienteId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_EmpresaId_Fecha",
                schema: "erp",
                table: "MovimientoCaja",
                columns: new[] { "EmpresaId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_EmpresaId_Origen_OrigenId",
                schema: "erp",
                table: "MovimientoCaja",
                columns: new[] { "EmpresaId", "Origen", "OrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_EmpresaId_ProveedorId",
                schema: "erp",
                table: "MovimientoCaja",
                columns: new[] { "EmpresaId", "ProveedorId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_EmpresaId",
                schema: "erp",
                table: "MovimientoInventario",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_EmpresaId_ProductoId_Fecha",
                schema: "erp",
                table: "MovimientoInventario",
                columns: new[] { "EmpresaId", "ProductoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_ProductoId",
                schema: "erp",
                table: "MovimientoInventario",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedido_ClienteId",
                schema: "erp",
                table: "NotaPedido",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedido_CotizacionId",
                schema: "erp",
                table: "NotaPedido",
                column: "CotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedido_EmpresaId",
                schema: "erp",
                table: "NotaPedido",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedido_EmpresaId_ClienteId_Fecha",
                schema: "erp",
                table: "NotaPedido",
                columns: new[] { "EmpresaId", "ClienteId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedido_EmpresaId_ComprobanteId",
                schema: "erp",
                table: "NotaPedido",
                columns: new[] { "EmpresaId", "ComprobanteId" });

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedido_EmpresaId_CotizacionId",
                schema: "erp",
                table: "NotaPedido",
                columns: new[] { "EmpresaId", "CotizacionId" });

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedido_EmpresaId_Serie_Correlativo",
                schema: "erp",
                table: "NotaPedido",
                columns: new[] { "EmpresaId", "Serie", "Correlativo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedidoDetalle_NotaPedidoId",
                schema: "erp",
                table: "NotaPedidoDetalle",
                column: "NotaPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaPedidoDetalle_ProductoId",
                schema: "erp",
                table: "NotaPedidoDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_NubefactOperacion_ComprobanteId",
                schema: "erp",
                table: "NubefactOperacion",
                column: "ComprobanteId");

            migrationBuilder.CreateIndex(
                name: "IX_NubefactOperacion_EmpresaId",
                schema: "erp",
                table: "NubefactOperacion",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_CompraId",
                schema: "erp",
                table: "PagoProveedor",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_EmpresaId_CompraId",
                schema: "erp",
                table: "PagoProveedor",
                columns: new[] { "EmpresaId", "CompraId" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_EmpresaId_ProveedorId_FechaPago",
                schema: "erp",
                table: "PagoProveedor",
                columns: new[] { "EmpresaId", "ProveedorId", "FechaPago" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_ProveedorId",
                schema: "erp",
                table: "PagoProveedor",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Permiso_Modulo_Accion",
                schema: "erp",
                table: "Permiso",
                columns: new[] { "Modulo", "Accion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producto_EmpresaId",
                schema: "erp",
                table: "Producto",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_EmpresaId_Nombre",
                schema: "erp",
                table: "Producto",
                columns: new[] { "EmpresaId", "Nombre" });

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_EmpresaId",
                schema: "erp",
                table: "Proveedor",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_EmpresaId_NumeroDocumento",
                schema: "erp",
                table: "Proveedor",
                columns: new[] { "EmpresaId", "NumeroDocumento" });

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_EmpresaId_RazonSocial",
                schema: "erp",
                table: "Proveedor",
                columns: new[] { "EmpresaId", "RazonSocial" });

            migrationBuilder.CreateIndex(
                name: "IX_RolPermiso_PermisoId",
                schema: "erp",
                table: "RolPermiso",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermiso_RolId",
                schema: "erp",
                table: "RolPermiso",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEmpresa_EmpresaId",
                schema: "erp",
                table: "UsuarioEmpresa",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEmpresa_UsuarioId_EmpresaId",
                schema: "erp",
                table: "UsuarioEmpresa",
                columns: new[] { "UsuarioId", "EmpresaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Categoria",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CompraDetalle",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "ComprobanteCobroAplicado",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "ComprobanteDetalle",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "ConfiguracionEmpresa",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CotizacionDetalle",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Devolucion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "ErrorAplicacion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Gasto",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Ingreso",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Moneda",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "MovimientoInventario",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "NotaPedidoDetalle",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "NubefactOperacion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "PagoProveedor",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "RolPermiso",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "UsuarioEmpresa",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CobroCliente",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CategoriaGasto",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CategoriaIngreso",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "MovimientoCaja",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Producto",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Compra",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Permiso",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Rol",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Comprobante",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Proveedor",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "MotivoNotaCredito",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "NotaPedido",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Cotizacion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Cliente",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "Empresa",
                schema: "erp");
        }
    }
}
