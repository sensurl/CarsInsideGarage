using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsInsideGarage.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Area)
                .HasConversion(
                    v => v.ToString(),
                    v => (Area)Enum.Parse(typeof(Area), v))
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(l => l.ParkingCoordinates)
                .HasColumnType("geography")
                .IsRequired();

            /*
			// Seed using FK
            builder.HasData(
                new Location
                {
                    Id = 1,
                    Area = Area.Center,
                    AddressCoordinatesId = 1
                },
                new Location
                {
                    Id = 2,
                    Area = Area.Mladost,
                    AddressCoordinatesId = 2
                },
                new Location
                {
                    Id = 3,
                    Area = Area.Lyulin,
                    AddressCoordinatesId = 3
                }
            );
			*/
        }
    }
}
