using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models.AuthModels;
using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("token-al")]
        [Authorize]
        public IActionResult GetToken()
        {
            // Session'dan JWT token'ı al
            var jwtToken = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(jwtToken))
            {
                return Unauthorized("Token bulunamadı");
            }

            return Ok(new { token = jwtToken });
        }

        [HttpGet("giris-yap")]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                // Session bilgilerini kontrol et
                var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
                var jwtToken = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");

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
                    _httpContextAccessor.HttpContext?.Session.Clear();

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
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Geçersiz giriş bilgileri");
                }
                return await LoginProcessAsync(model, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login işleminde hata oluştu");
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        [HttpGet("sifremi-unuttum")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("sifremi-unuttum")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Geçersiz şifre sıfırlama bilgileri");
                }

                var response = await _authApiService.SendResetPasswordEmailAsync(model, cancellationToken);

                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForgotPassword işleminde hata oluştu");
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        [HttpPost("sifre-sifirla")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Geçersiz şifre sıfırlama bilgileri");
                }
                var resetResponse = await _authApiService.ResetPasswordAsync(model, cancellationToken);

                if (!resetResponse.IsSuccess)
                {
                    return BadRequest(resetResponse);
                }

                var loginModel = new LoginViewModel
                {
                    TCNo = model.TCNo,
                    Password = model.NewPassword,
                    RememberMe = true
                };
                return await LoginProcessAsync(loginModel, cancellationToken);
            }
            catch (Exception)
            {
                return BadRequest("Şifre sıfırlanırken bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        [HttpGet("cikis-yap")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            try
            {
                await _authApiService.LogoutAsync(cancellationToken);

                // Session'ı temizle
                _httpContextAccessor.HttpContext?.Session.Clear();

                // Cookie authentication'ı temizle
                if (_httpContextAccessor.HttpContext != null)
                {
                    await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }

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
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            var response = await _userApiService.GetMyUserAsync(cancellationToken);
            return View(response);
        }

        [HttpPut("profil-guncelle")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([FromBody] UpdateUserViewModel model, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }

            model.RoleName = null;
            model.IsActive = true;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.UpdateUserAsync(userId, model, cancellationToken);
            if (response.IsSuccess && response.Data != null)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    httpContext.Session.SetString("TCNo", response.Data.TCNo);
                    httpContext.Session.SetString("FullNameWithExp", response.Data.FullNameWithExp);
                    httpContext.Session.SetString("RoleName", response.Data.RoleName);
                    httpContext.Session.SetString("FullName", response.Data.FullName);
                    await UpdateUserClaims(response.Data);
                }
            }
            return View(response);
        }

        private async Task UpdateUserClaims(UserViewModel user)
        {
            // Mevcut kimlik bilgilerini al
            if (User.Identity is not ClaimsIdentity identity)
            {
                return;
            }

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
            identity.AddClaim(new Claim("TCNo", user.TCNo));
            identity.AddClaim(new Claim("FullNameWithExp", user.FullNameWithExp));
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
                "Admin" or "Yönetici" or "Şef" or "Personel" => "/ana-sayfa",
                _ => "/erisim-engellendi"
            };
        }

        private async Task<IActionResult> LoginProcessAsync(LoginViewModel model, CancellationToken cancellationToken)
        {
            var response = await _authApiService.LoginAsync(model, cancellationToken);

            if (!response.IsSuccess)
            {
                var errorMessage = !string.IsNullOrEmpty(response.ErrorMessage)
                    ? response.ErrorMessage
                    : !string.IsNullOrEmpty(response.ErrorDetail)
                        ? response.ErrorDetail
                        : "Giriş başarısız";
                return BadRequest(new[] { errorMessage });
            }

            // Session'a kullanıcı bilgilerini kaydet
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && response.Data != null)
            {
                httpContext.Session.SetString("JwtToken", response.Data.Token);
                httpContext.Session.SetString("RefreshToken", response.Data.RefreshToken);
                httpContext.Session.SetString("UserId", response.Data.UserId);
                httpContext.Session.SetString("TCNo", response.Data.TCNo);
                httpContext.Session.SetString("FullNameWithExp", response.Data.FullNameWithExp);
                httpContext.Session.SetString("FullName", response.Data.FullName);
                httpContext.Session.SetString("RoleName", response.Data.RoleName);
                httpContext.Session.SetString("DepartmentName", response.Data.DepartmentName);
                httpContext.Session.SetString("DepartmentDutyName", response.Data.DepartmentDutyName);

                // Cookie authentication için claims oluştur
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, response.Data.UserId),
                        new Claim("TCNo", response.Data.TCNo),
                        new Claim("FullNameWithExp", response.Data.FullNameWithExp),
                        new Claim("FullName", response.Data.FullName),
                        new Claim("DepartmentName", response.Data.DepartmentName),
                        new Claim("DepartmentDutyName", response.Data.DepartmentDutyName),
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
            return Ok(new { redirectUrl = "/ana-sayfa" });
        }
    }
}