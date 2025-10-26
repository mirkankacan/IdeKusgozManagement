using IdeKusgozManagement.Application;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure;
using IdeKusgozManagement.Infrastructure.Data.Context;
using IdeKusgozManagement.Infrastructure.Data.Seed;
using IdeKusgozManagement.Infrastructure.Hubs;
using IdeKusgozManagement.WebAPI;
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

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var canConnect = context.Database.CanConnect();
        Console.WriteLine($"Can connect to database: {canConnect}");
        context.Database.Migrate();
        Console.WriteLine("Database migration completed successfully");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await IdentityDataSeeder.SeedAdminUserAndRolesAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database error: {Message}", ex.Message);
        Console.WriteLine($"Database error: {ex.Message}");
        Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
    }
}
// Environment-specific middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
// Security and CORS
app.UseCors();
app.UseRouting();

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