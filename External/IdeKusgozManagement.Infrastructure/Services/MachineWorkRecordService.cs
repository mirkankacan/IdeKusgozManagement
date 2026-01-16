using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.MachineWorkRecordDTOs;
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
    public class MachineWorkRecordService(IUnitOfWork unitOfWork, IIdentityService identityService, ILogger<WorkRecordService> logger, INotificationService notificationService) : IMachineWorkRecordService
    {
        private WorkRecordStatus? GetApproveStatusByRole()
        {
            var role = identityService.GetUserRole();
            WorkRecordStatus? status = null;
            switch (role)
            {
                case "Admin":
                case "Yönetici":
                    status = WorkRecordStatus.ApprovedByUnitManager;
                    break;

                case "Şef":
                    status = WorkRecordStatus.ApprovedByChief;
                    break;

                default:
                    throw new UnauthorizedAccessException($"{role} rolü için bu işlemi yapmaya yetkiniz yok");
                    break;
            }

            return status.Value;
        }

        private WorkRecordStatus? GetRejectStatusByRole()
        {
            var role = identityService.GetUserRole();
            WorkRecordStatus? status = null;
            switch (role)
            {
                case "Admin":
                case "Yönetici":
                    status = WorkRecordStatus.RejectedByUnitManager;
                    break;

                case "Şef":
                    status = WorkRecordStatus.RejectedByChief;
                    break;

                default:
                    throw new UnauthorizedAccessException($"{role} rolü için bu işlemi yapmaya yetkiniz yok");
                    break;
            }

            return status.Value;
        }

        public async Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> GetMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecords = await unitOfWork.GetRepository<IdtMachineWorkRecord>().WhereAsNoTracking(wr => wr.Date.Year == date.Date.Year && wr.Date.Month == date.Date.Month && wr.CreatedBy == userId)
                    .Include(x => x.Equipment)
                    .Include(x => x.Project)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .ToListAsync(cancellationToken);

                var workRecordDTOs = workRecords.Adapt<IEnumerable<MachineWorkRecordDTO>>();
                return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetMachineWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}", date, userId);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> BatchApproveMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecords = await unitOfWork.GetRepository<IdtMachineWorkRecord>()
                    .Where(x => x.CreatedBy == userId && x.Date.Year == date.Year && x.Date.Month == date.Month && x.Status != WorkRecordStatus.ApprovedByUnitManager)
                     .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .ToListAsync(cancellationToken);

                if (!workRecords.Any())
                {
                    return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Error("Belirtilen tarih aralığında puantaj kaydı bulunamadı ya da hepsi yönetici tarafından onaylanmış");
                }

                // Kayıtları onaylı duruma getir
                var status = GetApproveStatusByRole().Value;
                foreach (var workRecord in workRecords)
                {
                    workRecord.Status = status;
                }

                // Sadece YENİ onaylanan kayıtların masraflarını hesapla
                var workRecordIdsToApprove = workRecords.Select(x => x.Id).ToList();

                unitOfWork.GetRepository<IdtMachineWorkRecord>().UpdateRange(workRecords);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.LogInformation("{UserId} kullanıcısı {Year}/{Month} ayı için {ApproverUserId} tarafından toplam {Count} makine puantajı toplu olarak onayladı", userId, date.Year, date.Month, identityService.GetUserId(), workRecords.Count());

                var mappedWorkRecords = workRecords.Adapt<IEnumerable<MachineWorkRecordDTO>>();
                var firstRecord = mappedWorkRecords.First();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{firstRecord.UpdatedByFullName} tarafından, {firstRecord.Date:MM/yyyy} ayı için makine puantaj kayıtlarınız onaylandı.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/makine-puantaj/ekle",
                    TargetUsers = new List<string> { firstRecord.CreatedBy }
                };

                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);

                return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Success(mappedWorkRecords,
                    $"{workRecords.Count} adet makine puantaj kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchApproveWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. UserId: {UserId}, Month: {Month}, Year: {Year}",
                    userId, date.Month, date.Year);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> BatchRejectMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecords = await unitOfWork.GetRepository<IdtMachineWorkRecord>().Where(x => x.CreatedBy == userId && x.Date.Year == date.Year && x.Date.Month == date.Month)
                     .Include(x => x.CreatedByUser)
                  .Include(x => x.UpdatedByUser)
                  .ToListAsync(cancellationToken);

                if (!workRecords.Any())
                {
                    return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Error("Belirtilen tarih aralığında puantaj kaydı bulunamadı");
                }
                var status = GetRejectStatusByRole().Value;
                foreach (var workRecord in workRecords)
                {
                    workRecord.Status = status;
                    workRecord.RejectReason = !string.IsNullOrEmpty(rejectReason) ? rejectReason : workRecord.RejectReason;
                }

                unitOfWork.GetRepository<IdtMachineWorkRecord>().UpdateRange(workRecords);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.LogInformation("{UserId} kullanıcısı {Year}/{Month} ayı için {ApproverUserId} tarafından toplam {Count} makine puantajı toplu olarak reddedildi", userId, date.Year, date.Month, identityService.GetUserId(), workRecords.Count());

                var mappedWorkRecords = workRecords.Adapt<IEnumerable<MachineWorkRecordDTO>>();

                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecords.First().UpdatedByFullName} tarafından, {mappedWorkRecords.First().Date:MM/yyyy} ayı için makine puantajı kayıtlarınız reddedildi.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/makine-puantaj/ekle",
                    TargetUsers = new List<string> { mappedWorkRecords.First().CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);

                return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Success(mappedWorkRecords, $"{workRecords.Count} adet makine puantajı kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchApproveWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. UserId: {UserId},  Month: {Month}, Year: {Year}", userId, date.Date.Month, date.Date.Year);
                throw;
            }
        }

        public async Task<ServiceResponse<MachineWorkRecordDTO>> ApproveMachineWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecord = await unitOfWork.GetRepository<IdtMachineWorkRecord>().GetByIdAsync(id, cancellationToken);

                if (workRecord == null)
                {
                    return ServiceResponse<MachineWorkRecordDTO>.Error("Belirtilen puantaj kaydı bulunamadı");
                }

                workRecord.Status = GetApproveStatusByRole().Value;

                unitOfWork.GetRepository<IdtMachineWorkRecord>().Update(workRecord);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.LogInformation("{UserId} kullanıcısına ait {MachineWorkRecordId} ID'li makine puantaj kaydı {ApproverUserId} tarafından onaylandı", workRecord.CreatedBy, id, identityService.GetUserId());

                var approvedWorkRecord = await unitOfWork.GetRepository<IdtMachineWorkRecord>()
                  .WhereAsNoTracking(x => x.Id == id)
                  .Include(x => x.CreatedByUser)
                  .Include(x => x.UpdatedByUser)
                  //.Include(x => x.Equipment)
                  //.Include(x => x.Project)
                  .FirstOrDefaultAsync(cancellationToken);
                var mappedWorkRecord = approvedWorkRecord.Adapt<MachineWorkRecordDTO>();

                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecord.UpdatedByFullName} tarafından, {mappedWorkRecord.Date:dd/MM/yyyy} tarihi için makine puantaj kaydınız onaylandı.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/makine-puantaj/ekle",
                    TargetUsers = new List<string> { mappedWorkRecord.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);

                return ServiceResponse<MachineWorkRecordDTO>.Success(mappedWorkRecord, $"Makine puantaj kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "ApproveMachineWorkRecordByIdAsync işleminde hata oluştu. MachineWorkRecordId: {MachineWorkRecordId}", id);
                throw;
            }
        }

        public async Task<ServiceResponse<MachineWorkRecordDTO>> RejectMachineWorkRecordByIdAsync(string id, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecord = await unitOfWork.GetRepository<IdtMachineWorkRecord>().GetByIdAsync(id, cancellationToken);

                if (workRecord == null)
                {
                    return ServiceResponse<MachineWorkRecordDTO>.Error(message: "Belirtilen makine puantaj kaydı bulunamadı");
                }

                workRecord.Status = GetRejectStatusByRole().Value;
                workRecord.RejectReason = !string.IsNullOrEmpty(rejectReason) ? rejectReason : workRecord.RejectReason;
                unitOfWork.GetRepository<IdtMachineWorkRecord>().Update(workRecord);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.LogInformation("{UserId} kullanıcısına ait {MachineWorkRecordId} ID'li puantaj kaydı {ApproverUserId} tarafından reddedildi", workRecord.CreatedBy, id, identityService.GetUserId());

                var rejectedWorkRecord = await unitOfWork.GetRepository<IdtMachineWorkRecord>()
                 .WhereAsNoTracking(x => x.Id == id)
                 .Include(x => x.CreatedByUser)
                 .Include(x => x.UpdatedByUser)
                 //.Include(x => x.Equipment)
                 //.Include(x => x.Project)
                 //.Include(x => x.WorkRecordExpenses)
                 //    .ThenInclude(x => x.Expense)
                 //.Include(x => x.WorkRecordExpenses)
                 //    .ThenInclude(x => x.File)
                 .FirstOrDefaultAsync(cancellationToken);
                var mappedWorkRecord = rejectedWorkRecord.Adapt<MachineWorkRecordDTO>();

                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecord.UpdatedByFullName} tarafından, {mappedWorkRecord.Date:dd/MM/yyyy} tarihi için makine puantajı kaydınız reddedildi.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/makine-puantaj/ekle",
                    TargetUsers = new List<string> { mappedWorkRecord.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ServiceResponse<MachineWorkRecordDTO>.Success(mappedWorkRecord, $"Makine puantaj kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "RejectMachineWorkRecordByIdAsync işleminde hata oluştu. MachineWorkRecordId: {MachineWorkRecordId}", id);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> BatchUpdateMachineWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyMachineWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var dates = workRecordDTOs.Select(x => x.Date.Date).Distinct().ToList();
                var existingWorkRecords = await unitOfWork.GetRepository<IdtMachineWorkRecord>().Where(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId).ToListAsync(cancellationToken);

                var recordsToUpdate = new List<IdtMachineWorkRecord>();
                var changedFieldsSet = new HashSet<string>();

                foreach (var element in workRecordDTOs)
                {
                    // Saat kontrolü
                    var check = CheckHoursIfValid(element);
                    if (!check.Item1)
                        return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Error(check.Item2 ?? "Saat kontrolü başarısız");

                    // İlgili tarihin mevcut kaydı
                    var existingWorkRecord = existingWorkRecords.FirstOrDefault(x => x.Date.Date == element.Date.Date);

                    // ========== VAROLAN GÜNCELLEME ==========
                    if (existingWorkRecord is not null)
                    {
                        if (existingWorkRecord.Status == WorkRecordStatus.ApprovedByUnitManager)
                            continue;

                        var changedFields = GetChangedFields(existingWorkRecord, element);
                        if (changedFields.Any())
                        {
                            changedFieldsSet.UnionWith(changedFields);
                            UpdateWorkRecordFields(existingWorkRecord, element);
                            recordsToUpdate.Add(existingWorkRecord);
                        }
                    }
                    // ========== KAYIT YOK ==========
                    else
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Error($"{element.Date.Date} tarihli makine puantaj kaydı veritabanında bulunamadı");
                    }
                }

                if (recordsToUpdate.Any())
                {
                    unitOfWork.GetRepository<IdtMachineWorkRecord>().UpdateRange(recordsToUpdate);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var updatedCount = recordsToUpdate.Count();

                logger.LogInformation("{UserId} kullanıcısının {Year}/{Month} ayı için {UpdaterUser} tarafından {UpdatedCount} tane makine puantajı güncellendi", userId, workRecordDTOs.First().Date.Year, workRecordDTOs.First().Date.Month, identityService.GetUserId(), updatedCount);

                var mappedRecords = await GetFinalWorkRecords(dates, userId, cancellationToken);

                // Bildirim gönder
                if (updatedCount > 0)
                {
                    var firstRecord = mappedRecords.First();
                    string? changedFieldsText = changedFieldsSet.Any() ? $" Değişen alanlar: {string.Join(", ", changedFieldsSet)}." : "";
                    var message = $"{firstRecord.UpdatedByFullName} tarafından {firstRecord.Date:MM/yyyy} ayı için makine puantaj kayıt(lar)ı güncellendi. {updatedCount} kayıt güncellendi";

                    message += changedFieldsText;

                    var notificationDTO = new CreateNotificationDTO
                    {
                        Message = message,
                        Type = NotificationType.WorkRecord,
                        RedirectUrl = "/makine-puantaj/ekle",
                        TargetUsers = new List<string> { firstRecord.CreatedBy }
                    };
                    await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                }

                return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Success(mappedRecords, $"Makine puantaj kayıtları işlendi. {updatedCount} kayıt güncellendi.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchUpdateMachineWorkRecordsByUserIdAsync işleminde hata oluştu.");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> BatchCreateOrModifyMachineWorkRecordsAsync(IEnumerable<CreateOrModifyMachineWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var dates = workRecordDTOs.Select(x => x.Date.Date).Distinct().ToList();
                var existingWorkRecords = await unitOfWork.GetRepository<IdtMachineWorkRecord>()
                    .Where(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId)
                    .ToListAsync(cancellationToken);

                var recordsToAdd = new List<IdtMachineWorkRecord>();
                var recordsToUpdate = new List<IdtMachineWorkRecord>();

                foreach (var element in workRecordDTOs)
                {

                    // İlgili tarihin mevcut kayıtları (aynı günde birden fazla kayıt olabilir)
                    // Önce aynı tarih, saat ve diğer özelliklerle eşleşen kayıt var mı kontrol et
                    var existingWorkRecord = existingWorkRecords
                        .FirstOrDefault(x => x.Date.Date == element.Date.Date &&
                            x.StartTime == element.StartTime &&
                            x.EndTime == element.EndTime &&
                            x.ProjectId == element.ProjectId &&
                            x.EquipmentId == element.EquipmentId &&
                            x.Province == element.Province &&
                            x.District == element.District);

                    // ========== VAROLAN GÜNCELLEME ==========
                    if (existingWorkRecord is not null)
                    {
                        if (existingWorkRecord.Status is WorkRecordStatus.ApprovedByUnitManager or WorkRecordStatus.ApprovedByChief)
                            continue;

                        var changedFields = GetChangedFields(existingWorkRecord, element);

                        if (changedFields.Any())
                        {
                            UpdateWorkRecordFields(existingWorkRecord, element);
                            recordsToUpdate.Add(existingWorkRecord);
                            // Bu kaydı listeden çıkar ki tekrar kullanılmasın
                            existingWorkRecords.Remove(existingWorkRecord);
                        }
                    }
                    // ========== YENİ KAYIT ==========
                    else
                    {
                        var newRecord = CreateNewMachineWorkRecord(element);
                        recordsToAdd.Add(newRecord);
                    }
                }

                if (recordsToUpdate.Any())
                {
                    unitOfWork.GetRepository<IdtMachineWorkRecord>().UpdateRange(recordsToUpdate);
                }

                if (recordsToAdd.Any())
                {
                    await unitOfWork.GetRepository<IdtMachineWorkRecord>().AddRangeAsync(recordsToAdd, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var createdCount = recordsToAdd.Count();
                var updatedCount = recordsToUpdate.Count();

                logger.LogInformation("{UserId} kullanıcısı {Year}/{Month} ayı için {CreatedCount} tane makine puantajı oluşturdu {UpdatedCount} tane makine puantajı güncellendi", userId, workRecordDTOs.First().Date.Year, workRecordDTOs.First().Date.Month, createdCount, updatedCount);

                var mappedRecords = await GetFinalWorkRecords(dates, userId, cancellationToken);

                // Bildirim gönder
                if (updatedCount > 0 || createdCount > 0)
                {
                    var firstRecord = mappedRecords.First();
                    var message = $"{firstRecord.CreatedByFullName} tarafından {firstRecord.Date:MM/yyyy} ayı için makine puantaj kayıt(lar)ı işlendi. {createdCount} kayıt eklendi, {updatedCount} kayıt güncellendi";

                    message += ".";

                    var notificationDTO = new CreateNotificationDTO
                    {
                        Message = message,
                        Type = NotificationType.WorkRecord,
                        RedirectUrl = "/makine-puantaj",
                        TargetUsers = await identityService.GetUserSuperiorsAsync(cancellationToken)
                    };
                    await notificationService.SendNotificationToSuperiorsAsync(notificationDTO, cancellationToken);
                }

                return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Success(mappedRecords, $"Makine puantaj kayıtları işlendi. {createdCount} kayıt eklendi, {updatedCount} kayıt güncellendi.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchCreateOrModifyMachineWorkRecordsAsync işleminde hata oluştu.");
                throw;
            }
        }

        private void UpdateWorkRecordFields(IdtMachineWorkRecord existingRecord, CreateOrModifyMachineWorkRecordDTO element)
        {
            existingRecord.DailyStatus = element.DailyStatus;
            existingRecord.StartTime = element.StartTime;
            existingRecord.EndTime = element.EndTime;
            existingRecord.ProjectId = element.ProjectId;
            existingRecord.EquipmentId = element.EquipmentId;
            existingRecord.Province = element.Province;
            existingRecord.District = element.District;
            existingRecord.HasInternalTransport = element.HasInternalTransport;
            existingRecord.Description = element.Description;
            existingRecord.Status = WorkRecordStatus.Pending;
        }

        private IdtMachineWorkRecord CreateNewMachineWorkRecord(CreateOrModifyMachineWorkRecordDTO element)
        {
            return new IdtMachineWorkRecord
            {
                Date = element.Date,
                DailyStatus = element.DailyStatus,
                StartTime = element.StartTime,
                EndTime = element.EndTime,
                ProjectId = element.ProjectId,
                EquipmentId = element.EquipmentId,
                Province = element.Province,
                District = element.District,
                HasInternalTransport = element.HasInternalTransport,
                Description = element.Description ?? null,
                Status = WorkRecordStatus.Pending
            };
        }

        private async Task<IEnumerable<MachineWorkRecordDTO>> GetFinalWorkRecords(List<DateTime> dates, string userId, CancellationToken cancellationToken)
        {
            var finalRecords = await unitOfWork.GetRepository<IdtMachineWorkRecord>()
               .WhereAsNoTracking(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId)
               .Include(x => x.Equipment)
               .Include(x => x.Project)
               .Include(x => x.CreatedByUser)
               .Include(x => x.UpdatedByUser)
               .ToListAsync(cancellationToken);
            return finalRecords.Adapt<IEnumerable<MachineWorkRecordDTO>>();
        }

        private List<string> GetChangedFields(IdtMachineWorkRecord existing, CreateOrModifyMachineWorkRecordDTO incoming)
        {
            var changedFields = new List<string>();

            if (existing.StartTime != incoming.StartTime)
                changedFields.Add("Başlama Saati");

            if (existing.EndTime != incoming.EndTime)
                changedFields.Add("Bitiş Saati");

            if (existing.ProjectId != incoming.ProjectId)
                changedFields.Add("Proje");

            if (existing.EquipmentId != incoming.EquipmentId)
                changedFields.Add("Ekipman");

            if (existing.Province != incoming.Province)
                changedFields.Add("İl");

            if (existing.District != incoming.District)
                changedFields.Add("İlçe");

            return changedFields;
        }

        private (bool, string?) CheckHoursIfValid(CreateOrModifyMachineWorkRecordDTO dto)
        {
            if (dto.StartTime.HasValue && dto.EndTime.HasValue)
            {
                if (dto.StartTime.Value == dto.EndTime.Value)
                {
                    return (false, $"{dto.Date:dd.MM.yyyy} tarihinde başlangıç ve bitiş saati aynı olamaz.");
                }

                // Eğer endTime < startTime ise, bu gece vardiyası demektir (ertesi güne geçiyor)
                TimeSpan workDuration;
                if (dto.EndTime.Value < dto.StartTime.Value)
                {
                    // Gece vardiyası - ertesi güne geçiyor
                    workDuration = (TimeSpan.FromHours(24) - dto.StartTime.Value) + dto.EndTime.Value;
                }
                else
                {
                    // Normal vardiya - aynı gün içinde
                    workDuration = dto.EndTime.Value - dto.StartTime.Value;
                }

                // Maksimum 24 saat çalışma süresi kontrolü
                if (workDuration.TotalHours > 24)
                {
                    return (false, $"{dto.Date:dd.MM.yyyy} tarihinde çalışma süresi 24 saati aşamaz.");
                }
            }
            else if (dto.StartTime.HasValue || dto.EndTime.HasValue)
            {
                // Biri dolu biri boş olamaz
                return (false, $"{dto.Date:dd.MM.yyyy} tarihinde başlangıç ve bitiş saati birlikte girilmelidir.");
            }

            if (dto.StartTime.HasValue && dto.EndTime.HasValue)
            {
                if (dto.StartTime.Value == dto.EndTime.Value)
                {
                    return (false, $"{dto.Date:dd.MM.yyyy} tarihinde ek başlangıç ve ek bitiş saati aynı olamaz.");
                }

                // Eğer endTime < startTime ise, bu gece vardiyası demektir (ertesi güne geçiyor)
                TimeSpan workDuration;
                if (dto.EndTime.Value < dto.StartTime.Value)
                {
                    // Gece vardiyası - ertesi güne geçiyor
                    workDuration = (TimeSpan.FromHours(24) - dto.StartTime.Value) + dto.EndTime.Value;
                }
                else
                {
                    // Normal vardiya - aynı gün içinde
                    workDuration = dto.EndTime.Value - dto.StartTime.Value;
                }

                // Maksimum 24 saat çalışma süresi kontrolü
                if (workDuration.TotalHours > 24)
                {
                    return (false, $"{dto.Date:dd.MM.yyyy} tarihinde ek çalışma süresi 24 saati aşamaz.");
                }
            }
            else if (dto.StartTime.HasValue || dto.EndTime.HasValue)
            {
                // Biri dolu biri boş olamaz
                return (false, $"{dto.Date:dd.MM.yyyy} tarihinde ek başlangıç ve ek bitiş saati birlikte girilmelidir.");
            }

            return (true, string.Empty);
        }

        public async Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> GetMachineWorkRecordsByUserIdDateStatusAsync(string userId, DateTime date, WorkRecordStatus status, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecords = await unitOfWork.GetRepository<IdtMachineWorkRecord>().WhereAsNoTracking(wr => wr.Date.Year == date.Date.Year && wr.Date.Month == date.Date.Month && wr.CreatedBy == userId && wr.Status == status)
                    .Include(x => x.Equipment)
                    .Include(x => x.Project)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .ToListAsync(cancellationToken);

                var workRecordDTOs = workRecords.Adapt<IEnumerable<MachineWorkRecordDTO>>();
                return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetMachineWorkRecordsByUserIdDateStatusAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}, Status: {Status}", date, userId, status);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> GetApprovedMachineWorkRecordsByUserAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecords = await unitOfWork.GetRepository<IdtMachineWorkRecord>().WhereAsNoTracking(wr => wr.Date.Year == date.Date.Year && wr.Date.Month == date.Date.Month && wr.CreatedBy == userId && (wr.Status == WorkRecordStatus.ApprovedByUnitManager || wr.Status == WorkRecordStatus.ApprovedByChief))
                    .Include(x => x.Equipment)
                    .Include(x => x.Project)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .ToListAsync(cancellationToken);

                var workRecordDTOs = workRecords.Adapt<IEnumerable<MachineWorkRecordDTO>>();
                return ServiceResponse<IEnumerable<MachineWorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetApprovedMachineWorkRecordsByUserAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}", date, userId);
                throw;
            }
        }
    }
}