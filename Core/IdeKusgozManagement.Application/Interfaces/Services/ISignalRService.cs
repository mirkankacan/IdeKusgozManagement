using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface ISignalRService
    {
        Task SendNotificationToAllAsync(string message, NotificationType type, string? redirectUrl = null, CancellationToken cancellationToken = default);

        Task SendNotificationToUserAsync(string targetUserId, string message, NotificationType type, string? redirectUrl = null, CancellationToken cancellationToken = default);

        Task SendNotificationToRolesAsync(string[] targetRoleNames, string message, NotificationType type, string? redirectUrl = null, CancellationToken cancellationToken = default);

        Task SendMessageToAllAsync(MessageDTO messageDTO, CancellationToken cancellationToken = default);
    }
}