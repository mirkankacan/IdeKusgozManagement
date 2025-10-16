using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.LeaveRequestModels;
using IdeKusgozManagement.WebUI.Models.ProjectModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILeaveRequestApiService _leaveRequestApiService;
        private readonly IProjectApiService _projectApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILeaveRequestApiService leaveRequestApiService, IProjectApiService projectApiService, IHttpContextAccessor httpContextAccessor)
        {
            _leaveRequestApiService = leaveRequestApiService;
            _projectApiService = projectApiService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpGet("ana-sayfa")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("sosyal")]
        public IActionResult Social()
        {
            var jwtToken = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            ViewData["JwtToken"] = jwtToken;
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
            var roleName = _httpContextAccessor.HttpContext.Session.GetString("RoleName");
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Rol bilgisi bulunamadı.");
            }

            ApiResponse<IEnumerable<LeaveRequestViewModel>> response;

            switch (roleName)
            {
                // 1 onaylanmış izinleri getir
                case "Admin":
                case "Yönetici":
                case "Şef":
                    response = await _leaveRequestApiService.GetLeaveRequestsByStatusAsync(1, null, cancellationToken);
                    break;

                case "Personel":
                    var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
                    if (string.IsNullOrEmpty(userId))
                        return Unauthorized("Kullanıcı bilgisi bulunamadı");

                    response = await _leaveRequestApiService.GetLeaveRequestsByStatusAsync(1, userId, cancellationToken);
                    break;

                default:
                    return Forbid("Yetkisiz erişim");
            }
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("takvim/projeler")]
        public async Task<IActionResult> CalendarProjects(CancellationToken cancellationToken = default)
        {
            var roleName = _httpContextAccessor.HttpContext.Session.GetString("RoleName");
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Rol bilgisi bulunamadı.");
            }

            ApiResponse<IEnumerable<ProjectViewModel>> response;

            switch (roleName)
            {
                case "Admin":
                case "Yönetici":
                case "Şef":
                    response = await _projectApiService.GetActiveProjectsAsync(cancellationToken);
                    break;

                case "Personel":
                    var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
                    if (string.IsNullOrEmpty(userId))
                        return Unauthorized("Kullanıcı bilgisi bulunamadı");
                    response = await _projectApiService.GetActiveProjectsAsync(cancellationToken);
                    break;

                default:
                    return Forbid("Yetkisiz erişim");
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}