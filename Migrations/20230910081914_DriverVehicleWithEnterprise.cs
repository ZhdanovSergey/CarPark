using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Migrations
{
    /// <inheritdoc />
    public partial class DriverVehicleWithEnterprise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DriversVehicles",
                table: "DriversVehicles");

            migrationBuilder.AddColumn<int>(
                name: "EnterpriseId",
                table: "DriversVehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriversVehicles",
                table: "DriversVehicles",
                columns: new[] { "EnterpriseId", "DriverId", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriversVehicles_DriverId",
                table: "DriversVehicles",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriversVehicles_Enterprises_EnterpriseId",
                table: "DriversVehicles",
                column: "EnterpriseId",
                principalTable: "Enterprises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriversVehicles_Enterprises_EnterpriseId",
                table: "DriversVehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriversVehicles",
                table: "DriversVehicles");

            migrationBuilder.DropIndex(
                name: "IX_DriversVehicles_DriverId",
                table: "DriversVehicles");

            migrationBuilder.DropColumn(
                name: "EnterpriseId",
                table: "DriversVehicles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriversVehicles",
                table: "DriversVehicles",
                columns: new[] { "DriverId", "VehicleId" });
        }
    }
}
