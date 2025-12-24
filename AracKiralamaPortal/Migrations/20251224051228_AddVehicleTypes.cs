using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AracKiralamaPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleSubTypeId",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleTypeId",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleSubTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleSubTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleSubTypes_VehicleTypes_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "VehicleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "VehicleTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Otomobil" },
                    { 2, "Motosiklet" },
                    { 3, "ATV / Arazi Aracı" },
                    { 4, "Minivan" },
                    { 5, "Elektrikli Araç" }
                });

            migrationBuilder.InsertData(
                table: "VehicleSubTypes",
                columns: new[] { "Id", "Name", "VehicleTypeId" },
                values: new object[,]
                {
                    { 1, "Sedan", 1 },
                    { 2, "SUV", 1 },
                    { 3, "Hatchback", 1 },
                    { 4, "Coupe", 1 },
                    { 5, "Cabrio / Üstü Açılır", 1 },
                    { 6, "Station Wagon", 1 },
                    { 7, "Naked", 2 },
                    { 8, "Sport / Racing", 2 },
                    { 9, "Cruiser", 2 },
                    { 10, "Scooter", 2 },
                    { 11, "Touring", 2 },
                    { 12, "Sport ATV", 3 },
                    { 13, "Utility ATV", 3 },
                    { 14, "Quad Bike", 3 },
                    { 15, "Minivan", 4 },
                    { 16, "Panelvan", 4 },
                    { 17, "Elektrikli Sedan", 5 },
                    { 18, "Elektrikli SUV", 5 },
                    { 19, "Elektrikli Hatchback", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_VehicleSubTypeId",
                table: "Cars",
                column: "VehicleSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_VehicleTypeId",
                table: "Cars",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleSubTypes_VehicleTypeId",
                table: "VehicleSubTypes",
                column: "VehicleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_VehicleSubTypes_VehicleSubTypeId",
                table: "Cars",
                column: "VehicleSubTypeId",
                principalTable: "VehicleSubTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_VehicleTypes_VehicleTypeId",
                table: "Cars",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_VehicleSubTypes_VehicleSubTypeId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_VehicleTypes_VehicleTypeId",
                table: "Cars");

            migrationBuilder.DropTable(
                name: "VehicleSubTypes");

            migrationBuilder.DropTable(
                name: "VehicleTypes");

            migrationBuilder.DropIndex(
                name: "IX_Cars_VehicleSubTypeId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_VehicleTypeId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "VehicleSubTypeId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "VehicleTypeId",
                table: "Cars");
        }
    }
}
