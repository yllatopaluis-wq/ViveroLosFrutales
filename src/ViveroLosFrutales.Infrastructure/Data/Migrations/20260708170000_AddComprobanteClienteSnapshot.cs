using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    public partial class AddComprobanteClienteSnapshot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClienteDireccion",
                schema: "erp",
                table: "Comprobante",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClienteEmail",
                schema: "erp",
                table: "Comprobante",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClienteNombre",
                schema: "erp",
                table: "Comprobante",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClienteNombreComercial",
                schema: "erp",
                table: "Comprobante",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClienteNumeroDocumento",
                schema: "erp",
                table: "Comprobante",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClienteTelefono",
                schema: "erp",
                table: "Comprobante",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteTipoDocumento",
                schema: "erp",
                table: "Comprobante",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ClienteDireccion", schema: "erp", table: "Comprobante");
            migrationBuilder.DropColumn(name: "ClienteEmail", schema: "erp", table: "Comprobante");
            migrationBuilder.DropColumn(name: "ClienteNombre", schema: "erp", table: "Comprobante");
            migrationBuilder.DropColumn(name: "ClienteNombreComercial", schema: "erp", table: "Comprobante");
            migrationBuilder.DropColumn(name: "ClienteNumeroDocumento", schema: "erp", table: "Comprobante");
            migrationBuilder.DropColumn(name: "ClienteTelefono", schema: "erp", table: "Comprobante");
            migrationBuilder.DropColumn(name: "ClienteTipoDocumento", schema: "erp", table: "Comprobante");
        }
    }
}