using System.Security.Claims;
using IdeKusgozManagement.WebUI.Models.AuthModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApiService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthApiService authApiService, ILogger<AccountController> logger)
        {
            _authApiService = authApiService;
            _logger = logger;
        }

        [HttpGet("giris-yap")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                return RedirectByRole(role);
            }
            return View();
        }

        [HttpPost("giris-yap")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Geçersiz giriş bilgileri");
                }

                var response = await _authApiService.LoginAsync(model, cancellationToken);

                if (!response.IsSuccess)
                {
                    return BadRequest(response.Errors?.Any() == true ? response.Errors : new[] { response.Message });
                }

                // Session'a kullanıcı bilgilerini kaydet
                var httpContext = HttpContext;
                if (httpContext != null && response.Data != null)
                {
                    httpContext.Session.SetString("JwtToken", response.Data.Token);
                    httpContext.Session.SetString("RefreshToken", response.Data.RefreshToken);
                    httpContext.Session.SetString("UserId", response.Data.UserId);
                    httpContext.Session.SetString("UserName", response.Data.UserName);
                    httpContext.Session.SetString("FullName", response.Data.FullName);
                    httpContext.Session.SetString("RoleName", response.Data.RoleName);

                    // Cookie authentication için claims oluştur
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, response.Data.UserId),
                        new Claim(ClaimTypes.Name, response.Data.UserName),
                        new Claim("FullName", response.Data.FullName),
                        new Claim(ClaimTypes.GivenName, response.Data.Name),
                        new Claim(ClaimTypes.Surname, response.Data.Surname),
                        new Claim(ClaimTypes.Role, response.Data.RoleName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);
                }

                var role = response.Data?.RoleName;
                var redirectUrl = GetRoleRedirectUrl(role);

                return Ok(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login işleminde hata oluştu");
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        [HttpGet("cikis-yap")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            try
            {
                await _authApiService.LogoutAsync(cancellationToken);

                // Session'ı temizle
                HttpContext.Session.Clear();

                // Cookie authentication'ı temizle
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Redirect("/giris-yap");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout işleminde hata oluştu");
                return Redirect("/giris-yap");
            }
        }

        private IActionResult RedirectByRole(string? role)
        {
            var redirectUrl = GetRoleRedirectUrl(role);
            return Redirect(redirectUrl);
        }

        private string GetRoleRedirectUrl(string? role)
        {
            return role switch
            {
                "Admin" => "/admin/ana-sayfa",
                "Şef" => "/sef/ana-sayfa",
                "Personel" => "/personel/ana-sayfa",
                _ => "/erisim-engellendi"
            };
        }
    }
}