using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace IdeKusgozManagement.WebAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
        {
            var connectionString = configuration.GetConnectionString("SqlConnection");
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            using var tempServiceProvider = services.BuildServiceProvider();
            var emailOptions = tempServiceProvider.GetRequiredService<IOptions<EmailOptionsDTO>>();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "IdeKusgoz.WebAPI")
                .Enrich.WithProperty("Environment", environment)
                .Enrich.WithClientIp()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Logger(lc =>
                {
                    if (environment == "Development")
                    {
                        lc.WriteTo.Console(
                            restrictedToMinimumLevel: LogEventLevel.Information,
                            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
                    }
                })
                .WriteTo.File("logs/api-application-.log",
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information || evt.Level == LogEventLevel.Debug)
                    .WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            TableName = "InformationLogs",
                            SchemaName = "dbo",
                            AutoCreateSqlTable = true,
                            BatchPostingLimit = 1000,
                            BatchPeriod = TimeSpan.FromSeconds(30)
                        },
                        columnOptions: GetInformationColumnOptions()))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level >= LogEventLevel.Warning)
                    .WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            TableName = "ErrorLogs",
                            SchemaName = "dbo",
                            AutoCreateSqlTable = true,
                            BatchPostingLimit = 100,
                            BatchPeriod = TimeSpan.FromSeconds(10)
                        },
                        columnOptions: GetErrorColumnOptions()))
                .WriteTo.Email(
                    from: configuration["EmailConfiguration:FromEmail"],
                    to: GetEmailRecipients(emailOptions),
                    host: configuration["EmailConfiguration:Host"],
                    port: int.Parse(configuration["EmailConfiguration:Port"] ?? "587"),
                    connectionSecurity: MailKit.Security.SecureSocketOptions.StartTls,
                    credentials: new NetworkCredential(
                        configuration["EmailConfiguration:FromEmail"],
                        configuration["EmailConfiguration:Password"]
                    ),
                    subject: "🚨 Kuşgöz API Hata Bildirimi - {Level} - {Timestamp:yyyy-MM-dd HH:mm}",
                    restrictedToMinimumLevel: LogEventLevel.Error
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

            services.AddDbContext<ApplicationDbContext>(opts =>
               opts.UseSqlServer(connectionString,
                   sqlOptions =>
                   {
                       sqlOptions.UseCompatibilityLevel(120); // SQL Server 2014 = 120
                   }));

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // CORS konfigürasyonu - SignalR için güncellenmiş
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost:5290") // Tüm client URL'leri
                          .WithMethods("GET", "POST", "PUT", "DELETE")
                          .WithHeaders(
                              "Content-Type",
                              "Authorization",
                              "X-Requested-With",
                              "Accept",
                              "Origin",
                              "X-SignalR-User-Agent"
                          )
                          .WithExposedHeaders("X-Pagination")
                          .AllowCredentials();
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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer();

            services.AddAuthorization();

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true; // Hata detayları için
            });

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

        // Diğer metodlar aynı kalır...
        private static ColumnOptions GetInformationColumnOptions()
        {
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Add(StandardColumn.LogEvent);
            columnOptions.DisableTriggers = true;
            columnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { ColumnName = "UserId", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true },
                new SqlColumn { ColumnName = "Action", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
                new SqlColumn { ColumnName = "Module", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
                new SqlColumn { ColumnName = "ClientIP", DataType = SqlDbType.NVarChar, DataLength = 45, AllowNull = true }
            };
            return columnOptions;
        }

        private static ColumnOptions GetErrorColumnOptions()
        {
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Add(StandardColumn.LogEvent);
            columnOptions.DisableTriggers = true;
            columnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { ColumnName = "UserId", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true },
                new SqlColumn { ColumnName = "Action", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
                new SqlColumn { ColumnName = "Module", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
                new SqlColumn { ColumnName = "ClientIP", DataType = SqlDbType.NVarChar, DataLength = 45, AllowNull = true },
                new SqlColumn { ColumnName = "UserAgent", DataType = SqlDbType.NVarChar, DataLength = 500, AllowNull = true },
                new SqlColumn { ColumnName = "RequestId", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true }
            };
            return columnOptions;
        }

        private static string GetEmailRecipients(IOptions<EmailOptionsDTO> options)
        {
            var smtpOptions = options.Value;
            if (smtpOptions.ToEmails != null && smtpOptions.ToEmails.Length > 0)
            {
                return string.Join(",", smtpOptions.ToEmails);
            }
            return smtpOptions.FromEmail;
        }
    }
}