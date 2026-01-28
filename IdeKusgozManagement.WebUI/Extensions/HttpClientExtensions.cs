using IdeKusgozManagement.WebUI.Handlers;
using IdeKusgozManagement.WebUI.Services;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddApiHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["ApiSettings:BaseUrl"];

            services.AddHttpClient<IApiService, ApiService>(client => ConfigureClient(client, baseUrl)).AddHttpMessageHandler<JwtTokenHandler>();
            // Token gerektiren API servisleri
            services.AddScoped<IUserApiService, UserApiService>();
            services.AddScoped<IRoleApiService, RoleApiService>();
            services.AddScoped<IWorkRecordApiService, WorkRecordApiService>();
            services.AddScoped<IEquipmentApiService, EquipmentApiService>();
            services.AddScoped<IExpenseApiService, ExpenseApiService>();
            services.AddScoped<IDepartmentApiService, DepartmentApiService>();
            services.AddScoped<IDocumentApiService, DocumentApiService>();
            services.AddScoped<IFileApiService, FileApiService>();
            services.AddScoped<ICompanyApiService, CompanyApiService>();
            services.AddScoped<IMessageApiService, MessageApiService>();
            services.AddScoped<IHolidayApiService, HolidayApiService>();
            services.AddScoped<ILeaveRequestApiService, LeaveRequestApiService>();
            services.AddScoped<IMachineWorkRecordApiService, MachineWorkRecordApiService>();
            services.AddScoped<IUserBalanceApiService, UserBalanceApiService>();
            services.AddScoped<ITrafficTicketApiService, TrafficTicketApiService>();
            services.AddScoped<IProjectApiService, ProjectApiService>();
            services.AddScoped<IAdvanceApiService, AdvanceApiService>();
            services.AddScoped<INotificationApiService, NotificationApiService>();
            services.AddScoped<ICompanyPaymentApiService, CompanyPaymentApiService>();
            services.AddScoped<IParameterApiService, ParameterApiService>();

            // Auth servisi (özel durum)
            services.AddScoped<IAuthApiService, AuthApiService>();

            // Named clients
            services.AddHttpClient("AuthApiWithToken", client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient("AuthApiWithoutToken", client => ConfigureClient(client, baseUrl));

            return services;
        }

        private static void ConfigureClient(HttpClient client, string baseUrl)
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        }
    }
}