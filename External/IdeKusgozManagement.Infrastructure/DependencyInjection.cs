using IdeKusgozManagement.Application.Interfaces.Providers;
using IdeKusgozManagement.Application.Interfaces.Repositories;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authentication;
using IdeKusgozManagement.Infrastructure.Data.Context;
using IdeKusgozManagement.Infrastructure.OptionsSetup;
using IdeKusgozManagement.Infrastructure.Repositories;
using IdeKusgozManagement.Infrastructure.Services;
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
            // Providers
            services.AddScoped<IJwtProvider, JwtProvider>();
            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEquipmentService, EquipmentService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWorkRecordService, WorkRecordService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISignalRService, SignalRService>();

            services.ConfigureOptions<JwtOptionsSetup>();
            services.ConfigureOptions<JwtBearerOptionsSetup>();
            services.ConfigureOptions<HolidayApiOptionsSetup>();

            services.AddMemoryCache();

            return services;
        }
    }
}