using Microsoft.EntityFrameworkCore;

namespace CarsInsideGarage.Data
{
    public static class DbContextOptionsFactory
    {
        public static void Configure(DbContextOptionsBuilder options, string environment, string connectionString)
        {
            if (environment == "Development")
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseSqlServer(connectionString, x => x.UseNetTopologySuite());
            }
        }
    }
}
