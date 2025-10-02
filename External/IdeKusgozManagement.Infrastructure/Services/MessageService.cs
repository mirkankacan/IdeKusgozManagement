using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure.Hubs;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class MessageService(IUnitOfWork unitOfWork, ILogger<MessageService> logger, IHubContext<CommunicationHub> hubContext, IIdentityService identityService) : IMessageService
    {
        private async Task<ApiResponse<MessageDTO>> CreateMessageAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = createMessageDTO.Adapt<IdtMessage>();

                await unitOfWork.GetRepository<IdtMessage>().AddAsync(message, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var createdMessage = await unitOfWork.GetRepository<IdtMessage>().Where(x => x.Id == message.Id).Include(x => x.CreatedByUser).FirstOrDefaultAsync();

                var messageDTO = createdMessage.Adapt<MessageDTO>();

                return ApiResponse<MessageDTO>.Success(messageDTO, "Mesaj başarıyla gönderildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateMessageAsync işleminde hata oluştu");
                return ApiResponse<MessageDTO>.Error("Mesaj gönderilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = await unitOfWork.GetRepository<IdtMessage>().GetByIdAsync(messageId, cancellationToken);

                if (message == null)
                {
                    return ApiResponse<bool>.Error("Mesaj bulunamadı");
                }

                unitOfWork.GetRepository<IdtMessage>().Remove(message);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Mesaj başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteMessageAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Mesaj silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<PagedResult<MessageDTO>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();
                var messages = await unitOfWork.GetRepository<IdtMessage>()
                    .WhereAsNoTracking(x => x.TargetUsers.Contains(userId) || x.TargetRoles.Contains(userRole) || (x.TargetUsers == null && x.TargetRoles == null))
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.CreatedByUser)
                    .ToListAsync(cancellationToken);

                var messagesCount = await unitOfWork.GetRepository<IdtMessage>()
                    .Where(x => x.TargetUsers.Contains(userId) || x.TargetRoles.Contains(userRole) || (x.TargetUsers == null && x.TargetRoles == null)).CountAsync(cancellationToken);

                PagedResult<MessageDTO> mappedMessages = new()
                {
                    Data = messages.Adapt<IEnumerable<MessageDTO>>(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = messagesCount
                };
                return ApiResponse<PagedResult<MessageDTO>>.Success(mappedMessages, "Mesajlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetMessagesAsync işleminde hata oluştu");
                return ApiResponse<PagedResult<MessageDTO>>.Error("Mesajlar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<MessageDTO>> SendMessageToAllAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await CreateMessageAsync(createMessageDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    await hubContext.Clients.Group("Messages").SendAsync("NewMessage", createMessageDTO);
                    logger.LogInformation("Mesaj herkese gönderildi. MessageDTO: {MessageDTO}", createMessageDTO);
                    return ApiResponse<MessageDTO>.Success(response.Data, "Mesaj herkese gönderildi");
                }
                else
                {
                    logger.LogWarning("Mesaj oluşturulamadı. Content: {Content}", createMessageDTO.Content);
                    return ApiResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj herkese gönderilemedi. Content: {Content}", createMessageDTO.Content);
                return ApiResponse<MessageDTO>.Error("Mesaj herkese gönderilemedi");
            }
        }

        public async Task<ApiResponse<MessageDTO>> SendMessageToRolesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                if (createMessageDTO.TargetRoles == null || !createMessageDTO.TargetRoles.Any())
                {
                    logger.LogWarning("TargetRoles boş, mesaj gönderilemedi");
                    return ApiResponse<MessageDTO>.Error("Roller bulunamadı, mesaj gönderilemedi");
                }

                var response = await CreateMessageAsync(createMessageDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var role in createMessageDTO.TargetRoles)
                    {
                        var groupName = $"Role_{role}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewMessage", response.Data, cancellationToken);
                    }

                    logger.LogInformation("Mesaj rollere gönderildi. Roles: {RoleNames}, Content: {Content}",
                        string.Join(", ", createMessageDTO.TargetRoles), createMessageDTO.Content);
                    return ApiResponse<MessageDTO>.Success(response.Data, "Mesaj rollere gönderildi");
                }
                else
                {
                    logger.LogWarning("Mesaj oluşturulamadı. Roles: {RoleNames}",
                        string.Join(", ", createMessageDTO.TargetRoles));
                    return ApiResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj rollere gönderilemedi. Roles: {RoleNames}, Content: {Content}",
                    createMessageDTO.TargetRoles != null ? string.Join(", ", createMessageDTO.TargetRoles) : "null",
                    createMessageDTO.Content);
                return ApiResponse<MessageDTO>.Error("Mesaj rollere gönderilemedi");
            }
        }

        public async Task<ApiResponse<MessageDTO>> SendMessageToSubordinatesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var subordinateIds = await identityService.GetUserSubordinatesAsync(cancellationToken);
                if (subordinateIds == null || !subordinateIds.Any())
                {
                    logger.LogWarning("Kullanıcının altları bulunamadı, mesaj gönderilemedi");
                    return ApiResponse<MessageDTO>.Error("Kullanıcının altları bulunamadı, mesaj gönderilemedi");
                }

                var response = await CreateMessageAsync(createMessageDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var userId in subordinateIds)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewMessage", response.Data, cancellationToken);
                    }
                    return ApiResponse<MessageDTO>.Success(response.Data, "Mesaj altlara gönderildi");
                }
                else
                {
                    return ApiResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj altlara gönderilemedi");
                return ApiResponse<MessageDTO>.Error("Mesaj altlara gönderilemedi");
            }
        }

        public async Task<ApiResponse<MessageDTO>> SendMessageToSuperiorsAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var superiorIds = await identityService.GetUserSuperiorsAsync(cancellationToken);
                if (superiorIds == null || !superiorIds.Any())
                {
                    logger.LogWarning("Kullanıcının üstleri bulunamadı, mesaj gönderilemedi");
                    return ApiResponse<MessageDTO>.Error("Kullanıcının üstleri bulunamadı, mesaj gönderilemedi");
                }

                var response = await CreateMessageAsync(createMessageDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var userId in superiorIds)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewMessage", response.Data, cancellationToken);
                    }
                    return ApiResponse<MessageDTO>.Success(response.Data, "Mesaj üstlere gönderildi");
                }
                else
                {
                    return ApiResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj üstlere gönderilemedi");
                return ApiResponse<MessageDTO>.Error("Mesaj üstlere gönderilemedi");
            }
        }

        public async Task<ApiResponse<MessageDTO>> SendMessageToUsersAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                if (createMessageDTO.TargetUsers == null || !createMessageDTO.TargetUsers.Any())
                {
                    logger.LogWarning("Gönderilecek kullanıcılar bulunamadı, mesaj gönderilemedi");
                    return ApiResponse<MessageDTO>.Error("Gönderilecek kullanıcılar bulunamadı, mesaj gönderilemedi");
                }

                var response = await CreateMessageAsync(createMessageDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var userId in createMessageDTO.TargetUsers)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewMessage", response.Data, cancellationToken);
                    }

                    logger.LogInformation("Mesaj kullanıcılara gönderildi. TargetUsers: {UserIds}, Content: {Content}",
                        string.Join(", ", createMessageDTO.TargetUsers), createMessageDTO.Content);
                    return ApiResponse<MessageDTO>.Success(response.Data, "Mesaj kullanıcılara gönderildi");
                }
                else
                {
                    logger.LogWarning("Mesaj oluşturulamadı. TargetUsers: {message: UserIds}",
                        string.Join(", ", createMessageDTO.TargetUsers));
                    return ApiResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj kullanıcılara gönderilemedi. TargetUsers: {UserIds}, Content: {Content}",
                    createMessageDTO.TargetUsers != null ? string.Join(", ", createMessageDTO.TargetUsers) : "null",
                    createMessageDTO.Content);
                return ApiResponse<MessageDTO>.Error("Mesaj kullanıcılara gönderilemedi");
            }
        }
    }
}