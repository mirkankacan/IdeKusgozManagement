using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.NotificationModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface INotificationApiService
    {
        Task<ApiResponse<PagedResult<NotificationViewModel>>> GetNotificationsAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);
        Task<ApiResponse<int>> GetUnreadNotificationCountAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<NotificationViewModel>> CreateNotificationAsync(CreateNotificationViewModel model, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteNotificationAsync(string notificationId, CancellationToken cancellationToken = default);
    }
}
