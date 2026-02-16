using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CarsInsideGarage.Data
{
    public class GarageDbContextFactory : IDesignTimeDbContextFactory<GarageDbContext>
    {
        public GarageDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<GarageDbContext>();

            // Shared helper to be used by the factory and the program.cs
            DbContextOptionsFactory.Configure(optionsBuilder, environment, connectionString);

            return new GarageDbContext(optionsBuilder.Options);
        }
    }
}
