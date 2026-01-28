using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(INotificationService notificationService) : ControllerBase
    {
        /// <summary>
        /// Kullanıcının bildirimlerini getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var result = await notificationService.GetNotificationsAsync(pageSize, pageNumber, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Kullanıcının okunmamış bildirim sayısını getirir
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCount(CancellationToken cancellationToken)
        {
            var result = await notificationService.GetUnreadNotificationCountAsync(cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Bildirimi okundu olarak işaretler
        /// </summary>
        [HttpPut("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(string notificationId, CancellationToken cancellationToken)
        {
            var result = await notificationService.MarkAsReadAsync(notificationId, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Tüm bildirimleri okundu olarak işaretler
        /// </summary>
        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            var result = await notificationService.MarkAllAsReadAsync(cancellationToken);
            return result.ToActionResult();
        }
    }
}