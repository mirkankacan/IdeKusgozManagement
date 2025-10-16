using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
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
    public class NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger, IIdentityService identityService, IHubContext<CommunicationHub> hubContext) : INotificationService
    {
        public async Task<ApiResponse<PagedResult<NotificationDTO>>> GetNotificationsAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();

                // Ortak Where koşulu - DÜZELTİLMİŞ
                var baseQuery = unitOfWork.GetRepository<IdtNotification>()
                    .WhereAsNoTracking(x =>
                        (x.TargetUsers.Contains(userId) ||
                         x.TargetRoles.Contains(userRole) ||
                         (x.TargetUsers == null && x.TargetRoles == null))
                        && x.CreatedBy != userId); // Parantez dışında

                var notifications = await baseQuery
                    .OrderBy(n => n.NotificationReads.Any(nr => nr.CreatedBy == userId)) // Okunmuşlar sona
                    .ThenByDescending(n => n.CreatedDate) // Sonra tarihe göre
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.CreatedByUser)
                    .Include(n => n.NotificationReads)
                    .ToListAsync(cancellationToken);

                var totalCount = await baseQuery.CountAsync(cancellationToken);

                var notificationDTO = notifications.Select(n =>
                {
                    var dto = n.Adapt<NotificationDTO>();
                    var userRead = n.NotificationReads.FirstOrDefault(nr => nr.CreatedBy == userId);
                    dto.IsRead = userRead != null;
                    dto.ReadDate = userRead?.CreatedDate;
                    return dto;
                });

                var pagedResult = new PagedResult<NotificationDTO>
                {
                    Data = notificationDTO,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                };

                return ApiResponse<PagedResult<NotificationDTO>>.Success(pagedResult, "Bildirimler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetNotificationsAsync işleminde hata oluştu");
                return ApiResponse<PagedResult<NotificationDTO>>.Error("Bildirimler getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<int>> GetUnreadNotificationCountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();

                var unreadCount = await unitOfWork.GetRepository<IdtNotification>()
                    .WhereAsNoTracking(x =>
                        (x.TargetUsers.Contains(userId) ||
                         x.TargetRoles.Contains(userRole) ||
                         (x.TargetUsers == null && x.TargetRoles == null))
                        && x.CreatedBy != userId) // Parantez dışında
                    .Where(n => !n.NotificationReads.Any(nr => nr.CreatedBy == userId))
                    .CountAsync(cancellationToken); // Include kaldırıldı

                return ApiResponse<int>.Success(unreadCount, "Okunmamış bildirim sayısı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUnreadNotificationCountAsync işleminde hata oluştu");
                return ApiResponse<int>.Error("Okunmamış bildirim sayısı getirilirken hata oluştu");
            }
        }

        private async Task<ApiResponse<NotificationDTO>> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = createNotificationDTO.Adapt<IdtNotification>();

                await unitOfWork.GetRepository<IdtNotification>().AddAsync(notification, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var createdNotification = await unitOfWork.GetRepository<IdtNotification>()
                    .WhereAsNoTracking(x => x.Id == notification.Id)
                    .Include(n => n.CreatedByUser)
                    .Include(n => n.NotificationReads)
                    .FirstOrDefaultAsync(cancellationToken);

                var notificationDTO = createdNotification.Adapt<NotificationDTO>();

                return ApiResponse<NotificationDTO>.Success(notificationDTO, "Bildirim başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateNotificationAsync işleminde hata oluştu");
                return ApiResponse<NotificationDTO>.Error("Bildirim oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();

                var isNotificationRead = await unitOfWork.GetRepository<IdtNotificationRead>().AnyAsync(x => x.CreatedBy == userId && x.NotificationId == notificationId, cancellationToken);
                if (isNotificationRead)
                {
                    return ApiResponse<bool>.Error("Bildirim daha önceden okundu olarak işaretlenmiş");
                }
                var notification = await unitOfWork.GetRepository<IdtNotification>()
                .Where(x => x.Id == notificationId)
                .FirstOrDefaultAsync(cancellationToken);

                if (notification == null)
                {
                    return ApiResponse<bool>.Error("Bildirim bulunamadı");
                }

                var notificationRead = new IdtNotificationRead
                {
                    NotificationId = notificationId,
                    IsRead = true
                };
                await unitOfWork.GetRepository<IdtNotificationRead>().AddAsync(notificationRead, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Bildirim okundu olarak işaretlendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MarkAsReadAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Bildirim okundu olarak işaretlenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();

                var notifications = await unitOfWork.GetRepository<IdtNotification>()
                    .Where(x => x.TargetUsers.Contains(userId) || x.TargetRoles.Contains(userRole) || (x.TargetUsers == null && x.TargetRoles == null) && x.CreatedBy != userId && !x.NotificationReads.Any(x => x.CreatedBy == userId))
                    .Include(n => n.NotificationReads)
                    .ToListAsync(cancellationToken);

                var notificationReads = notifications.Select(n => new IdtNotificationRead
                {
                    NotificationId = n.Id,
                    IsRead = true
                }).ToList();

                await unitOfWork.GetRepository<IdtNotificationRead>().AddRangeAsync(notificationReads, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Tüm bildirimler okundu olarak işaretlendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MarkAllAsReadAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Tüm bildirimler okundu olarak işaretlenirken hata oluştu");
            }
        }

        public async Task SendNotificationToAllAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);

                if (response.IsSuccess)
                {
                    await hubContext.Clients.All.SendAsync("NewNotification", response.Data, cancellationToken);
                    logger.LogInformation("Bildirim herkese gönderildi. Message: {Message}", createNotificationDTO.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim herkese gönderilemedi. Message: {Message}", createNotificationDTO.Message);
            }
        }

        public async Task SendNotificationToRolesAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                if (createNotificationDTO.TargetRoles == null || !createNotificationDTO.TargetRoles.Any())
                {
                    logger.LogWarning("TargetRoles boş, bildirim gönderilemedi");
                }

                var response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var role in createNotificationDTO.TargetRoles)
                    {
                        var groupName = $"Role_{role}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }

                    logger.LogInformation(message: "Bildirim rollere gönderildi. Roles: {RoleNames}, Message: {Message}",
                        string.Join(", ", createNotificationDTO.TargetRoles), createNotificationDTO.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim rollere gönderilemedi. Roles: {RoleNames}, Message: {Message}",
                    createNotificationDTO.TargetRoles != null ? string.Join(", ", createNotificationDTO.TargetRoles) : "null",
                    createNotificationDTO.Message);
            }
        }

        public async Task SendNotificationToSubordinatesAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var subordinateIds = await identityService.GetUserSubordinatesAsync(cancellationToken);
                if (subordinateIds == null || !subordinateIds.Any())
                {
                    logger.LogWarning("Kullanıcının altları bulunamadı, bildirim gönderilemedi");
                    return;
                }

                ApiResponse<NotificationDTO>? response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var userId in subordinateIds)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim altlara gönderilemedi");
            }
        }

        public async Task SendNotificationToSuperiorsAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var superiorIds = await identityService.GetUserSuperiorsAsync(cancellationToken);
                if (superiorIds == null || !superiorIds.Any())
                {
                    logger.LogWarning("Kullanıcının üstleri bulunamadı, bildirim gönderilemedi");
                    return;
                }

                ApiResponse<NotificationDTO>? response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var userId in superiorIds)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim üstlere gönderilemedi");
            }
        }

        public async Task SendNotificationToUsersAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                if (createNotificationDTO.TargetUsers == null || !createNotificationDTO.TargetUsers.Any())
                {
                    logger.LogWarning("TargetUsers boş, bildirim gönderilemedi");
                    return;
                }

                var response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    foreach (var userId in createNotificationDTO.TargetUsers)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }

                    logger.LogInformation("Bildirim kullanıcılara gönderildi. TargetUsers: {UserIds}, Message: {Message}",
                        string.Join(", ", createNotificationDTO.TargetUsers), createNotificationDTO.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim kullanıcılara gönderilemedi. TargetUsers: {UserIds}, Message: {Message}",
                    createNotificationDTO.TargetUsers != null ? string.Join(", ", createNotificationDTO.TargetUsers) : "null",
                    createNotificationDTO.Message);
            }
        }
    }
}