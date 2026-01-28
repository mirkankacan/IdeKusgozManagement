using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.MessageModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class MessageApiService : IMessageApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<MessageApiService> _logger;
        private const string BaseEndpoint = "api/messages";

        public MessageApiService(
            IApiService apiService,
            ILogger<MessageApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<MessageViewModel>> CreateMessageAsync(CreateMessageViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<MessageViewModel>(BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{messageId}", cancellationToken);
        }

        public async Task<ApiResponse<PagedResult<MessageViewModel>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<PagedResult<MessageViewModel>>($"{BaseEndpoint}?pageSize={pageSize}&pageNumber={pageNumber}", cancellationToken);
        }
    }
}
