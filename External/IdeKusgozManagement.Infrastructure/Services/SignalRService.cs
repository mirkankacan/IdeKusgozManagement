using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Enums;
using IdeKusgozManagement.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<CommunicationHub> _hubContext;
        private readonly ILogger<SignalRService> _logger;
        private readonly INotificationService _notificationService;

        public SignalRService(IHubContext<CommunicationHub> hubContext, ILogger<SignalRService> logger, INotificationService notificationService)
        {
            _hubContext = hubContext;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task SendMessageToAllAsync(MessageDTO messageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubContext.Clients.Group("Messages").SendAsync("NewMessage", messageDTO);
                _logger.LogInformation("Mesaj herkese gönderildi. MessageDTO: {MessageDTO}", messageDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj herkese gönderilemedi. MessageDTO: {MessageDTO}", messageDTO);
            }
        }

        public async Task SendNotificationToAllAsync(string message, NotificationType type, string? redirectUrl = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var createNotificationDTO = new CreateNotificationDTO
                {
                    Message = message,
                    Type = type,
                    RedirectUrl = redirectUrl
                };

                var response = await _notificationService.CreateNotificationAsync(createNotificationDTO, cancellationToken);

                if (response.IsSuccess)
                {
                    await _hubContext.Clients.All.SendAsync("NewNotification", response.Data, cancellationToken);
                    _logger.LogInformation("Bildirim herkese gönderildi. Message: {Message}", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bildirim herkese gönderilemedi. Message: {Message}", message);
            }
        }

        public async Task SendNotificationToRolesAsync(string[] targetRoleNames, string message, NotificationType type, string? redirectUrl = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var createNotificationDTO = new CreateNotificationDTO
                {
                    Message = message,
                    Type = type,
                    TargetRole = string.Join(";", targetRoleNames),
                    RedirectUrl = redirectUrl
                };

                var response = await _notificationService.CreateNotificationAsync(createNotificationDTO, cancellationToken);

                if (response.IsSuccess)
                {
                    foreach (var item in targetRoleNames)
                    {
                        var groupName = $"Role_{item}";
                        await _hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }
                }

                _logger.LogInformation("Bildirim {RoleCount} role gönderildi. Roles: {RoleNames}, Message: {Message}",
                    targetRoleNames.Length, string.Join(";", targetRoleNames), message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bildirim rollerе gönderilemedi. Roles: {RoleNames}, Message: {Message}",
                    string.Join(";", targetRoleNames), message);
            }
        }

        public async Task SendNotificationToUserAsync(string targetUserId, string message, NotificationType type, string? redirectUrl = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var createNotificationDTO = new CreateNotificationDTO
                {
                    Message = message,
                    Type = type,
                    TargetUserId = targetUserId,
                    RedirectUrl = redirectUrl
                };

                var response = await _notificationService.CreateNotificationAsync(createNotificationDTO, cancellationToken);

                if (response.IsSuccess)
                {
                    var groupName = $"User_{targetUserId}";
                    await _hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    _logger.LogInformation("Bildirim role gönderildi. TargetUserId: {TargetUserId}, Message: {Message}", targetUserId, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bildirim role gönderilemedi. TargetUserId: {TargetUserId}, Message: {Message}", targetUserId, message);
            }
        }
    }
}