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
        public DbSet<GarageFee> GarageFees { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<ParkingSession> ParkingSessions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GarageDbContext).Assembly);
        }
    }
}
