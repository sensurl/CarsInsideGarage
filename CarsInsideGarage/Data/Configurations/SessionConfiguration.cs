using CarsInsideGarage.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsInsideGarage.Data.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Entities.ParkingSession>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Entities.ParkingSession> builder)
        {
            builder.ToTable("ParkingSessions");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EntryTime)
                   .IsRequired();

            builder.Property(e => e.ExitTime)
                   .IsRequired(false);

            builder.Property(e => e.IsCleared)
                   .IsRequired();

            builder.Property(e => e.AmountPaid)
                 .HasPrecision(18, 2)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            // Relationship with Garage
            builder.HasOne(ps => ps.Garage)
                   .WithMany(g => g.Sessions)
                   .HasForeignKey(ps => ps.GarageId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Car
            builder.HasOne(ps => ps.Car)
                   .WithMany(c => c.Sessions)
                   .HasForeignKey(ps => ps.CarId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.HourlyRate)
    .HasPrecision(18, 2);

            builder.Property(p => p.DailyRate)
                .HasPrecision(18, 2);

            builder.Property(p => p.MonthlyRate)
                .HasPrecision(18, 2);
            }
    }
}

           