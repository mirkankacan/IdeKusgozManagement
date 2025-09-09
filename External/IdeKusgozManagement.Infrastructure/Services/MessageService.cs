using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Application.Interfaces.Repositories;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IUnitOfWork unitOfWork, ILogger<MessageService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> CreateMessageAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = createMessageDTO.Adapt<IdtMessage>();

                await _unitOfWork.Repository<IdtMessage>().AddAsync(message, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Mesaj başarıyla gönderildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateMessageAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Mesaj gönderilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = await _unitOfWork.Repository<IdtMessage>()
                    .GetByIdNoTrackingAsync(messageId, cancellationToken);

                if (message == null)
                {
                    return ApiResponse<bool>.Error("Mesaj bulunamadı");
                }

                await _unitOfWork.Repository<IdtMessage>().DeleteAsync(message, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Mesaj başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteMessageAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Mesaj silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<MessageDTO>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var messages = await _unitOfWork.Repository<IdtMessage>()
                    .GetAllNoTrackingAsync(cancellationToken, m => m.CreatedByUser);

                var orderedMessages = messages
                    .OrderByDescending(m => m.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                var messageDTO = orderedMessages.Adapt<IEnumerable<MessageDTO>>();

                return ApiResponse<IEnumerable<MessageDTO>>.Success(messageDTO, "Mesajlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMessagesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<MessageDTO>>.Error("Mesajlar getirilirken hata oluştu");
            }
        }
    }
}