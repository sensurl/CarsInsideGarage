using CarsInsideGarage.Data.Configurations;
using CarsInsideGarage.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Data
{
    public class GarageDbContext : DbContext
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

            modelBuilder.ApplyConfiguration(new CarConfiguration());
            modelBuilder.ApplyConfiguration(new FeeConfiguration());
            modelBuilder.ApplyConfiguration(new GarageConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new SessionConfiguration());

            base.OnModelCreating(modelBuilder);
        
        }

    }
}
