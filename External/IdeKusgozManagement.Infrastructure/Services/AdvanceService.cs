using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.AdvanceDTOs;
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
    public class AdvanceService(IUnitOfWork unitOfWork, ILogger<AdvanceService> logger, IIdentityService identityService, INotificationService notificationService) : IAdvanceService
    {
        public async Task<ApiResponse<bool>> ApproveAdvanceAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ApiResponse<bool>.Error("Avans isteği bulunamadı");
                }
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();
                switch (userRole)
                {
                    case "Admin":
                        advance.Status = AdvanceStatus.ApprovedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        break;

                    case "Yönetici":
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            return ApiResponse<bool>.Error("Avans isteği zaten birim yönetici tarafından onaylanmış");
                        }
                        advance.Status = AdvanceStatus.ApprovedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;

                        break;

                    case "Şef":
                        if (advance.Status == AdvanceStatus.ApprovedByChief)
                        {
                            return ApiResponse<bool>.Error("Avans isteği zaten şef tarafından onaylanmış");
                        }
                        advance.Status = AdvanceStatus.ApprovedByChief;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        break;

                    default:
                        return ApiResponse<bool>.Error("Avans isteğini onaylamak için yetkiniz yok");
                }

                unitOfWork.GetRepository<IdtAdvance>().Update(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var approvedAdvance = await unitOfWork.GetRepository<IdtAdvance>()
                    .WhereAsNoTracking(e => e.Id == advance.Id)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .FirstOrDefaultAsync(cancellationToken);
                var mappedAdvance = approvedAdvance.Adapt<AdvanceDTO>();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedAdvance.UpdatedByFullName} tarafından, {mappedAdvance.CreatedDate:dd/MM/yyyy} tarihinde oluşturduğunuz {mappedAdvance.Reason} avans isteğiniz onaylandı.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/avans/listem",
                    TargetUsers = new List<string> { mappedAdvance.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ApiResponse<bool>.Success(true, "Avans isteği başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ApproveAdvanceAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Avans isteği onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> CreateAdvanceAsync(CreateAdvanceDTO createAdvanceDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var newAdvance = createAdvanceDTO.Adapt<IdtAdvance>();
                await unitOfWork.GetRepository<IdtAdvance>().AddAsync(newAdvance, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var createdAdvance = await unitOfWork.GetRepository<IdtAdvance>()
                 .WhereAsNoTracking(e => e.Id == newAdvance.Id)
                 .Include(x => x.ChiefUser)
                 .Include(x => x.UnitManagerUser)
                 .Include(x => x.CreatedByUser)
                 .Include(x => x.UpdatedByUser)
                 .FirstOrDefaultAsync(cancellationToken);
                var mappedAdvance = createdAdvance.Adapt<AdvanceDTO>();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedAdvance.CreatedByFullName} tarafından, {mappedAdvance.CreatedDate:dd/MM/yyyy} tarihinde {mappedAdvance.Reason} avans isteği oluşturuldu.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/avans",
                    TargetUsers = await identityService.GetUserSuperiorsAsync(cancellationToken)
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ApiResponse<string>.Success(newAdvance.Id, "Avans isteği başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateAdvanceAsync işleminde hata oluştu");
                return ApiResponse<string>.Error("Avans isteği oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAdvanceAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);

                if (advance == null)
                {
                    return ApiResponse<bool>.Error("Avans isteği bulunamadı");
                }

                var isAdvanceApproved = await unitOfWork.GetRepository<IdtAdvance>().AnyAsync(x => x.Status == AdvanceStatus.ApprovedByUnitManager && x.Id == advance.Id, cancellationToken);

                if (isAdvanceApproved)
                {
                    return ApiResponse<bool>.Error("Bu avans isteği birim yönetici tarafından onaylandığı için silinemez");
                }

                unitOfWork.GetRepository<IdtAdvance>().Remove(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Avans isteği başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteAdvanceAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Avans isteği silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<AdvanceDTO>> GetAdvanceByIdAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(e => e.Id == advanceId)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser).FirstOrDefaultAsync(cancellationToken);

                if (advance == null)
                {
                    return ApiResponse<AdvanceDTO>.Success(null, "Avans isteği bulunamadı");
                }

                var advanceDTO = advance.Adapt<AdvanceDTO>();

                return ApiResponse<AdvanceDTO>.Success(advanceDTO, "Avans isteği başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvanceByIdAsync işleminde hata oluştu");
                return ApiResponse<AdvanceDTO>.Error("Avans isteği getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<AdvanceDTO>>> GetAdvancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var advances = await unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(x => x.Id != null)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null)
                {
                    return ApiResponse<IEnumerable<AdvanceDTO>>.Success(null, "Avans istekleri bulunamadı");
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ApiResponse<IEnumerable<AdvanceDTO>>.Success(advanceDTOs, "Avans istekleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvancesByUserIdAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<AdvanceDTO>>.Error("Avans istekleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<AdvanceDTO>>> GetAdvancesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advances = await unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(e => e.CreatedBy == userId)
                      .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null)
                {
                    return ApiResponse<IEnumerable<AdvanceDTO>>.Success(null, "Kullanıcının avans istekleri bulunamadı");
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ApiResponse<IEnumerable<AdvanceDTO>>.Success(advanceDTOs, "Kullanıcının avans istekleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvancesByUserIdAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<AdvanceDTO>>.Error("Kullanıcının avans istekleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<AdvanceDTO>>> GetChiefProcessedAdvancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                AdvanceStatus[] statuses = new AdvanceStatus[] { AdvanceStatus.ApprovedByChief, AdvanceStatus.ApprovedByUnitManager, AdvanceStatus.RejectedByUnitManager };
                var advances = await unitOfWork.GetRepository<IdtAdvance>()
                    .WhereAsNoTracking(x => statuses.Contains(x.Status))
                      .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null)
                {
                    return ApiResponse<IEnumerable<AdvanceDTO>>.Success(null, "Şef onaylı avans istekleri bulunamadı");
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ApiResponse<IEnumerable<AdvanceDTO>>.Success(advanceDTOs, "Şef onaylı avans istekleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetChiefProcessedAdvancesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<AdvanceDTO>>.Error("Şef onaylı avans istekleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> RejectAdvanceAsync(string advanceId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ApiResponse<bool>.Error("Avans isteği bulunamadı");
                }
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();
                switch (userRole)
                {
                    case "Admin":
                        advance.Status = AdvanceStatus.RejectedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        advance.UnitManagerRejectReason = rejectReason ?? null;
                        advance.ChiefRejectReason = rejectReason ?? null;
                        break;

                    case "Yönetici":
                        if (advance.Status == AdvanceStatus.RejectedByUnitManager)
                        {
                            return ApiResponse<bool>.Error("Avans isteği zaten birim yönetici tarafından reddedilmiş");
                        }
                        advance.Status = AdvanceStatus.RejectedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;
                        advance.UnitManagerRejectReason = rejectReason ?? null;
                        break;

                    case "Şef":
                        if (advance.Status == AdvanceStatus.RejectedByChief)
                        {
                            return ApiResponse<bool>.Error("Avans isteği zaten şef tarafından reddedilmiş");
                        }
                        advance.Status = AdvanceStatus.RejectedByChief;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        advance.ChiefRejectReason = rejectReason ?? null;
                        break;

                    default:
                        return ApiResponse<bool>.Error("Avans isteği reddetmek için yetkiniz yok");
                }
                unitOfWork.GetRepository<IdtAdvance>().Update(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var approvedAdvance = await unitOfWork.GetRepository<IdtAdvance>()
                .WhereAsNoTracking(e => e.Id == advance.Id)
                .Include(x => x.ChiefUser)
                .Include(x => x.UnitManagerUser)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(cancellationToken);
                var mappedAdvance = approvedAdvance.Adapt<AdvanceDTO>();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedAdvance.UpdatedByFullName} tarafından, {mappedAdvance.CreatedDate:dd/MM/yyyy} tarihinde oluşturduğunuz {mappedAdvance.Reason} avans isteğiniz reddedildi.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/avans/listem",
                    TargetUsers = new List<string> { mappedAdvance.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ApiResponse<bool>.Success(true, "Avans isteği başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RejectAdvanceAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Avans isteği reddedilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAdvanceAsync(string advanceId, UpdateAdvanceDTO updateAdvanceDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ApiResponse<bool>.Error("Avans isteği bulunamadı");
                }
                updateAdvanceDTO.Adapt(advance);

                unitOfWork.GetRepository<IdtAdvance>().Update(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ApiResponse<bool>.Success(true, "Avans isteği başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RejectAdvanceAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Avans isteği reddedilirken hata oluştu");
            }
        }
    }
}