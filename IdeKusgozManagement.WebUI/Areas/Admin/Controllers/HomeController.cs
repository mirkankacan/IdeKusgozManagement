using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("admin")]
    public class HomeController : Controller
    {
        private readonly IUserApiService _userApiService;

        public HomeController(IUserApiService userApiService)
        {
            _userApiService = userApiService;
        }

        [HttpGet("ana-sayfa")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.GetAllUsersAsync(cancellationToken);
            if (response.IsSuccess)
            {
                var userCount = response.Data?.Count() ?? 0;

                ViewBag.UserCount = userCount;
                ViewBag.UserProgressPercent = CalculateProgressPercent(userCount, 1000);
            }
            else
            {
                ViewBag.UserCount = 0;
                ViewBag.UserProgressPercent = 0;
            }
            return View();
        }

        private static int CalculateProgressPercent(int current, int max)
        {
            if (max <= 0) return 0;
            return Math.Min(100, (current * 100) / max);
        }
    }
}