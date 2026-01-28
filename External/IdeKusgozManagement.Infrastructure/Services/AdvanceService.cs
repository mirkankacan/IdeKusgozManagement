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
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class AdvanceService(IUnitOfWork unitOfWork, ILogger<AdvanceService> logger, IIdentityService identityService, INotificationService notificationService, IParameterService parameterService) : IAdvanceService
    {
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

        private async Task IncreaseBalanceByPartAsync(IdtAdvance advance, decimal amount, CancellationToken cancellationToken)
        {
            var type = GetBalanceType(advance);

            var userBalance = await unitOfWork.GetRepository<IdtUserBalance>().Where(x => x.Type == type && x.UserId == advance.UserId).SingleOrDefaultAsync(cancellationToken);
            if (userBalance == null)
            {
                logger.LogError("Kullanıcı bakiyesi bulunamadı. Type: {Type}, UserId: {UserId}", type, advance.UserId);
                throw new ArgumentNullException($"Kullanıcı {type} bakiyesi bulunamadı");
            }
            userBalance.Balance += amount;
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

        public async Task<ServiceResult<bool>> ApproveAdvanceAsync(string advanceId, ApproveAdvanceDTO? approveAdvanceDTO = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var paramServiceResult = await parameterService.GetParameterByKeyAsync("ChiefAdvanceApprovalThreshold", cancellationToken);
                if (!paramServiceResult.IsSuccess)
                    return ServiceResult<bool>.Error("Parametre Hata", "Avans onaylama sınır parametresi alınamadı.", HttpStatusCode.NotFound);

                decimal approvalThreshold = decimal.Parse(paramServiceResult.Data.Value);

                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ServiceResult<bool>.Error("Avans İsteği Bulunamadı", "Belirtilen ID'ye sahip avans isteği bulunamadı.", HttpStatusCode.NotFound);
                }

                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();
                bool isOverThreshold = advance.Amount > approvalThreshold;

                // Parça validasyonu - parçalar UI'dan her zaman gönderilmelidir
                if (approveAdvanceDTO == null || approveAdvanceDTO.Parts == null || !approveAdvanceDTO.Parts.Any())
                {
                    return ServiceResult<bool>.Error("Parça Validasyon Hatası", "Avans onayı için en az bir parça belirtilmelidir.", HttpStatusCode.BadRequest);
                }

                var totalPartsAmount = approveAdvanceDTO.Parts.Sum(p => p.Amount);
                if (totalPartsAmount != advance.Amount)
                {
                    return ServiceResult<bool>.Error("Parça Tutar Uyumsuzluğu", $"Parça tutarlarının toplamı ({totalPartsAmount}) avans tutarına ({advance.Amount}) eşit olmalıdır.", HttpStatusCode.BadRequest);
                }

                // Rol kontrolü ve status güncelleme
                switch (userRole)
                {
                    case "Admin":
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Onaylanmış", "Bu avans isteği daha önce onaylanmış durumda.", HttpStatusCode.BadRequest);
                        }
                        advance.Status = AdvanceStatus.ApprovedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        break;

                    case "Yönetici":
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Onaylanmış", "Bu avans isteği daha önce yönetici tarafından onaylanmış durumda.", HttpStatusCode.BadRequest);
                        }

                        advance.Status = AdvanceStatus.ApprovedByUnitManager;
                        advance.ProcessedByUnitManagerId = userId;
                        advance.UnitManagerProcessedDate = DateTime.Now;
                        break;

                    case "Şef":
                        if (isOverThreshold)
                        {
                            return ServiceResult<bool>.Error("Yetki Hatası", $"₺{approvalThreshold} üzerindeki avans istekleri sadece yönetici tarafından onaylanabilir.", HttpStatusCode.Forbidden);
                        }
                        if (advance.Status == AdvanceStatus.ApprovedByChief)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Onaylanmış", "Bu avans isteği daha önce şef tarafından onaylanmış durumda.", HttpStatusCode.BadRequest);
                        }
                        if (advance.Status == AdvanceStatus.ApprovedByUnitManager)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Onaylanmış", "Bu avans isteği daha önce yönetici tarafından onaylanmış durumda.", HttpStatusCode.BadRequest);
                        }
                        advance.Status = AdvanceStatus.ApprovedByChief;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        break;

                    default:
                        return ServiceResult<bool>.Error("Yetki Hatası", "Avans isteğini onaylamak için yeterli yetkiniz bulunmamaktadır.", HttpStatusCode.Forbidden);
                }
                var advanceParts = new List<IdtAdvancePart>();
                foreach (var partDto in approveAdvanceDTO.Parts)
                {
                    // Tarih validasyonu
                    try
                    {
                        var approvalDate = new DateTime(partDto.Year, partDto.Month, partDto.Day);
                        var advancePart = new IdtAdvancePart
                        {
                            AdvanceId = advanceId,
                            Amount = partDto.Amount,
                            ApprovalDate = approvalDate,
                            ApprovedById = userId,
                            ApprovedDate = DateTime.Now
                        };
                        advanceParts.Add(advancePart);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return ServiceResult<bool>.Error("Geçersiz Tarih", $"Belirtilen tarih geçersizdir: {partDto.Day}/{partDto.Month}/{partDto.Year}", HttpStatusCode.BadRequest);
                    }
                }

                // Parçaları veritabanına kaydet - HER ZAMAN en az 1 parça kaydedilecek
                // (Eğer bölünmemişse 1 parça, bölünmüşse n parça)
                await unitOfWork.GetRepository<IdtAdvancePart>().AddRangeAsync(advanceParts, cancellationToken);
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
                    .Include(x => x.AdvanceParts)
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
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "ApproveAdvanceAsync (with parts) işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<string>> CreateAdvanceAsync(CreateAdvanceDTO createAdvanceDTO, CancellationToken cancellationToken = default)
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
                return ServiceResult<string>.SuccessAsCreated(newAdvance.Id, $"/api/advances/{newAdvance.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteAdvanceAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);

                if (advance == null)
                {
                    return ServiceResult<bool>.Error("Avans İsteği Bulunamadı", "Belirtilen ID'ye sahip avans isteği bulunamadı.", HttpStatusCode.NotFound);
                }

                var isAdvanceApproved = await unitOfWork.GetRepository<IdtAdvance>().AnyAsync(x => x.Status == AdvanceStatus.ApprovedByUnitManager && x.Id == advance.Id, cancellationToken);

                if (isAdvanceApproved)
                {
                    return ServiceResult<bool>.Error("Silme İşlemi Başarısız", "Bu avans isteği birim yönetici tarafından onaylandığı için silinemez.", HttpStatusCode.BadRequest);
                }

                unitOfWork.GetRepository<IdtAdvance>().Remove(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<AdvanceDTO>> GetAdvanceByIdAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(e => e.Id == advanceId)
                    .Include(x => x.User)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.AdvanceParts)
                    .FirstOrDefaultAsync(cancellationToken);

                if (advance == null)
                {
                    return ServiceResult<AdvanceDTO>.Error("Avans İsteği Bulunamadı", "Belirtilen ID'ye sahip avans isteği bulunamadı.", HttpStatusCode.NotFound);
                }

                var advanceDTO = advance.Adapt<AdvanceDTO>();

                return ServiceResult<AdvanceDTO>.SuccessAsOk(advanceDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvanceByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<AdvanceDTO>>> GetAdvancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var paramServiceResult = await parameterService.GetParameterByKeyAsync("ChiefAdvanceApprovalThreshold", cancellationToken);
                if (!paramServiceResult.IsSuccess)
                    return ServiceResult<IEnumerable<AdvanceDTO>>.Error("Parametre Hata", "Avans onaylama sınır parametresi alınamadı.", HttpStatusCode.NotFound);

                decimal approvalThreshold = decimal.Parse(paramServiceResult.Data.Value);

                var userRole = identityService.GetUserRole();
                var userId = identityService.GetUserId();

                var query = unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(x => x.Id != null);

                // Şef sadece approvalThreshold ve altı, bekleyen avansları görebilir
                if (userRole == "Şef")
                {
                    var parameterLastModified = (paramServiceResult.Data.UpdatedDate ?? paramServiceResult.Data.CreatedDate).Date;

                    query = query.Where(x =>
                        x.Amount <= approvalThreshold &&
                        (
                            x.ProcessedByChiefId == userId || // Daha önce bu şef işlem yaptıysa
                            (x.UpdatedDate ?? x.CreatedDate) >= parameterLastModified // VEYA yeni tarihli avans ise
                        )
                    );
                }
                // Admin ve diğer roller tüm avansları görebilir

                var advances = await query
                    .Include(x => x.User)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.AdvanceParts)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null || !advances.Any())
                {
                    return ServiceResult<IEnumerable<AdvanceDTO>>.SuccessAsOk(Enumerable.Empty<AdvanceDTO>());
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ServiceResult<IEnumerable<AdvanceDTO>>.SuccessAsOk(advanceDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvancesByUserIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<AdvanceDTO>>> GetAdvancesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var advances = await unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(e => e.UserId == userId)
                      .Include(x => x.User)
                      .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.AdvanceParts)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null || !advances.Any())
                {
                    return ServiceResult<IEnumerable<AdvanceDTO>>.SuccessAsOk(Enumerable.Empty<AdvanceDTO>());
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ServiceResult<IEnumerable<AdvanceDTO>>.SuccessAsOk(advanceDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvancesByUserIdAsync işleminde hata oluştu");
                throw;
            }
        }

        // AdvanceService.cs içine ekle
        public async Task<ServiceResult<AdvanceStatisticDTO>> GetAdvanceStatisticsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userRole = identityService.GetUserRole();
                var query = unitOfWork.GetRepository<IdtAdvance>().WhereAsNoTracking(x => x.Id != null);

                var advances = await query.ToListAsync(cancellationToken);

                var pendingCount = advances.Count(x => x.Status == AdvanceStatus.Pending);
                var approvedCount = advances.Count(x => x.Status == AdvanceStatus.ApprovedByChief ||
                                                          x.Status == AdvanceStatus.ApprovedByUnitManager);
                var rejectedCount = advances.Count(x => x.Status == AdvanceStatus.RejectedByChief ||
                                                          x.Status == AdvanceStatus.RejectedByUnitManager);
                var completedCount = advances.Count(x => x.Status == AdvanceStatus.Completed);
                var totalCount = advances.Count;

                var statistics = new AdvanceStatisticDTO
                {
                    PendingCount = pendingCount,
                    ApprovedCount = approvedCount,
                    RejectedCount = rejectedCount,
                    CompletedCount = completedCount,
                    TotalCount = totalCount,
                    PendingPercentage = totalCount > 0 ? Math.Round((decimal)pendingCount / totalCount * 100, 2) : 0,
                    ApprovedPercentage = totalCount > 0 ? Math.Round((decimal)approvedCount / totalCount * 100, 2) : 0,
                    RejectedPercentage = totalCount > 0 ? Math.Round((decimal)rejectedCount / totalCount * 100, 2) : 0,
                    CompletedPercentage = totalCount > 0 ? Math.Round((decimal)completedCount / totalCount * 100, 2) : 0
                };

                return ServiceResult<AdvanceStatisticDTO>.SuccessAsOk(statistics);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAdvanceStatisticsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<AdvanceDTO>>> GetApprovedAdvancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var advances = await unitOfWork.GetRepository<IdtAdvance>()
                    .WhereAsNoTracking(x => x.Status == AdvanceStatus.ApprovedByUnitManager || x.Status == AdvanceStatus.ApprovedByChief)
                      .Include(x => x.User)
                      .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.AdvanceParts)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null || !advances.Any())
                {
                    return ServiceResult<IEnumerable<AdvanceDTO>>.SuccessAsOk(Enumerable.Empty<AdvanceDTO>());
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ServiceResult<IEnumerable<AdvanceDTO>>.SuccessAsOk(advanceDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetApprovedAdvancesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<AdvanceDTO>>> GetCompletedAdvancesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var advances = await unitOfWork.GetRepository<IdtAdvance>()
                    .WhereAsNoTracking(x => x.Status == AdvanceStatus.Completed)
                      .Include(x => x.User)
                      .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.CompletedByUser)
                    .Include(x => x.AdvanceParts)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (advances == null || !advances.Any())
                {
                    return ServiceResult<IEnumerable<AdvanceDTO>>.SuccessAsOk(Enumerable.Empty<AdvanceDTO>());
                }

                var advanceDTOs = advances.Adapt<IEnumerable<AdvanceDTO>>();

                return ServiceResult<IEnumerable<AdvanceDTO>>.SuccessAsOk(advanceDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompletedAdvancesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> RejectAdvanceAsync(string advanceId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var paramServiceResult = await parameterService.GetParameterByKeyAsync("ChiefAdvanceApprovalThreshold", cancellationToken);
                if (!paramServiceResult.IsSuccess)
                    return ServiceResult<bool>.Error("Parametre Hata", "Avans onaylama sınır parametresi alınamadı.", HttpStatusCode.NotFound);

                decimal approvalThreshold = decimal.Parse(paramServiceResult.Data.Value);
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ServiceResult<bool>.Error("Avans İsteği Bulunamadı", "Belirtilen ID'ye sahip avans isteği bulunamadı.", HttpStatusCode.NotFound);
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
                            return ServiceResult<bool>.Error("Avans Zaten Reddedilmiş", "Bu avans isteği daha önce reddedilmiş durumda.", HttpStatusCode.BadRequest);
                        }
                        if (advance.Status == AdvanceStatus.RejectedByFinance)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Reddedilmiş", "Bu avans isteği daha önce finans tarafından reddedilmiş durumda.", HttpStatusCode.BadRequest);
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
                            return ServiceResult<bool>.Error("Avans Zaten Reddedilmiş", "Bu avans isteği daha önce reddedilmiş durumda.", HttpStatusCode.BadRequest);
                        }
                        if (advance.Status == AdvanceStatus.RejectedByFinance)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Reddedilmiş", "Bu avans isteği daha önce finans tarafından reddedilmiş durumda.", HttpStatusCode.BadRequest);
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
                            return ServiceResult<bool>.Error("Yetki Hatası", $"₺{approvalThreshold} üzerindeki avans istekleri sadece yönetici tarafından reddedilebilir.", HttpStatusCode.Forbidden);
                        }
                        if (advance.Status == AdvanceStatus.RejectedByChief)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Reddedilmiş", "Bu avans isteği daha önce şef tarafından reddedilmiş durumda.", HttpStatusCode.BadRequest);
                        }
                        if (advance.Status == AdvanceStatus.RejectedByUnitManager)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Reddedilmiş", "Bu avans isteği daha önce yönetici tarafından reddedilmiş durumda.", HttpStatusCode.BadRequest);
                        }
                        if (advance.Status == AdvanceStatus.RejectedByFinance)
                        {
                            return ServiceResult<bool>.Error("Avans Zaten Reddedilmiş", "Bu avans isteği daha önce finans tarafından reddedilmiş durumda.", HttpStatusCode.BadRequest);
                        }

                        advance.Status = AdvanceStatus.RejectedByChief;
                        advance.ProcessedByChiefId = userId;
                        advance.ChiefProcessedDate = DateTime.Now;
                        advance.ChiefRejectReason = rejectReason ?? null;
                        break;

                    default:
                        return ServiceResult<bool>.Error("Yetki Hatası", "Avans isteği reddetmek için yeterli yetkiniz bulunmamaktadır.", HttpStatusCode.Forbidden);
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
                .Include(x => x.AdvanceParts)
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
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "RejectAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateAdvanceAsync(string advanceId, UpdateAdvanceDTO updateAdvanceDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ServiceResult<bool>.Error("Avans İsteği Bulunamadı", "Belirtilen ID'ye sahip avans isteği bulunamadı.", HttpStatusCode.NotFound);
                }
                updateAdvanceDTO.Adapt(advance);

                unitOfWork.GetRepository<IdtAdvance>().Update(advance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RejectAdvanceAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> CompleteAdvanceAsync(string advanceId, ApproveAdvanceDTO? approveAdvanceDTO = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                var userRole = identityService.GetUserRole();
                var userDepartmentDuty = identityService.GetUserDepartmentDuty();
                bool isAuthorized = userRole == "Admin" ||
                             userRole == "Yönetici" ||
                             userDepartmentDuty == "Muhasebe Meslek Elemanı" ||
                             userDepartmentDuty == "Muhasebe Müdürü" ||
                             userDepartmentDuty == "Finans Uzmanı";
                if (!isAuthorized)
                {
                    return ServiceResult<bool>.Error("Yetki Hatası", "Avans isteğini tamamlamak için yeterli yetkiniz bulunmamaktadır.", HttpStatusCode.Forbidden);
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var advance = await unitOfWork.GetRepository<IdtAdvance>().GetByIdAsync(advanceId, cancellationToken);
                if (advance == null)
                {
                    return ServiceResult<bool>.Error("Avans İsteği Bulunamadı", "Belirtilen ID'ye sahip avans isteği bulunamadı.", HttpStatusCode.NotFound);
                }

                // Parça validasyonu - parçalar UI'dan her zaman gönderilmelidir
                if (approveAdvanceDTO == null || approveAdvanceDTO.Parts == null || !approveAdvanceDTO.Parts.Any())
                {
                    return ServiceResult<bool>.Error("Parça Validasyon Hatası", "Avans onayı için en az bir parça belirtilmelidir.", HttpStatusCode.BadRequest);
                }

                var totalPartsAmount = approveAdvanceDTO.Parts.Sum(p => p.Amount);
                if (totalPartsAmount != advance.Amount)
                {
                    return ServiceResult<bool>.Error("Parça Tutar Uyumsuzluğu", $"Parça tutarlarının toplamı ({totalPartsAmount}) avans tutarına ({advance.Amount}) eşit olmalıdır.", HttpStatusCode.BadRequest);
                }

                if (advance.Status == AdvanceStatus.Completed)
                {
                    return ServiceResult<bool>.Error("Avans Zaten Tamamlanmış", "Bu avans isteği daha önce tamamlanmış durumda.", HttpStatusCode.BadRequest);
                }
                DateTime now = DateTime.Now;
                advance.Status = AdvanceStatus.Completed;
                advance.CompletedDate = now;
                advance.CompletedById = userId;
                var advanceParts = await unitOfWork.GetRepository<IdtAdvancePart>().Where(x => x.AdvanceId == advance.Id).ToListAsync(cancellationToken);

                foreach (var part in advanceParts)
                {
                    part.CompletedById = userId;
                    part.CompletedDate = now;
                    await IncreaseBalanceByPartAsync(advance, part.Amount, cancellationToken);
                }
                unitOfWork.GetRepository<IdtAdvance>().Update(advance);
                unitOfWork.GetRepository<IdtAdvancePart>().UpdateRange(advanceParts);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var approvedAdvance = await unitOfWork.GetRepository<IdtAdvance>()
                    .WhereAsNoTracking(e => e.Id == advance.Id)
                    .Include(x => x.User)
                    .Include(x => x.ChiefUser)
                    .Include(x => x.UnitManagerUser)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.AdvanceParts)
                    .FirstOrDefaultAsync(cancellationToken);
                var mappedAdvance = approvedAdvance.Adapt<AdvanceDTO>();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedAdvance.UpdatedByFullName} tarafından, {mappedAdvance.CreatedDate:dd/MM/yyyy} tarihinde oluşturduğunuz {mappedAdvance.Reason} avans isteğiniz tamamlandı olarak güncellendi.",
                    Type = NotificationType.Advance,
                    RedirectUrl = "/avans/listem",
                    TargetUsers = new List<string> { advance.UserId }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "ApproveAdvanceAsync (with parts) işleminde hata oluştu");
                throw;
            }
        }
    }
}