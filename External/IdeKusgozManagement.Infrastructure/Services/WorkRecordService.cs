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
        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
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
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}", date, userId);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Tarih ve kullanıcıya göre puantaj kayıtları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<UpdateWorkRecordDTO> updateWorkRecordDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Seçilen kullanıcı bulunamadı.");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var dates = updateWorkRecordDTOs.Select(x => x.Date.Date).Distinct().ToList();

                var existingWorkRecords = await unitOfWork.GetRepository<IdtWorkRecord>()
                   .Where(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId)
                   .ToListAsync(cancellationToken);

                var recordsToUpdate = new List<IdtWorkRecord>();
                var expensesToDelete = new List<string>();
                var expensesToUpdate = new List<UpdateWorkRecordExpenseDTO>();

                foreach (var dto in updateWorkRecordDTOs)
                {
                    var existingRecord = existingWorkRecords.FirstOrDefault(x => x.Date.Date == dto.Date.Date);

                    // Onaylanmış kayıtları atla
                    if (existingRecord?.Status == WorkRecordStatus.Approved)
                        continue;

                    if (existingRecord != null)
                    {
                        // ========== GÜNCELLEME ==========
                        existingRecord.ExcuseReason = dto.ExcuseReason;
                        existingRecord.StartTime = dto.StartTime;
                        existingRecord.EndTime = dto.EndTime;
                        existingRecord.ProjectId = dto.ProjectId;
                        existingRecord.EquipmentId = dto.EquipmentId;
                        existingRecord.Province = dto.Province;
                        existingRecord.District = dto.District;
                        existingRecord.HasBreakfast = dto.HasBreakfast;
                        existingRecord.HasLunch = dto.HasLunch;
                        existingRecord.HasDinner = dto.HasDinner;
                        existingRecord.HasNightMeal = dto.HasNightMeal;
                        existingRecord.Status = WorkRecordStatus.Pending;

                        recordsToUpdate.Add(existingRecord);

                        // Eski expense'leri sil
                        if (existingRecord.WorkRecordExpenses?.Any() == true)
                        {
                            expensesToDelete.AddRange(existingRecord.WorkRecordExpenses.Select(e => e.Id));
                        }

                        // Yeni expense'leri ekle
                        if (dto.WorkRecordExpenses?.Any() == true)
                        {
                            foreach (var expense in dto.WorkRecordExpenses)
                            {
                                expense.WorkRecordId = existingRecord.Id;
                            }
                            expensesToUpdate.AddRange(dto.WorkRecordExpenses);
                        }
                    }
                }

                // WorkRecord işlemleri
                if (recordsToUpdate.Any())
                {
                    unitOfWork.GetRepository<IdtWorkRecord>().UpdateRange(recordsToUpdate);
                }

                if (expensesToDelete.Any())
                {
                    await workRecordExpenseService.BatchDeleteWorkRecordExpensesAsync(expensesToDelete, cancellationToken);
                }

                if (expensesToUpdate.Any())
                {
                    await workRecordExpenseService.BatchUpdateWorkRecordExpensesAsync(expensesToUpdate, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var updatedCount = recordsToUpdate.Count;

                logger.LogInformation("Toplu puantaj güncelleme işlemi başarıyla tamamlandı. UpdatedCount: {UpdatedCount}, UserId: {UserId}", updatedCount, userId);

                // Bildirim gönder
                var updatedRecords = await unitOfWork.GetRepository<IdtWorkRecord>()
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
                var mappedUpdatedRecords = updatedRecords.Adapt<IEnumerable<WorkRecordDTO>>();

                if (!mappedUpdatedRecords.Any())
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("İşlenmiş puantaj kayıtları bulunamadı");
                }
                var firstRecord = mappedUpdatedRecords.First();
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{firstRecord.UpdatedByFullName} tarafından, {firstRecord.Date:MM/yyyy} ayı için puantajınız güncellendi",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new[] { firstRecord.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(mappedUpdatedRecords, $"Toplu puantaj kayıtları başarıyla güncellendi. {updatedCount} kayıt güncellendi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchUpdateWorkRecordsByUserIdAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Toplu puantaj kayıtları güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchApproveWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcı kimliği bulunamadı. Lütfen sisteme tekrar giriş yapın.");
                }

                var workRecords = await unitOfWork.GetRepository<IdtWorkRecord>().Where(x => x.CreatedBy == userId && x.Date.Year == date.Year && x.Date.Month == date.Month).ToListAsync(cancellationToken);

                if (!workRecords.Any())
                {
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Belirtilen tarih aralığında puantaj kaydı bulunamadı");
                }

                // Tüm kayıtları onaylı duruma getir
                foreach (var workRecord in workRecords)
                {
                    if (workRecord.Status == WorkRecordStatus.Approved)
                    {
                        continue;
                    }
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
                    Message = $"{mappedWorkRecords.First().UpdatedByFullName} tarafından, {mappedWorkRecords.First().Date:MM/yyyy} ayı için puantaj kayıtlarınız onaylandı",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new[] { mappedWorkRecords.First().CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(mappedWorkRecords, $"{workRecords.Count} adet puantaj kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchApproveWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. UserId: {UserId}, Month: {Month}, Year: {Year}", userId, date.Date.Month, date.Date.Year);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Toplu puantaj kaydı onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcı kimliği bulunamadı. Lütfen sisteme tekrar giriş yapın.");
                }

                var workRecords = await unitOfWork.GetRepository<IdtWorkRecord>().Where(x => x.CreatedBy == userId && x.Date.Year == date.Year && x.Date.Month == date.Month).ToListAsync(cancellationToken);

                if (!workRecords.Any())
                {
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Belirtilen tarih aralığında puantaj kaydı bulunamadı");
                }

                foreach (var workRecord in workRecords)
                {
                    if (workRecord.Status == WorkRecordStatus.Rejected)
                    {
                        continue;
                    }
                    workRecord.Status = WorkRecordStatus.Rejected;
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
                    Message = $"{mappedWorkRecords.First().UpdatedByFullName} tarafından, {mappedWorkRecords.First().Date:MM/yyyy} ayı için puantaj kayıtlarınız reddedildi",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new[] { mappedWorkRecords.First().CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(mappedWorkRecords, $"{workRecords.Count} adet puantaj kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchApproveWorkRecordsByUserIdAndDateAsync işleminde hata oluştu. UserId: {UserId},  Month: {Month}, Year: {Year}", userId, date.Date.Month, date.Date.Year);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Toplu puantaj kaydı reddedilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<WorkRecordDTO>> ApproveWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecord = await unitOfWork.GetRepository<IdtWorkRecord>()
                    .Where(x => x.Id == id)
                    .Include(x => x.Equipment)
                    .Include(x => x.Project)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.Expense)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.File)
                    .FirstOrDefaultAsync(cancellationToken);

                if (workRecord == null)
                {
                    return ApiResponse<WorkRecordDTO>.Error("Belirtilen puantaj kaydı bulunamadı");
                }
                if (workRecord.Status == WorkRecordStatus.Approved)
                {
                    return ApiResponse<WorkRecordDTO>.Error(message: "Puantaj kaydı zaten daha önceden onaylanmış");
                }
                workRecord.Status = WorkRecordStatus.Approved;

                unitOfWork.GetRepository<IdtWorkRecord>().Update(workRecord);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var mappedWorkRecord = workRecord.Adapt<WorkRecordDTO>();

                var approverUserId = identityService.GetUserId;
                logger.LogInformation($"{id} ID'li puantaj kaydı {approverUserId} tarafından onaylandı");
                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecord.UpdatedByFullName} tarafından, {mappedWorkRecord.Date:dd/MM/yyyy} tarihi için puantaj kaydınız onaylandı",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new[] { mappedWorkRecord.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);

                return ApiResponse<WorkRecordDTO>.Success(mappedWorkRecord, $"Puantaj kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "ApproveWorkRecordByIdAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return ApiResponse<WorkRecordDTO>.Error("Puantaj kaydı onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<WorkRecordDTO>> RejectWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecord = await unitOfWork.GetRepository<IdtWorkRecord>()
                    .Where(x => x.Id == id)
                    .Include(x => x.Equipment)
                    .Include(x => x.Project)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.Expense)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.File)
                    .FirstOrDefaultAsync(cancellationToken);

                if (workRecord == null)
                {
                    return ApiResponse<WorkRecordDTO>.Error(message: "Belirtilen puantaj kaydı bulunamadı");
                }
                if (workRecord.Status == WorkRecordStatus.Rejected)
                {
                    return ApiResponse<WorkRecordDTO>.Error(message: "Puantaj kaydı zaten daha önceden reddedilmiş");
                }
                workRecord.Status = WorkRecordStatus.Rejected;

                unitOfWork.GetRepository<IdtWorkRecord>().Update(workRecord);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var mappedWorkRecord = workRecord.Adapt<WorkRecordDTO>();

                var rejecterUserId = identityService.GetUserId;
                logger.LogInformation($"{mappedWorkRecord.Id} ID'li puantaj kaydı {rejecterUserId} tarafından reddedildi");

                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{mappedWorkRecord.UpdatedByFullName} tarafından, {mappedWorkRecord.Date:dd/MM/yyyy} tarihi için puantaj kaydınız reddedildi",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj/ekle",
                    TargetUsers = new[] { mappedWorkRecord.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                return ApiResponse<WorkRecordDTO>.Success(mappedWorkRecord, $"Puantaj kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "RejectWorkRecordByIdAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return ApiResponse<WorkRecordDTO>.Error("Puantaj kaydı reddedilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchCreateOrModifyWorkRecordsAsync(IEnumerable<CreateWorkRecordDTO> createWorkRecordDTOs, CancellationToken cancellationToken = default)
        {
            var userId = identityService.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcı kimliği bulunamadı. Lütfen sisteme tekrar giriş yapın.");
            }

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var dates = createWorkRecordDTOs.Select(x => x.Date.Date).Distinct().ToList();

                var existingWorkRecords = await unitOfWork.GetRepository<IdtWorkRecord>()
                    .Where(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId)
                    .Include(x => x.WorkRecordExpenses)
                    .ThenInclude(x => x.File)
                    .ToListAsync(cancellationToken);

                var recordsToUpdate = new List<IdtWorkRecord>();
                var recordsToAdd = new List<IdtWorkRecord>();
                var expensesToAdd = new List<CreateWorkRecordExpenseDTO>();
                var expensesToDelete = new List<string>();

                foreach (var dto in createWorkRecordDTOs)
                {
                    // Saat kontrolü - nullable için
                    var check = CheckHoursIfValid(dto);
                    if (!check.Item1)
                        return ApiResponse<IEnumerable<WorkRecordDTO>>.Error(check.Item2);

                    var existingRecord = existingWorkRecords.FirstOrDefault(x => x.Date.Date == dto.Date.Date);

                    // Onaylanmış kayıtları atla
                    if (existingRecord?.Status == WorkRecordStatus.Approved)
                        continue;

                    if (existingRecord != null)
                    {
                        // ========== GÜNCELLEME ==========
                        existingRecord.ExcuseReason = dto.ExcuseReason;
                        existingRecord.StartTime = dto.StartTime;
                        existingRecord.EndTime = dto.EndTime;
                        existingRecord.ProjectId = dto.ProjectId;
                        existingRecord.EquipmentId = dto.EquipmentId;
                        existingRecord.Province = dto.Province;
                        existingRecord.District = dto.District;
                        existingRecord.HasBreakfast = dto.HasBreakfast;
                        existingRecord.HasLunch = dto.HasLunch;
                        existingRecord.HasDinner = dto.HasDinner;
                        existingRecord.HasNightMeal = dto.HasNightMeal;
                        existingRecord.AdditionalStartTime = dto.AdditionalStartTime;
                        existingRecord.AdditionalEndTime = dto.AdditionalEndTime;
                        existingRecord.HasTravel = dto.HasTravel;
                        existingRecord.Status = WorkRecordStatus.Pending;

                        recordsToUpdate.Add(existingRecord);

                        // Eski expense'leri sil
                        if (existingRecord.WorkRecordExpenses?.Any() == true)
                        {
                            expensesToDelete.AddRange(existingRecord.WorkRecordExpenses.Select(e => e.Id));
                        }

                        // Yeni expense'leri ekle
                        if (dto.WorkRecordExpenses?.Any() == true)
                        {
                            foreach (var expense in dto.WorkRecordExpenses)
                            {
                                expense.WorkRecordId = existingRecord.Id;
                                if (expense.File != null)
                                    expense.File.FileType = FileType.Expense;
                            }
                            expensesToAdd.AddRange(dto.WorkRecordExpenses);
                        }
                    }
                    else
                    {
                        // ========== YENİ KAYIT ==========
                        var newRecord = new IdtWorkRecord
                        {
                            Date = dto.Date,
                            ExcuseReason = dto.ExcuseReason,
                            StartTime = dto.StartTime,
                            EndTime = dto.EndTime,
                            ProjectId = dto.ProjectId,
                            EquipmentId = dto.EquipmentId,
                            Province = dto.Province,
                            District = dto.District,
                            HasBreakfast = dto.HasBreakfast,
                            HasLunch = dto.HasLunch,
                            HasDinner = dto.HasDinner,
                            HasNightMeal = dto.HasNightMeal,
                            AdditionalStartTime = dto.AdditionalStartTime,
                            AdditionalEndTime = dto.AdditionalEndTime,
                            HasTravel = dto.HasTravel,
                            Status = WorkRecordStatus.Pending,
                        };

                        recordsToAdd.Add(newRecord);

                        if (dto.WorkRecordExpenses?.Any() == true)
                        {
                            foreach (var expense in dto.WorkRecordExpenses)
                            {
                                expense.WorkRecordId = newRecord.Id;
                                if (expense.File != null)
                                    expense.File.FileType = FileType.Expense;
                            }
                            expensesToAdd.AddRange(dto.WorkRecordExpenses);
                        }
                    }
                }

                // Eğer hiçbir işlem yapılmadıysa (tüm kayıtlar onaylı)
                if (!recordsToUpdate.Any() && !recordsToAdd.Any())
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Seçilen tarihlerdeki tüm kayıtlar onaylanmış durumda. İşlem yapılamadı.");
                }

                // WorkRecord işlemleri
                if (recordsToUpdate.Any())
                {
                    unitOfWork.GetRepository<IdtWorkRecord>().UpdateRange(recordsToUpdate);
                }

                if (recordsToAdd.Any())
                {
                    await unitOfWork.GetRepository<IdtWorkRecord>().AddRangeAsync(recordsToAdd, cancellationToken);
                }

                if (expensesToDelete.Any())
                {
                    await workRecordExpenseService.BatchDeleteWorkRecordExpensesAsync(expensesToDelete, cancellationToken);
                }

                if (expensesToAdd.Any())
                {
                    await workRecordExpenseService.BatchCreateWorkRecordExpensesAsync(expensesToAdd, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var updatedCount = recordsToUpdate.Count;
                var addedCount = recordsToAdd.Count;

                logger.LogInformation(
                    "Toplu puantaj kaydı işlemi başarıyla tamamlandı. UpdatedCount: {UpdatedCount}, AddedCount: {AddedCount}, UserId: {UserId}",
                    updatedCount, addedCount, userId);

                // Bildirim gönder
                var createdRecords = await unitOfWork.GetRepository<IdtWorkRecord>()
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

                var mappedCreatedRecords = createdRecords.Adapt<IEnumerable<WorkRecordDTO>>();

                if (!mappedCreatedRecords.Any())
                {
                    // Bu durumda rollback'e gerek yok, zaten commit edildi
                    logger.LogWarning("İşlem başarılı ama döndürülecek kayıt bulunamadı. UserId: {UserId}", userId);
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(
                        Enumerable.Empty<WorkRecordDTO>(),
                        $"Toplu puantaj kayıtları başarıyla işlendi. {updatedCount} kayıt güncellendi, {addedCount} yeni kayıt eklendi.");
                }

                var firstRecord = mappedCreatedRecords.First();
                var notificationDTO = new CreateNotificationDTO
                {
                    Message = $"{firstRecord.CreatedByFullName} tarafından, {firstRecord.Date:MM/yyyy} ayı için toplu puantaj kayıtları başarıyla işlendi. {updatedCount} kayıt güncellendi, {addedCount} yeni kayıt eklendi",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj",
                    TargetUsers = await identityService.GetUserSuperiorsAsync(cancellationToken)
                };
                await notificationService.SendNotificationToSuperiorsAsync(notificationDTO, cancellationToken);

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(
                    mappedCreatedRecords,
                    $"Toplu puantaj kayıtları başarıyla işlendi. {updatedCount} kayıt güncellendi, {addedCount} yeni kayıt eklendi.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchCreateOrModifyWorkRecordsAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Toplu puantaj kayıtları işlenirken hata oluştu");
            }
        }
        private (bool, string?) CheckHoursIfValid(CreateWorkRecordDTO dto)
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