using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<ApiResponse<PagedResult<NotificationDTO>>> GetNotificationsAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<ApiResponse<int>> GetUnreadNotificationCountAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default);

        Task SendNotificationToAllAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task SendNotificationToUsersAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task SendNotificationToRolesAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task SendNotificationToSubordinatesAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task SendNotificationToSuperiorsAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);
    }
}