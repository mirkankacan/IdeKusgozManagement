using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Authorize]
    [Route("bildirim")]
    public class NotificationController : Controller
    {
        private readonly INotificationApiService _notificationApiService;

        public NotificationController(INotificationApiService notificationApiService)
        {
            _notificationApiService = notificationApiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var response = await _notificationApiService.GetNotificationsAsync(pageSize, pageNumber, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("okunmayan-sayisi")]
        public async Task<IActionResult> GetUnreadNotificationCount(CancellationToken cancellationToken)
        {
            var response = await _notificationApiService.GetUnreadNotificationCountAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{notificationId}/okundu")]
        public async Task<IActionResult> MarkAsRead(string notificationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(notificationId))
            {
                return BadRequest("Bildirim ID'si gereklidir");
            }
            var response = await _notificationApiService.MarkAsReadAsync(notificationId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("hepsini-okundu")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            var response = await _notificationApiService.MarkAllAsReadAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(string notificationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(notificationId))
            {
                return BadRequest("Bildirim ID'si gereklidir");
            }
            var response = await _notificationApiService.DeleteNotificationAsync(notificationId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}