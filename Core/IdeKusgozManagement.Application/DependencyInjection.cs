using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace IdeKusgozManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMapster();
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(ApplicationAssembly).Assembly);
            return services;
        }
    }
}