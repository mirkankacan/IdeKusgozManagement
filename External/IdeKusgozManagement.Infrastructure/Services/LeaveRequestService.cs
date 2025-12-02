using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class LeaveRequestService(IUnitOfWork unitOfWork, ILogger<LeaveRequestService> logger, IFileService fileService, INotificationService notificationService, IIdentityService identityService, IHolidayService holidayService) : ILeaveRequestService
    {
        public async Task<ServiceResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequests = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .WhereAsNoTracking(null)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate).ToListAsync(cancellationToken);

                // Mapping config dosyasında CreatedByUser ve UpdatedByUser için gerekli dönüşümler yapıldı
                var mappedLeaveRequests = leaveRequests.Adapt<IEnumerable<LeaveRequestDTO>>();

                return ServiceResponse<IEnumerable<LeaveRequestDTO>>.Success(mappedLeaveRequests, "İzin talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<LeaveRequestDTO>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .WhereAsNoTracking(x => x.Id == leaveRequestId)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefaultAsync(cancellationToken);

                if (leaveRequest == null)
                {
                    return ServiceResponse<LeaveRequestDTO>.Error("İzin talebi bulunamadı");
                }

                var leaveRequestDTO = leaveRequest.Adapt<LeaveRequestDTO>();

                return ServiceResponse<LeaveRequestDTO>.Success(leaveRequestDTO, "İzin talebi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<LeaveRequestDTO>> CreateLeaveRequestAsync(CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Tarih kontrolü
                if (createLeaveRequestDTO.StartDate > createLeaveRequestDTO.EndDate)
                {
                    return ServiceResponse<LeaveRequestDTO>.Error("Başlangıç tarihi bitiş tarihinden önce olmalıdır");
                }

                // Geçmiş tarih kontrolü
                if (createLeaveRequestDTO.StartDate.Date < DateTime.Today.Date)
                {
                    return ServiceResponse<LeaveRequestDTO>.Error("Geçmiş tarihli izin talebi oluşturulamaz");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var leaveRequest = createLeaveRequestDTO.Adapt<IdtLeaveRequest>();
                leaveRequest.Status = LeaveRequestStatus.Pending; // Yeni talepler beklemede başlar

                var calcResponse = await holidayService.CalculateWorkingDaysAsync(
                          createLeaveRequestDTO.StartDate.Date,
                          createLeaveRequestDTO.EndDate.Date);

                leaveRequest.Duration = $"{calcResponse.Data} gün";

                if (createLeaveRequestDTO.File != null && createLeaveRequestDTO.File.FormFile.Length > 0)
                {
                    createLeaveRequestDTO.File.TargetUserId = identityService.GetUserId();
                    createLeaveRequestDTO.File.DocumentTypeId = "ED9DA894-0421-4AB1-8672-0F2104B85A5F";
                    var fileList = new List<UploadFileDTO> { createLeaveRequestDTO.File };

                    var fileResult = await fileService.UploadFileAsync(fileList, cancellationToken);

                    if (!fileResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);

                        return ServiceResponse<LeaveRequestDTO>.Error(fileResult.Message);
                    }

                    leaveRequest.FileId = fileResult.Data.FirstOrDefault().Id;
                }
                await unitOfWork.GetRepository<IdtLeaveRequest>().AddAsync(leaveRequest, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var createdLeaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .WhereAsNoTracking(x => x.Id == leaveRequest.Id)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .FirstOrDefaultAsync(cancellationToken);

                var leaveRequestDTO = createdLeaveRequest.Adapt<LeaveRequestDTO>();

                CreateNotificationDTO createNotification = new()
                {
                    Message = $"{leaveRequestDTO.CreatedByFullName} tarafından {leaveRequestDTO.CreatedDate.ToString("dd.MM.yyyy HH:mm")} tarihinde yeni bir izin talebi oluşturuldu.",
                    Type = NotificationType.LeaveRequest,
                    RedirectUrl = "/izin",
                    TargetUsers = await identityService.GetUserSuperiorsAsync()
                };
                await notificationService.SendNotificationToSuperiorsAsync(createNotification, cancellationToken);
                return ServiceResponse<LeaveRequestDTO>.Success(leaveRequestDTO, "İzin talebi başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);

                logger.LogError(ex, "CreateLeaveRequestAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteLeaveRequestAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>().GetByIdAsync(id, cancellationToken);

                if (leaveRequest == null)
                {
                    return ServiceResponse<bool>.Error("İzin talebi bulunamadı");
                }

                // Sadece beklemede olan talepler silinebilir
                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ServiceResponse<bool>.Error("Sadece beklemede olan izin talepleri silinebilir");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (!string.IsNullOrEmpty(leaveRequest.FileId))
                    await fileService.DeleteFileAsync(leaveRequest.FileId, cancellationToken);

                unitOfWork.GetRepository<IdtLeaveRequest>().Remove(leaveRequest);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ServiceResponse<bool>.Success(true, "İzin talebi başarıyla silindi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "DeleteLeaveRequestAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>().GetByIdAsync(leaveRequestId, cancellationToken);

                if (leaveRequest == null)
                {
                    return ServiceResponse<bool>.Error("İzin talebi bulunamadı");
                }

                // Sadece beklemede olan talepler onaylanabilir
                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ServiceResponse<bool>.Error("Sadece beklemede olan izin talepleri onaylanabilir");
                }

                leaveRequest.Status = LeaveRequestStatus.Approved;

                unitOfWork.GetRepository<IdtLeaveRequest>().Update(leaveRequest);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var approvedLeaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                 .Where(x => x.Id == leaveRequestId)
                 .Include(x => x.CreatedByUser)
                 .Include(x => x.UpdatedByUser)
                 .Include(x => x.File)
                 .FirstOrDefaultAsync(cancellationToken);
                var mappedLeaveRequest = approvedLeaveRequest.Adapt<LeaveRequestDTO>();
                CreateNotificationDTO createNotification = new()
                {
                    Message = $"{mappedLeaveRequest.UpdatedByFullName} tarafından, {mappedLeaveRequest.UpdatedDate?.ToString("dd.MM.yyyy HH:mm")} tarihinde bir izin talebiniz onaylandı.",
                    Type = NotificationType.LeaveRequest,
                    RedirectUrl = "/izin/olustur",
                    TargetUsers = new List<string> { mappedLeaveRequest.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(createNotification, cancellationToken);
                return ServiceResponse<bool>.Success(true, $"İzin talebi başarıyla onaylandı. LeaveRequestId: {leaveRequestId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ApproveLeaveRequestAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>().GetByIdAsync(leaveRequestId, cancellationToken);
                if (leaveRequest == null)
                {
                    return ServiceResponse<bool>.Error("İzin talebi bulunamadı");
                }

                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ServiceResponse<bool>.Error("Sadece beklemede olan izin talepleri reddedilebilir");
                }

                leaveRequest.Status = LeaveRequestStatus.Rejected;
                leaveRequest.RejectReason = !string.IsNullOrEmpty(rejectReason) ? rejectReason : null;

                unitOfWork.GetRepository<IdtLeaveRequest>().Update(leaveRequest);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var rejectedLeaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                  .WhereAsNoTracking(x => x.Id == leaveRequestId)
                  .Include(x => x.CreatedByUser)
                  .Include(x => x.UpdatedByUser)
                  .Include(x => x.File)
                  .FirstOrDefaultAsync(cancellationToken);
                var mappedLeaveRequest = rejectedLeaveRequest.Adapt<LeaveRequestDTO>();
                CreateNotificationDTO createNotification = new()
                {
                    Message = $"{mappedLeaveRequest.UpdatedByFullName} tarafından, {mappedLeaveRequest.UpdatedDate?.ToString("dd.MM.yyyy HH:mm")} tarihinde bir izin talebiniz reddedildi.",
                    Type = NotificationType.LeaveRequest,
                    RedirectUrl = "/izin/olustur",
                    TargetUsers = new List<string> { mappedLeaveRequest.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(createNotification, cancellationToken);
                return ServiceResponse<bool>.Success(true, $"İzin talebi başarıyla reddedildi. LeaveRequestId: {leaveRequestId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RejectLeaveRequestAsync işleminde hata oluştu. LeaveRequestId: {LeaveRequestId}", leaveRequestId);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequests = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .WhereAsNoTracking(x => x.CreatedBy == userId)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                var leaveRequestDTOs = leaveRequests.Adapt<IEnumerable<LeaveRequestDTO>>();

                return ServiceResponse<IEnumerable<LeaveRequestDTO>>.Success(leaveRequestDTOs, $"Kullanıcının izin talepleri başarıyla getirildi. UserId: {userId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestsByUserIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByStatusAsync(LeaveRequestStatus status, string? userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var baseQuery = unitOfWork.GetRepository<IdtLeaveRequest>().WhereAsNoTracking(x => x.Status == status);

                if (!string.IsNullOrEmpty(userId))
                {
                    baseQuery = baseQuery.Where(x => x.CreatedBy == userId);
                }

                var leaveRequests = await baseQuery
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                var leaveRequestDTOs = leaveRequests.Adapt<IEnumerable<LeaveRequestDTO>>();

                return ServiceResponse<IEnumerable<LeaveRequestDTO>>.Success(leaveRequestDTOs, "Duruma göre izin talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestByStatus işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateLeaveRequestAsync(string leaveRequestId, UpdateLeaveRequestDTO updateLeaveRequestDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Tarih kontrolü
                if (updateLeaveRequestDTO.StartDate > updateLeaveRequestDTO.EndDate)
                {
                    return ServiceResponse<bool>.Error("Başlangıç tarihi bitiş tarihinden önce olmalıdır");
                }
                // Geçmiş tarih kontrolü
                if (updateLeaveRequestDTO.StartDate.Date < DateTime.Today.Date)
                {
                    return ServiceResponse<bool>.Error("Geçmiş tarihli izin talebi oluşturulamaz");
                }

                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var userRole = identityService.GetUserRole();
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>().GetByIdAsync(leaveRequestId, cancellationToken);
                if (leaveRequest == null)
                {
                    return ServiceResponse<bool>.Error("İzin talebi bulunamadı");
                }
                if (leaveRequest.Status == LeaveRequestStatus.Approved && (userRole != "Yönetici" || userRole != "Admin"))
                {
                    return ServiceResponse<bool>.Error("İzin talebi daha önceden onaylanmış güncelleme yetkiniz bulunmamaktadır");
                }

                var changedFields = GetChangedFields(leaveRequest, updateLeaveRequestDTO);

                string changedFieldsText = changedFields.Any() ? $" Değişen alanlar: {string.Join(", ", changedFields)}." : "";

                var calcResponse = await holidayService.CalculateWorkingDaysAsync(
                          updateLeaveRequestDTO.StartDate.Date,
                          updateLeaveRequestDTO.EndDate.Date);

                leaveRequest.StartDate = updateLeaveRequestDTO.StartDate;
                leaveRequest.EndDate = updateLeaveRequestDTO.EndDate;
                leaveRequest.Reason = updateLeaveRequestDTO.Reason;
                leaveRequest.Description = updateLeaveRequestDTO.Description ?? null;
                leaveRequest.Status = LeaveRequestStatus.Pending;
                leaveRequest.Duration = $"{calcResponse.Data} gün";

                bool hasFileChange = updateLeaveRequestDTO.File != null && updateLeaveRequestDTO.File.FormFile != null;

                if (hasFileChange)
                {
                    string? oldFileId = leaveRequest.FileId ?? null;

                    updateLeaveRequestDTO.File.TargetUserId = identityService.GetUserId();
                    updateLeaveRequestDTO.File.DocumentTypeId = "ED9DA894-0421-4AB1-8672-0F2104B85A5F";
                    var fileList = new List<UploadFileDTO> { updateLeaveRequestDTO.File };

                    var fileUploadResult = await fileService.UploadFileAsync(fileList, cancellationToken);

                    if (!fileUploadResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);

                        return ServiceResponse<bool>.Error(fileUploadResult.Message);
                    }
                    if (!string.IsNullOrEmpty(oldFileId))
                    {
                        var fileDeleteResult = await fileService.DeleteFileAsync(oldFileId, cancellationToken);
                        if (!fileDeleteResult.IsSuccess)
                        {
                            logger.LogWarning("Dosya silinirken hata oluştu. FileId: {FileId}, Error: {Error}",
                                oldFileId, fileDeleteResult.Message);
                        }
                    }

                    leaveRequest.FileId = fileUploadResult.Data.FirstOrDefault()?.Id;
                }

                unitOfWork.GetRepository<IdtLeaveRequest>().Update(leaveRequest);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var updatedLeaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .WhereAsNoTracking(x => x.Id == leaveRequest.Id)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .FirstOrDefaultAsync(cancellationToken);

                var leaveRequestDTO = updatedLeaveRequest.Adapt<LeaveRequestDTO>();

                CreateNotificationDTO createNotification = new()
                {
                    Message = $"{leaveRequestDTO.UpdatedByFullName} tarafından {leaveRequestDTO.UpdatedDate?.ToString("dd.MM.yyyy HH:mm")} tarihinde, {leaveRequest.CreatedDate.ToString("dd.MM.yyyy HH:mm")} tarihinde oluşturmuş olduğunuz izin talebiniz güncellendi.{changedFieldsText}",
                    Type = NotificationType.LeaveRequest,
                    RedirectUrl = "/izin/istek-olustur",
                    TargetUsers = new() { leaveRequestDTO.CreatedBy }
                };

                await notificationService.SendNotificationToUsersAsync(createNotification, cancellationToken);
                return ServiceResponse<bool>.Success(true, "İzin talebi başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "UpdateLeaveRequestAsync işleminde hata oluştu");
                throw;
            }
        }

        private List<string> GetChangedFields(IdtLeaveRequest existing, UpdateLeaveRequestDTO incoming)
        {
            var changedFields = new List<string>();

            if (existing.StartDate.Date != incoming.StartDate.Date)
                changedFields.Add("Başlangıç Tarihi");

            if (existing.EndDate.Date != incoming.EndDate.Date)
                changedFields.Add("Bitiş Tarihi");

            if (existing.Reason != incoming.Reason)
                changedFields.Add("İzin Sebebi");

            if (existing.Description != incoming.Description)
                changedFields.Add("Açıklama");

            if (!string.IsNullOrEmpty(existing.FileId) && incoming.File != null && incoming.File.FormFile != null)
                changedFields.Add("Döküman");

            return changedFields;
        }

        public async Task<ServiceResponse<IEnumerable<LeaveRequestDTO>>> GetSubordinateLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var superiorId = identityService.GetUserId();
                if (superiorId == null)
                    return ServiceResponse<IEnumerable<LeaveRequestDTO>>.Error("Kullanıcı bilgisi alınamadı");

                var subordinateIds = await unitOfWork.GetRepository<IdtUserHierarchy>()
                    .Where(x => x.SuperiorId == superiorId)
                    .Select(x => x.SubordinateId)
                    .ToListAsync(cancellationToken);

                var leaveRequests = await unitOfWork.GetRepository<IdtLeaveRequest>().WhereAsNoTracking(x => subordinateIds.Contains(x.CreatedBy))
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                var leaveRequestDTOs = leaveRequests.Adapt<IEnumerable<LeaveRequestDTO>>();

                return ServiceResponse<IEnumerable<LeaveRequestDTO>>.Success(leaveRequestDTOs, "Kullanıcının astlarının izin talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestsBySuperiorIdAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}