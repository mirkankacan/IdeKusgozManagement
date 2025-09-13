using IdeKusgozManagement.Application.DTOs.MessageDTOs;
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
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ISignalRService _signalRService;

        public MessagesController(IMessageService messageService, ISignalRService signalRService)
        {
            _messageService = messageService;
            _signalRService = signalRService;
        }

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
            var result = await _messageService.GetMessagesAsync(pageSize, pageNumber, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Mesaj oluşturur
        /// </summary>
        [HttpPost]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            var result = await _messageService.CreateMessageAsync(createMessageDTO, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            await _signalRService.SendMessageToAllAsync(result.Data, cancellationToken);
            await _signalRService.SendNotificationToAllAsync($"Yeni bir mesaj {result.Data.CreatedByName} tarafından {result.Data.CreatedDate.ToString("dd.MM.yyyy HH:mm")} tarihinde atıldı", NotificationType.Message, "/sosyal");
            return Ok(result);
        }

        /// <summary>
        /// Mesajı siler
        /// </summary>
        /// <param name="messageId">Mesaj ID'si</param>

        [HttpDelete("{messageId}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> DeleteMessage(string messageId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(messageId))
            {
                return BadRequest("Mesaj ID'si gereklidir");
            }
            var result = await _messageService.DeleteMessageAsync(messageId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}