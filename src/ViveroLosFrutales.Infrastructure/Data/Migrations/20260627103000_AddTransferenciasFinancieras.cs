using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260627103000_AddTransferenciasFinancieras")]
    public partial class AddTransferenciasFinancieras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    UsuarioAnulacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_EmpresaId_Fecha",
                schema: "erp",
                table: "TransferenciaFinanciera",
                columns: new[] { "EmpresaId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_EmpresaId_CuentaOrigenId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                columns: new[] { "EmpresaId", "CuentaOrigenId" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_EmpresaId_CuentaDestinoId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                columns: new[] { "EmpresaId", "CuentaDestinoId" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_CuentaOrigenId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                column: "CuentaOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaFinanciera_CuentaDestinoId",
                schema: "erp",
                table: "TransferenciaFinanciera",
                column: "CuentaDestinoId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TransferenciaFinanciera", schema: "erp");
        }
    }
}

