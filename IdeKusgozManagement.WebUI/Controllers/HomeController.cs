using System.Security.Claims;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.LeaveRequestModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILeaveRequestApiService _leaveRequestApiService;

        public HomeController(ILeaveRequestApiService leaveRequestApiService)
        {
            _leaveRequestApiService = leaveRequestApiService;
        }

        [Authorize]
        [HttpGet("ana-sayfa")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("takvim")]
        public IActionResult Calendar()
        {
            return View();
        }

        [Authorize]
        [HttpGet("takvim/izinler")]
        public async Task<IActionResult> CalendarLeaves(CancellationToken cancellationToken = default)
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (roleClaim == null)
            {
                return BadRequest("Rol bilgisi bulunamadı.");
            }

            ApiResponse<IEnumerable<LeaveRequestViewModel>> response;

            switch (roleClaim.Value)
            {
                // 1 onaylanmış izinleri getir
                case "Admin":
                case "Yönetici":
                case "Şef":
                    response = await _leaveRequestApiService.GetLeaveRequestsByStatusAsync(1, cancellationToken);
                    break;

                case "Personel":
                    response = await _leaveRequestApiService.GetMyLeaveRequestsByStatusAsync(1, cancellationToken);
                    break;

                default:
                    return Forbid("Yetkisiz erişim.");
            }
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}