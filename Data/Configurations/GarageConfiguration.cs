using CarsInsideGarage.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsInsideGarage.Data.Configurations
{
    public class GarageConfiguration : IEntityTypeConfiguration<Entities.Garage>
    {
        public void Configure(EntityTypeBuilder<Garage> builder)
        {
            builder.ToTable("Garages");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(50);

            builder.HasOne(g => g.GarageFee)
                .WithMany(f => f.Garages)
                .HasForeignKey(g => g.ParkingFeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Garage has One Location
            builder.HasOne(g => g.Location)
                .WithOne()
                .HasForeignKey<Garage>(g => g.LocationId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting Garage deletes the Location record too.

            /*
            // Seed data
                        builder.HasData(
                            new Garage
                            {
                                Id = 1,
                                Name = "Uptown Garage",
                                LocationId = 1,
                                Capacity = 150,
                                GarageFeeId = 1

                            },
                            new Garage
                            {
                                Id = 2,
                                Name = "Downtown Garage",
                                LocationId = 2,
                                Capacity = 60,
                                GarageFeeId = 2
                            },
                            new Garage
                            {
                                Id = 3,
                                Name = "Suburban Garage",
                                LocationId = 3,
                                Capacity = 200,
                                GarageFeeId = 1
                            }

                        );
            */
        }
    }
}
