using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixMotivoNotaCreditoSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 16, 10, 54, 21, 378, DateTimeKind.Unspecified).AddTicks(4169));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 16, 10, 54, 21, 378, DateTimeKind.Unspecified).AddTicks(4246));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 16, 10, 54, 21, 378, DateTimeKind.Unspecified).AddTicks(4253));

            migrationBuilder.UpdateData(
                schema: "erp",
                table: "MotivoNotaCredito",
                keyColumn: "MotivoNotaCreditoId",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 7, 16, 10, 54, 21, 378, DateTimeKind.Unspecified).AddTicks(4258));
        }
    }
}
