using IdeKusgozManagement.Application;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure;
using IdeKusgozManagement.Infrastructure.Data.Context;
using IdeKusgozManagement.Infrastructure.Data.Seed;
using IdeKusgozManagement.Infrastructure.Hubs;
using IdeKusgozManagement.WebAPI;
using IdeKusgozManagement.WebAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddWebAPIServices(builder.Configuration, builder.Host);

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

// Database initialization
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    var context = services.GetRequiredService<ApplicationDbContext>();

    // Database connection check (all environments)
    var canConnect = context.Database.CanConnect();
    if (!canConnect)
    {
        logger.LogError("Database connection failed!");
        throw new Exception("Database is not accessible");
    }

    Console.WriteLine("Database connection: OK");
    logger.LogInformation("Database connection established successfully");

    // Development-only migration
    if (app.Environment.IsDevelopment())
    {
        context.Database.Migrate();
        Console.WriteLine("Database migration completed successfully");
        logger.LogInformation("Database migration completed");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await IdentityDataSeeder.SeedAdminUserAndRolesAsync(userManager, roleManager);
    }

    logger.LogInformation("Database initialization completed successfully");
}
catch (Exception ex)
{
    logger.LogError(ex, "Database initialization failed: {Message}", ex.Message);
    Console.WriteLine($"Database error: {ex.Message}");
    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

    // Fail fast in production
    if (!app.Environment.IsDevelopment())
    {
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "IdeKusgozManagement.WebAPI v1");
        options.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

// Security and CORS
app.UseCors();

app.UseRouting();
app.UseMiddleware<GlobalExceptionMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Controllers
    endpoints.MapControllers();

    // SignalR Hubs
    endpoints.MapHub<CommunicationHub>("/communicationHub");
});

app.Lifetime.ApplicationStopping.Register(Log.CloseAndFlush);

app.Run();