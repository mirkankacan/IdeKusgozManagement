using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<ServiceResponse<PagedResult<NotificationDTO>>> GetNotificationsAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<ServiceResponse<int>> GetUnreadNotificationCountAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default);

        Task SendNotificationToAllAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task SendNotificationToUsersAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task SendNotificationToRolesAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task SendNotificationToSubordinatesAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);

        Task SendNotificationToSuperiorsAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default);
    }
}