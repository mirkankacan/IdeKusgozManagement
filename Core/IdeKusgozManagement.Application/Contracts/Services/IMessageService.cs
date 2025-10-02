using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IMessageService
    {
        Task<ApiResponse<PagedResult<MessageDTO>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default);

        Task<ApiResponse<MessageDTO>> SendMessageToAllAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<MessageDTO>> SendMessageToUsersAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<MessageDTO>> SendMessageToRolesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<MessageDTO>> SendMessageToSubordinatesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<MessageDTO>> SendMessageToSuperiorsAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);
    }
}