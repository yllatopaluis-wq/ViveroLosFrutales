using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260712133000_AddFinanzaClienteProveedor")]
public partial class AddFinanzaClienteProveedor : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Gasto', N'ClienteId') IS NULL
BEGIN
    ALTER TABLE [erp].[Gasto] ADD [ClienteId] int NULL;
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Ingreso', N'ProveedorId') IS NULL
BEGIN
    ALTER TABLE [erp].[Ingreso] ADD [ProveedorId] int NULL;
END;

IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL
   AND COL_LENGTH(N'erp.Gasto', N'ClienteId') IS NOT NULL
   AND OBJECT_ID(N'erp.Cliente', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Gasto_Cliente_ClienteId')
BEGIN
    ALTER TABLE [erp].[Gasto]
        ADD CONSTRAINT [FK_Gasto_Cliente_ClienteId]
        FOREIGN KEY ([ClienteId]) REFERENCES [erp].[Cliente] ([ClienteId]) ON DELETE NO ACTION;
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL
   AND COL_LENGTH(N'erp.Ingreso', N'ProveedorId') IS NOT NULL
   AND OBJECT_ID(N'erp.Proveedor', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Ingreso_Proveedor_ProveedorId')
BEGIN
    ALTER TABLE [erp].[Ingreso]
        ADD CONSTRAINT [FK_Ingreso_Proveedor_ProveedorId]
        FOREIGN KEY ([ProveedorId]) REFERENCES [erp].[Proveedor] ([ProveedorId]) ON DELETE NO ACTION;
END;

IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL
   AND COL_LENGTH(N'erp.Gasto', N'ClienteId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Gasto_EmpresaId_ClienteId' AND object_id = OBJECT_ID(N'erp.Gasto'))
BEGIN
    CREATE INDEX [IX_Gasto_EmpresaId_ClienteId] ON [erp].[Gasto] ([EmpresaId], [ClienteId]);
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL
   AND COL_LENGTH(N'erp.Ingreso', N'ProveedorId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Ingreso_EmpresaId_ProveedorId' AND object_id = OBJECT_ID(N'erp.Ingreso'))
BEGIN
    CREATE INDEX [IX_Ingreso_EmpresaId_ProveedorId] ON [erp].[Ingreso] ([EmpresaId], [ProveedorId]);
END;
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Gasto_Cliente_ClienteId')
    ALTER TABLE [erp].[Gasto] DROP CONSTRAINT [FK_Gasto_Cliente_ClienteId];
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Ingreso_Proveedor_ProveedorId')
    ALTER TABLE [erp].[Ingreso] DROP CONSTRAINT [FK_Ingreso_Proveedor_ProveedorId];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Gasto_EmpresaId_ClienteId' AND object_id = OBJECT_ID(N'erp.Gasto'))
    DROP INDEX [IX_Gasto_EmpresaId_ClienteId] ON [erp].[Gasto];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Ingreso_EmpresaId_ProveedorId' AND object_id = OBJECT_ID(N'erp.Ingreso'))
    DROP INDEX [IX_Ingreso_EmpresaId_ProveedorId] ON [erp].[Ingreso];
IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Gasto', N'ClienteId') IS NOT NULL
    ALTER TABLE [erp].[Gasto] DROP COLUMN [ClienteId];
IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Ingreso', N'ProveedorId') IS NOT NULL
    ALTER TABLE [erp].[Ingreso] DROP COLUMN [ProveedorId];
");
    }
}
