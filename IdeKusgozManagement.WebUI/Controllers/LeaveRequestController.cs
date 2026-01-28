using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models.LeaveRequestModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("izin")]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestApiService _leaveRequestApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LeaveRequestController(ILeaveRequestApiService leaveRequestApiService, IHttpContextAccessor httpContextAccessor)
        {
            _leaveRequestApiService = leaveRequestApiService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetLeaveRequests(CancellationToken cancellationToken)
        {
            var response = await _leaveRequestApiService.GetLeaveRequestsAsync(cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("ast-liste")]
        public async Task<IActionResult> GetSubordinateLeaveRequests(CancellationToken cancellationToken)
        {
            var response = await _leaveRequestApiService.GetSubordinateLeaveRequestsAsync(cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequestById(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.GetLeaveRequestByIdAsync(id, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("istek-olustur")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("istek-olustur")]
        public async Task<IActionResult> CreateLeaveRequest([FromForm] CreateLeaveRequestViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _leaveRequestApiService.CreateLeaveRequestAsync(model, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPut("guncelle/{leaveRequestId}")]
        public async Task<IActionResult> UpdateLeaveRequest(string leaveRequestId, [FromForm] UpdateLeaveRequestViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(leaveRequestId))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _leaveRequestApiService.UpdateLeaveRequestAsync(leaveRequestId, model, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpDelete("{leaveRequestId}")]
        public async Task<IActionResult> DeleteLeaveRequest(string leaveRequestId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.DeleteLeaveRequestAsync(leaveRequestId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("durum/{status}")]
        public async Task<IActionResult> GetLeaveRequestsByStatus(int status, [FromQuery] string? userId, CancellationToken cancellationToken)
        {
            var response = await _leaveRequestApiService.GetLeaveRequestsByStatusAsync(status, userId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("listem/durum/{status}")]
        public async Task<IActionResult> GetMyLeaveRequestsByStatus(int status, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Lütfen tekrar giriş yapınız");
            }
            var response = await _leaveRequestApiService.GetLeaveRequestsByStatusAsync(status, userId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("listem")]
        public async Task<IActionResult> GetMyLeaveRequests(CancellationToken cancellationToken)
        {
            var response = await _leaveRequestApiService.GetMyLeaveRequestsAsync(cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("kullanici/{userId}")]
        public async Task<IActionResult> GetLeaveRequestsByUserId(string userId, CancellationToken cancellationToken)
        {
            var response = await _leaveRequestApiService.GetLeaveRequestsByUserIdAsync(userId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{leaveRequestId}/onayla")]
        public async Task<IActionResult> ApproveLeaveRequest(string leaveRequestId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.ApproveLeaveRequestAsync(leaveRequestId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{leaveRequestId}/reddet")]
        public async Task<IActionResult> RejectLeaveRequest(string leaveRequestId, [FromQuery] string? rejectReason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.RejectLeaveRequestAsync(leaveRequestId, rejectReason,
            cancellationToken);
            return response.ToActionResult();
        }
    }
}