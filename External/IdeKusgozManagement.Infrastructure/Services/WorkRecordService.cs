using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class WorkRecordService(IUnitOfWork unitOfWork, IIdentityService identityService, ILogger<WorkRecordService> logger, INotificationService notificationService, IWorkRecordExpenseService workRecordExpenseService) : IWorkRecordService
    {
        public async Task<ServiceResponse<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecords = await unitOfWork.GetRepository<IdtWorkRecord>().Where(wr => wr.Date.Year == date.Date.Year && wr.Date.Month == date.Date.Month && wr.CreatedBy == userId)
                    .AsNoTracking()
                    .Include(x => x.Equipment)
                    .Include(x => x.Project)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.Expense)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.File)
                    .ToListAsync(cancellationToken);

                var workRecordDTOs = workRecords.Adapt<IEnumerable<WorkRecordDTO>>();
                return ServiceResponse<IEnumerable<WorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}", date, userId);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<WorkRecordDTO>>> BatchApproveWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (string.IsNullOrEmpty(userId))
                {
                    return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcı kimliği bulunamadı. Lütfen sisteme tekrar giriş yapın.");
                }

                var workRecords = await unitOfWork.GetRepository<IdtWorkRecord>().Where(x => x.CreatedBy == userId && x.Date.Year == date.Year && x.Date.Month == date.Month).ToListAsync(cancellationToken);

                if (!workRecords.Any())
                {
                    return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error("Belirtilen tarih aralığında puantaj kaydı bulunamadı");
                }

                // Tüm kayıtları onaylı duruma getir
                foreach (var workRecord in workRecords)
                {
                    if (workRecord.Status == WorkRecordStatus.Approved)
                        continue;

                    workRecord.Status = WorkRecordStatus.Approved;
                }

                unitOfWork.GetRepository<IdtWorkRecord>().UpdateRange(workRecords);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var mappedWorkRecords = workRecords.Adapt<IEnumerable<WorkRecordDTO>>();

                var approverUserId = identityService.GetUserId;
                logger.LogInformation("Toplu puantaj kaydı onaylandı. UserId: {UserId}, Month: {Month}, Year: {Year}, Count: {Count}, SuperiorId: {SuperiorId}",
                    userId, date.Date.Month, date.Date.Year, workRecords.Count, approverUserId);

                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecords.First().UpdatedByFullName} tarafından, {mappedWorkRecords.First().Date:MM/yyyy} ayı için puantaj kayıtlarınız onaylandı.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new List<string> { mappedWorkRecords.First().CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ServiceResponse<IEnumerable<WorkRecordDTO>>.Success(mappedWorkRecords, $"{workRecords.Count} adet puantaj kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchApproveWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. UserId: {UserId}, Month: {Month}, Year: {Year}", userId, date.Date.Month, date.Date.Year);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<WorkRecordDTO>>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (string.IsNullOrEmpty(userId))
                {
                    return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcı kimliği bulunamadı. Lütfen sisteme tekrar giriş yapın.");
                }

                var workRecords = await unitOfWork.GetRepository<IdtWorkRecord>().Where(x => x.CreatedBy == userId && x.Date.Year == date.Year && x.Date.Month == date.Month).ToListAsync(cancellationToken);

                if (!workRecords.Any())
                {
                    return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error("Belirtilen tarih aralığında puantaj kaydı bulunamadı");
                }

                foreach (var workRecord in workRecords)
                {
                    workRecord.Status = WorkRecordStatus.Rejected;
                    workRecord.RejectReason = rejectReason ?? null;
                }

                unitOfWork.GetRepository<IdtWorkRecord>().UpdateRange(workRecords);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var mappedWorkRecords = workRecords.Adapt<IEnumerable<WorkRecordDTO>>();

                var rejecterUserId = identityService.GetUserId;
                logger.LogInformation("Toplu puantaj kaydı reddedildi. UserId: {UserId}, Month: {Month}, Year: {Year}, Count: {Count}, SuperiorId: {SuperiorId}",
                    userId, date.Date.Month, date.Date.Year, workRecords.Count, rejecterUserId);

                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecords.First().UpdatedByFullName} tarafından, {mappedWorkRecords.First().Date:MM/yyyy} ayı için puantaj kayıtlarınız reddedildi.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new List<string> { mappedWorkRecords.First().CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);

                return ServiceResponse<IEnumerable<WorkRecordDTO>>.Success(mappedWorkRecords, $"{workRecords.Count} adet puantaj kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchApproveWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. UserId: {UserId},  Month: {Month}, Year: {Year}", userId, date.Date.Month, date.Date.Year);
                throw;
            }
        }

        public async Task<ServiceResponse<WorkRecordDTO>> ApproveWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecord = await unitOfWork.GetRepository<IdtWorkRecord>().GetByIdAsync(id, cancellationToken);

                if (workRecord == null)
                {
                    return ServiceResponse<WorkRecordDTO>.Error("Belirtilen puantaj kaydı bulunamadı");
                }
                if (workRecord.Status == WorkRecordStatus.Approved)
                {
                    return ServiceResponse<WorkRecordDTO>.Error(message: "Puantaj kaydı zaten daha önceden onaylanmış");
                }
                workRecord.Status = WorkRecordStatus.Approved;

                unitOfWork.GetRepository<IdtWorkRecord>().Update(workRecord);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var approverUserId = identityService.GetUserId;
                var approvedWorkRecord = await unitOfWork.GetRepository<IdtWorkRecord>()
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
                var mappedWorkRecord = approvedWorkRecord.Adapt<WorkRecordDTO>();

                logger.LogInformation($"{id} ID'li puantaj kaydı {approverUserId} tarafından onaylandı");
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecord.UpdatedByFullName} tarafından, {mappedWorkRecord.Date:dd/MM/yyyy} tarihi için puantaj kaydınız onaylandı.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new List<string> { mappedWorkRecord.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);

                return ServiceResponse<WorkRecordDTO>.Success(mappedWorkRecord, $"Puantaj kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "ApproveWorkRecordByIdAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                throw;
            }
        }

        public async Task<ServiceResponse<WorkRecordDTO>> RejectWorkRecordByIdAsync(string id, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecord = await unitOfWork.GetRepository<IdtWorkRecord>().GetByIdAsync(id, cancellationToken);

                if (workRecord == null)
                {
                    return ServiceResponse<WorkRecordDTO>.Error(message: "Belirtilen puantaj kaydı bulunamadı");
                }

                workRecord.Status = WorkRecordStatus.Rejected;
                workRecord.RejectReason = rejectReason ?? null;
                unitOfWork.GetRepository<IdtWorkRecord>().Update(workRecord);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var rejecterUserId = identityService.GetUserId;

                var rejectedWorkRecord = await unitOfWork.GetRepository<IdtWorkRecord>()
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
                var mappedWorkRecord = rejectedWorkRecord.Adapt<WorkRecordDTO>();

                logger.LogInformation($"{mappedWorkRecord.Id} ID'li puantaj kaydı {rejecterUserId} tarafından reddedildi");

                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecord.UpdatedByFullName} tarafından, {mappedWorkRecord.Date:dd/MM/yyyy} tarihi için puantaj kaydınız reddedildi.",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new List<string> { mappedWorkRecord.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ServiceResponse<WorkRecordDTO>.Success(mappedWorkRecord, $"Puantaj kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "RejectWorkRecordByIdAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<WorkRecordDTO>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var dates = workRecordDTOs.Select(x => x.Date.Date).Distinct().ToList();
                var existingWorkRecords = await unitOfWork.GetRepository<IdtWorkRecord>()
                    .Where(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId)
                    .ToListAsync(cancellationToken);

                var recordsToUpdate = new List<IdtWorkRecord>();
                var expensesToProcess = new List<CreateOrModifyWorkRecordExpenseDTO>();
                var changedFieldsSet = new HashSet<string>();

                foreach (var element in workRecordDTOs)
                {
                    // Saat kontrolü
                    var check = CheckHoursIfValid(element);
                    if (!check.Item1)
                        return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error(check.Item2 ?? "Saat kontrolü başarısız");

                    // İlgili tarihin mevcut kaydı
                    var existingWorkRecord = existingWorkRecords.FirstOrDefault(x => x.Date.Date == element.Date.Date);

                    // ========== VAROLAN GÜNCELLEME ==========
                    if (existingWorkRecord is not null)
                    {
                        if (existingWorkRecord.Status == WorkRecordStatus.Approved)
                            continue;

                        var changedFields = GetChangedFields(existingWorkRecord, element);
                        if (changedFields.Any())
                        {
                            changedFieldsSet.UnionWith(changedFields);
                            UpdateWorkRecordFields(existingWorkRecord, element);
                            recordsToUpdate.Add(existingWorkRecord);
                        }

                        ProcessWorkRecordExpenses(element, existingWorkRecord.Id, expensesToProcess);
                    }
                    // ========== KAYIT YOK ==========
                    else
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error($"{element.Date.Date} tarihli puantaj kaydı veritabanında bulunamadı");
                    }
                }

                if (recordsToUpdate.Any())
                {
                    unitOfWork.GetRepository<IdtWorkRecord>().UpdateRange(recordsToUpdate);
                }

                if (expensesToProcess.Any())
                {
                    var expenseResult = await workRecordExpenseService.BatchUpdateWorkRecordByUserIdExpensesAsync(userId, expensesToProcess, cancellationToken);
                    if (!expenseResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error($"Puantaj masraf kayıtları işlenirken hata oluştu: {expenseResult.Message}");
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var updatedCount = recordsToUpdate.Count();

                // Sadece gerçek masrafları say (dummy expense'leri hariç tut)
                var actualExpenseCount = expensesToProcess.Count(e => e.ExpenseId != Guid.Empty.ToString());
                var processedExpenseCount = actualExpenseCount;

                var mappedRecords = await GetFinalWorkRecords(dates, userId, cancellationToken);

                // Bildirim gönder
                if (updatedCount > 0 || processedExpenseCount > 0)
                {
                    var firstRecord = mappedRecords.First();
                    string? changedFieldsText = changedFieldsSet.Any() ? $" Değişen alanlar: {string.Join(", ", changedFieldsSet)}." : "";
                    var message = $"{firstRecord.UpdatedByFullName} tarafından {firstRecord.Date:MM/yyyy} ayı için puantaj kayıt(lar)ı güncellendi. {updatedCount} kayıt güncellendi";

                    if (processedExpenseCount > 0)
                    {
                        message += $", {processedExpenseCount} masraf işlendi";
                    }

                    message += changedFieldsText;

                    var notificationDTO = new CreateNotificationDTO
                    {
                        Message = message,
                        Type = NotificationType.WorkRecord,
                        RedirectUrl = "/puantaj/ekle",
                        TargetUsers = new List<string> { firstRecord.CreatedBy }
                    };
                    await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                }

                return ServiceResponse<IEnumerable<WorkRecordDTO>>.Success(mappedRecords, $"Puantaj kayıtları işlendi. {updatedCount} kayıt güncellendi.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchUpdateWorkRecordsByUserIdAsync işleminde hata oluştu.");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<WorkRecordDTO>>> BatchCreateOrModifyWorkRecordsAsync(IEnumerable<CreateOrModifyWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default)
        {
            var userId = identityService.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcı kimliği bulunamadı. Lütfen sisteme tekrar giriş yapın.");
            }

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var dates = workRecordDTOs.Select(x => x.Date.Date).Distinct().ToList();
                var existingWorkRecords = await unitOfWork.GetRepository<IdtWorkRecord>()
                    .Where(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId)
                    .ToListAsync(cancellationToken);

                var recordsToAdd = new List<IdtWorkRecord>();
                var recordsToUpdate = new List<IdtWorkRecord>();
                var expensesToProcess = new List<CreateOrModifyWorkRecordExpenseDTO>();

                foreach (var element in workRecordDTOs)
                {
                    // Saat kontrolü
                    var check = CheckHoursIfValid(element);
                    if (!check.Item1)
                        return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error(check.Item2 ?? "Saat kontrolü başarısız");

                    // İlgili tarihin mevcut kaydı
                    var existingWorkRecord = existingWorkRecords.FirstOrDefault(x => x.Date.Date == element.Date.Date);

                    // ========== VAROLAN GÜNCELLEME ==========
                    if (existingWorkRecord is not null)
                    {
                        if (existingWorkRecord.Status == WorkRecordStatus.Approved)
                            continue;

                        var changedFields = GetChangedFields(existingWorkRecord, element);

                        if (changedFields.Any())
                        {
                            UpdateWorkRecordFields(existingWorkRecord, element);
                            recordsToUpdate.Add(existingWorkRecord);
                        }

                        ProcessWorkRecordExpenses(element, existingWorkRecord.Id, expensesToProcess);
                    }
                    // ========== YENİ KAYIT ==========
                    else
                    {
                        var newRecord = CreateNewWorkRecord(element);
                        recordsToAdd.Add(newRecord);

                        ProcessWorkRecordExpenses(element, newRecord.Id, expensesToProcess);
                    }
                }

                if (recordsToUpdate.Any())
                {
                    unitOfWork.GetRepository<IdtWorkRecord>().UpdateRange(recordsToUpdate);
                }

                if (recordsToAdd.Any())
                {
                    await unitOfWork.GetRepository<IdtWorkRecord>().AddRangeAsync(recordsToAdd, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                if (expensesToProcess.Any())
                {
                    var expenseResult = await workRecordExpenseService.BatchCreateOrModifyWorkRecordExpensesAsync(expensesToProcess, cancellationToken);
                    if (!expenseResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ServiceResponse<IEnumerable<WorkRecordDTO>>.Error($"Puantaj masraf kayıtları işlenirken hata oluştu: {expenseResult.Message}");
                    }
                }

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var createdCount = recordsToAdd.Count();
                var updatedCount = recordsToUpdate.Count();

                // Sadece gerçek masrafları say (dummy expense'leri hariç tut)
                var actualExpenseCount = expensesToProcess.Count(e => e.ExpenseId != Guid.Empty.ToString());
                var processedExpenseCount = actualExpenseCount;

                var mappedRecords = await GetFinalWorkRecords(dates, userId, cancellationToken);

                // Bildirim gönder
                if (updatedCount > 0 || createdCount > 0 || processedExpenseCount > 0)
                {
                    var firstRecord = mappedRecords.First();
                    var message = $"{firstRecord.CreatedByFullName} tarafından {firstRecord.Date:MM/yyyy} ayı için puantaj kayıt(lar)ı işlendi. {createdCount} kayıt eklendi, {updatedCount} kayıt güncellendi";

                    if (processedExpenseCount > 0)
                    {
                        message += $", {processedExpenseCount} masraf işlendi";
                    }

                    message += ".";

                    var notificationDTO = new CreateNotificationDTO
                    {
                        Message = message,
                        Type = NotificationType.WorkRecord,
                        RedirectUrl = "/puantaj",
                        TargetUsers = await identityService.GetUserSuperiorsAsync(cancellationToken)
                    };
                    await notificationService.SendNotificationToSuperiorsAsync(notificationDTO, cancellationToken);
                }

                return ServiceResponse<IEnumerable<WorkRecordDTO>>.Success(mappedRecords, $"Puantaj kayıtları işlendi. {createdCount} kayıt eklendi, {updatedCount} kayıt güncellendi.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchCreateOrModifyWorkRecordsAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        private void UpdateWorkRecordFields(IdtWorkRecord existingRecord, CreateOrModifyWorkRecordDTO element)
        {
            existingRecord.DailyStatus = element.DailyStatus;
            existingRecord.StartTime = element.StartTime;
            existingRecord.EndTime = element.EndTime;
            existingRecord.ProjectId = element.ProjectId;
            existingRecord.EquipmentId = element.EquipmentId;
            existingRecord.Province = element.Province;
            existingRecord.District = element.District;
            existingRecord.HasBreakfast = element.HasBreakfast;
            existingRecord.HasLunch = element.HasLunch;
            existingRecord.HasDinner = element.HasDinner;
            existingRecord.HasNightMeal = element.HasNightMeal;
            existingRecord.AdditionalStartTime = element.AdditionalStartTime ?? null;
            existingRecord.AdditionalEndTime = element.AdditionalEndTime ?? null;
            existingRecord.TravelExpenseAmount = element.TravelExpenseAmount;
            existingRecord.Status = WorkRecordStatus.Pending;
        }

        private void ProcessWorkRecordExpenses(CreateOrModifyWorkRecordDTO element, string workRecordId, List<CreateOrModifyWorkRecordExpenseDTO> expensesToProcess)
        {
            if (element.WorkRecordExpenses?.Any() == true)
            {
                foreach (var expense in element.WorkRecordExpenses)
                {
                    expense.WorkRecordId = workRecordId;
                }
                expensesToProcess.AddRange(element.WorkRecordExpenses);
            }
            else
            {
                // 1 kayıt var ve silinmek isteniyorsa WorkRecordExpenses null geliyor bu yüzden dummy kayıt oluşturuyoruz
                var dummyExpense = new CreateOrModifyWorkRecordExpenseDTO
                {
                    WorkRecordId = workRecordId,
                    Amount = 1,
                    Description = "dummy expense",
                    ExpenseId = Guid.Empty.ToString(),
                    File = null,
                    Id = Guid.Empty.ToString()
                };

                expensesToProcess.Add(dummyExpense);
            }
        }

        private IdtWorkRecord CreateNewWorkRecord(CreateOrModifyWorkRecordDTO element)
        {
            return new IdtWorkRecord
            {
                Date = element.Date,
                DailyStatus = element.DailyStatus,
                StartTime = element.StartTime,
                EndTime = element.EndTime,
                ProjectId = element.ProjectId,
                EquipmentId = element.EquipmentId,
                Province = element.Province,
                District = element.District,
                HasBreakfast = element.HasBreakfast,
                HasLunch = element.HasLunch,
                HasDinner = element.HasDinner,
                HasNightMeal = element.HasNightMeal,
                AdditionalStartTime = element.AdditionalStartTime ?? null,
                AdditionalEndTime = element.AdditionalEndTime ?? null,
                TravelExpenseAmount = element.TravelExpenseAmount ?? null,
                Status = WorkRecordStatus.Pending
            };
        }

        private async Task<IEnumerable<WorkRecordDTO>> GetFinalWorkRecords(List<DateTime> dates, string userId, CancellationToken cancellationToken)
        {
            var finalRecords = await unitOfWork.GetRepository<IdtWorkRecord>()
               .WhereAsNoTracking(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId)
               .Include(x => x.Equipment)
               .Include(x => x.Project)
               .Include(x => x.CreatedByUser)
               .Include(x => x.UpdatedByUser)
               .Include(x => x.WorkRecordExpenses)
                   .ThenInclude(x => x.Expense)
               .Include(x => x.WorkRecordExpenses)
                   .ThenInclude(x => x.File)
               .ToListAsync(cancellationToken);
            return finalRecords.Adapt<IEnumerable<WorkRecordDTO>>();
        }

        private List<string> GetChangedFields(IdtWorkRecord existing, CreateOrModifyWorkRecordDTO incoming)
        {
            var changedFields = new List<string>();

            if (existing.DailyStatus != incoming.DailyStatus)
                changedFields.Add("Günlük Durum");

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

            if (existing.HasBreakfast != incoming.HasBreakfast)
                changedFields.Add("Sabah Yemeği");

            if (existing.HasLunch != incoming.HasLunch)
                changedFields.Add("Öğle Yemeği");

            if (existing.HasDinner != incoming.HasDinner)
                changedFields.Add("Akşam Yemeği");

            if (existing.HasNightMeal != incoming.HasNightMeal)
                changedFields.Add("Gece Yemeği");

            if (existing.AdditionalStartTime != (incoming.AdditionalStartTime ?? null))
                changedFields.Add("Ek Başlama Saati");

            if (existing.AdditionalEndTime != (incoming.AdditionalEndTime ?? null))
                changedFields.Add("Ek Bitiş Saati");

            if (existing.TravelExpenseAmount != (incoming.TravelExpenseAmount ?? null))
                changedFields.Add("Yol Masrafı");

            return changedFields;
        }

        private (bool, string?) CheckHoursIfValid(CreateOrModifyWorkRecordDTO dto)
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
    }
}