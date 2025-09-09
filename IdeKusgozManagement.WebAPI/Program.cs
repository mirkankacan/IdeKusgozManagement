using IdeKusgozManagement.Application;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure;
using IdeKusgozManagement.Infrastructure.Data.Seed;
using IdeKusgozManagement.WebAPI;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebAPIServices(builder.Configuration, builder.Host);

var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await IdentityDataSeeder.SeedAdminUserAndRolesAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database seeding");
    }
}
// Environment-specific middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdeKusgoz Management API V1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Security and CORS
app.UseCors();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Endpoint mapping
app.MapControllers();

app.Lifetime.ApplicationStopping.Register(Log.CloseAndFlush);

app.Run();