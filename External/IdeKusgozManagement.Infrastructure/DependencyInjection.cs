using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
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
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

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
            services.AddScoped<ITrafficTicketService, TrafficTicketService>();
            services.AddScoped<IHolidayService, HolidayService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IAIService, OpenAIService>();
            services.AddSingleton<ChatClient>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<OpenAiOptionsDTO>>().Value;

                var clientOptions = new OpenAIClientOptions
                {
                    OrganizationId = options.OrganizationId,
                    ProjectId = options.ProjectId
                };

                var apiKeyCredential = new System.ClientModel.ApiKeyCredential(options.ApiKey);
                var openAIClient = new OpenAIClient(apiKeyCredential, clientOptions);

                return openAIClient.GetChatClient(options.Model);
            });
            services.ConfigureOptions<JwtOptionsSetup>();
            services.ConfigureOptions<JwtBearerOptionsSetup>();
            services.ConfigureOptions<HolidayApiOptionsSetup>();
            services.ConfigureOptions<EmailOptionsSetup>();
            services.ConfigureOptions<OpenAiOptionsSetup>();

            services.AddMemoryCache();

            return services;
        }
    }
}