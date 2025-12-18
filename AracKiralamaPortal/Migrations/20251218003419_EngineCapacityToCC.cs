using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracKiralamaPortal.Migrations
{
    /// <inheritdoc />
    public partial class EngineCapacityToCC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EngineCapacity",
                table: "Cars",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "EngineCapacity",
                table: "Cars",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
