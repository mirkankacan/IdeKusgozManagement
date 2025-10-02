using IdeKusgozManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(INotificationService notificationService, IIdentityService identityService) : ControllerBase
    {
        /// <summary>
        /// Kullanıcının bildirimlerini getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var userId = identityService.GetUserId();
            var userRole = identityService.GetUserRole();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await notificationService.GetNotificationsAsync(userId, userRole, pageSize, pageNumber, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcının okunmamış bildirim sayısını getirir
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCount(CancellationToken cancellationToken = default)
        {
            var userId = identityService.GetUserId();
            var userRole = identityService.GetUserRole();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await notificationService.GetUnreadNotificationCountAsync(userId, userRole, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Bildirimi okundu olarak işaretler
        /// </summary>
        [HttpPut("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(string notificationId, CancellationToken cancellationToken = default)
        {
            var userId = identityService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await notificationService.MarkAsReadAsync(notificationId, userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tüm bildirimleri okundu olarak işaretler
        /// </summary>
        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
        {
            var userId = identityService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı");
            }

            var result = await notificationService.MarkAllAsReadAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}