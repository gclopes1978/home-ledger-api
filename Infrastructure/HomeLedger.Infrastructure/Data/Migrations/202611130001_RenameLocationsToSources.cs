using HomeLedger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeLedger.Infrastructure.Data.Migrations;

[DbContext(typeof(HomeLedgerDbContext))]
[Migration("202611130001_RenameLocationsToSources")]
public sealed class RenameLocationsToSources : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Expenses_Locations_LocationId",
            table: "Expenses");

        migrationBuilder.RenameTable(name: "Locations", newName: "Sources");
        migrationBuilder.RenameColumn(name: "LocationId", table: "Expenses", newName: "SourceId");
        migrationBuilder.RenameIndex(name: "IX_Expenses_LocationId", table: "Expenses", newName: "IX_Expenses_SourceId");
        migrationBuilder.Sql("EXEC sp_rename N'dbo.PK_Locations', N'PK_Sources', N'OBJECT';");

        migrationBuilder.AddForeignKey(
            name: "FK_Expenses_Sources_SourceId",
            table: "Expenses",
            column: "SourceId",
            principalTable: "Sources",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Expenses_Sources_SourceId",
            table: "Expenses");

        migrationBuilder.RenameColumn(name: "SourceId", table: "Expenses", newName: "LocationId");
        migrationBuilder.RenameIndex(name: "IX_Expenses_SourceId", table: "Expenses", newName: "IX_Expenses_LocationId");
        migrationBuilder.RenameTable(name: "Sources", newName: "Locations");
        migrationBuilder.Sql("EXEC sp_rename N'dbo.PK_Sources', N'PK_Locations', N'OBJECT';");

        migrationBuilder.AddForeignKey(
            name: "FK_Expenses_Locations_LocationId",
            table: "Expenses",
            column: "LocationId",
            principalTable: "Locations",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
