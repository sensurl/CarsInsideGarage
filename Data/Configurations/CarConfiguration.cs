using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace CarsInsideGarage.Data.Configurations
{
    public class CarConfiguration : IEntityTypeConfiguration<Entities.Car>
    {
        public void Configure(EntityTypeBuilder<Entities.Car> builder)
        {
            builder.ToTable("Cars");

            // Configure the Car builder here
            builder.HasKey(c => c.Id);

            builder.Property(c => c.LicensePlate)
                .IsRequired()
                .HasMaxLength(12);

            // Ensure License Plate is unique to avoid duplicate car entities
            builder.HasIndex(c => c.LicensePlate)
                .IsUnique();


            // Seed data
            builder.HasData(
                new Entities.Car
                {
                    Id = 1,
                    LicensePlate = "ABC123"
                },
                new Entities.Car
                {
                    Id = 2,
                    LicensePlate = "XYZ789"
                },
                new Entities.Car
                {
                    Id = 3,
                    LicensePlate = "LMN456"
                }
                );
        }
    }
}
