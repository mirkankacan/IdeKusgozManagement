using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Enums;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly ICurrentUserService _currentUserService;

        public LeaveRequestsController(ILeaveRequestService leaveRequestService, ICurrentUserService currentUserService)
        {
            _leaveRequestService = leaveRequestService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Tüm izin taleplerini getirir
        /// </summary>
        [HttpGet]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetLeaveRequests(CancellationToken cancellationToken = default)
        {
            var result = await _leaveRequestService.GetLeaveRequestsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Oturumda olan kullanıcının kendi izin isteklerini getirir
        /// </summary>
        [HttpGet("my-leave-requests")]
        public async Task<IActionResult> GetMyLeaveRequests(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }
            var result = await _leaveRequestService.GetLeaveRequestsByUserIdAsync(currentUserId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcının izin taleplerini getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetLeaveRequestsByUserId(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }
            var result = await _leaveRequestService.GetLeaveRequestsByUserIdAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Duruma göre izin taleplerini getirir
        /// </summary>
        /// <param name="status">Durum int cinsinden</param>

        [HttpGet("status/{status}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetLeaveRequestsByStatus(LeaveRequestStatus status, CancellationToken cancellationToken = default)
        {
            var result = await _leaveRequestService.GetLeaveRequestsByStatusAsync(status, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Duruma göre izin taleplerini getirir
        /// </summary>
        /// <param name="status">Durum int cinsinden</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpGet("user/{userId}/status/{status}")]
        public async Task<IActionResult> GetLeaveRequestsByUserIdAndStatus(string userId, LeaveRequestStatus status, CancellationToken cancellationToken = default)
        {
            var result = await _leaveRequestService.GetLeaveRequestsByUserIdAndStatusAsync(userId, status, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Duruma göre izin taleplerini getirir
        /// </summary>
        /// <param name="status">Durum int cinsinden</param>

        [HttpGet("my-leave-requests/status/{status}")]
        public async Task<IActionResult> GetMyLeaveRequestsByStatus(LeaveRequestStatus status, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }
            var result = await _leaveRequestService.GetLeaveRequestsByUserIdAndStatusAsync(currentUserId, status, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre izin talebi getirir
        /// </summary>
        /// <param name="leaveRequestId">İzin talebi ID'si</param>
        [HttpGet("{leaveRequestId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetLeaveRequestById(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin talebi ID'si gereklidir");
            }

            var result = await _leaveRequestService.GetLeaveRequestByIdAsync(leaveRequestId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni izin talebi oluşturur
        /// </summary>
        /// <param name="createLeaveRequestDTO">İzin talebi bilgileri</param>
        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _leaveRequestService.CreateLeaveRequestAsync(createLeaveRequestDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İzin talebini siler
        /// </summary>
        /// <param name="leaveRequestId">İzin talebi ID'si</param>
        [HttpDelete("{leaveRequestId}")]
        public async Task<IActionResult> DeleteLeaveRequest(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin talebi ID'si gereklidir");
            }

            var result = await _leaveRequestService.DeleteLeaveRequestAsync(leaveRequestId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İzin talebini onaylar
        /// </summary>
        /// <param name="leaveRequestId">İzin talebi ID'si</param>
        [HttpPut("{leaveRequestId}/approve")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> ApproveLeaveRequest(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin talebi ID'si gereklidir");
            }

            var result = await _leaveRequestService.ApproveLeaveRequestAsync(leaveRequestId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İzin talebini reddeder
        /// </summary>
        /// <param name="leaveRequestId">İzin talebi ID'si</param>
        [HttpPut("{leaveRequestId}/reject")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> RejectLeaveRequest(string leaveRequestId, [FromQuery] string rejectReason, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
            {
                return BadRequest("İzin talebi ID'si gereklidir");
            }

            var result = await _leaveRequestService.RejectLeaveRequestAsync(leaveRequestId, rejectReason, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}