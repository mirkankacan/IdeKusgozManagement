using System.Security.Claims;
using IdeKusgozManagement.WebUI.Models.AuthModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApiService;

        public AccountController(IAuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet("giris-yap")]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                switch (role)
                {
                    case "Admin":
                        return Redirect("/admin/ana-sayfa");

                    case "Şef":
                        return Redirect("/sef/ana-sayfa");

                    case "Personel":
                        return Redirect("/personel/ana-sayfa");
                }
            }
            return View();
        }

        [HttpPost("giris-yap")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _authApiService.LoginAsync(model, cancellationToken);
            if (!response.IsSuccess)
            {
                return BadRequest(response.Errors);
            }
            var role = response.Data.RoleName;
            switch (role)
            {
                case "Admin":
                    return Ok("/admin/ana-sayfa");

                case "Şef":
                    return Ok("/sef/ana-sayfa");

                case "Personel":
                    return Ok("/personel/ana-sayfa");

                default:
                    return Ok("/403");
            }
        }

        [HttpGet("cikis-yap")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            await _authApiService.LogoutAsync(cancellationToken);
            return Redirect("/giris-yap");
        }
    }
}