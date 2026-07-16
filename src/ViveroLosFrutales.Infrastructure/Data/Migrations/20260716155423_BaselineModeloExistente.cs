using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    public partial class BaselineModeloExistente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline del modelo ya existente.
            // No crear ni modificar objetos de base de datos.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No revertir la estructura existente.
        }
    }
}