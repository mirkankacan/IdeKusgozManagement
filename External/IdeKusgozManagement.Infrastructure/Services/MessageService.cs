using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure.Hubs;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class MessageService(IUnitOfWork unitOfWork, ILogger<MessageService> logger, IHubContext<CommunicationHub> hubContext, IIdentityService identityService, UserManager<ApplicationUser> userManager) : IMessageService
    {
        private async Task<ServiceResponse<MessageDTO>> CreateMessageAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = createMessageDTO.Adapt<IdtMessage>();

                await unitOfWork.GetRepository<IdtMessage>().AddAsync(message, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var createdMessage = await unitOfWork.GetRepository<IdtMessage>()
                .WhereAsNoTracking(x => x.Id == message.Id)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(cancellationToken);
                var targetUserIds = !string.IsNullOrWhiteSpace(createdMessage.TargetUsers)
                         ? createdMessage.TargetUsers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                         : new List<string>();

                var userDictionary = new Dictionary<string, string>();

                if (targetUserIds.Any())
                {
                    var users = await userManager.Users
                        .Where(u => targetUserIds.Contains(u.Id))
                        .Select(u => new { u.Id, u.Name, u.Surname })
                        .ToListAsync(cancellationToken);

                    userDictionary = users.ToDictionary(
                        u => u.Id,
                        u => $"{u.Name} {u.Surname}"
                    );
                }

                // Manual mapping
                var messageDTO = new MessageDTO
                {
                    Id = createdMessage.Id,
                    Content = createdMessage.Content,
                    CreatedDate = createdMessage.CreatedDate,
                    CreatedBy = createdMessage.CreatedBy,
                    CreatedByFullName = $"{createdMessage.CreatedByUser.Name} {createdMessage.CreatedByUser.Surname}",
                    TargetUsers = targetUserIds.Any()
                        ? targetUserIds.Select(id => userDictionary.ContainsKey(id) ? userDictionary[id] : "Bilinmeyen Kullanıcı").ToList()
                        : null,
                    TargetRoles = !string.IsNullOrWhiteSpace(createdMessage.TargetRoles)
                        ? createdMessage.TargetRoles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                        : null
                };

                return ServiceResponse<MessageDTO>.Success(messageDTO, "Mesaj başarıyla gönderildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateMessageAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteMessageAsync(string messageId, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = await unitOfWork.GetRepository<IdtMessage>().GetByIdAsync(messageId, cancellationToken);

                if (message == null)
                {
                    return ServiceResponse<bool>.Error("Mesaj bulunamadı");
                }

                unitOfWork.GetRepository<IdtMessage>().Remove(message);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Mesaj başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteMessageAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<PagedResult<MessageDTO>>> GetMessagesAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();
                IQueryable<IdtMessage> baseQuery;

                if (userRole == "Admin" || userRole == "Yönetici")
                {
                    baseQuery = unitOfWork.GetRepository<IdtMessage>()
                       .WhereAsNoTracking(x => x.Id != null);
                }
                else
                {
                    baseQuery = unitOfWork.GetRepository<IdtMessage>()
                        .WhereAsNoTracking(x =>
                        x.TargetUsers.Contains(userId) ||
                        x.TargetRoles.Contains(userRole) ||
                        (x.TargetUsers == null && x.TargetRoles == null));
                }

                var messages = await baseQuery
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.CreatedByUser)
                    .ToListAsync(cancellationToken);

                var messagesCount = await baseQuery.CountAsync(cancellationToken);

                var allTargetUserIds = messages
                    .Where(m => !string.IsNullOrWhiteSpace(m.TargetUsers))
                    .SelectMany(m => m.TargetUsers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    .Distinct()
                    .ToList();

                // TEK SORGU ile tüm kullanıcıları çek
                var userDictionary = new Dictionary<string, string>();

                if (allTargetUserIds.Any())
                {
                    var users = await userManager.Users
                        .Where(u => allTargetUserIds.Contains(u.Id))
                        .Select(u => new { u.Id, u.Name, u.Surname })
                        .ToListAsync(cancellationToken);

                    userDictionary = users.ToDictionary(
                        u => u.Id,
                        u => $"{u.Name} {u.Surname}"
                    );
                }

                var mappedMessages = messages.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    Content = m.Content,
                    CreatedDate = m.CreatedDate,
                    CreatedBy = m.CreatedBy,
                    CreatedByFullName = $"{m.CreatedByUser.Name} {m.CreatedByUser.Surname}",
                    TargetUsers = !string.IsNullOrWhiteSpace(m.TargetUsers)
                        ? m.TargetUsers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(id => userDictionary.ContainsKey(id) ? userDictionary[id] : "Bilinmeyen Kullanıcı")
                            .ToList()
                        : null,
                    TargetRoles = !string.IsNullOrWhiteSpace(m.TargetRoles)
                        ? m.TargetRoles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                        : null
                }).ToList();

                PagedResult<MessageDTO> pagedResult = new()
                {
                    Data = mappedMessages,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = messagesCount
                };

                return ServiceResponse<PagedResult<MessageDTO>>.Success(pagedResult, "Mesajlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetMessagesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<MessageDTO>> SendMessageToAllAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await CreateMessageAsync(createMessageDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    await hubContext.Clients.Group("Messages").SendAsync("NewMessage", response.Data);
                    logger.LogInformation("Mesaj herkese gönderildi. MessageDTO: {MessageDTO}", response.Data);
                    return ServiceResponse<MessageDTO>.Success(response.Data, "Mesaj herkese gönderildi");
                }
                else
                {
                    logger.LogWarning("Mesaj oluşturulamadı. Content: {Content}", createMessageDTO.Content);
                    return ServiceResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj herkese gönderilemedi. Content: {Content}", createMessageDTO.Content);
                throw;
            }
        }

        public async Task<ServiceResponse<MessageDTO>> SendMessageToRolesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                if (createMessageDTO.TargetRoles == null || !createMessageDTO.TargetRoles.Any())
                {
                    logger.LogWarning("TargetRoles boş, mesaj gönderilemedi");
                    return ServiceResponse<MessageDTO>.Error("Roller bulunamadı, mesaj gönderilemedi");
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
                    return ServiceResponse<MessageDTO>.Success(response.Data, "Mesaj rollere gönderildi");
                }
                else
                {
                    logger.LogWarning("Mesaj oluşturulamadı. Roles: {RoleNames}",
                        string.Join(", ", createMessageDTO.TargetRoles));
                    return ServiceResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj rollere gönderilemedi. Roles: {RoleNames}, Content: {Content}",
                    createMessageDTO.TargetRoles != null ? string.Join(", ", createMessageDTO.TargetRoles) : "null",
                    createMessageDTO.Content);
                throw;
            }
        }

        public async Task<ServiceResponse<MessageDTO>> SendMessageToSubordinatesAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var subordinateIds = await identityService.GetUserSubordinatesAsync(cancellationToken);
                if (subordinateIds == null || !subordinateIds.Any())
                {
                    logger.LogWarning("Kullanıcının altları bulunamadı, mesaj gönderilemedi");
                    return ServiceResponse<MessageDTO>.Error("Kullanıcının altları bulunamadı, mesaj gönderilemedi");
                }

                var response = await CreateMessageAsync(createMessageDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var userId in subordinateIds)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewMessage", response.Data, cancellationToken);
                    }
                    return ServiceResponse<MessageDTO>.Success(response.Data, "Mesaj altlara gönderildi");
                }
                else
                {
                    return ServiceResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj altlara gönderilemedi");
                throw;
            }
        }

        public async Task<ServiceResponse<MessageDTO>> SendMessageToSuperiorsAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var superiorIds = await identityService.GetUserSuperiorsAsync(cancellationToken);
                if (superiorIds == null || !superiorIds.Any())
                {
                    logger.LogWarning("Kullanıcının üstleri bulunamadı, mesaj gönderilemedi");
                    return ServiceResponse<MessageDTO>.Error("Kullanıcının üstleri bulunamadı, mesaj gönderilemedi");
                }

                var response = await CreateMessageAsync(createMessageDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var userId in superiorIds)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewMessage", response.Data, cancellationToken);
                    }
                    return ServiceResponse<MessageDTO>.Success(response.Data, "Mesaj üstlere gönderildi");
                }
                else
                {
                    return ServiceResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj üstlere gönderilemedi");
                throw;
            }
        }

        public async Task<ServiceResponse<MessageDTO>> SendMessageToUsersAsync(CreateMessageDTO createMessageDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                if (createMessageDTO.TargetUsers == null || !createMessageDTO.TargetUsers.Any())
                {
                    logger.LogWarning("Gönderilecek kullanıcılar bulunamadı, mesaj gönderilemedi");
                    return ServiceResponse<MessageDTO>.Error("Gönderilecek kullanıcılar bulunamadı, mesaj gönderilemedi");
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
                    return ServiceResponse<MessageDTO>.Success(response.Data, "Mesaj kullanıcılara gönderildi");
                }
                else
                {
                    logger.LogWarning("Mesaj oluşturulamadı. TargetUsers: {message: UserIds}",
                        string.Join(", ", createMessageDTO.TargetUsers));
                    return ServiceResponse<MessageDTO>.Error("Mesaj oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj kullanıcılara gönderilemedi. TargetUsers: {UserIds}, Content: {Content}",
                    createMessageDTO.TargetUsers != null ? string.Join(", ", createMessageDTO.TargetUsers) : "null",
                    createMessageDTO.Content);
                throw;
            }
        }
    }
}