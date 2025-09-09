using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.Interfaces.Repositories;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<NotificationDTO>>> GetNotificationsAsync(string userId, int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                var notifications = await _unitOfWork.Repository<IdtNotification>()
                    .GetPagedNoTrackingAsync(pageNumber, pageSize, cancellationToken, n => n.CreatedByUser, n => n.NotificationReads);

                var notificationDTOs = notifications.Data.Select(n =>
                {
                    var dto = n.Adapt<NotificationDTO>();

                    // NotificationReads'te kayıt varsa okunmuş, yoksa okunmamış
                    var readRecord = n.NotificationReads?.FirstOrDefault(nr => nr.CreatedBy == userId);
                    dto.IsRead = readRecord != null;
                    dto.ReadDate = readRecord?.CreatedDate;

                    return dto;
                }).ToList();

                return ApiResponse<IEnumerable<NotificationDTO>>.Success(notificationDTOs, "Bildirimler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetNotificationsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<NotificationDTO>>.Error("Bildirimler getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<int>> GetUnreadNotificationCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var notifications = await _unitOfWork.Repository<IdtNotification>()
                    .GetAllNoTrackingAsync(cancellationToken, n => n.NotificationReads);

                var unreadCount = notifications.Count(n => !n.NotificationReads.Any(nr => nr.CreatedBy == userId));

                return ApiResponse<int>.Success(unreadCount, "Okunmamış bildirim sayısı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUnreadNotificationCountAsync işleminde hata oluştu");
                return ApiResponse<int>.Error("Okunmamış bildirim sayısı getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<NotificationDTO>> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = createNotificationDTO.Adapt<IdtNotification>();

                await _unitOfWork.Repository<IdtNotification>().AddAsync(notification, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Get the created notification with user information
                var createdNotification = await _unitOfWork.Repository<IdtNotification>()
                    .GetByIdNoTrackingAsync(notification.Id, cancellationToken, n => n.CreatedByUser, n => n.NotificationReads);

                var notificationDTO = createdNotification.Adapt<NotificationDTO>();

                return ApiResponse<NotificationDTO>.Success(notificationDTO, "Bildirim başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateNotificationAsync işleminde hata oluştu");
                return ApiResponse<NotificationDTO>.Error("Bildirim oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(string notificationId, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if notification exists
                var notification = await _unitOfWork.Repository<IdtNotification>().GetByIdAsync(notificationId, cancellationToken, n => n.NotificationReads);

                if (notification == null)
                {
                    return ApiResponse<bool>.Error("Bildirim bulunamadı");
                }
                var alreadyRead = notification.NotificationReads?.Any(nr => nr.CreatedBy == userId) ?? false;

                if (alreadyRead)
                {
                    return ApiResponse<bool>.Success(true, "Bildirim zaten okundu olarak işaretlenmiş");
                }
                var notificationRead = new IdtNotificationRead
                {
                    NotificationId = notificationId
                };
                await _unitOfWork.Repository<IdtNotificationRead>().AddAsync(notificationRead, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Bildirim okundu olarak işaretlendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MarkAsReadAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Bildirim okundu olarak işaretlenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var unreadNotificationIds = await _unitOfWork.Repository<IdtNotification>()
                    .SelectNoTrackingAsync(
                        selector: n => n.Id,
                        predicate: n => !n.NotificationReads.Any(nr => nr.CreatedBy == userId),
                        cancellationToken
                    );

                var unreadNotificationIdsList = unreadNotificationIds.ToList();

                if (unreadNotificationIdsList.Any())
                {
                    var notificationReads = unreadNotificationIdsList.Select(notificationId => new IdtNotificationRead
                    {
                        NotificationId = notificationId
                    }).ToList();

                    await _unitOfWork.Repository<IdtNotificationRead>().AddRangeAsync(notificationReads, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return ApiResponse<bool>.Success(true, "Tüm bildirimler okundu olarak işaretlendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MarkAllAsReadAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Tüm bildirimler okundu olarak işaretlenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteNotificationAsync(string notificationId, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = await _unitOfWork.Repository<IdtNotification>()
                    .GetByIdAsync(notificationId, cancellationToken);

                if (notification == null)
                {
                    return ApiResponse<bool>.Error("Bildirim bulunamadı");
                }

                await _unitOfWork.Repository<IdtNotification>().DeleteAsync(notification, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Bildirim başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteNotificationAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Bildirim silinirken hata oluştu");
            }
        }
    }
}