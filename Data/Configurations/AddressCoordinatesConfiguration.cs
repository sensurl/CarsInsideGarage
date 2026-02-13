using CarsInsideGarage.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CarsInsideGarage.Data.Enums;
using Microsoft.EntityFrameworkCore;


namespace CarsInsideGarage.Data.Configurations
{
    public class AddressCoordinatesConfiguration : IEntityTypeConfiguration<AddressCoordinates>
    {
        
        public void Configure(EntityTypeBuilder<AddressCoordinates> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Latitude)
                   .HasPrecision(9, 6)
                   .IsRequired();

            builder.Property(x => x.Longitude)
                   .HasPrecision(9, 6)
                   .IsRequired();

            // Ensure uniqueness of the combination
            builder.HasIndex(x => new { x.Latitude, x.Longitude })
                   .IsUnique();

         
            /*
			builder.HasData(
                new AddressCoordinates
                {
                    Id = 1,
                    Latitude = 42.659893m,
                    Longitude = 23.315801m
                },
                new AddressCoordinates
                {
                    Id = 2,
                    Latitude = 42.661221m,
                    Longitude = 23.350871m
                },
                new AddressCoordinates
                {
                    Id = 3,
                    Latitude = 42.718979m,
                    Longitude = 23.276871m
                }
            );
			*/
        }
    }
    
    
}
