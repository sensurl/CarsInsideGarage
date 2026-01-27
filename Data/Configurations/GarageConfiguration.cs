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

            // Configuring the One-to-One or One-to-Many Relationship
            // Here we say: One Garage HAS ONE GarageFee.
            builder.HasOne(g => g.GarageFee)
                .WithMany() // A Fee set could potentially be used by many garages
                .HasForeignKey(g => g.GarageFeeId)
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict means: "You can't delete a Fee if a Garage is still using it."

            // Linkage: One Garage has One Location
            builder.HasOne(g => g.Location)
                .WithOne()
                .HasForeignKey<Garage>(g => g.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade means: If you delete the Garage, delete the Location record too.

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
        }
    }
}
