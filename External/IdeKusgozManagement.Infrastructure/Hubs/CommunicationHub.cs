using IdeKusgozManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Hubs
{
    [Authorize]
    public class CommunicationHub(ILogger<CommunicationHub> logger, IIdentityService identityService) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = identityService.GetUserId();
                var roleName = identityService.GetUserRole();

                // Genel mesajlar grubuna katıl
                await Groups.AddToGroupAsync(Context.ConnectionId, "Messages");

                // Rol bazlı grup (örneğin, Admin, Şef, Personel)
                var roleGroupName = $"Role_{roleName}";
                await Groups.AddToGroupAsync(Context.ConnectionId, roleGroupName);

                // Kullanıcıya özel grup (kişisel bildirimler için)
                var userGroupName = $"User_{userId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, userGroupName);

                // Bağlantı başarılı bilgisi gönder
                await Clients.Caller.SendAsync("ConnectionStatus", new
                {
                    IsConnected = true,
                    Message = "Bağlantı başarılı",
                    UserId = userId,
                    RoleName = roleName,
                });

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SignalR Hub OnConnectedAsync işleminde hata");
            }
        }

        // Kullanıcı bağlantıyı kestiğinde
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                if (exception != null)
                {
                    logger.LogError(exception, "SignalR Hub OnDisconnectedAsync işleminde Exception hata");
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SignalR Hub OnDisconnectedAsync işleminde hata");
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
                logger.LogError(ex, $"SignalR Hub JoinGroup işleminde hata. GroupName:{groupName}");
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
                logger.LogError(ex, $"SignalR Hub LeaveGroup işleminde hata. GroupName:{groupName}");
            }
        }
    }
}