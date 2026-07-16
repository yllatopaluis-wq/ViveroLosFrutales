using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdenCompraYPagoProveedorAplicacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor",
                type: "int",
                nullable: true);

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
                name: "OrdenCompraId",
                schema: "erp",
                table: "Compra",
                type: "int",
                nullable: true);

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
                name: "FK_PagoProveedor_OrdenCompra_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropTable(
                name: "OrdenCompraDetalle",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "PagoProveedorAplicacion",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "OrdenCompra",
                schema: "erp");

            migrationBuilder.DropIndex(
                name: "IX_PagoProveedor_EmpresaId_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor");

            migrationBuilder.DropIndex(
                name: "IX_PagoProveedor_OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor");

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

            migrationBuilder.DropColumn(
                name: "OrdenCompraId",
                schema: "erp",
                table: "PagoProveedor");

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
                name: "OrdenCompraId",
                schema: "erp",
                table: "Compra");
        }
    }
}
