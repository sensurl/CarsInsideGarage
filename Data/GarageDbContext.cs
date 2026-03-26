using CarsInsideGarage.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CarsInsideGarage.Data
{
    public class GarageDbContext : IdentityDbContext<ApplicationUser>
    {
        public GarageDbContext(DbContextOptions<GarageDbContext> options) : base(options)
        {
        }

        public DbSet<Garage> Garages { get; set; } = null!;
        public DbSet<Car> Cars { get; set; } = null!;
        public DbSet<PricingRule> PricingRules { get; set; } = null!;
        public DbSet<ParkingSession> ParkingSessions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Filter out soft deleted entities from queries
            modelBuilder.Entity<ApplicationUser>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Car>()
                .HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<Garage>()
                .HasQueryFilter(g => !g.IsDeleted);

            modelBuilder.Entity<ParkingSession>()
                .HasQueryFilter(ps => !ps.Car.IsDeleted && ps.ExitTime == null);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GarageDbContext).Assembly);
        }
    }
}
