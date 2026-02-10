using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarsInsideGarage.Migrations
{
    /// <inheritdoc />
    public partial class SchemaRemake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LicensePlate = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarageFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DailyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageFees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Area = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AddressCoordinates = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Garages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    GarageFeeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Garages_GarageFees_GarageFeeId",
                        column: x => x.GarageFeeId,
                        principalTable: "GarageFees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Garages_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParkingSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GarageId = table.Column<int>(type: "INTEGER", nullable: false),
                    CarId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HourlyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    DailyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    MonthlyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsCleared = table.Column<bool>(type: "INTEGER", nullable: false),
                    GarageId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingSessions_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParkingSessions_Garages_GarageId",
                        column: x => x.GarageId,
                        principalTable: "Garages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParkingSessions_Garages_GarageId1",
                        column: x => x.GarageId1,
                        principalTable: "Garages",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "LicensePlate" },
                values: new object[,]
                {
                    { 1, "ABC123" },
                    { 2, "XYZ789" },
                    { 3, "LMN456" }
                });

            migrationBuilder.InsertData(
                table: "GarageFees",
                columns: new[] { "Id", "DailyRate", "HourlyRate", "MonthlyRate" },
                values: new object[,]
                {
                    { 1, 20.00m, 2.00m, 200.00m },
                    { 2, 40.00m, 4.00m, 400.00m }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "AddressCoordinates", "Area" },
                values: new object[,]
                {
                    { 1, "42.659892717892355, 23.315800826629413", "Center" },
                    { 2, "42.66122100925116, 23.350871009687925", "Mladost" },
                    { 3, "42.71897920798329, 23.276870966086467", "Lyulin" }
                });

            migrationBuilder.InsertData(
                table: "Garages",
                columns: new[] { "Id", "Capacity", "GarageFeeId", "LocationId", "Name" },
                values: new object[,]
                {
                    { 1, 150, 1, 1, "Uptown Garage" },
                    { 2, 60, 2, 2, "Downtown Garage" },
                    { 3, 200, 1, 3, "Suburban Garage" }
                });

            migrationBuilder.InsertData(
                table: "ParkingSessions",
                columns: new[] { "Id", "AmountPaid", "CarId", "DailyRate", "EntryTime", "ExitTime", "GarageId", "GarageId1", "HourlyRate", "IsCleared", "MonthlyRate" },
                values: new object[,]
                {
                    { 1, 5.00m, 1, 0m, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 0m, true, 0m },
                    { 2, 0.00m, 2, 0m, new DateTime(2024, 2, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 0m, false, 0m },
                    { 3, 25.00m, 3, 0m, new DateTime(2024, 3, 3, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), 2, null, 0m, true, 0m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_LicensePlate",
                table: "Cars",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Garages_GarageFeeId",
                table: "Garages",
                column: "GarageFeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Garages_LocationId",
                table: "Garages",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_AddressCoordinates",
                table: "Locations",
                column: "AddressCoordinates",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSessions_CarId",
                table: "ParkingSessions",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSessions_GarageId",
                table: "ParkingSessions",
                column: "GarageId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSessions_GarageId1",
                table: "ParkingSessions",
                column: "GarageId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkingSessions");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Garages");

            migrationBuilder.DropTable(
                name: "GarageFees");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
