using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposComplementariosCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('erp.Compra', 'DiasCredito') IS NULL
                BEGIN
                    ALTER TABLE erp.Compra
                    ADD DiasCredito int NOT NULL
                        CONSTRAINT DF_Compra_DiasCredito DEFAULT (0);
                END;

                IF COL_LENGTH('erp.Compra', 'EstadoEntrega') IS NULL
                BEGIN
                    ALTER TABLE erp.Compra
                    ADD EstadoEntrega int NOT NULL
                        CONSTRAINT DF_Compra_EstadoEntrega DEFAULT (0);
                END;

                IF COL_LENGTH('erp.Compra', 'FechaVencimiento') IS NULL
                BEGIN
                    ALTER TABLE erp.Compra
                    ADD FechaVencimiento datetime2 NULL;
                END;

                IF COL_LENGTH('erp.Compra', 'Moneda') IS NULL
                BEGIN
                    ALTER TABLE erp.Compra
                    ADD Moneda nvarchar(20) NOT NULL
                        CONSTRAINT DF_Compra_Moneda DEFAULT (N'');
                END;

                IF COL_LENGTH('erp.Compra', 'TipoCambio') IS NULL
                BEGIN
                    ALTER TABLE erp.Compra
                    ADD TipoCambio decimal(18,4) NOT NULL
                        CONSTRAINT DF_Compra_TipoCambio DEFAULT (0);
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No eliminar columnas porque pueden contener información histórica.
        }
    }
}