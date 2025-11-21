using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Enums;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController(IMessageService messageService, INotificationService notificationService) : ControllerBase
    {
        /// <summary>
        /// Tüm mesajları getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            switch (pageSize)
            {
                case >= 50:
                    pageSize = 50;
                    break;

                case <= 0:
                    pageSize = 10;
                    break;
            }
            switch (pageNumber)
            {
                case <= 0:
                    pageNumber = 1;
                    break;
            }
            var result = await messageService.GetMessagesAsync(pageSize, pageNumber, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Mesaj oluşturur SignalR ile anlık olarak gönderir
        /// </summary>
        [HttpPost]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDTO createMessageDTO, CancellationToken cancellationToken)
        {
            ServiceResponse<MessageDTO> messageResponse = new();
            if (createMessageDTO.TargetRoles.Any())
                messageResponse = await messageService.SendMessageToRolesAsync(createMessageDTO, cancellationToken);

            if (createMessageDTO.TargetUsers.Any())
                messageResponse = await messageService.SendMessageToUsersAsync(createMessageDTO, cancellationToken);

            if (!createMessageDTO.TargetUsers.Any() && !createMessageDTO.TargetRoles.Any())
                messageResponse = await messageService.SendMessageToAllAsync(createMessageDTO, cancellationToken);

            var createNotification = new CreateNotificationDTO
            {
                Message = $"{messageResponse.Data.CreatedByFullName} tarafından {messageResponse.Data.CreatedDate:dd.MM.yyyy HH:mm} tarihinde yeni bir mesaj atıldı",
                Type = NotificationType.Message,
                RedirectUrl = "/sosyal",
                TargetRoles = createMessageDTO.TargetRoles,
                TargetUsers = createMessageDTO.TargetUsers
            };

            if (createMessageDTO.TargetRoles.Any())
                await notificationService.SendNotificationToRolesAsync(createNotification, cancellationToken);

            if (createMessageDTO.TargetUsers.Any())
                await notificationService.SendNotificationToUsersAsync(createNotification, cancellationToken);

            if (!createMessageDTO.TargetRoles.Any() && !createMessageDTO.TargetUsers.Any())
                await notificationService.SendNotificationToAllAsync(createNotification, cancellationToken);

            return Ok(messageResponse);
        }

        /// <summary>
        /// Mesajı siler
        /// </summary>
        /// <param name="messageId">Mesaj ID'si</param>

        [HttpDelete("{messageId}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> DeleteMessage(string messageId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(messageId))
            {
                return BadRequest("Mesaj ID'si gereklidir");
            }
            var result = await messageService.DeleteMessageAsync(messageId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}