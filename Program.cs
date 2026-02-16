using AutoMapper;
using CarsInsideGarage.Data;
using CarsInsideGarage.Data.Entities;
using CarsInsideGarage.Mappings;
using CarsInsideGarage.Services.Car;
using CarsInsideGarage.Services.CarService;
using CarsInsideGarage.Services.Fee;
using CarsInsideGarage.Services.Garage;
using CarsInsideGarage.Services.GarageSession;
using CarsInsideGarage.Services.Location;
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

builder.Services.AddAutoMapper(typeof(MappingProfile));

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

try
{
    mapperConfig.AssertConfigurationIsValid();
}
catch (AutoMapperConfigurationException ex)
{

    Debug.WriteLine(ex.Message);
    throw;
}

builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IGarageService, GarageService>();
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IParkingSessionService, ParkingSessionService>();
builder.Services.AddScoped<IGarageLocationService, GarageLocationService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// --- Ensure ApplicationUser tables exist and seed initial data ---

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Driver", "GarageOwner", "Admin" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

Console.WriteLine($"ENVIRONMENT: {builder.Environment.EnvironmentName}");


// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        Console.WriteLine(error?.ToString());
        await context.Response.WriteAsync("Something went wrong. Check console logs.");
    });
});

app.Run();
