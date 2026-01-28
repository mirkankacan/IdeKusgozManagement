using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure.Hubs;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class NotificationService(
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger,
        IIdentityService identityService,
        IHubContext<CommunicationHub> hubContext,
        IOneSignalService oneSignalService) : INotificationService
    {
        public async Task<ServiceResult<PagedResult<NotificationDTO>>> GetNotificationsAsync(int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();

                var baseQuery = unitOfWork.GetRepository<IdtNotification>()
                    .WhereAsNoTracking(x =>
                        (x.TargetUsers.Contains(userId) ||
                         x.TargetRoles.Contains(userRole) ||
                         (x.TargetUsers == null && x.TargetRoles == null))
                        && x.CreatedBy != userId);

                var notifications = await baseQuery
                    .OrderBy(n => n.NotificationReads.Any(nr => nr.CreatedBy == userId))
                    .ThenByDescending(n => n.CreatedDate)
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

                return ServiceResult<PagedResult<NotificationDTO>>.SuccessAsOk(pagedResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetNotificationsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<int>> GetUnreadNotificationCountAsync(CancellationToken cancellationToken = default)
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
                        && x.CreatedBy != userId)
                    .Where(n => !n.NotificationReads.Any(nr => nr.CreatedBy == userId))
                    .CountAsync(cancellationToken);

                return ServiceResult<int>.SuccessAsOk(unreadCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUnreadNotificationCountAsync işleminde hata oluştu");
                throw;
            }
        }

        private async Task<ServiceResult<NotificationDTO>> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
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

                return ServiceResult<NotificationDTO>.SuccessAsCreated(notificationDTO, $"/api/notifications/{notificationDTO.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateNotificationAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();

                var isNotificationRead = await unitOfWork.GetRepository<IdtNotificationRead>().AnyAsync(x => x.CreatedBy == userId && x.NotificationId == notificationId, cancellationToken);
                if (isNotificationRead)
                {
                    return ServiceResult<bool>.Error("Bildirim Zaten Okunmuş", "Bildirim daha önceden okundu olarak işaretlenmiş.", HttpStatusCode.BadRequest);
                }
                var notification = await unitOfWork.GetRepository<IdtNotification>()
                .Where(x => x.Id == notificationId)
                .FirstOrDefaultAsync(cancellationToken);

                if (notification == null)
                {
                    return ServiceResult<bool>.Error("Bildirim Bulunamadı", "Belirtilen ID'ye sahip bildirim bulunamadı.", HttpStatusCode.NotFound);
                }

                var notificationRead = new IdtNotificationRead
                {
                    NotificationId = notificationId,
                    IsRead = true
                };
                await unitOfWork.GetRepository<IdtNotificationRead>().AddAsync(notificationRead, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MarkAsReadAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default)
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

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MarkAllAsReadAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task SendNotificationToAllAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);

                if (response.IsSuccess)
                {
                    // SignalR bildirimi (Web tarayıcı için)
                    await hubContext.Clients.All.SendAsync("NewNotification", response.Data, cancellationToken);

                    // OneSignal push bildirimi (Mobil cihazlar için)
                    await oneSignalService.SendNotificationAsync(
                        message: createNotificationDTO.Message,
                        heading: "Yeni Bildirim",
                        additionalData: new Dictionary<string, object>
                        {
                            { "notificationId", response.Data.Id },
                            { "type", createNotificationDTO.Type.ToString() ?? "general" }
                        }
                    );

                    logger.LogInformation("Bildirim herkese gönderildi. Message: {Message}", createNotificationDTO.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim herkese gönderilemedi. Message: {Message}", createNotificationDTO.Message);
                throw;
            }
        }

        public async Task SendNotificationToRolesAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                if (createNotificationDTO.TargetRoles == null || !createNotificationDTO.TargetRoles.Any())
                {
                    logger.LogWarning("TargetRoles boş, bildirim gönderilemedi");
                    return;
                }

                var response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    // SignalR bildirimi
                    foreach (var role in createNotificationDTO.TargetRoles)
                    {
                        var groupName = $"Role_{role}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }

                    // OneSignal push bildirimi (rollere göre)
                    await oneSignalService.SendNotificationAsync(
                        message: createNotificationDTO.Message,
                        heading: "Yeni Bildirim",
                        roles: createNotificationDTO.TargetRoles,
                        additionalData: new Dictionary<string, object>
                        {
                            { "notificationId", response.Data.Id },
                            { "type", createNotificationDTO.Type.ToString() ?? "general" }
                        }
                    );

                    logger.LogInformation("Bildirim rollere gönderildi. Roles: {RoleNames}, Message: {Message}",
                        string.Join(", ", createNotificationDTO.TargetRoles), createNotificationDTO.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim rollere gönderilemedi. Roles: {RoleNames}, Message: {Message}",
                    createNotificationDTO.TargetRoles != null ? string.Join(", ", createNotificationDTO.TargetRoles) : "null",
                    createNotificationDTO.Message);
                throw;
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

                var response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    // SignalR bildirimi
                    foreach (var userId in subordinateIds)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }

                    // OneSignal push bildirimi
                    await oneSignalService.SendNotificationAsync(
                        message: createNotificationDTO.Message,
                        heading: "Yeni Bildirim",
                        userIds: subordinateIds,
                        additionalData: new Dictionary<string, object>
                        {
                            { "notificationId", response.Data.Id },
                            { "type", createNotificationDTO.Type.ToString() ?? "general" }
                        }
                    );

                    logger.LogInformation("Bildirim altlara gönderildi. Count: {Count}", subordinateIds.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim altlara gönderilemedi");
                throw;
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

                var response = await CreateNotificationAsync(createNotificationDTO, cancellationToken);
                if (response.IsSuccess)
                {
                    // SignalR bildirimi
                    foreach (var userId in superiorIds)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }

                    // OneSignal push bildirimi
                    await oneSignalService.SendNotificationAsync(
                        message: createNotificationDTO.Message,
                        heading: "Yeni Bildirim",
                        userIds: superiorIds,
                        additionalData: new Dictionary<string, object>
                        {
                            { "notificationId", response.Data.Id },
                            { "type", createNotificationDTO.Type.ToString() ?? "general" }
                        }
                    );

                    logger.LogInformation("Bildirim üstlere gönderildi. Count: {Count}", superiorIds.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim üstlere gönderilemedi");
                throw;
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
                    // SignalR bildirimi
                    foreach (var userId in createNotificationDTO.TargetUsers)
                    {
                        var groupName = $"User_{userId}";
                        await hubContext.Clients.Group(groupName).SendAsync("NewNotification", response.Data, cancellationToken);
                    }

                    // OneSignal push bildirimi
                    await oneSignalService.SendNotificationAsync(
                        message: createNotificationDTO.Message,
                        heading: "Yeni Bildirim",
                        userIds: createNotificationDTO.TargetUsers,

                        additionalData: new Dictionary<string, object>
                        {
                            { "notificationId", response.Data.Id },
                            { "type", createNotificationDTO.Type.ToString() ?? "general" }
                        }
                    );

                    logger.LogInformation("Bildirim kullanıcılara gönderildi. TargetUsers: {UserIds}, Message: {Message}",
                        string.Join(", ", createNotificationDTO.TargetUsers), createNotificationDTO.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bildirim kullanıcılara gönderilemedi. TargetUsers: {UserIds}, Message: {Message}",
                    createNotificationDTO.TargetUsers != null ? string.Join(", ", createNotificationDTO.TargetUsers) : "null",
                    createNotificationDTO.Message);
                throw;
            }
        }
    }
}