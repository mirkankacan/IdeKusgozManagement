using IdeKusgozManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Hubs
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
                var roleName = _currentUserService.GetCurrentUserRole();

                _logger.LogInformation("Kullanıcı bağlandı. UserId: {UserId}, UserName: {UserName}, RoleName: {RoleName}, ConnectionId: {ConnectionId}",
                    userId, userName, roleName, Context.ConnectionId);

                // Genel mesajlar grubuna katıl
                await Groups.AddToGroupAsync(Context.ConnectionId, "Messages");
                _logger.LogInformation("Kullanıcı Messages grubuna eklendi. ConnectionId: {ConnectionId}", Context.ConnectionId);

                // Not: Genel bildirimler grubuna otomatik katılmıyoruz
                // Bu grup sadece özel durumlarda manuel olarak kullanılmalı

                // Role bazlı grup (örneğin: Admin, User)
                var roleGroupName = $"Role_{roleName}";
                await Groups.AddToGroupAsync(Context.ConnectionId, roleGroupName);
                _logger.LogInformation("Kullanıcı role grubuna eklendi. GroupName: {GroupName}, ConnectionId: {ConnectionId}", roleGroupName, Context.ConnectionId);

                // Kullanıcıya özel grup (kişisel bildirimler için)
                var userGroupName = $"User_{userId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, userGroupName);
                _logger.LogInformation("Kullanıcı user grubuna eklendi. GroupName: {GroupName}, ConnectionId: {ConnectionId}", userGroupName, Context.ConnectionId);

                // Bağlantı başarılı bilgisi gönder
                await Clients.Caller.SendAsync("ConnectionStatus", new
                {
                    IsConnected = true,
                    Message = "Bağlantı başarılı",
                    UserId = userId,
                    RoleName = roleName,
                    UserName = userName
                });

                _logger.LogInformation("ConnectionStatus gönderildi. UserId: {UserId}, RoleName: {RoleName}", userId, roleName);

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
                if (exception != null)
                {
                    _logger.LogError(exception, "SignalR Hub OnDisconnectedAsync işleminde Exception hata");
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SignalR Hub JoinGroup işleminde hata. GroupName:{groupName}");
            }
        }

        public async Task LeaveGroup(string groupName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SignalR Hub LeaveGroup işleminde hata. GroupName:{groupName}");
            }
        }
    }
}