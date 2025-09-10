using System.Security.Claims;
using IdeKusgozManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdeKusgozManagement.WebAPI.Hubs
{
    [Authorize]
    public class CommunicationHub : Hub
    {
        private readonly ILogger<CommunicationHub> _logger;
        private readonly ICurrentUserService _currentUserService;
        public CommunicationHub(ILogger<CommunicationHub> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var userName = _currentUserService.GetCurrentUserName();

                _logger.LogInformation($"Kullanıcı bağlandı: {userName} ({userId}) - Connection: {Context.ConnectionId}");

                // Genel mesajlar grubuna katıl
                await Groups.AddToGroupAsync(Context.ConnectionId, "Messages");

                // Genel bildirimler grubuna katıl
                await Groups.AddToGroupAsync(Context.ConnectionId, "Notifications");

                // Kullanıcıya özel grup (kişisel bildirimler için)
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                }

                // Bağlantı başarılı bilgisi gönder
                await Clients.Caller.SendAsync("ConnectionStatus", new
                {
                    IsConnected = true,
                    Message = "Bağlantı başarılı",
                    UserId = userId,
                    UserName = userName
                });

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR Hub OnConnectedAsync işleminde hata");
            }
        }

        // Kullanıcı bağlantıyı kestiğinde
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var userName = _currentUserService.GetCurrentUserName();

                _logger.LogInformation($"User disconnected: {userName} ({userId}) - Connection: {Context.ConnectionId}");

                if (exception != null)
                {
                    _logger.LogError(exception, "Disconnection error");
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR Hub OnDisconnectedAsync işleminde hata");
            }
        }

        // Grup yönetimi
        public async Task JoinGroup(string groupName)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                _logger.LogInformation($"User {Context.User?.Identity?.Name} joined group {groupName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SignalR Hub JoinGroup işleminde hata");
            }
        }

        public async Task LeaveGroup(string groupName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                _logger.LogInformation($"User {Context.User?.Identity?.Name} left group {groupName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SignalR Hub LeaveGroup işleminde hata");
            }
        }
    }
}