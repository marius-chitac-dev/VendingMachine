using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterUserDepositToBeDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Deposit",
                table: "Users",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValueSql: "((0))",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "((0))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Deposit",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValueSql: "((0))",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValueSql: "((0))");
        }
    }
}
