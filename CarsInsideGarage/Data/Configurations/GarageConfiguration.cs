using CarsInsideGarage.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CarsInsideGarage.Data.Enums;

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

            builder.HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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

                pricing.Navigation(p => p.Rules)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            builder.OwnsOne(g => g.Location, location =>
{
    location.Property(l => l.Area)
        .HasConversion(
            v => v.ToString(),
            v => (Area)Enum.Parse(typeof(Area), v))
        .HasMaxLength(20)
        .IsRequired();

    location.Property(l => l.Latitude)
        .HasColumnName("Latitude")
        .IsRequired();

    location.Property(l => l.Longitude)
        .HasColumnName("Longitude")
        .IsRequired();

    location.Property(l => l.ParkingCoordinates)
        .HasColumnType("geography")
        .IsRequired();

    location.HasIndex(l => new { l.Latitude, l.Longitude })
            .IsUnique();
});

          


        }
    }
    }
