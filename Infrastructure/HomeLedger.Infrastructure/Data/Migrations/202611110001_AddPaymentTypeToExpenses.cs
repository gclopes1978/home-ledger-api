using HomeLedger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeLedger.Infrastructure.Data.Migrations;

[DbContext(typeof(HomeLedgerDbContext))]
[Migration("202611110001_AddPaymentTypeToExpenses")]
public sealed class AddPaymentTypeToExpenses : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "PaymentTypeId",
            table: "Expenses",
            type: "int",
            nullable: true);

        migrationBuilder.Sql("""
            IF EXISTS (SELECT 1 FROM [Expenses] WHERE [PaymentTypeId] IS NULL)
               AND NOT EXISTS (SELECT 1 FROM [PaymentTypes] WHERE [Description] = N'Unspecified')
            BEGIN
                INSERT INTO [PaymentTypes] ([Description]) VALUES (N'Unspecified');
            END;

            UPDATE [Expenses]
            SET [PaymentTypeId] = (
                SELECT TOP (1) [Id]
                FROM [PaymentTypes]
                WHERE [Description] = N'Unspecified'
                ORDER BY [Id]
            )
            WHERE [PaymentTypeId] IS NULL;
            """);

        migrationBuilder.AlterColumn<int>(
            name: "PaymentTypeId",
            table: "Expenses",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Expenses_PaymentTypeId",
            table: "Expenses",
            column: "PaymentTypeId");

        migrationBuilder.AddForeignKey(
            name: "FK_Expenses_PaymentTypes_PaymentTypeId",
            table: "Expenses",
            column: "PaymentTypeId",
            principalTable: "PaymentTypes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Expenses_PaymentTypes_PaymentTypeId",
            table: "Expenses");

        migrationBuilder.DropIndex(
            name: "IX_Expenses_PaymentTypeId",
            table: "Expenses");

        migrationBuilder.DropColumn(
            name: "PaymentTypeId",
            table: "Expenses");
    }
}