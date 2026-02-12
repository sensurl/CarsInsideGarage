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

            // Configure the Car builder
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CarPlateNumber)
                .IsRequired()
                .HasMaxLength(12);

            // Ensure License Plate is unique to avoid duplicate car entities
            builder.HasIndex(c => c.CarPlateNumber)
                .IsUnique();


            // Seed data
            builder.HasData(
                new Entities.Car
                {
                    Id = 1,
                    CarPlateNumber = "ABC123"
                },
                new Entities.Car
                {
                    Id = 2,
                    CarPlateNumber = "XYZ789"
                },
                new Entities.Car
                {
                    Id = 3,
                    CarPlateNumber = "LMN456"
                }
                );
        }
    }
}
