using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.NotificationModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class NotificationApiService : INotificationApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<NotificationApiService> _logger;
        private const string BaseEndpoint = "api/notifications";

        public NotificationApiService(
            IApiService apiService,
            ILogger<NotificationApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResult<NotificationViewModel>>> GetNotificationsAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<PagedResult<NotificationViewModel>>($"{BaseEndpoint}?pageSize={pageSize}&pageNumber={pageNumber}", cancellationToken);
        }

        public async Task<ApiResponse<int>> GetUnreadNotificationCountAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<int>($"{BaseEndpoint}/unread-count", cancellationToken);
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{notificationId}/mark-as-read", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/mark-all-as-read", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteNotificationAsync(string notificationId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{notificationId}", cancellationToken);
        }
    }
}
