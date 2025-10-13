using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.Interfaces.Providers;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Infrastructure.Authentication;
using IdeKusgozManagement.Infrastructure.OptionsSetup;
using IdeKusgozManagement.Infrastructure.Repositories;
using IdeKusgozManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace IdeKusgozManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            string connectionString = configuration.GetConnectionString("SqlConnection")!;

            // Providers
            services.AddScoped<IJwtProvider, JwtProvider>();
            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IEquipmentService, EquipmentService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWorkRecordService, WorkRecordService>();
            services.AddScoped<IWorkRecordExpenseService, WorkRecordExpenseService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(environment.WebRootPath)));
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IAdvanceService, AdvanceService>();

            services.ConfigureOptions<JwtOptionsSetup>();
            services.ConfigureOptions<JwtBearerOptionsSetup>();
            services.ConfigureOptions<HolidayApiOptionsSetup>();

            services.AddMemoryCache();

            return services;
        }
    }
}