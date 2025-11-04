using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVRenter_Data.Migrations
{
    /// <inheritdoc />
    public partial class NewVer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Models_ModelID",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverAndReturn_Vehicles_VehicleID",
                table: "HandoverAndReturn");

            migrationBuilder.RenameColumn(
                name: "ModelID",
                table: "Bookings",
                newName: "VehicleID");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ModelID",
                table: "Bookings",
                newName: "IX_Bookings_VehicleID");

            migrationBuilder.RenameColumn(
                name: "AmenityName",
                table: "Amenities",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ChargePower",
                table: "Models",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChargingTime",
                table: "Models",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Amenities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Vehicles_VehicleID",
                table: "Bookings",
                column: "VehicleID",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverAndReturn_Vehicles_VehicleID",
                table: "HandoverAndReturn",
                column: "VehicleID",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Vehicles_VehicleID",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverAndReturn_Vehicles_VehicleID",
                table: "HandoverAndReturn");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ChargePower",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "ChargingTime",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Amenities");

            migrationBuilder.RenameColumn(
                name: "VehicleID",
                table: "Bookings",
                newName: "ModelID");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_VehicleID",
                table: "Bookings",
                newName: "IX_Bookings_ModelID");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Amenities",
                newName: "AmenityName");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Models_ModelID",
                table: "Bookings",
                column: "ModelID",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverAndReturn_Vehicles_VehicleID",
                table: "HandoverAndReturn",
                column: "VehicleID",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
