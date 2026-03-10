using CarsInsideGarage.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsInsideGarage.Data.Configurations
{
    public class GarageConfiguration : IEntityTypeConfiguration<Garage>
    {
        public void Configure(EntityTypeBuilder<Garage> builder)
        {
            builder.ToTable("Garages");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(50);

            builder.HasOne(g => g.Location)
                .WithOne()
                .HasForeignKey<Garage>(g => g.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(g => g.PricingPolicy, pricing =>
            {
                pricing.Property(p => p.HourlyRate)
                    .HasPrecision(18, 2)
                    .IsRequired();

                pricing.Property(p => p.DailyRate)
                    .HasPrecision(18, 2)
                    .IsRequired();

                pricing.Property(p => p.MonthlyRate)
                    .HasPrecision(18, 2)
                    .IsRequired();

                pricing.Navigation(p => p.Rules)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);


                pricing.OwnsMany(p => p.Rules, rule =>
                {
                    rule.ToTable("PricingRules");

                    rule.WithOwner().HasForeignKey("GarageId");

                    rule.HasKey(r => r.Id);

                    rule.Property(r => r.Multiplier)
                        .HasPrecision(5, 2);

                    rule.Property(r => r.Adjustment)
                        .HasPrecision(18, 2);

                    rule.Property(r => r.StartHour)
                        .HasColumnType("time");

                    rule.Property(r => r.EndHour)
                        .HasColumnType("time");
                });
            });
        }
    }

}

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
