using System.Data;
using System.Net;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace IdeKusgozManagement.WebAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
        {
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Information()
           .Enrich.FromLogContext()
           .Enrich.WithProperty("Application", "IdeKusgoz.WebAPI")
           .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
           .Enrich.WithClientIp()
           .WriteTo.Console(
               restrictedToMinimumLevel: LogEventLevel.Information,
               outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
           .WriteTo.File("logs/api-application-.log",
               restrictedToMinimumLevel: LogEventLevel.Information,
               rollingInterval: RollingInterval.Day,
               retainedFileCountLimit: 30,
               fileSizeLimitBytes: 10_000_000,
               rollOnFileSizeLimit: true,
               outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
           .WriteTo.MSSqlServer(
               connectionString: configuration.GetConnectionString("SqlConnection"),
               sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
               {
                   TableName = "Logs",
                   AutoCreateSqlTable = true,
                   BatchPostingLimit = 50,
                   BatchPeriod = TimeSpan.FromSeconds(5)
               },
               restrictedToMinimumLevel: LogEventLevel.Information,
               columnOptions: GetColumnOptions())
           .WriteTo.Email(
               from: configuration["EmailConfiguration:FromEmail"],
               to: GetEmailRecipients(configuration),
               host: configuration["EmailConfiguration:Host"],
               port: int.Parse(configuration["EmailConfiguration:Port"] ?? "587"),
               connectionSecurity: MailKit.Security.SecureSocketOptions.StartTls,
               credentials: new NetworkCredential(
                   configuration["EmailConfiguration:FromEmail"],
                   configuration["EmailConfiguration:Password"]
               ),
               subject: "🚨 Kuşgöz API Hata Bildirimi - {Level} - {Timestamp:yyyy-MM-dd HH:mm}",
               restrictedToMinimumLevel: LogEventLevel.Warning

               )
           .CreateLogger();

            host.UseSerilog();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = int.MaxValue;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.Zero;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                         .WithMethods("GET", "POST", "PUT", "DELETE")
                         .WithHeaders(
                             "Content-Type",
                             "Authorization",
                             "X-Requested-With",
                             "Accept",
                             "Origin"
                         )
                         .WithExposedHeaders("X-Pagination")
                         .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                });
            });

            services.AddControllers(options =>
            {
                options.ModelValidatorProviders.Clear();
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddAuthorization();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(setup =>
            {
                var jwtSecuritySheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecuritySheme.Reference.Id, jwtSecuritySheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecuritySheme, Array.Empty<string>() }
                });
            });

            return services;
        }

        private static ColumnOptions GetColumnOptions()
        {
            var columnOptions = new ColumnOptions();

            // Varsayılan kolonları kaldır
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.Store.Remove(StandardColumn.Properties);

            // Custom kolonlar ekle
            columnOptions.AdditionalColumns = new List<SqlColumn>
            {
                new SqlColumn("UserId", SqlDbType.NVarChar, dataLength: 50),
                new SqlColumn("UserName", SqlDbType.NVarChar, dataLength: 100),
                new SqlColumn("RequestPath", SqlDbType.NVarChar, dataLength: 255),
                new SqlColumn("RequestMethod", SqlDbType.NVarChar, dataLength: 10),
                new SqlColumn("TraceId", SqlDbType.NVarChar, dataLength: 50),
                new SqlColumn("MachineName", SqlDbType.NVarChar, dataLength: 50),
                new SqlColumn("Environment", SqlDbType.NVarChar, dataLength: 20)
            };

            return columnOptions;
        }

        private static string GetEmailRecipients(IConfiguration configuration)
        {
            var toEmails = configuration.GetSection("EmailConfiguration:ToEmails").Get<string[]>();

            if (toEmails != null && toEmails.Length > 0)
            {
                return string.Join(",", toEmails); // Çoklu email için virgül ile ayır
            }

            // Fallback to single email
            return configuration["EmailConfiguration:ToEmail"] ?? configuration["EmailConfiguration:FromEmail"];
        }
    }
}