using IdeKusgozManagement.WebUI.Models.NotificationModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
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
        public async Task<IActionResult> GetUnreadNotificationCount(CancellationToken cancellationToken = default)
        {
            var response = await _notificationApiService.GetUnreadNotificationCountAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _notificationApiService.CreateNotificationAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{notificationId}/okundu")]
        public async Task<IActionResult> MarkAsRead(string notificationId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(notificationId))
            {
                return BadRequest("Bildirim ID'si gereklidir");
            }
            var response = await _notificationApiService.MarkAsReadAsync(notificationId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("hepsini-okundu")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
        {
            var response = await _notificationApiService.MarkAllAsReadAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(string notificationId, CancellationToken cancellationToken = default)
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