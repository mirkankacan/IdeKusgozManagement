using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "__RequestVerificationToken";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Name = ".IdeKusgozManagement.WebUISession";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/giris-yap";
        options.LogoutPath = "/cikis-yap";
        options.AccessDeniedPath = "/erisim-engellendi";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.SlidingExpiration = true;
        options.Cookie.Name = "IdeKusgozManagementAuthCookie";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("FinansPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.IsInRole("Yönetici") ||
            context.User.IsInRole("Þef") ||
            (context.User.IsInRole("Personel") &&
             (context.User.HasClaim(c => c.Type == "DepartmentDutyName" &&
                (c.Value == "Muhasebe Meslek Elemaný" ||
                 c.Value == "Muhasebe Müdürü" ||
                 c.Value == "Finans Uzmaný"))))
        ));
    options.AddPolicy("MakinePolicy", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole("Admin") ||
           context.User.IsInRole("Yönetici") ||
           context.User.IsInRole("Þef") ||
           (context.User.IsInRole("Personel") &&
            (context.User.HasClaim(c => c.Type == "DepartmentDutyName" &&
               (c.Value == "Þoför-Yük Taþýma" ||
                c.Value == "Vinç Operatörü" ||
                c.Value == "Platform Operatörü"))))
       ));
});
builder.Services.AddHttpContextAccessor();
// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

builder.Services.AddTransient<JwtTokenHandler>();

// HTTP Client Services
builder.Services.AddApiHttpClients(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection(); // Added for production security
}
else
{
    app.UseDeveloperExceptionPage(); // Better error handling in development
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Redirect root to login
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/giris-yap");
        return;
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();