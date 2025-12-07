using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracKiralamaPortal.Migrations
{
    /// <inheritdoc />
    public partial class CarTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Cars",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "DailyPrice",
                table: "Cars",
                newName: "Price");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Cars",
                newName: "DailyPrice");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Cars",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
