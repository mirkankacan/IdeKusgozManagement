using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IMessageService
    {
        Task<ApiResponse<IEnumerable<MessageDTO>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> CreateMessageAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default);
    }
}