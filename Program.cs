using AutoMapper;
using CarsInsideGarage.Areas.Admin.Services;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Data.Seed;
using CarsInsideGarage.Interfaces;
using CarsInsideGarage.Mappings;
using CarsInsideGarage.Repositories;
using CarsInsideGarage.Services.Car;
using CarsInsideGarage.Services.CarService;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.GarageSession;
using CarsInsideGarage.Services.PricingCalculator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<GarageDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GarageDbContext>(options =>
{
    var environment = builder.Environment.EnvironmentName;
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    DbContextOptionsFactory.Configure(options, environment, connectionString);
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // ToDo - Check every 0 minutes if the user is still valid
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/Error/403";
    options.Events.OnValidatePrincipal = async context =>
    {
        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(context.Principal);

        if (user == null || user.IsDeleted)
        {
            context.RejectPrincipal();
            await context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>().SignOutAsync();
        }
    };
});


builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IGarageService, GarageService>();
builder.Services.AddScoped<IParkingSessionService, ParkingSessionService>();
builder.Services.AddScoped<IPricingCalculator, PricingCalculator>();


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

var app = builder.Build();

// --- Ensure ApplicationUser tables exist and seed initial data ---

using (var scope = app.Services.CreateScope())
{
    
    var services = scope.ServiceProvider;
    await DbSeeder.SeedAsync(services);


    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    try
    {
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
    catch (AutoMapperConfigurationException ex)
    {
        Console.WriteLine("=== AutoMapper configuration error ===");
        Console.WriteLine(ex.ToString());

        foreach (var error in ex.Errors)
        {
            Console.WriteLine($"TypeMap: {error.TypeMap?.SourceType?.Name} -> {error.TypeMap?.DestinationType?.Name}");

            foreach (var unmappedProperty in error.UnmappedPropertyNames)
            {
                Console.WriteLine($"  Unmapped property: {unmappedProperty}");
            }
        }

        throw;
    }
}

Console.WriteLine($"ENVIRONMENT: {builder.Environment.EnvironmentName}");


// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    // For production: Handles 500 errors (Exceptions)
    app.UseExceptionHandler("/Home/Error/500");
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Home/Error/500");
}

app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// Endpoints
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
