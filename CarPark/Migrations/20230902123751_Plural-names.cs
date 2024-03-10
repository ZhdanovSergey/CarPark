using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Migrations
{
    /// <inheritdoc />
    public partial class Pluralnames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Driver_Enterprise_EnterpriseId",
                table: "Driver");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverVehicle_Driver_DriversId",
                table: "DriverVehicle");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Enterprise_EnterpriseId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enterprise",
                table: "Enterprise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Driver",
                table: "Driver");

            migrationBuilder.RenameTable(
                name: "Enterprise",
                newName: "Enterprises");

            migrationBuilder.RenameTable(
                name: "Driver",
                newName: "Drivers");

            migrationBuilder.RenameIndex(
                name: "IX_Driver_EnterpriseId",
                table: "Drivers",
                newName: "IX_Drivers_EnterpriseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enterprises",
                table: "Enterprises",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Drivers",
                table: "Drivers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Enterprises_EnterpriseId",
                table: "Drivers",
                column: "EnterpriseId",
                principalTable: "Enterprises",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverVehicle_Drivers_DriversId",
                table: "DriverVehicle",
                column: "DriversId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Enterprises_EnterpriseId",
                table: "Vehicles",
                column: "EnterpriseId",
                principalTable: "Enterprises",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Enterprises_EnterpriseId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverVehicle_Drivers_DriversId",
                table: "DriverVehicle");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Enterprises_EnterpriseId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enterprises",
                table: "Enterprises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Drivers",
                table: "Drivers");

            migrationBuilder.RenameTable(
                name: "Enterprises",
                newName: "Enterprise");

            migrationBuilder.RenameTable(
                name: "Drivers",
                newName: "Driver");

            migrationBuilder.RenameIndex(
                name: "IX_Drivers_EnterpriseId",
                table: "Driver",
                newName: "IX_Driver_EnterpriseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enterprise",
                table: "Enterprise",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Driver",
                table: "Driver",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Driver_Enterprise_EnterpriseId",
                table: "Driver",
                column: "EnterpriseId",
                principalTable: "Enterprise",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverVehicle_Driver_DriversId",
                table: "DriverVehicle",
                column: "DriversId",
                principalTable: "Driver",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Enterprise_EnterpriseId",
                table: "Vehicles",
                column: "EnterpriseId",
                principalTable: "Enterprise",
                principalColumn: "Id");
        }
    }
}
