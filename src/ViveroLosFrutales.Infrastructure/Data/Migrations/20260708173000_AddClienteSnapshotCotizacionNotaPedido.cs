using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    public partial class AddClienteSnapshotCotizacionNotaPedido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddClienteSnapshotColumns(migrationBuilder, "Cotizacion");
            AddClienteSnapshotColumns(migrationBuilder, "NotaPedido");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropClienteSnapshotColumns(migrationBuilder, "NotaPedido");
            DropClienteSnapshotColumns(migrationBuilder, "Cotizacion");
        }

        private static void AddClienteSnapshotColumns(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.AddColumn<string>(name: "ClienteDireccion", schema: "erp", table: table, type: "nvarchar(500)", maxLength: 500, nullable: true);
            migrationBuilder.AddColumn<string>(name: "ClienteEmail", schema: "erp", table: table, type: "nvarchar(120)", maxLength: 120, nullable: true);
            migrationBuilder.AddColumn<string>(name: "ClienteNombre", schema: "erp", table: table, type: "nvarchar(250)", maxLength: 250, nullable: true);
            migrationBuilder.AddColumn<string>(name: "ClienteNombreComercial", schema: "erp", table: table, type: "nvarchar(250)", maxLength: 250, nullable: true);
            migrationBuilder.AddColumn<string>(name: "ClienteNumeroDocumento", schema: "erp", table: table, type: "nvarchar(20)", maxLength: 20, nullable: true);
            migrationBuilder.AddColumn<string>(name: "ClienteTelefono", schema: "erp", table: table, type: "nvarchar(40)", maxLength: 40, nullable: true);
            migrationBuilder.AddColumn<int>(name: "ClienteTipoDocumento", schema: "erp", table: table, type: "int", nullable: true);
        }

        private static void DropClienteSnapshotColumns(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.DropColumn(name: "ClienteDireccion", schema: "erp", table: table);
            migrationBuilder.DropColumn(name: "ClienteEmail", schema: "erp", table: table);
            migrationBuilder.DropColumn(name: "ClienteNombre", schema: "erp", table: table);
            migrationBuilder.DropColumn(name: "ClienteNombreComercial", schema: "erp", table: table);
            migrationBuilder.DropColumn(name: "ClienteNumeroDocumento", schema: "erp", table: table);
            migrationBuilder.DropColumn(name: "ClienteTelefono", schema: "erp", table: table);
            migrationBuilder.DropColumn(name: "ClienteTipoDocumento", schema: "erp", table: table);
        }
    }
}