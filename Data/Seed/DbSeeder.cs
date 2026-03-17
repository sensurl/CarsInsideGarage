using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Enums;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite;
using NetTopologySuite.Geometries;


namespace CarsInsideGarage.Data.Seed
{
    public class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var context = services.GetRequiredService<GarageDbContext>();

            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager, roleManager);
            await SeedOwnersAsync(userManager, roleManager);
            await SeedGarages(context, userManager);

            await context.SaveChangesAsync();
        }


        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Driver", "GarageOwner", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            const string adminEmail = "admin@sofia-parking.com";
            const string adminPassword = "AdminZer07!";

            // Create Admin role if missing
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Check if admin user exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }


        private static async Task SeedOwnersAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            var owners = new[]
            {
        "owner1@sofia-parking.com",
        "owner2@sofia-parking.com"
    };

            foreach (var email in owners)
            {
                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(user, "0wnerP@$$");
                    await userManager.AddToRoleAsync(user, "GarageOwner");
                }
            }
        }

        private static async Task SeedGarages(GarageDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (context.Garages.Any())
                return;

            var owner = await userManager.FindByEmailAsync("owner1@sofia-parking.com");

            if (owner == null)
                throw new Exception("Owner not found during seeding.");

            var garagesData = new[]
{
    new { Name = "Center Garage", Lat = 42.6977, Lng = 23.3219, Area = Area.Center },
    new { Name = "Mladost Garage", Lat = 42.6500, Lng = 23.3800, Area = Area.Mladost },
    new { Name = "Lyulin Garage", Lat = 42.7200, Lng = 23.2500, Area = Area.Lyulin }
};

                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            foreach (var g in garagesData)
            {

                var location = new CarsInsideGarage.Data.Entities.Location
                {
                    Area = g.Area,
                    ParkingCoordinates = geometryFactory.CreatePoint(new Coordinate(g.Lng, g.Lat))

                };

                context.Locations.Add(location);
                await context.SaveChangesAsync();

                var policy = new PricingPolicy(2, 15, 200);

                var garage = new Garage(
                    g.Name,
                    50,
                    location.Id,
                    policy,
                    owner.Id
                );

                context.Garages.Add(garage);
            }


        }
    }
}
