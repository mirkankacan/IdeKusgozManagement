using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using IdeKusgozManagement.WebAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<CommunicationHub> _hubContext;

        public NotificationsController(INotificationService notificationService, IHubContext<CommunicationHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Kullanıcının bildirimlerini getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await _notificationService.GetNotificationsAsync(userId, pageSize, pageNumber, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcının okunmamış bildirim sayısını getirir
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCount(CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await _notificationService.GetUnreadNotificationCountAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni bildirim oluşturur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            var result = await _notificationService.CreateNotificationAsync(createNotificationDTO, cancellationToken);

            if (result.IsSuccess && result.Data != null)
            {
                // Send real-time notification to all users in Notifications group
                await _hubContext.Clients.Group("Notifications").SendAsync("NewNotification", result.Data);
            }

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Bildirimi okundu olarak işaretler
        /// </summary>
        [HttpPost("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(string notificationId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await _notificationService.MarkAsReadAsync(notificationId, userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tüm bildirimleri okundu olarak işaretler
        /// </summary>
        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Bildirimi siler
        /// </summary>
        [HttpDelete("{notificationId}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> DeleteNotification(string notificationId, CancellationToken cancellationToken = default)
        {
            var result = await _notificationService.DeleteNotificationAsync(notificationId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}