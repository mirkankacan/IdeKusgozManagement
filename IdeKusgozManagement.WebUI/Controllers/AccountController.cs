using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [AllowAnonymous]
    public class AccountController(IAuthApiService authApiService) : Controller
    {
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
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken cancellationToken)
        {
            var response = await authApiService.LoginAsync(request, cancellationToken);
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessage);
            }
            var role = response.Data;
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
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            await authApiService.LogoutAsync(cancellationToken);
            return Redirect("/giris-yap");
        }
    }
}