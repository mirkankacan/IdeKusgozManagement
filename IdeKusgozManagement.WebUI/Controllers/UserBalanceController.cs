using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models.UserBalanceModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("kullanici-bakiye")]
    public class UserBalanceController : Controller
    {
        private readonly IUserBalanceApiService _userBalanceApiService;

        public UserBalanceController(IUserBalanceApiService userBalanceApiService)
        {
            _userBalanceApiService = userBalanceApiService;
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserBalanceByUser(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş olamaz");
            }
            var response = await _userBalanceApiService.GetUserBalancesByUserAsync(userId, cancellationToken);

            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("bakiyem")]
        public async Task<IActionResult> GetMyBalance(CancellationToken cancellationToken)
        {
            var response = await _userBalanceApiService.GetMyBalancesAsync(cancellationToken);

            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("{userId}/arttir")]
        public async Task<IActionResult> IncreaseUserBalance(string userId, UpdateUserBalanceViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş olamaz");
            }
            var response = await _userBalanceApiService.IncreaseUserBalanceAsync(userId, model, cancellationToken);

            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("{userId}/azalt")]
        public async Task<IActionResult> DecreaseUserBalance(string userId, UpdateUserBalanceViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş olamaz");
            }
            var response = await _userBalanceApiService.DecreaseUserBalanceAsync(userId, model, cancellationToken);

            return response.ToActionResult();
        }
    }
}