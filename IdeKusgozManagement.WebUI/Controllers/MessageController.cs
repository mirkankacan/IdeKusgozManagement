using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models.MessageModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("mesaj")]
    public class MessageController : Controller
    {
        private readonly IMessageApiService _messageApiService;

        public MessageController(IMessageApiService messageApiService)
        {
            _messageApiService = messageApiService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var response = await _messageApiService.GetMessagesAsync(pageSize, pageNumber, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageViewModel model, CancellationToken cancellationToken)
        {
            var response = await _messageApiService.CreateMessageAsync(model, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Yönetici")]
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(string messageId, CancellationToken cancellationToken)
        {
            var response = await _messageApiService.DeleteMessageAsync(messageId, cancellationToken);
            return response.ToActionResult();
        }
    }
}