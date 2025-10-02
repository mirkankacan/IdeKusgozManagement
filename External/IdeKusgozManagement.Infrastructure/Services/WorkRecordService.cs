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
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.File)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.Expense)
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

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchCreateOrModifyWorkRecordsAsync(
    IEnumerable<CreateWorkRecordDTO> createWorkRecordDTOs,
    CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var userId = identityService.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcı kimliği bulunamadı.");
                }

                if (createWorkRecordDTOs == null || !createWorkRecordDTOs.Any())
                {
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("İşlenecek kayıt bulunamadı");
                }

                var dates = createWorkRecordDTOs.Select(x => x.Date.Date).Distinct().ToList();
                var existingWorkRecords = await unitOfWork.GetRepository<IdtWorkRecord>()
                    .Where(x => dates.Contains(x.Date.Date) && x.CreatedBy == userId)
                    .ToListAsync(cancellationToken);

                var recordsToUpdate = new List<IdtWorkRecord>();
                var recordsToAdd = new List<IdtWorkRecord>();
                var expenseOperations = new List<(string workRecordId, IEnumerable<CreateWorkRecordExpenseDTO> expenses)>();

                foreach (var dto in createWorkRecordDTOs)
                {
                    var existingRecord = existingWorkRecords.FirstOrDefault(x => x.Date.Date == dto.Date.Date);

                    if (existingRecord != null && existingRecord.Status == WorkRecordStatus.Approved)
                    {
                        continue;
                    }

                    if (existingRecord != null)
                    {
                        // Update existing record
                        existingRecord.IsWeekend = dto.IsWeekend;
                        existingRecord.StartTime = dto.StartTime;
                        existingRecord.EndTime = dto.EndTime;
                        existingRecord.Project = dto.Project;
                        existingRecord.EquipmentId = dto.EquipmentId;
                        existingRecord.Province = dto.Province;
                        existingRecord.District = dto.District;
                        existingRecord.HasBreakfast = dto.HasBreakfast;
                        existingRecord.HasLunch = dto.HasLunch;
                        existingRecord.HasDinner = dto.HasDinner;
                        existingRecord.HasNightMeal = dto.HasNightMeal;
                        existingRecord.Status = WorkRecordStatus.Pending;
                        existingRecord.UpdatedBy = userId;
                        existingRecord.UpdatedDate = DateTime.UtcNow;

                        recordsToUpdate.Add(existingRecord);

                        // Eski expense'leri sil (servis üzerinden)
                        if (existingRecord.WorkRecordExpenses?.Any() == true)
                        {
                            foreach (var expense in existingRecord.WorkRecordExpenses)
                            {
                                await workRecordExpenseService.DeleteWorkRecordExpenseAsync(expense.Id, cancellationToken);
                            }
                        }

                        // Yeni expense'leri ekle
                        if (dto.WorkRecordExpenses?.Any() == true)
                        {
                            expenseOperations.Add((existingRecord.Id, dto.WorkRecordExpenses));
                        }
                    }
                    else
                    {
                        // Create new record
                        var newRecord = new IdtWorkRecord
                        {
                            Date = dto.Date,
                            IsWeekend = dto.IsWeekend,
                            StartTime = dto.StartTime,
                            EndTime = dto.EndTime,
                            Project = dto.Project,
                            EquipmentId = dto.EquipmentId,
                            Province = dto.Province,
                            District = dto.District,
                            HasBreakfast = dto.HasBreakfast,
                            HasLunch = dto.HasLunch,
                            HasDinner = dto.HasDinner,
                            HasNightMeal = dto.HasNightMeal,
                            Status = WorkRecordStatus.Pending,
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow
                        };

                        recordsToAdd.Add(newRecord);

                        // Yeni kayıt için expense'leri sakla
                        if (dto.WorkRecordExpenses?.Any() == true)
                        {
                            expenseOperations.Add((newRecord.Id, dto.WorkRecordExpenses));
                        }
                    }
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

                await unitOfWork.SaveChangesAsync(cancellationToken);

                // Expense işlemleri (servis üzerinden)
                foreach (var (workRecordId, expenses) in expenseOperations)
                {
                    var expenseResult = await workRecordExpenseService
                        .BatchCreateWorkRecordExpensesAsync(workRecordId, expenses, cancellationToken);

                    if (!expenseResult.IsSuccess)
                    {
                        throw new Exception($"Harcama kayıtları oluşturulurken hata: {expenseResult.Message}");
                    }
                }

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                // Map to DTOs
                var allRecords = recordsToUpdate.Concat(recordsToAdd).ToList();
                var resultWorkRecords = allRecords.Adapt<IEnumerable<WorkRecordDTO>>();

                var processedCount = resultWorkRecords?.Count() ?? 0;
                var updatedCount = recordsToUpdate?.Count ?? 0;
                var addedCount = recordsToAdd?.Count ?? 0;

                logger.LogInformation(
                    "Toplu puantaj kaydı işlemi başarıyla tamamlandı. İşlenen kayıt sayısı: {Count}, UpdatedCount: {UpdatedCount}, AddedCount: {AddedCount}, UserId: {UserId}",
                    processedCount, updatedCount, addedCount, userId);

                CreateNotificationDTO notificationDTO = new()
                {
                    Message = $"{resultWorkRecords.First().CreatedByFullName} tarafından, {resultWorkRecords.First().Date.Month}/{resultWorkRecords.First().Date.Year} ayı için toplu puantaj kayıtları başarıyla işlendi. {recordsToUpdate.Count} kayıt güncellendi, {recordsToAdd.Count} yeni kayıt eklendi",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj",
                    TargetUsers = await identityService.GetUserSuperiorsAsync(cancellationToken)
                };
                await notificationService.SendNotificationToSuperiorsAsync(notificationDTO, cancellationToken);

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(resultWorkRecords,
                    $"Toplu puantaj kayıtları başarıyla işlendi. {recordsToUpdate.Count} kayıt güncellendi, {recordsToAdd.Count} yeni kayıt eklendi.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "BatchCreateWorkRecordsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Toplu puantaj kayıtları işlenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<UpdateWorkRecordDTO> updateWorkRecordDTOs, CancellationToken cancellationToken = default)
        {
            throw new ArgumentException("Bu metot artık kullanılmamaktadır. Lütfen BatchCreateOrModifyWorkRecordsAsync metodunu kullanın.");
            //CreateNotificationDTO notificationDTO = new()
            //{
            //    Message = $"{result.Data.FirstOrDefault().UpdatedByFullName} tarafından, {result.Data.FirstOrDefault().Date.Month}/{result.Data.FirstOrDefault().Date.Year} ayı için puantaj kaydınız güncellendi",
            //    Type = NotificationType.WorkRecord,
            //    RedirectUrl = "/puantaj",
            //    TargetUsers = new[] { result.Data.FirstOrDefault().CreatedBy }
            //};
            //await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
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
                    Message = $"{mappedWorkRecords.First().UpdatedByFullName} tarafından, {mappedWorkRecords.First().Date.Month}/{mappedWorkRecords.First().Date.Year} ayı için puantaj kayıtlarınız onaylandı",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj",
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
                    Message = $"{mappedWorkRecords.First().UpdatedByFullName} tarafından, {mappedWorkRecords.First().Date.Month}/{mappedWorkRecords.First().Date.Year} ayı için puantaj kayıtlarınız reddedildi",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj",
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
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.File)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.Expense)
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
                    Message = $"{mappedWorkRecord.UpdatedByFullName} tarafından, {mappedWorkRecord.Date.Day}.{mappedWorkRecord.Date.Month}.{mappedWorkRecord.Date.Year} günü için puantaj kaydınız onaylandı",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj",
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
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.File)
                    .Include(x => x.WorkRecordExpenses)
                        .ThenInclude(x => x.Expense)
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
                    Message = $"{mappedWorkRecord.UpdatedByFullName} tarafından, {mappedWorkRecord.Date.Day}.{mappedWorkRecord.Date.Month}.{mappedWorkRecord.Date.Year} günü için puantaj kaydınız reddedildi",
                    Type = NotificationType.WorkRecord,
                    RedirectUrl = "/puantaj",
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
    }
}