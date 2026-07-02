using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260627090000_AddCuentaFinancieraCajaBancos")]
    public partial class AddCuentaFinancieraCajaBancos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "Gasto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
INSERT INTO erp.CuentaFinanciera (Nombre, Tipo, Banco, NumeroCuenta, Moneda, SaldoInicial, FechaSaldoInicial, Activo, EmpresaId, FechaRegistro, UsuarioRegistro, Estado, UsuarioModificacion)
SELECT N'Caja principal', 1, N'', N'', N'PEN', 0, CAST(GETDATE() AS date), CAST(1 AS bit), e.EmpresaId, SYSUTCDATETIME(), N'migration', 1, N''
FROM erp.Empresa e
WHERE NOT EXISTS (
    SELECT 1 FROM erp.CuentaFinanciera c WHERE c.EmpresaId = e.EmpresaId AND c.Nombre = N'Caja principal'
);

UPDATE m
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM erp.MovimientoCaja m
INNER JOIN erp.CuentaFinanciera c ON c.EmpresaId = m.EmpresaId AND c.Nombre = N'Caja principal'
WHERE m.CuentaFinancieraId IS NULL;

UPDATE cc
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM erp.CobroCliente cc
INNER JOIN erp.CuentaFinanciera c ON c.EmpresaId = cc.EmpresaId AND c.Nombre = N'Caja principal'
WHERE cc.CuentaFinancieraId IS NULL;

UPDATE g
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM erp.Gasto g
INNER JOIN erp.CuentaFinanciera c ON c.EmpresaId = g.EmpresaId AND c.Nombre = N'Caja principal'
WHERE g.CuentaFinancieraId IS NULL;

UPDATE i
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM erp.Ingreso i
INNER JOIN erp.CuentaFinanciera c ON c.EmpresaId = i.EmpresaId AND c.Nombre = N'Caja principal'
WHERE i.CuentaFinancieraId IS NULL;

UPDATE p
SET CuentaFinancieraId = c.CuentaFinancieraId
FROM erp.PagoProveedor p
INNER JOIN erp.CuentaFinanciera c ON c.EmpresaId = p.EmpresaId AND c.Nombre = N'Caja principal'
WHERE p.CuentaFinancieraId IS NULL;");

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
                name: "IX_MovimientoCaja_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "MovimientoCaja",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_PagoProveedor_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "PagoProveedor",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "Gasto",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

            migrationBuilder.CreateIndex(
                name: "IX_CobroCliente_EmpresaId_CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente",
                columns: new[] { "EmpresaId", "CuentaFinancieraId" });

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
                name: "FK_Ingreso_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "Ingreso",
                column: "CuentaFinancieraId",
                principalSchema: "erp",
                principalTable: "CuentaFinanciera",
                principalColumn: "CuentaFinancieraId",
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
                name: "FK_CobroCliente_CuentaFinanciera_CuentaFinancieraId",
                schema: "erp",
                table: "CobroCliente",
                column: "CuentaFinancieraId",
                principalSchema: "erp",
                principalTable: "CuentaFinanciera",
                principalColumn: "CuentaFinancieraId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_MovimientoCaja_CuentaFinanciera_CuentaFinancieraId", schema: "erp", table: "MovimientoCaja");
            migrationBuilder.DropForeignKey(name: "FK_PagoProveedor_CuentaFinanciera_CuentaFinancieraId", schema: "erp", table: "PagoProveedor");
            migrationBuilder.DropForeignKey(name: "FK_Ingreso_CuentaFinanciera_CuentaFinancieraId", schema: "erp", table: "Ingreso");
            migrationBuilder.DropForeignKey(name: "FK_Gasto_CuentaFinanciera_CuentaFinancieraId", schema: "erp", table: "Gasto");
            migrationBuilder.DropForeignKey(name: "FK_CobroCliente_CuentaFinanciera_CuentaFinancieraId", schema: "erp", table: "CobroCliente");

            migrationBuilder.DropTable(name: "CuentaFinanciera", schema: "erp");

            migrationBuilder.DropIndex(name: "IX_MovimientoCaja_EmpresaId_CuentaFinancieraId", schema: "erp", table: "MovimientoCaja");
            migrationBuilder.DropIndex(name: "IX_PagoProveedor_EmpresaId_CuentaFinancieraId", schema: "erp", table: "PagoProveedor");
            migrationBuilder.DropIndex(name: "IX_Ingreso_EmpresaId_CuentaFinancieraId", schema: "erp", table: "Ingreso");
            migrationBuilder.DropIndex(name: "IX_Gasto_EmpresaId_CuentaFinancieraId", schema: "erp", table: "Gasto");
            migrationBuilder.DropIndex(name: "IX_CobroCliente_EmpresaId_CuentaFinancieraId", schema: "erp", table: "CobroCliente");

            migrationBuilder.DropColumn(name: "CuentaFinancieraId", schema: "erp", table: "MovimientoCaja");
            migrationBuilder.DropColumn(name: "CuentaFinancieraId", schema: "erp", table: "PagoProveedor");
            migrationBuilder.DropColumn(name: "CuentaFinancieraId", schema: "erp", table: "Ingreso");
            migrationBuilder.DropColumn(name: "CuentaFinancieraId", schema: "erp", table: "Gasto");
            migrationBuilder.DropColumn(name: "CuentaFinancieraId", schema: "erp", table: "CobroCliente");
        }
    }
}

