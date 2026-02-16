using CarsInsideGarage.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsInsideGarage.Data.Configurations
{
    public class FeeConfiguration : IEntityTypeConfiguration<GarageFee>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Entities.GarageFee> builder)
        {
            builder.ToTable("GarageFees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.HourlyRate)
                .HasPrecision(18, 2)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(e => e.DailyRate)
                .HasPrecision(18, 2)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(e => e.MonthlyRate)
               .HasPrecision(18, 2)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            /*
			// Seed data
            builder.HasData(
                new Entities.GarageFee
                {
                    Id = 1,
                    HourlyRate = 2.00m,
                    DailyRate = 20.00m,
                    MonthlyRate = 200.00m
                },
                new Entities.GarageFee
                {
                    Id = 2,
                    HourlyRate = 4.00m,
                    DailyRate = 40.00m,
                    MonthlyRate = 400.00m
                }
            );
			*/
        }
    }
}
