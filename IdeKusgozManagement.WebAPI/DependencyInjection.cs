using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure.Data.Context;
using IdeKusgozManagement.Infrastructure.Data.Interceptors;
using IdeKusgozManagement.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Text.Json;

namespace IdeKusgozManagement.WebAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
        {
            services.AddScoped<GlobalExceptionMiddleware>();

            var connectionString = configuration.GetConnectionString("SqlConnection");
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            using var tempServiceProvider = services.BuildServiceProvider();
            var emailOptions = tempServiceProvider.GetRequiredService<IOptions<EmailOptionsDTO>>();

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .MinimumLevel.Override("System", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
               .Enrich.WithProperty("Application", "IdeKusgozManagement.WebAPI")
               .Enrich.WithProperty("Environment", environment)
               .Enrich.WithClientIp()
               .Enrich.WithMachineName()
               .Enrich.WithThreadId()
               .Enrich.WithCorrelationId()
               .WriteTo.Logger(lc =>
               {
                   if (environment == "Development")
                   {
                       lc.WriteTo.Console(
                           restrictedToMinimumLevel: LogEventLevel.Debug,
                           outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
                   }
               })
               .WriteTo.File("logs/api-.log",
                   restrictedToMinimumLevel: LogEventLevel.Information,
                   rollingInterval: RollingInterval.Day,
                   retainedFileCountLimit: 30,
                   fileSizeLimitBytes: 10_000_000,
                   rollOnFileSizeLimit: true,
                   shared: true,
                   outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
               .WriteTo.MSSqlServer(
                   connectionString: connectionString,
                   sinkOptions: new MSSqlServerSinkOptions
                   {
                       TableName = "IdtLogs",
                       SchemaName = "dbo",
                       AutoCreateSqlTable = true,
                       BatchPostingLimit = 500,
                       BatchPeriod = TimeSpan.FromSeconds(15)
                   },
                   restrictedToMinimumLevel: LogEventLevel.Information,
                   columnOptions: GetColumnOptions())
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
                    subject: "🚨 IdeKusgozManagement.WebAPI - {Level} - {Timestamp:dd-MM-yyyy HH:mm}",
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
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("FinansPolicy", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin") ||
                        context.User.IsInRole("Yönetici") ||
                        context.User.IsInRole("Şef") ||
                        (context.User.IsInRole("Personel") &&
                         (context.User.HasClaim(c => c.Type == "DepartmentDutyName" &&
                            (c.Value == "Muhasebe Meslek Elemanı" ||
                             c.Value == "Muhasebe Müdürü" ||
                             c.Value == "Finans Uzmanı"))))
                    ));
                options.AddPolicy("MakinePolicy", policy =>
                   policy.RequireAssertion(context =>
                       context.User.IsInRole("Admin") ||
                       context.User.IsInRole("Yönetici") ||
                       context.User.IsInRole("Şef") ||
                       (context.User.IsInRole("Personel") &&
                        (context.User.HasClaim(c => c.Type == "DepartmentDutyName" &&
                           (c.Value == "Şoför-Yük Taşıma" ||
                            c.Value == "Vinç Operatörü" ||
                            c.Value == "Platform Operatörü"))))
                   ));
            });
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddDbContext<ApplicationDbContext>((serviceProvider, opts) =>
            {
                opts.UseSqlServer(connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.UseCompatibilityLevel(120); // SQL Server 2014 = 120
                    })
                    .AddInterceptors(serviceProvider.GetRequiredService<AuditLogInterceptor>());
            });
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
                    policy.WithOrigins(
                      "http://localhost:5290",
                      "https://localhost:8484",
                      "http://192.168.2.253:5290",
                      "http://192.168.2.253:80",
                      "http://192.168.2.253",
                      "http://92.45.19.226:80",
                      "http://92.45.19.226",
                      "http://portal.izmircrane.com",
                      "https://portal.izmircrane.com"
           )
                        .WithMethods("GET", "POST", "PUT", "DELETE")
                        .WithHeaders(
                           "Content-Type",
                        "Authorization",
                        "X-Requested-With",
                        "Accept",
                        "Origin",
                        "X-SignalR-User-Agent",
                        "X-CSRF-TOKEN",
                        "Cache-Control"
                        )
                        .WithExposedHeaders("X-Pagination")
                        .AllowCredentials()
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

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                options.StreamBufferCapacity = 10;
                options.MaximumParallelInvocationsPerClient = 1;
            })
               .AddJsonProtocol(options =>
               {
                   options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
               });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "IdeKusgozManagement.WebAPI",
                    Version = "v1",
                    Description = "Kuşgöz İzmir Vinç Yönetim Sistemi API"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT token giriniz. Sadece token yazın, 'Bearer' eklemeyin.\n\nÖrnek: eyJhbGciOiJIUzI1NiIs..."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }

        private static ColumnOptions GetColumnOptions()
        {
            var columnOptions = new ColumnOptions();

            columnOptions.Store.Remove(StandardColumn.MessageTemplate);

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