using IdeKusgozManagement.Application.Mappings;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace IdeKusgozManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMapster();
            UserMappingConfig.Configure();
            RoleMappingConfig.Configure();
            WorkRecordMappingConfig.Configure();
            WorkRecordExpenseMappingConfig.Configure();
            return services;
        }
    }
}