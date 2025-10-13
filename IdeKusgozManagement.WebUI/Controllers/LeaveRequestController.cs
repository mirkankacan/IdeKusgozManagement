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

        public LeaveRequestController(ILeaveRequestApiService leaveRequestApiService)
        {
            _leaveRequestApiService = leaveRequestApiService;
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetLeaveRequests(CancellationToken cancellationToken = default)
        {
            var response = await _leaveRequestApiService.GetLeaveRequestsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("kullanici/{userId}/durum/{status}")]
        public async Task<IActionResult> GetLeaveRequestsByUserIdAndStatus(string userId, int status, CancellationToken cancellationToken = default)
        {
            var response = await _leaveRequestApiService.GetLeaveRequestsByUserIdAndStatusAsync(userId, status, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("durum/{status}")]
        public async Task<IActionResult> GetLeaveRequestsByStatus(int status, CancellationToken cancellationToken = default)
        {
            var response = await _leaveRequestApiService.GetLeaveRequestsByStatusAsync(status, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("listem")]
        public async Task<IActionResult> GetMyLeaveRequests(CancellationToken cancellationToken = default)
        {
            var response = await _leaveRequestApiService.GetMyLeaveRequestsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("listem/durum/{status}")]
        public async Task<IActionResult> GetMyLeaveRequestsByStatus(int status, CancellationToken cancellationToken = default)
        {
            var response = await _leaveRequestApiService.GetMyLeaveRequestsByStatusAsync(status, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("kullanici/{userId}")]
        public async Task<IActionResult> GetLeaveRequestsByUserId(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _leaveRequestApiService.GetLeaveRequestsByUserIdAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequestById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.GetLeaveRequestByIdAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
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
        public async Task<IActionResult> CreateLeaveRequest([FromForm] CreateLeaveRequestViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _leaveRequestApiService.CreateLeaveRequestAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpDelete("{leaveRequestId}")]
        public async Task<IActionResult> DeleteLeaveRequest(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.DeleteLeaveRequestAsync(leaveRequestId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{leaveRequestId}/onayla")]
        public async Task<IActionResult> ApproveLeaveRequest(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.ApproveLeaveRequestAsync(leaveRequestId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{leaveRequestId}/reddet")]
        public async Task<IActionResult> RejectLeaveRequest(string leaveRequestId, [FromQuery] string? rejectReason, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.RejectLeaveRequestAsync(leaveRequestId, rejectReason,
            cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}