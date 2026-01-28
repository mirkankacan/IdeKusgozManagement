using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IMessageService
    {
        Task<ServiceResult<PagedResult<MessageDTO>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default);

        Task<ServiceResult<MessageDTO>> SendMessageToAllAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<MessageDTO>> SendMessageToUsersAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<MessageDTO>> SendMessageToRolesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<MessageDTO>> SendMessageToSubordinatesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<MessageDTO>> SendMessageToSuperiorsAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default);
    }
}