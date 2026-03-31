using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsInsideGarage.Data.Configurations
{
    public class CarConfiguration : IEntityTypeConfiguration<Entities.Car>
    {
        public void Configure(EntityTypeBuilder<Entities.Car> builder)
        {
            builder.ToTable("Cars");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CarPlateNumber)
                .IsRequired()
                .HasMaxLength(12);

            // Ensure License Plate is unique to avoid duplicate car entities
            builder.HasIndex(c => c.CarPlateNumber)
                .IsUnique();

            
        }
    }
}
