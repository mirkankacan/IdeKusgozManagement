using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using IdeKusgozManagement.WebAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessagesController(IMessageService messageService, IHubContext<MessageHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
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

            if (result.IsSuccess && result.Data != null)
            {
                // Send real-time notification to all connected clients
                await _hubContext.Clients.Group("Messages").SendAsync("NewMessage", result.Data);
            }

            return result.IsSuccess ? Ok(result) : BadRequest(result);
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