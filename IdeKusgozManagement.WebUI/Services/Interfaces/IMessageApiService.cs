using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.MessageModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IMessageApiService
    {
        Task<ApiResponse<IEnumerable<MessageViewModel>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> CreateMessageAsync(CreateMessageViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default);
    }
}