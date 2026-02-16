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

            /*
			// Seed data

            builder.HasData(
                new ParkingSession
                {
                    Id = 1,
                    GarageId = 1,
                    CarId = 1,
                    EntryTime = new DateTime(2024, 1, 1, 10, 0, 0),
                    ExitTime = new DateTime(2024, 1, 1, 12, 0, 0),
                    AmountPaid = 5.00m,
                    IsCleared = true
                },
                new ParkingSession
                {
                    Id = 2,
                    GarageId = 1,
                    CarId = 2,
                    EntryTime = new DateTime(2024, 2, 2, 10, 0, 0),
                    ExitTime = new DateTime(2024, 2, 2, 12, 0, 0),
                    AmountPaid = 0.00m,
                    IsCleared = false
                },
                new ParkingSession
                {
                    Id = 3,
                    GarageId = 2,
                    CarId = 3,
                    EntryTime = new DateTime(2024, 3, 3, 10, 0, 0),
                    ExitTime = new DateTime(2024, 3, 3, 12, 0, 0),
                    AmountPaid = 25.00m,
                    IsCleared = true
                }
            );
			*/
        }
    }
}
