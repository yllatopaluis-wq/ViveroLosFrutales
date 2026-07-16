using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdenCompraYPagoProveedorAplicacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CompraId",
                schema: "erp",
                table: "PagoProveedor",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FormaPago",
                schema: "erp",
                table: "NotaPedido",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Observacion",
                schema: "erp",
                table: "NotaPedido",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                schema: "erp",
                table: "Ingreso",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentoReferencia",
                schema: "erp",
                table: "Ingreso",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "Gasto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentoReferencia",
                schema: "erp",
                table: "Gasto",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProveedorId",
                schema: "erp",
                table: "Gasto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FirmaContenido",
                schema: "erp",
                table: "Empresa",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirmaContentType",
                schema: "erp",
                table: "Empresa",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirmaNombre",
                schema: "erp",
                table: "Empresa",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepresentanteLegalCargo",
                schema: "erp",
                table: "Empresa",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepresentanteLegalDocumento",
                schema: "erp",
                table: "Empresa",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepresentanteLegalNombre",
                schema: "erp",
                table: "Empresa",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrdenCompraId",
                schema: "erp",
                table: "Devolucion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PagoProveedorId",
                schema: "erp",
                table: "Devolucion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadRecibida",
                schema: "erp",
                table: "CompraDetalle",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DiasCredito",
                schema: "erp",
                table: "Compra",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoEntrega",
                schema: "erp",
                table: "Compra",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimiento",
                schema: "erp",
                table: "Compra",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Moneda",
                schema: "erp",
                table: "Compra",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrdenCompraId",
                schema: "erp",
                table: "Compra",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TipoCambio",
                schema: "erp",
                table: "Compra",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                schema: "erp",
                table: "Cliente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CondicionComercialPlantilla",
                schema: "erp",
                columns: table => new
                {
                    CondicionComercialPlantillaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: true),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    TipoDocumento = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    EsPredeterminada = table.Column<bool>(type: "bit", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionComercialPlantilla", x => x.CondicionComercialPlantillaId);
                    table.ForeignKey(
                        name: "FK_CondicionComercialPlantilla_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionCondicionSnapshot",
                schema: "erp",
                columns: table => new
                {
                    CotizacionCondicionSnapshotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    Etiqueta = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionCondicionSnapshot", x => x.CotizacionCondicionSnapshotId);
                    table.ForeignKey(
                        name: "FK_CotizacionCondicionSnapshot_Cotizacion_CotizacionId",
                        column: x => x.CotizacionId,
                        principalSchema: "erp",
                        principalTable: "Cotizacion",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentaFinanciera",
                schema: "erp",
                columns: table => new
                {
                    CuentaFinancieraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Banco = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    NumeroCuenta = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SaldoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaSaldoInicial = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaFinanciera", x => x.CuentaFinancieraId);
                    table.ForeignKey(
                        name: "FK_CuentaFinanciera_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormularioConfiguracion",
                schema: "erp",
                columns: table => new
                {
                    FormularioConfiguracionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: true),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    TipoDocumento = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioConfiguracion", x => x.FormularioConfiguracionId);
                    table.ForeignKey(
                        name: "FK_FormularioConfiguracion_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompra",
                schema: "erp",
                columns: table => new
                {
                    OrdenCompraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correlativo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaEntregaEsperada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LugarEntrega = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FormaPago = table.Column<int>(type: "int", nullable: false),
                    CondicionPago = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CondicionEntrega = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Garantia = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PorcentajeAdelanto = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PlazoDias = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFacturado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPagado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAplicado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDevuelto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoDisponible = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendienteFacturar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoDocumento = table.Column<int>(type: "int", nullable: false),
                    EstadoAprobacion = table.Column<int>(type: "int", nullable: false),
                    EstadoFacturacion = table.Column<int>(type: "int", nullable: false),
                    EstadoRecepcion = table.Column<int>(type: "int", nullable: false),
                    EstadoFinanciero = table.Column<int>(type: "int", nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UsuarioAnulacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ProveedorTipoDocumento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProveedorNumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProveedorRazonSocial = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ProveedorNombreComercial = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ProveedorDireccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProveedorTelefono = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProveedorEmail = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompra", x => x.OrdenCompraId);
                    table.ForeignKey(
                        name: "FK_OrdenCompra_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenCompra_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalSchema: "erp",
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagoProveedorAplicacion",
                schema: "erp",
                columns: table => new
                {
                    PagoProveedorAplicacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    PagoProveedorId = table.Column<int>(type: "int", nullable: false),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    MontoAplicado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    UsuarioAnulacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoProveedorAplicacion", x => x.PagoProveedorAplicacionId);
                    table.ForeignKey(
                        name: "FK_PagoProveedorAplicacion_Compra_CompraId",
                        column: x => x.CompraId,
                        principalSchema: "erp",
                        principalTable: "Compra",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagoProveedorAplicacion_PagoProveedor_PagoProveedorId",
                        column: x => x.PagoProveedorId,
                        principalSchema: "erp",
                        principalTable: "PagoProveedor",
                        principalColumn: "PagoProveedorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlantillaDocumento",
                schema: "erp",
                columns: table => new
                {
                    PlantillaDocumentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: true),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    TipoDocumento = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    EsPredeterminada = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillaDocumento", x => x.PlantillaDocumentoId);
                    table.ForeignKey(
                        name: "FK_PlantillaDocumento_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CondicionComercialItem",
                schema: "erp",
                columns: table => new
                {
                    CondicionComercialItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CondicionComercialPlantillaId = table.Column<int>(type: "int", nullable: false),
                    Etiqueta = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionComercialItem", x => x.CondicionComercialItemId);
                    table.ForeignKey(
                        name: "FK_CondicionComercialItem_CondicionComercialPlantilla_CondicionComercialPlantillaId",
                        column: x => x.CondicionComercialPlantillaId,
                        principalSchema: "erp",
                        principalTable: "CondicionComercialPlantilla",
                        principalColumn: "CondicionComercialPlantillaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferenciaFinanciera",
                schema: "erp",
                columns: table => new
                {
                    TransferenciaFinancieraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CuentaOrigenId = table.Column<int>(type: "int", nullable: false),
                    CuentaDestinoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MovimientoEgresoId = table.Column<int>(type: "int", nullable: true),
                    MovimientoIngresoId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_TransferenciaFinanciera", x => x.TransferenciaFinancieraId);
                    table.ForeignKey(
                        name: "FK_TransferenciaFinanciera_CuentaFinanciera_CuentaDestinoId",
                        column: x => x.CuentaDestinoId,
                        principalSchema: "erp",
                        principalTable: "CuentaFinanciera",
                        principalColumn: "CuentaFinancieraId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferenciaFinanciera_CuentaFinanciera_CuentaOrigenId",
                        column: x => x.CuentaOrigenId,
                        principalSchema: "erp",
                        principalTable: "CuentaFinanciera",
                        principalColumn: "CuentaFinancieraId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferenciaFinanciera_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "erp",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferenciaFinanciera_MovimientoCaja_MovimientoEgresoId",
                        column: x => x.MovimientoEgresoId,
                        principalSchema: "erp",
                        principalTable: "MovimientoCaja",
                        principalColumn: "MovimientoCajaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferenciaFinanciera_MovimientoCaja_MovimientoIngresoId",
                        column: x => x.MovimientoIngresoId,
                        principalSchema: "erp",
                        principalTable: "MovimientoCaja",
                        principalColumn: "MovimientoCajaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormularioBloqueConfiguracion",
                schema: "erp",
                columns: table => new
                {
                    FormularioBloqueConfiguracionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormularioConfiguracionId = table.Column<int>(type: "int", nullable: false),
                    Bloque = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Colapsado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioBloqueConfiguracion", x => x.FormularioBloqueConfiguracionId);
                    table.ForeignKey(
                        name: "FK_FormularioBloqueConfiguracion_FormularioConfiguracion_FormularioConfiguracionId",
                        column: x => x.FormularioConfiguracionId,
                        principalSchema: "erp",
                        principalTable: "FormularioConfiguracion",
                        principalColumn: "FormularioConfiguracionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormularioBloqueProductoConfiguracion",
                schema: "erp",
                columns: table => new
                {
                    FormularioBloqueProductoConfiguracionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormularioConfiguracionId = table.Column<int>(type: "int", nullable: false),
                    UnirProductosDuplicados = table.Column<bool>(type: "bit", nullable: false),
                    CantidadInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PermitirEditarPrecio = table.Column<bool>(type: "bit", nullable: false),
                    PermitirDescuento = table.Column<bool>(type: "bit", nullable: false),
                    MostrarStock = table.Column<bool>(type: "bit", nullable: false),
                    BloquearSinStock = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioBloqueProductoConfiguracion", x => x.FormularioBloqueProductoConfiguracionId);
                    table.ForeignKey(
                        name: "FK_FormularioBloqueProductoConfiguracion_FormularioConfiguracion_FormularioConfiguracionId",
                        column: x => x.FormularioConfiguracionId,
                        principalSchema: "erp",
                        principalTable: "FormularioConfiguracion",
                        principalColumn: "FormularioConfiguracionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormularioCampoConfiguracion",
                schema: "erp",
                columns: table => new
                {
                    FormularioCampoConfiguracionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormularioConfiguracionId = table.Column<int>(type: "int", nullable: false),
                    Bloque = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Campo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Etiqueta = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false),
                    Obligatorio = table.Column<bool>(type: "bit", nullable: false),
                    SoloLectura = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Ancho = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ValorDefecto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioCampoConfiguracion", x => x.FormularioCampoConfiguracionId);
                    table.ForeignKey(
                        name: "FK_FormularioCampoConfiguracion_FormularioConfiguracion_FormularioConfiguracionId",
                        column: x => x.FormularioConfiguracionId,
                        principalSchema: "erp",
                        principalTable: "FormularioConfiguracion",
                        principalColumn: "FormularioConfiguracionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompraDetalle",
                schema: "erp",
                columns: table => new
                {
                    OrdenCompraDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenCompraId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    CantidadFacturada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadRecibida = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompraDetalle", x => x.OrdenCompraDetalleId);
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalle_OrdenCompra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalSchema: "erp",
                        principalTable: "OrdenCompra",
                        principalColumn: "OrdenCompraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "erp",
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlantillaDocumentoBloque",
                schema: "erp",
                columns: table => new
                {
                    PlantillaDocumentoBloqueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantillaDocumentoId = table.Column<int>(type: "int", nullable: false),
                    Bloque = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillaDocumentoBloque", x => x.PlantillaDocumentoBloqueId);
                    table.ForeignKey(
                        name: "FK_PlantillaDocumentoBloque_PlantillaDocumento_PlantillaDocumentoId",
                        column: x => x.PlantillaDocumentoId,
                        principalSchema: "erp",
                        principalTable: "PlantillaDocumento",
                        principalColumn: "PlantillaDocumentoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 15, 18, 45, 34, 633, DateTimeKind.Unspecified).AddTicks(8957));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 15, 18, 45, 34, 633, DateTimeKind.Unspecified).AddTicks(9009));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 15, 18, 45, 34, 633, DateTimeKind.Unspecified).AddTicks(9012));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 15, 18, 45, 34, 633, DateTimeKind.Unspecified).AddTicks(9014));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 26,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver TESORERIA", "TESORERIA" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 27,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver TESORERIA_CAJA", "TESORERIA_CAJA" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 28,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver TESORERIA_CAJABANCOS", "TESORERIA_CAJABANCOS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 29,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver TESORERIA_CUENTASFINANCIERAS", "TESORERIA_CUENTASFINANCIERAS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 30,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Crear TESORERIA_CUENTASFINANCIERAS", "TESORERIA_CUENTASFINANCIERAS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 31,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Editar TESORERIA_CUENTASFINANCIERAS", "TESORERIA_CUENTASFINANCIERAS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 32,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Anular TESORERIA_CUENTASFINANCIERAS", "TESORERIA_CUENTASFINANCIERAS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 33,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver TESORERIA_COBROS", "TESORERIA_COBROS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 34,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Crear TESORERIA_COBROS", "TESORERIA_COBROS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 35,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular TESORERIA_COBROS", "TESORERIA_COBROS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 36,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver TESORERIA_PAGOSPROVEEDORES", "TESORERIA_PAGOSPROVEEDORES" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 37,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Registrar", "Registrar TESORERIA_PAGOSPROVEEDORES", "TESORERIA_PAGOSPROVEEDORES" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 38,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver TESORERIA_TRANSFERENCIAS", "TESORERIA_TRANSFERENCIAS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 39,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear TESORERIA_TRANSFERENCIAS", "TESORERIA_TRANSFERENCIAS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 40,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Anular TESORERIA_TRANSFERENCIAS", "TESORERIA_TRANSFERENCIAS" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 41,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver TESORERIA_CUENTASCLIENTES", "TESORERIA_CUENTASCLIENTES" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 42,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver TESORERIA_CUENTASPROVEEDORES", "TESORERIA_CUENTASPROVEEDORES" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 43,
                columns: new[] { "Accion", "Descripcion" },
                values: new object[] { "Ver", "Ver Categorias" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 44,
                columns: new[] { "Accion", "Descripcion" },
                values: new object[] { "Crear", "Crear Categorias" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 45,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Categorias", "Categorias" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 46,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Categorias", "Categorias" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 47,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Productos", "Productos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 48,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Productos", "Productos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 49,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Productos", "Productos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 50,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Productos", "Productos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 51,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Clientes", "Clientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 52,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Clientes", "Clientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 53,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Clientes", "Clientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 54,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Clientes", "Clientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 55,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Proveedores", "Proveedores" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 56,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Proveedores", "Proveedores" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 57,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Proveedores", "Proveedores" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 58,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Proveedores", "Proveedores" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 59,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 60,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 61,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 62,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "RegistrarPago", "RegistrarPago Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 63,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "AnularPago", "AnularPago Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 64,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 65,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Registrar", "Registrar OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 66,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 67,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Aprobar", "Aprobar OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 68,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "RegistrarPago", "RegistrarPago OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 69,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "RegistrarCompra", "RegistrarCompra OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 70,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "AplicarPagos", "AplicarPagos OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 71,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "SolicitarDevolucion", "SolicitarDevolucion OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 72,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Cerrar", "Cerrar OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 73,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular OrdenesCompra", "OrdenesCompra" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 74,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Aplicar", "Aplicar PagoProveedorAplicacion", "PagoProveedorAplicacion" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 75,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "AnularAplicacion", "AnularAplicacion PagoProveedorAplicacion", "PagoProveedorAplicacion" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 76,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Gastos", "Gastos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 77,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Gastos", "Gastos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 78,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Gastos", "Gastos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 79,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Gastos", "Gastos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 80,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Ingresos", "Ingresos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 81,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Ingresos", "Ingresos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 82,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Ingresos", "Ingresos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 83,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Ingresos", "Ingresos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 84,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Devoluciones", "Devoluciones" });

            migrationBuilder.InsertData(
                schema: "erp",
                table: "Permiso",
                columns: new[] { "PermisoId", "Accion", "Descripcion", "Estado", "Modulo" },
                values: new object[,]
                {
                    { 85, "Registrar", "Registrar Devoluciones", 1, "Devoluciones" },
                    { 86, "Ver", "Ver Caja", 1, "Caja" },
                    { 87, "Ver", "Ver ReporteGeneral", 1, "ReporteGeneral" },
                    { 88, "Ver", "Ver PropuestasComerciales", 1, "PropuestasComerciales" },
                    { 89, "Ver", "Ver ReporteNotasPedido", 1, "ReporteNotasPedido" },
                    { 90, "Ver", "Ver ReporteComprobantes", 1, "ReporteComprobantes" },
                    { 91, "Ver", "Ver CuentasPorPagar", 1, "CuentasPorPagar" },
                    { 92, "Ver", "Ver DevolucionesProveedor", 1, "DevolucionesProveedor" },
                    { 93, "Ver", "Ver ReporteCaja", 1, "ReporteCaja" },
                    { 94, "Ver", "Ver EstadoCuentaClientes", 1, "EstadoCuentaClientes" },
                    { 95, "Ver", "Ver Empresas", 1, "Empresas" },
                    { 96, "Crear", "Crear Empresas", 1, "Empresas" },
                    { 97, "Editar", "Editar Empresas", 1, "Empresas" },
                    { 98, "Anular", "Anular Empresas", 1, "Empresas" },
                    { 99, "Ver", "Ver Usuarios", 1, "Usuarios" },
                    { 100, "Crear", "Crear Usuarios", 1, "Usuarios" },
                    { 101, "Editar", "Editar Usuarios", 1, "Usuarios" },
                    { 102, "RestablecerPassword", "RestablecerPassword Usuarios", 1, "Usuarios" },
                    { 103, "Ver", "Ver Roles", 1, "Roles" },
                    { 104, "Crear", "Crear Roles", 1, "Roles" },
                    { 105, "Editar", "Editar Roles", 1, "Roles" },
                    { 106, "Ver", "Ver Configuracion", 1, "Configuracion" },
                    { 107, "Crear", "Crear Configuracion", 1, "Configuracion" },
                    { 108, "Editar", "Editar Configuracion", 1, "Configuracion" },
                    { 109, "Configurar", "Configurar Configuracion", 1, "Configuracion" },
                    { 110, "Ver", "Ver NubefactLogs", 1, "NubefactLogs" },
                    { 111, "Ver", "Ver ErroresAplicacion", 1, "ErroresAplicacion" },
                    { 112, "Revisar", "Revisar ErroresAplicacion", 1, "ErroresAplicacion" }
                });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 85,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 85, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 86,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 86, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 87,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 87, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 88,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 88, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 89,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 89, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 90,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 90, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 91,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 91, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 92,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 92, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 93,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 93, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 94,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 94, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 95,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 95, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 96,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 96, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 97,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 97, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 98,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 98, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 99,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 99, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 100,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 100, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 101,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 101, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 102,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 102, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 103,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 103, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 104,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 104, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 105,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 105, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 106,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 106, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 107,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 107, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 108,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 108, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 109,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 109, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 110,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 110, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 111,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 111, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 112,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 112, 1 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 113,
                column: "PermisoId",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 114,
                column: "PermisoId",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 115,
                column: "PermisoId",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 116,
                column: "PermisoId",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 117,
                column: "PermisoId",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 118,
                column: "PermisoId",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 119,
                column: "PermisoId",
                value: 7);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 120,
                column: "PermisoId",
                value: 8);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 121,
                column: "PermisoId",
                value: 9);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 122,
                column: "PermisoId",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 123,
                column: "PermisoId",
                value: 11);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 124,
                column: "PermisoId",
                value: 12);

            migrationBuilder.InsertData(
                schema: "erp",
                table: "RolPermiso",
                columns: new[] { "RolPermisoId", "PermisoId", "RolId" },
                values: new object[,]
                {
                    { 125, 13, 2 },
                    { 126, 14, 2 },
                    { 127, 15, 2 },
                    { 128, 16, 2 },
                    { 129, 17, 2 },
                    { 130, 18, 2 },
                    { 131, 19, 2 },
                    { 132, 20, 2 },
                    { 133, 22, 2 },
                    { 134, 23, 2 },
                    { 135, 24, 2 },
                    { 136, 25, 2 },
                    { 137, 26, 2 },
                    { 138, 27, 2 },
                    { 139, 28, 2 },
                    { 140, 33, 2 },
                    { 141, 34, 2 },
                    { 142, 35, 2 },
                    { 143, 38, 2 },
                    { 144, 39, 2 },
                    { 145, 40, 2 },
                    { 146, 41, 2 },
                    { 147, 43, 2 },
                    { 148, 44, 2 },
                    { 149, 45, 2 },
                    { 150, 46, 2 },
                    { 151, 47, 2 },
                    { 152, 48, 2 },
                    { 153, 49, 2 },
                    { 154, 50, 2 },
                    { 155, 51, 2 },
                    { 156, 52, 2 },
                    { 157, 53, 2 },
                    { 158, 54, 2 },
                    { 159, 84, 2 },
                    { 160, 85, 2 },
                    { 161, 86, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor",
                column: "CuentaFinancieraId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_EmpresaId_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor",
                columns: new[] { "EmpresaId", "OrdenCompraId" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja",
                column: "CuentaFinancieraId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_ClienteId",
                schema: "erp",
                table: "Ingreso",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso",
                column: "CuentaFinancieraId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_EmpresaId_ClienteId",
                schema: "erp",
                table: "Ingreso",
                columns: new[] { "EmpresaId", "ClienteId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_CuentaFinancieraId",
                schema: "erp",
                table: "Gasto",
                column: "CuentaFinancieraId");

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "Gasto",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_EmpresaId_ProveedorId",
                schema: "erp",
                table: "Gasto",
                columns: new[] { "EmpresaId", "ProveedorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_ProveedorId",
                schema: "erp",
                table: "Gasto",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_OrdenCompraId",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "OrdenCompraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_EmpresaId_PagoProveedorId",
                schema: "erp",
                table: "Devolucion",
                columns: new[] { "EmpresaId", "PagoProveedorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_OrdenCompraId",
                schema: "erp",
                table: "Devolucion",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucion_PagoProveedorId",
                schema: "erp",
                table: "Devolucion",
                column: "PagoProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Compra_EmpresaId_OrdenCompraId",
                schema: "erp",
                table: "Compra",
                columns: new[] { "EmpresaId", "OrdenCompraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Compra_OrdenCompraId",
                schema: "erp",
                table: "Compra",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente",
                column: "CuentaFinancieraId");

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_EmpresaId",
                schema: "erp",
                table: "Cliente",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_CondicionComercialItem_CondicionComercialPlantillaId",
                schema: "erp",
                table: "CondicionComercialItem",
                column: "CondicionComercialPlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_CondicionComercialPlantilla_EmpresaId_TeamId_TipoDocumento_EsPredeterminada_Activa",
                schema: "erp",
                table: "CondicionComercialPlantilla",
                columns: new[] { "EmpresaId", "TeamId", "TipoDocumento", "EsPredeterminada", "Activa" });

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionCondicionSnapshot_CotizacionId_Orden",
                schema: "erp",
                table: "CotizacionCondicionSnapshot",
                columns: new[] { "CotizacionId", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_CuentaFinanciera_EmpresaId_Nombre",
                schema: "erp",
                table: "CuentaFinanciera",
                columns: new[] { "EmpresaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuentaFinanciera_EmpresaId_Tipo",
                schema: "erp",
                table: "CuentaFinanciera",
                columns: new[] { "EmpresaId", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_FormularioBloqueConfiguracion_FormularioConfiguracionId_Bloque",
                schema: "erp",
                table: "FormularioBloqueConfiguracion",
                columns: new[] { "FormularioConfiguracionId", "Bloque" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormularioBloqueProductoConfiguracion_FormularioConfiguracionId",
                schema: "erp",
                table: "FormularioBloqueProductoConfiguracion",
                column: "FormularioConfiguracionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormularioCampoConfiguracion_FormularioConfiguracionId_Bloque_Campo",
                schema: "erp",
                table: "FormularioCampoConfiguracion",
                columns: new[] { "FormularioConfiguracionId", "Bloque", "Campo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormularioConfiguracion_EmpresaId_TeamId_TipoDocumento_Activo",
                schema: "erp",
                table: "FormularioConfiguracion",
                columns: new[] { "EmpresaId", "TeamId", "TipoDocumento", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_EmpresaId",
                schema: "erp",
                table: "OrdenCompra",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_EmpresaId_ProveedorId",
                schema: "erp",
                table: "OrdenCompra",
                columns: new[] { "EmpresaId", "ProveedorId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_EmpresaId_Serie_Correlativo",
                schema: "erp",
                table: "OrdenCompra",
                columns: new[] { "EmpresaId", "Serie", "Correlativo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_ProveedorId",
                schema: "erp",
                table: "OrdenCompra",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraDetalle_OrdenCompraId",
                schema: "erp",
                table: "OrdenCompraDetalle",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraDetalle_ProductoId",
                schema: "erp",
                table: "OrdenCompraDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedorAplicacion_CompraId",
                schema: "erp",
                table: "PagoProveedorAplicacion",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedorAplicacion_EmpresaId",
                schema: "erp",
                table: "PagoProveedorAplicacion",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedorAplicacion_EmpresaId_CompraId",
                schema: "erp",
                table: "PagoProveedorAplicacion",
                columns: new[] { "EmpresaId", "CompraId" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedorAplicacion_EmpresaId_Estado",
                schema: "erp",
                table: "PagoProveedorAplicacion",
                columns: new[] { "EmpresaId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedorAplicacion_EmpresaId_PagoProveedorId",
                schema: "erp",
                table: "PagoProveedorAplicacion",
                columns: new[] { "EmpresaId", "PagoProveedorId" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedorAplicacion_PagoProveedorId_CompraId",
                schema: "erp",
                table: "PagoProveedorAplicacion",
                columns: new[] { "PagoProveedorId", "CompraId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlantillaDocumento_EmpresaId_TeamId_TipoDocumento_EsPredeterminada_Activa",
                schema: "erp",
                table: "PlantillaDocumento",
                columns: new[] { "EmpresaId", "TeamId", "TipoDocumento", "EsPredeterminada", "Activa" });

            migrationBuilder.CreateIndex(
                name: "IX_PlantillaDocumentoBloque_PlantillaDocumentoId_Bloque",
                schema: "erp",
                table: "PlantillaDocumentoBloque",
                columns: new[] { "PlantillaDocumentoId", "Bloque" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_CuentaDestinoId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                column: "CuentaDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_CuentaOrigenId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                column: "CuentaOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_EmpresaId_CuentaDestinoId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                columns: new[] { "EmpresaId", "CuentaDestinoId" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_EmpresaId_CuentaOrigenId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                columns: new[] { "EmpresaId", "CuentaOrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_EmpresaId_Fecha",
                schema: "erp",
                table: "TransferenciaFinanciera",
                columns: new[] { "EmpresaId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_MovimientoEgresoId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                column: "MovimientoEgresoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_MovimientoIngresoId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                column: "MovimientoIngresoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_Empresa_EmpresaId",
                schema: "erp",
                table: "Cliente",
                column: "EmpresaId",
                principalSchema: "erp",
                principalTable: "Empresa",
                principalColumn: "EmpresaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CobroCliente_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente",
                column: "CuentaFinancieraId",
                principalSchema: "erp",
                principalTable: "CuentaFinanciera",
                principalColumn: "CuentaFinancieraId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_OrdenCompra_OrdenCompraId",
                schema: "erp",
                table: "Compra",
                column: "OrdenCompraId",
                principalSchema: "erp",
                principalTable: "OrdenCompra",
                principalColumn: "OrdenCompraId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devolucion_OrdenCompra_OrdenCompraId",
                schema: "erp",
                table: "Devolucion",
                column: "OrdenCompraId",
                principalSchema: "erp",
                principalTable: "OrdenCompra",
                principalColumn: "OrdenCompraId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devolucion_PagoProveedor_PagoProveedorId",
                schema: "erp",
                table: "Devolucion",
                column: "PagoProveedorId",
                principalSchema: "erp",
                principalTable: "PagoProveedor",
                principalColumn: "PagoProveedorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gasto_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "Gasto",
                column: "CuentaFinancieraId",
                principalSchema: "erp",
                principalTable: "CuentaFinanciera",
                principalColumn: "CuentaFinancieraId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gasto_Proveedor_ProveedorId",
                schema: "erp",
                table: "Gasto",
                column: "ProveedorId",
                principalSchema: "erp",
                principalTable: "Proveedor",
                principalColumn: "ProveedorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingreso_Cliente_ClienteId",
                schema: "erp",
                table: "Ingreso",
                column: "ClienteId",
                principalSchema: "erp",
                principalTable: "Cliente",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingreso_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso",
                column: "CuentaFinancieraId",
                principalSchema: "erp",
                principalTable: "CuentaFinanciera",
                principalColumn: "CuentaFinancieraId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientoCaja_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja",
                column: "CuentaFinancieraId",
                principalSchema: "erp",
                principalTable: "CuentaFinanciera",
                principalColumn: "CuentaFinancieraId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PagoProveedor_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor",
                column: "CuentaFinancieraId",
                principalSchema: "erp",
                principalTable: "CuentaFinanciera",
                principalColumn: "CuentaFinancieraId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PagoProveedor_OrdenCompra_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor",
                column: "OrdenCompraId",
                principalSchema: "erp",
                principalTable: "OrdenCompra",
                principalColumn: "OrdenCompraId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Empresa_EmpresaId",
                schema: "erp",
                table: "Cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_CobroCliente_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente");

            migrationBuilder.DropForeignKey(
                name: "FK_Compra_OrdenCompra_OrdenCompraId",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_Devolucion_OrdenCompra_OrdenCompraId",
                schema: "erp",
                table: "Devolucion");

            migrationBuilder.DropForeignKey(
                name: "FK_Devolucion_PagoProveedor_PagoProveedorId",
                schema: "erp",
                table: "Devolucion");

            migrationBuilder.DropForeignKey(
                name: "FK_Gasto_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropForeignKey(
                name: "FK_Gasto_Proveedor_ProveedorId",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingreso_Cliente_ClienteId",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingreso_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientoCaja_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_PagoProveedor_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropForeignKey(
                name: "FK_PagoProveedor_OrdenCompra_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropTable(
                name: "CondicionComercialItem",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CotizacionCondicionSnapshot",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "FormularioBloqueConfiguracion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "FormularioBloqueProductoConfiguracion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "FormularioCampoConfiguracion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "OrdenCompraDetalle",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "PagoProveedorAplicacion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "PlantillaDocumentoBloque",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "TransferenciaFinanciera",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CondicionComercialPlantilla",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "FormularioConfiguracion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "OrdenCompra",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "PlantillaDocumento",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CuentaFinanciera",
                schema: "erp");

            migrationBuilder.DropIndex(
                name: "IX_PagoProveedor_CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropIndex(
                name: "IX_PagoProveedor_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropIndex(
                name: "IX_PagoProveedor_EmpresaId_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropIndex(
                name: "IX_PagoProveedor_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropIndex(
                name: "IX_MovimientoCaja_CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja");

            migrationBuilder.DropIndex(
                name: "IX_MovimientoCaja_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja");

            migrationBuilder.DropIndex(
                name: "IX_Ingreso_ClienteId",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropIndex(
                name: "IX_Ingreso_CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropIndex(
                name: "IX_Ingreso_EmpresaId_ClienteId",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropIndex(
                name: "IX_Ingreso_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropIndex(
                name: "IX_Gasto_CuentaFinancieraId",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropIndex(
                name: "IX_Gasto_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropIndex(
                name: "IX_Gasto_EmpresaId_ProveedorId",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropIndex(
                name: "IX_Gasto_ProveedorId",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropIndex(
                name: "IX_Devolucion_EmpresaId_OrdenCompraId",
                schema: "erp",
                table: "Devolucion");

            migrationBuilder.DropIndex(
                name: "IX_Devolucion_EmpresaId_PagoProveedorId",
                schema: "erp",
                table: "Devolucion");

            migrationBuilder.DropIndex(
                name: "IX_Devolucion_OrdenCompraId",
                schema: "erp",
                table: "Devolucion");

            migrationBuilder.DropIndex(
                name: "IX_Devolucion_PagoProveedorId",
                schema: "erp",
                table: "Devolucion");

            migrationBuilder.DropIndex(
                name: "IX_Compra_EmpresaId_OrdenCompraId",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropIndex(
                name: "IX_Compra_OrdenCompraId",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropIndex(
                name: "IX_CobroCliente_CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente");

            migrationBuilder.DropIndex(
                name: "IX_CobroCliente_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_EmpresaId",
                schema: "erp",
                table: "Cliente");

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 87);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 88);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 89);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 90);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 91);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 92);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 93);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 94);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 95);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 96);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 97);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 98);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 99);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 100);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 101);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 102);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 103);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 104);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 105);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 106);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 107);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 108);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 109);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 110);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 111);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 112);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 125);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 126);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 127);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 128);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 129);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 130);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 131);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 132);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 133);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 134);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 135);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 136);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 137);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 138);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 139);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 140);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 141);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 142);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 143);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 144);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 145);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 146);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 147);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 148);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 149);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 150);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 151);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 152);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 153);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 154);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 155);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 156);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 157);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 158);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 159);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 160);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 161);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 85);

            migrationBuilder.DeleteData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 86);

            migrationBuilder.DropColumn(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropColumn(
                name: "OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropColumn(
                name: "FormaPago",
                schema: "erp",
                table: "NotaPedido");

            migrationBuilder.DropColumn(
                name: "Observacion",
                schema: "erp",
                table: "NotaPedido");

            migrationBuilder.DropColumn(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropColumn(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropColumn(
                name: "DocumentoReferencia",
                schema: "erp",
                table: "Ingreso");

            migrationBuilder.DropColumn(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropColumn(
                name: "DocumentoReferencia",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropColumn(
                name: "ProveedorId",
                schema: "erp",
                table: "Gasto");

            migrationBuilder.DropColumn(
                name: "FirmaContenido",
                schema: "erp",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "FirmaContentType",
                schema: "erp",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "FirmaNombre",
                schema: "erp",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "RepresentanteLegalCargo",
                schema: "erp",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "RepresentanteLegalDocumento",
                schema: "erp",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "RepresentanteLegalNombre",
                schema: "erp",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "OrdenCompraId",
                schema: "erp",
                table: "Devolucion");

            migrationBuilder.DropColumn(
                name: "PagoProveedorId",
                schema: "erp",
                table: "Devolucion");

            migrationBuilder.DropColumn(
                name: "CantidadRecibida",
                schema: "erp",
                table: "CompraDetalle");

            migrationBuilder.DropColumn(
                name: "DiasCredito",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "EstadoEntrega",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "Moneda",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "OrdenCompraId",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "TipoCambio",
                schema: "erp",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                schema: "erp",
                table: "Cliente");

            migrationBuilder.AlterColumn<int>(
                name: "CompraId",
                schema: "erp",
                table: "PagoProveedor",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 6, 24, 17, 55, 42, 375, DateTimeKind.Utc).AddTicks(7293));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 6, 24, 17, 55, 42, 375, DateTimeKind.Utc).AddTicks(7299));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 6, 24, 17, 55, 42, 375, DateTimeKind.Utc).AddTicks(7300));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 6, 24, 17, 55, 42, 375, DateTimeKind.Utc).AddTicks(7302));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 26,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver CobrosClientes", "CobrosClientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 27,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear CobrosClientes", "CobrosClientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 28,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular CobrosClientes", "CobrosClientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 29,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver Productos", "Productos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 30,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Crear Productos", "Productos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 31,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Editar Productos", "Productos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 32,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Anular Productos", "Productos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 33,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver Clientes", "Clientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 34,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Crear Clientes", "Clientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 35,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Clientes", "Clientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 36,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Clientes", "Clientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 37,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Proveedores", "Proveedores" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 38,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Proveedores", "Proveedores" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 39,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Proveedores", "Proveedores" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 40,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Anular Proveedores", "Proveedores" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 41,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver Categorias", "Categorias" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 42,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Categorias", "Categorias" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 43,
                columns: new[] { "Accion", "Descripcion" },
                values: new object[] { "Editar", "Editar Categorias" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 44,
                columns: new[] { "Accion", "Descripcion" },
                values: new object[] { "Anular", "Anular Categorias" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 45,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 46,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 47,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 48,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "RegistrarPago", "RegistrarPago Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 49,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "AnularPago", "AnularPago Compras", "Compras" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 50,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Gastos", "Gastos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 51,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Gastos", "Gastos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 52,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Gastos", "Gastos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 53,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Gastos", "Gastos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 54,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Ingresos", "Ingresos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 55,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Ingresos", "Ingresos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 56,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Ingresos", "Ingresos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 57,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Ingresos", "Ingresos" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 58,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Devoluciones", "Devoluciones" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 59,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Registrar", "Registrar Devoluciones", "Devoluciones" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 60,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Caja", "Caja" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 61,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver ReporteGeneral", "ReporteGeneral" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 62,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver PropuestasComerciales", "PropuestasComerciales" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 63,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver CuentasPorPagar", "CuentasPorPagar" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 64,
                columns: new[] { "Descripcion", "Modulo" },
                values: new object[] { "Ver DevolucionesProveedor", "DevolucionesProveedor" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 65,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver ReporteCaja", "ReporteCaja" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 66,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver EstadoCuentaClientes", "EstadoCuentaClientes" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 67,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Empresas", "Empresas" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 68,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Empresas", "Empresas" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 69,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Empresas", "Empresas" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 70,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Anular", "Anular Empresas", "Empresas" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 71,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Usuarios", "Usuarios" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 72,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Usuarios", "Usuarios" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 73,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Usuarios", "Usuarios" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 74,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "RestablecerPassword", "RestablecerPassword Usuarios", "Usuarios" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 75,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Roles", "Roles" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 76,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Roles", "Roles" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 77,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Roles", "Roles" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 78,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver Configuracion", "Configuracion" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 79,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Crear", "Crear Configuracion", "Configuracion" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 80,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Editar", "Editar Configuracion", "Configuracion" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 81,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Configurar", "Configurar Configuracion", "Configuracion" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 82,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver NubefactLogs", "NubefactLogs" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 83,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Ver", "Ver ErroresAplicacion", "ErroresAplicacion" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "Permiso",
                keyColumn: "PermisoId",
                keyValue: 84,
                columns: new[] { "Accion", "Descripcion", "Modulo" },
                values: new object[] { "Revisar", "Revisar ErroresAplicacion", "ErroresAplicacion" });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 85,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 1, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 86,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 2, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 87,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 88,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 4, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 89,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 5, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 90,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 6, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 91,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 7, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 92,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 8, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 93,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 9, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 94,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 10, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 95,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 11, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 96,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 12, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 97,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 13, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 98,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 14, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 99,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 15, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 100,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 16, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 101,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 17, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 102,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 18, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 103,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 19, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 104,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 20, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 105,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 22, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 106,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 23, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 107,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 24, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 108,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 25, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 109,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 26, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 110,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 27, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 111,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 28, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 112,
                columns: new[] { "PermisoId", "RolId" },
                values: new object[] { 29, 2 });

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 113,
                column: "PermisoId",
                value: 30);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 114,
                column: "PermisoId",
                value: 31);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 115,
                column: "PermisoId",
                value: 32);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 116,
                column: "PermisoId",
                value: 33);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 117,
                column: "PermisoId",
                value: 34);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 118,
                column: "PermisoId",
                value: 35);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 119,
                column: "PermisoId",
                value: 36);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 120,
                column: "PermisoId",
                value: 41);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 121,
                column: "PermisoId",
                value: 42);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 122,
                column: "PermisoId",
                value: 43);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 123,
                column: "PermisoId",
                value: 44);

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "RolPermiso",
                keyColumn: "RolPermisoId",
                keyValue: 124,
                column: "PermisoId",
                value: 58);
        }
    }
}
