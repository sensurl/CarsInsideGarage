using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CarsInsideGarage.Data
{
    public class GarageDbContextFactory : IDesignTimeDbContextFactory<GarageDbContext>
    {
        public GarageDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GarageDbContext>();
            optionsBuilder.UseSqlite("Data Source=study.db"); // same in appsettings.json

            return new GarageDbContext(optionsBuilder.Options);
        }
    }
}
