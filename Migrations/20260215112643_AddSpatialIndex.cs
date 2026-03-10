using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsInsideGarage.Migrations
{
    /// <inheritdoc />
    public partial class AddSpatialIndex : Migration
    {
        /// <inheritdoc />
         protected override void Up(MigrationBuilder migrationBuilder)

{

    migrationBuilder.Sql(

        @"CREATE SPATIAL INDEX IX_ParkingCoordinates

          ON Locations(ParkingCoordinates)

          USING GEOGRAPHY_AUTO_GRID;"

    );

}



protected override void Down(MigrationBuilder migrationBuilder)

{

    migrationBuilder.Sql(

        @"DROP INDEX IX_ParkingCoordinates

          ON Locations;"

    );
}
    }
}
