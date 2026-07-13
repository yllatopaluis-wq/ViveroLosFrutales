using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViveroLosFrutales.Infrastructure.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260712141000_AddFinanzaDocumentoReferencia")]
public partial class AddFinanzaDocumentoReferencia : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Gasto', N'DocumentoReferencia') IS NULL
BEGIN
    ALTER TABLE [erp].[Gasto]
        ADD [DocumentoReferencia] nvarchar(120) NOT NULL
            CONSTRAINT [DF_Gasto_DocumentoReferencia] DEFAULT N'';
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Ingreso', N'DocumentoReferencia') IS NULL
BEGIN
    ALTER TABLE [erp].[Ingreso]
        ADD [DocumentoReferencia] nvarchar(120) NOT NULL
            CONSTRAINT [DF_Ingreso_DocumentoReferencia] DEFAULT N'';
END;
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'erp.Gasto', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Gasto', N'DocumentoReferencia') IS NOT NULL
BEGIN
    ALTER TABLE [erp].[Gasto] DROP COLUMN [DocumentoReferencia];
END;

IF OBJECT_ID(N'erp.Ingreso', N'U') IS NOT NULL AND COL_LENGTH(N'erp.Ingreso', N'DocumentoReferencia') IS NOT NULL
BEGIN
    ALTER TABLE [erp].[Ingreso] DROP COLUMN [DocumentoReferencia];
END;
");
    }
}
