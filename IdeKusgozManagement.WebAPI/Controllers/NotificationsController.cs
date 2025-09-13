using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUserService;

        public NotificationsController(INotificationService notificationService, ICurrentUserService currentUserService)
        {
            _notificationService = notificationService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Kullanıcının bildirimlerini getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetCurrentUserId();
            var userRole = _currentUserService.GetCurrentUserRole();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await _notificationService.GetNotificationsAsync(userId, userRole, pageSize, pageNumber, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcının okunmamış bildirim sayısını getirir
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCount(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetCurrentUserId();
            var userRole = _currentUserService.GetCurrentUserRole();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await _notificationService.GetUnreadNotificationCountAsync(userId,userRole, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Bildirimi okundu olarak işaretler
        /// </summary>
        [HttpPost("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(string notificationId, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetCurrentUserId();
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
            var userId = _currentUserService.GetCurrentUserId();
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