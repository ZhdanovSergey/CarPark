using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Migrations
{
    /// <inheritdoc />
    public partial class ActiveDriverAndVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveDriverId",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ActiveDriverId",
                table: "Vehicles",
                column: "ActiveDriverId",
                unique: true,
                filter: "[ActiveDriverId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Drivers_ActiveDriverId",
                table: "Vehicles",
                column: "ActiveDriverId",
                principalTable: "Drivers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Drivers_ActiveDriverId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ActiveDriverId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ActiveDriverId",
                table: "Vehicles");
        }
    }
}
