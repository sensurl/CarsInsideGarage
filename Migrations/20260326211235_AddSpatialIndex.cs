using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsInsideGarage.Migrations
{
    /// <inheritdoc />
    public partial class AddSpatialIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(
        @"CREATE SPATIAL INDEX IX_Garages_ParkingCoordinates
          ON Garages(Location_ParkingCoordinates)
          USING GEOGRAPHY_AUTO_GRID;"
    );
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(
        @"DROP INDEX IX_Garages_ParkingCoordinates
          ON Garages;"
    );
}
    }
}
