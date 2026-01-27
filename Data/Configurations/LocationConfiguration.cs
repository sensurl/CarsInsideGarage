using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums; // Access your Enum
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsInsideGarage.Data.Configurations
{

    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(l => l.Id);

            // VALUE CONVERTER:
            // This tells EF Core: "When saving to DB, convert Enum to String. 
            // When reading from DB, convert String back to Enum."
            builder.Property(l => l.Area)
                .HasConversion(
                    v => v.ToString(),                    // To Database
                    v => (Area)Enum.Parse(typeof(Area), v) // From Database
                )
                .HasMaxLength(20) // Always set a max length for string columns!
                .IsRequired();

            builder.Property(l => l.AddressCoordinates)
                .IsRequired()
                .HasMaxLength(50);

            // Seed data
            builder.HasData(
                new Location
                {
                    Id = 1,
                    Area = Area.Center,
                    AddressCoordinates = "42.659892717892355, 23.315800826629413"
                },
                new Location
                {
                    Id = 2,
                    Area = Area.Mladost,
                    AddressCoordinates = "42.66122100925116, 23.350871009687925"
                },
                new Location
                {
                    Id = 3,
                    Area = Area.Lyulin,
                    AddressCoordinates = "42.71897920798329, 23.276870966086467"
                }
            );
        }
    }
}