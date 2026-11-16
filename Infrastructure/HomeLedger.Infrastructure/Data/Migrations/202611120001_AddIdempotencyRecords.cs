using HomeLedger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeLedger.Infrastructure.Data.Migrations;

[DbContext(typeof(HomeLedgerDbContext))]
[Migration("202611120001_AddIdempotencyRecords")]
public sealed class AddIdempotencyRecords : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "IdempotencyRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                IdempotencyKey = table.Column<string>(
                    type: "nvarchar(128)",
                    maxLength: 128,
                    nullable: false),
                RequestHash = table.Column<string>(
                    type: "nvarchar(64)",
                    maxLength: 64,
                    nullable: false),
                ResponseStatusCode = table.Column<int>(type: "int", nullable: true),
                ResponseContentType = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: true),
                ResponseLocation = table.Column<string>(
                    type: "nvarchar(2048)",
                    maxLength: 2048,
                    nullable: true),
                ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_IdempotencyRecords", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_IdempotencyRecords_IdempotencyKey",
            table: "IdempotencyRecords",
            column: "IdempotencyKey",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "IdempotencyRecords");
    }
}