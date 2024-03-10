using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseDriverVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnterpriseId",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Enterprise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprise", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Driver",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnterpriseId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Driver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Driver_Enterprise_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprise",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DriverVehicle",
                columns: table => new
                {
                    DriversId = table.Column<int>(type: "int", nullable: false),
                    VehiclesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverVehicle", x => new { x.DriversId, x.VehiclesId });
                    table.ForeignKey(
                        name: "FK_DriverVehicle_Driver_DriversId",
                        column: x => x.DriversId,
                        principalTable: "Driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverVehicle_Vehicles_VehiclesId",
                        column: x => x.VehiclesId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_EnterpriseId",
                table: "Vehicles",
                column: "EnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_Driver_EnterpriseId",
                table: "Driver",
                column: "EnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverVehicle_VehiclesId",
                table: "DriverVehicle",
                column: "VehiclesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Enterprise_EnterpriseId",
                table: "Vehicles",
                column: "EnterpriseId",
                principalTable: "Enterprise",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Enterprise_EnterpriseId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "DriverVehicle");

            migrationBuilder.DropTable(
                name: "Driver");

            migrationBuilder.DropTable(
                name: "Enterprise");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_EnterpriseId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "EnterpriseId",
                table: "Vehicles");
        }
    }
}
