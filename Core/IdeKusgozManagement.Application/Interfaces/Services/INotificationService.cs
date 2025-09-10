using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<ApiResponse<PagedResult<NotificationDTO>>> GetNotificationsAsync(string userId, int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<ApiResponse<int>> GetUnreadNotificationCountAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<NotificationDTO>> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> MarkAsReadAsync(string notificationId, string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteNotificationAsync(string notificationId, CancellationToken cancellationToken = default);
    }
}