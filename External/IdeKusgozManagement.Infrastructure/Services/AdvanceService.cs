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
        private const decimal approvalThreshold = 5000m;

        private BalanceType? GetBalanceType(IdtAdvance advance)
        {
            BalanceType? type = null;
            switch (advance.Reason.ToLower())
            {
                case "iş avansı":
                    type = BalanceType.Job;
                    break;

                case "maaş avansı":
                    type = BalanceType.Salary;
                    break;

                default:
                    logger.LogError("Bakiye tipi bulunamadı Reason: {Reason}", advance.Reason);
                    throw new ArgumentException($"{advance.Reason} bakiye tipi bulunamadı");
                    break;
            }
            return type.Value;
        }

        private async Task IncreaseBalanceAsync(IdtAdvance advance, CancellationToken cancellationToken)
        {
            var type = GetBalanceType(advance);

            var userBalance = await unitOfWork.GetRepository<IdtUserBalance>().Where(x => x.Type == type && x.UserId == advance.UserId).SingleOrDefaultAsync(cancellationToken);
            if (userBalance == null)
            {
                logger.LogError("Kullanıcı bakiyesi bulunamadı. Type: {Type}, UserId: {UserId}", type, advance.UserId);
                throw new ArgumentNullException($"Kullanıcı {type} bakiyesi bulunamadı");
            }
            userBalance.Balance += advance.Amount;
            unitOfWork.GetRepository<IdtUserBalance>().Update(userBalance);
        }

        private async Task DecraseBalanceAsync(IdtAdvance advance, CancellationToken cancellationToken)
        {
            var type = GetBalanceType(advance);
            var userBalance = await unitOfWork.GetRepository<IdtUserBalance>().Where(x => x.Type == type && x.UserId == advance.UserId).SingleOrDefaultAsync(cancellationToken);
            if (userBalance == null)
            {
                logger.LogError("Kullanıcı bakiyesi bulunamadı. Type: {Type}, UserId: {UserId}", type, advance.UserId);
                throw new ArgumentNullException($"Kullanıcı {type} bakiyesi bulunamadı");
            }
            userBalance.Balance -= advance.Amount;
            unitOfWork.GetRepository<IdtUserBalance>().Update(userBalance);
        }

        public async Task<ServiceResponse<bool>> ApproveAdvanceAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ServiceResponse<bool>.Error("Avans isteği bulunamadı");
                }
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();
                bool isOverThreshold = advance.Amount > approvalThreshold;

                switch (userRole)
                {
                    case "Admin":
                        // Admin her şeyi onaylayabilir
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            return ServiceResponse<bool>.Error("Avans isteği zaten onaylanmış");
                        }
                        advance.Status = AdvanceStatus.ApprovedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        break;

                    case "Yönetici":
                        // Yönetici zaten onaylanmış mı kontrol et
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            return ServiceResponse<bool>.Error("Avans isteği zaten yönetici tarafından onaylanmış");
                        }

                        // Yönetici her durumda onaylayabilir (şef onayını beklemez)
                        advance.Status = AdvanceStatus.ApprovedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;
                        break;

                    case "Şef":
                        // Şef 5000 TL üzeri göremez/onaylayamaz
                        if (isOverThreshold)
                        {
                            return ServiceResponse<bool>.Error("₺5000 üzerindeki avans istekleri sadece yönetici tarafından onaylanabilir");
                        }

                        // Şef zaten onaylanmış mı kontrol et
                        if (advance.Status == AdvanceStatus.ApprovedByChief)
                        {
                            return ServiceResponse<bool>.Error("Avans isteği zaten şef tarafından onaylanmış");
                        }

                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            return ServiceResponse<bool>.Error("Avans isteği zaten yönetici tarafından onaylanmış");
                        }

                        advance.Status = AdvanceStatus.ApprovedByChief;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        break;

                    default:
                        return ServiceResponse<bool>.Error("Avans isteğini onaylamak için yetkiniz yok");
                }

                // Bakiye sadece YÖNETİCİ ONAYINDA artırılır (her iki senaryo için de son adım)
                if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                {
                    await IncreaseBalanceAsync(advance, cancellationToken);
                }

                unitOfWork.GetRepository<IdtAdvance>().Update(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var approvedAdvance = await unitOfWork.GetRepository<IdtAdvance>()
                    .WhereAsNoTracking(e => e.Id == advance.Id)
                    .Include(x => x.User)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .FirstOrDefaultAsync(cancellationToken);
                var mappedAdvance = approvedAdvance.Adapt<AdvanceDTO>();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedAdvance.UpdatedByFullName} tarafından, {mappedAdvance.CreatedDate:dd/MM/yyyy} tarihinde oluşturduğunuz {mappedAdvance.Reason} avans isteğiniz onaylandı.",
                    Type = NotificationType.Advance,
                    RedirectUrl = "/avans/listem",
                    TargetUsers = new List<string> { advance.UserId }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ServiceResponse<bool>.Success(true, "Avans isteği başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "ApproveAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<string>> CreateAdvanceAsync(CreateAdvanceDTO createAdvanceDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var newAdvance = createAdvanceDTO.Adapt<IdtAdvance>();
                var currentUserId = identityService.GetUserId();

                // CreatedBy: Avansı oluşturan kişi (her zaman mevcut kullanıcı)
                newAdvance.CreatedBy = currentUserId;

                // UserId: Avansın sahibi (eğer belirtilmişse o kullanıcı, yoksa mevcut kullanıcı)
                if (!string.IsNullOrWhiteSpace(createAdvanceDTO.UserId))
                {
                    newAdvance.UserId = createAdvanceDTO.UserId;
                }
                else
                {
                    newAdvance.UserId = currentUserId;
                }

                await unitOfWork.GetRepository<IdtAdvance>().AddAsync(newAdvance, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var createdAdvance = await unitOfWork.GetRepository<IdtAdvance>()
                 .WhereAsNoTracking(e => e.Id == newAdvance.Id)
                 .Include(x => x.User)
                 .Include(x => x.ChiefUser)
                 .Include(x => x.UnitManagerUser)
                 .Include(x => x.CreatedByUser)
                 .Include(x => x.UpdatedByUser)
                 .FirstOrDefaultAsync(cancellationToken);
                var mappedAdvance = createdAdvance.Adapt<AdvanceDTO>();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedAdvance.CreatedByFullName} tarafından, {mappedAdvance.CreatedDate:dd/MM/yyyy} tarihinde {mappedAdvance.Reason} avans isteği oluşturuldu.",
                    Type = NotificationType.Advance,
                    RedirectUrl = "/avans",
                    TargetUsers = await identityService.GetUserSuperiorsAsync(cancellationToken)
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ServiceResponse<string>.Success(newAdvance.Id, "Avans isteği başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAdvanceAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);

                if (advance == null)
                {
                    return ServiceResponse<bool>.Error("Avans isteği bulunamadı");
                }

                var isAdvanceApproved = await unitOfWork.GetRepository<IdtAdvance>().AnyAsync(x => x.Status == AdvanceStatus.ApprovedByUnitManager && x.Id == advance.Id, cancellationToken);

                if (isAdvanceApproved)
                {
                    return ServiceResponse<bool>.Error("Bu avans isteği birim yönetici tarafından onaylandığı için silinemez");
                }

                unitOfWork.GetRepository<IdtAdvance>().Remove(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Avans isteği başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<AdvanceDTO>> GetAdvanceByIdAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(e => e.Id == advanceId)
                    .Include(x => x.User)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser).FirstOrDefaultAsync(cancellationToken);

                if (advance == null)
                {
                    return ServiceResponse<AdvanceDTO>.Success(null, "Avans isteği bulunamadı");
                }

                var advanceDTO = advance.Adapt<AdvanceDTO>();

                return ServiceResponse<AdvanceDTO>.Success(advanceDTO, "Avans isteği başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvanceByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<AdvanceDTO>>> GetAdvancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var advances = await unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(x => x.Id != null)
                    .Include(x => x.User)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null)
                {
                    return ServiceResponse<IEnumerable<AdvanceDTO>>.Success(null, "Avans istekleri bulunamadı");
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ServiceResponse<IEnumerable<AdvanceDTO>>.Success(advanceDTOs, "Avans istekleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvancesByUserIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<AdvanceDTO>>> GetAdvancesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advances = await unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(e => e.UserId == userId)
                      .Include(x => x.User)
                      .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null)
                {
                    return ServiceResponse<IEnumerable<AdvanceDTO>>.Success(null, "Kullanıcının avans istekleri bulunamadı");
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ServiceResponse<IEnumerable<AdvanceDTO>>.Success(advanceDTOs, "Kullanıcının avans istekleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvancesByUserIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<AdvanceDTO>>> GetChiefProcessedAdvancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                AdvanceStatus[] statuses = new AdvanceStatus[] { AdvanceStatus.ApprovedByChief, AdvanceStatus.ApprovedByUnitManager, AdvanceStatus.RejectedByUnitManager };
                var advances = await unitOfWork.GetRepository<IdtAdvance>()
                    .WhereAsNoTracking(x => statuses.Contains(x.Status))
                      .Include(x => x.User)
                      .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null)
                {
                    return ServiceResponse<IEnumerable<AdvanceDTO>>.Success(null, "Şef onaylı avans istekleri bulunamadı");
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ServiceResponse<IEnumerable<AdvanceDTO>>.Success(advanceDTOs, "Şef onaylı avans istekleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetChiefProcessedAdvancesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> RejectAdvanceAsync(string advanceId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ServiceResponse<bool>.Error("Avans isteği bulunamadı");
                }
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();
                bool isOverThreshold = advance.Amount > approvalThreshold;


                switch (userRole)
                {
                    case "Admin":
                        // Admin can reject any amount
                        if (advance.Status == AdvanceStatus.RejectedByUnitManager)
                        {
                            return ServiceResponse<bool>.Error("Avans isteği zaten reddedilmiş");
                        }
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            // Bakiye işlemleri
                            await DecraseBalanceAsync(advance, cancellationToken);
                        }
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
                            return ServiceResponse<bool>.Error("Avans isteği zaten reddedilmiş");
                        }
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            // Bakiye işlemleri
                            await DecraseBalanceAsync(advance, cancellationToken);
                        }
                        advance.Status = AdvanceStatus.RejectedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;
                        advance.UnitManagerRejectReason = rejectReason ?? null;
                        break;

                    case "Şef":
                        if (isOverThreshold)
                        {
                            return ServiceResponse<bool>.Error("₺5000 üzerindeki avans istekleri sadece yönetici tarafından reddedilebilir");
                        }
                        if (advance.Status == AdvanceStatus.RejectedByChief)
                        {
                            return ServiceResponse<bool>.Error("Avans isteği zaten şef tarafından reddedilmiş");
                        }
                        if (advance.Status == AdvanceStatus.RejectedByUnitManager)
                        {
                            return ServiceResponse<bool>.Error("Avans isteği zaten yönetici tarafından reddedilmiş");
                        }
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            return ServiceResponse<bool>.Error("Avans isteği yönetici tarafından onaylanmış, reddedemezsiniz");
                        }

                        advance.Status = AdvanceStatus.RejectedByChief;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        advance.ChiefRejectReason = rejectReason ?? null;
                        break;

                    default:
                        return ServiceResponse<bool>.Error("Avans isteği reddetmek için yetkiniz yok");
                }

                unitOfWork.GetRepository<IdtAdvance>().Update(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var approvedAdvance = await unitOfWork.GetRepository<IdtAdvance>()
                .WhereAsNoTracking(e => e.Id == advance.Id)
                .Include(x => x.User)
                .Include(x => x.ChiefUser)
                .Include(x => x.UnitManagerUser)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(cancellationToken);
                var mappedAdvance = approvedAdvance.Adapt<AdvanceDTO>();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedAdvance.UpdatedByFullName} tarafından, {mappedAdvance.CreatedDate:dd/MM/yyyy} tarihinde oluşturduğunuz {mappedAdvance.Reason} avans isteğiniz reddedildi.",
                    Type = NotificationType.Advance,
                    RedirectUrl = "/avans/listem",
                    TargetUsers = new List<string> { advance.UserId }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ServiceResponse<bool>.Success(true, "Avans isteği başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "RejectAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAdvanceAsync(string advanceId, UpdateAdvanceDTO updateAdvanceDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ServiceResponse<bool>.Error("Avans isteği bulunamadı");
                }
                updateAdvanceDTO.Adapt(advance);

                unitOfWork.GetRepository<IdtAdvance>().Update(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResponse<bool>.Success(true, "Avans isteği başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RejectAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}