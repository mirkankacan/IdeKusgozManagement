using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IMessageService
    {
        Task<ServiceResponse<PagedResult<MessageDTO>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<MessageDTO>> SendMessageToAllAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<MessageDTO>> SendMessageToUsersAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<MessageDTO>> SendMessageToRolesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<MessageDTO>> SendMessageToSubordinatesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<MessageDTO>> SendMessageToSuperiorsAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);
    }
}