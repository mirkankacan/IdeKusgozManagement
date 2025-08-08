using IdeKusgozManagement.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdeKusgozManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("SqlConnection")!;

            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseSqlServer(connectionString));

            return services;
        }
    }
}