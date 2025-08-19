using System.Security.Claims;
using System.Threading.Tasks;
using IdeKusgozManagement.WebUI.Models.AuthModels;
using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApiService;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserApiService _userApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IAuthApiService authApiService, ILogger<AccountController> logger, IUserApiService userApiService, IHttpContextAccessor httpContextAccessor)
        {
            _authApiService = authApiService;
            _logger = logger;
            _userApiService = userApiService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("giris-yap")]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                // Session bilgilerini kontrol et
                var userId = HttpContext.Session.GetString("UserId");
                var jwtToken = HttpContext.Session.GetString("JwtToken");

                // Eğer authenticated ama session bilgileri yoksa silent logout
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jwtToken))
                {
                    // Logout metodunu çağırmak yerine direkt temizle
                    try
                    {
                        await _authApiService.LogoutAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Silent logout API çağrısında hata");
                    }

                    // Session'ı temizle
                    HttpContext.Session.Clear();

                    // Cookie authentication'ı temizle
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    // Login sayfasını göster (redirect etme)
                    return View();
                }

                // Session bilgileri varsa normal yönlendirme
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

        [HttpGet("profil")]
        [Authorize]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _userApiService.GetUserByIdAsync(userId, cancellationToken);
            return View(response);
        }

        [HttpPut("profil-guncelle")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([FromBody] UpdateUserViewModel model, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            model.Id = userId;
            model.RoleName = null;
            model.IsActive = null;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.UpdateUserAsync(userId, model, cancellationToken);
            if (response.IsSuccess)
            {
                var httpContext = _httpContextAccessor.HttpContext;

                httpContext.Session.SetString("UserName", response.Data.UserName);
                httpContext.Session.SetString("FullName", response.Data.FullName);
                await UpdateUserClaims(response.Data);
            }
            return View(response);
        }

        private async Task UpdateUserClaims(UserViewModel user)
        {
            // Mevcut kimlik bilgilerini al
            var identity = (ClaimsIdentity)User.Identity;

            // Güncellenebilir claim'leri kaldır
            var claimsToRemove = identity.Claims.Where(c =>
                c.Type == ClaimTypes.Name ||
                c.Type == ClaimTypes.GivenName ||
                c.Type == "FullName" ||
                c.Type == ClaimTypes.Surname).ToList();

            foreach (var claim in claimsToRemove)
            {
                identity.RemoveClaim(claim);
            }

            // Yeni claim'leri ekle
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
            identity.AddClaim(new Claim("FullName", user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.Surname));

            // Yeni ClaimsPrincipal oluştur
            var principal = new ClaimsPrincipal(identity);

            // Authentication cookie'sini güncelle
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
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