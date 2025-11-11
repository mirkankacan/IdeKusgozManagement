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

            // Token gerektiren API servisleri
            services.AddHttpClient<IUserApiService, UserApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IRoleApiService, RoleApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IWorkRecordApiService, WorkRecordApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IEquipmentApiService, EquipmentApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IExpenseApiService, ExpenseApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IFileApiService, FileApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IDepartmentApiService, DepartmentApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IDocumentApiService, DocumentApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IMessageApiService, MessageApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IHolidayApiService, HolidayApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<ILeaveRequestApiService, LeaveRequestApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<ITrafficTicketApiService, TrafficTicketApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IProjectApiService, ProjectApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<IAdvanceApiService, AdvanceApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

            services.AddHttpClient<INotificationApiService, NotificationApiService>(client => ConfigureClient(client, baseUrl))
                    .AddHttpMessageHandler<JwtTokenHandler>();

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
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        }
    }
}