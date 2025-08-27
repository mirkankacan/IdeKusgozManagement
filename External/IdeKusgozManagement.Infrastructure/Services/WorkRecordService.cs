using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class WorkRecordService : IWorkRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WorkRecordService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public WorkRecordService(
            IUnitOfWork unitOfWork,
            ILogger<WorkRecordService> logger,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetAllWorkRecordsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecords = await _unitOfWork.Repository<IdtWorkRecord>().GetAllAsync(cancellationToken);
                var workRecordDTOs = new List<WorkRecordDTO>();

                foreach (var workRecord in workRecords)
                {
                    var workRecordDTO = workRecord.Adapt<WorkRecordDTO>();

                    // Expenses'leri getir
                    var expenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                        .GetWhereAsync(exp => exp.WorkRecordId == workRecord.Id, cancellationToken);

                    workRecordDTO.Expenses = expenses.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();
                    workRecordDTOs.Add(workRecordDTO);
                }

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllWorkRecordsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("İş kayıtları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<WorkRecordDTO>> GetWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(id, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<WorkRecordDTO>.Error("İş kaydı bulunamadı");
                }

                var workRecordDTO = workRecord.Adapt<WorkRecordDTO>();

                // Expenses'leri getir
                var expenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                    .GetWhereAsync(exp => exp.WorkRecordId == id, cancellationToken);

                workRecordDTO.Expenses = expenses.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();

                return ApiResponse<WorkRecordDTO>.Success(workRecordDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordByIdAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return ApiResponse<WorkRecordDTO>.Error("İş kaydı getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                    .GetWhereAsync(wr => wr.CreatedBy == userId, cancellationToken);

                var workRecordDTOs = new List<WorkRecordDTO>();

                foreach (var workRecord in workRecords)
                {
                    var workRecordDTO = workRecord.Adapt<WorkRecordDTO>();

                    // Expenses'leri getir
                    var expenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                        .GetWhereAsync(exp => exp.WorkRecordId == workRecord.Id, cancellationToken);

                    workRecordDTO.Expenses = expenses.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();
                    workRecordDTOs.Add(workRecordDTO);
                }

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordsByUserIdAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcının iş kayıtları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByDateAndUserAsync(DateTime date, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                    .GetWhereAsync(wr => wr.Date.Year == date.Date.Year && wr.Date.Month == date.Date.Month && wr.CreatedBy == userId, cancellationToken);

                var workRecordDTOs = new List<WorkRecordDTO>();

                foreach (var workRecord in workRecords)
                {
                    var workRecordDTO = workRecord.Adapt<WorkRecordDTO>();

                    // Expenses'leri getir
                    var expenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                        .GetWhereAsync(exp => exp.WorkRecordId == workRecord.Id, cancellationToken);

                    workRecordDTO.Expenses = expenses.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();
                    workRecordDTOs.Add(workRecordDTO);
                }

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordsByDateAndUserAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}", date, userId);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Tarih ve kullanıcıya göre iş kayıtları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchCreateWorkRecordsAsync(List<CreateWorkRecordDTO> createWorkRecordDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var currentUserId = _currentUserService.GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Kullanıcı kimliği bulunamadı");
                }

                // Tüm tarihler için mevcut kayıtları bir seferde al
                var dates = createWorkRecordDTOs.Select(dto => dto.Date.Date).Distinct().ToList();
                var existingWorkRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                    .GetWhereAsync(wr => dates.Contains(wr.Date.Date) && wr.CreatedBy == currentUserId, cancellationToken);

                var existingRecordsDict = existingWorkRecords.ToDictionary(wr => wr.Date.Date);

                var recordsToUpdate = new List<IdtWorkRecord>();
                var recordsToAdd = new List<IdtWorkRecord>();
                var expensesToDelete = new List<string>(); // WorkRecord ID'leri
                var allExpensesToAdd = new List<IdtWorkRecordExpense>();
                var resultWorkRecords = new List<WorkRecordDTO>();

                foreach (var createDTO in createWorkRecordDTOs)
                {
                    var dateKey = createDTO.Date.Date;

                    if (existingRecordsDict.TryGetValue(dateKey, out var existingWorkRecord))
                    {
                        // Onaylanmış kayıtları atla
                        if (existingWorkRecord.Status == WorkRecordStatus.Approved)
                        {
                            continue;
                        }

                        // Güncelleme için hazırla
                        existingWorkRecord.IsWeekend = createDTO.IsWeekend;
                        existingWorkRecord.StartTime = createDTO.StartTime;
                        existingWorkRecord.EndTime = createDTO.EndTime;
                        existingWorkRecord.Project = createDTO.Project;
                        existingWorkRecord.EquipmentId = createDTO.EquipmentId;
                        existingWorkRecord.Province = createDTO.Province;
                        existingWorkRecord.District = createDTO.District;
                        existingWorkRecord.HasBreakfast = createDTO.HasBreakfast;
                        existingWorkRecord.HasLunch = createDTO.HasLunch;
                        existingWorkRecord.HasDinner = createDTO.HasDinner;
                        existingWorkRecord.HasNightMeal = createDTO.HasNightMeal;
                        existingWorkRecord.Status = WorkRecordStatus.Pending; // Güncelleme sonrası tekrar pending yap

                        recordsToUpdate.Add(existingWorkRecord);

                        // Mevcut masrafları silmek için işaretle
                        expensesToDelete.Add(existingWorkRecord.Id);

                        // Yeni masrafları hazırla
                        if (createDTO.Expenses != null && createDTO.Expenses.Any())
                        {
                            var expenses = createDTO.Expenses.Select(exp =>
                            {
                                var expense = exp.Adapt<IdtWorkRecordExpense>();
                                expense.WorkRecordId = existingWorkRecord.Id;
                                return expense;
                            });
                            allExpensesToAdd.AddRange(expenses);
                        }

                        var resultDto = existingWorkRecord.Adapt<WorkRecordDTO>();
                        resultDto.Expenses = createDTO.Expenses?.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();
                        resultWorkRecords.Add(resultDto);
                    }
                    else
                    {
                        // Yeni kayıt oluşturmak için hazırla
                        var workRecord = createDTO.Adapt<IdtWorkRecord>();
                        workRecord.Status = WorkRecordStatus.Pending;

                        recordsToAdd.Add(workRecord);

                        // Sonuç için hazırla (expenses sonra eklenecek)
                        var resultDto = workRecord.Adapt<WorkRecordDTO>();
                        resultDto.Expenses = createDTO.Expenses?.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();
                        resultWorkRecords.Add(resultDto);
                    }
                }

                // Toplu güncelleme işlemleri
                if (recordsToUpdate.Any())
                {
                    await _unitOfWork.Repository<IdtWorkRecord>().UpdateRangeAsync(recordsToUpdate, cancellationToken);
                }

                // Toplu ekleme işlemleri
                IEnumerable<IdtWorkRecord> addedRecords = new List<IdtWorkRecord>();
                if (recordsToAdd.Any())
                {
                    addedRecords = await _unitOfWork.Repository<IdtWorkRecord>().AddRangeAsync(recordsToAdd, cancellationToken);
                }

                // Mevcut masrafları toplu silme
                if (expensesToDelete.Any())
                {
                    foreach (var workRecordId in expensesToDelete)
                    {
                        await _unitOfWork.Repository<IdtWorkRecordExpense>()
                            .DeleteRangeAsync(exp => exp.WorkRecordId == workRecordId, cancellationToken);
                    }
                }

                // Yeni eklenen kayıtlar için masrafları hazırla
                var addedRecordsList = addedRecords.ToList();
                for (int i = 0; i < addedRecordsList.Count; i++)
                {
                    var correspondingDTO = createWorkRecordDTOs
                        .Where(dto => !existingRecordsDict.ContainsKey(dto.Date.Date))
                        .Skip(i)
                        .FirstOrDefault();

                    if (correspondingDTO?.Expenses != null && correspondingDTO.Expenses.Any())
                    {
                        var expenses = correspondingDTO.Expenses.Select(exp =>
                        {
                            var expense = exp.Adapt<IdtWorkRecordExpense>();
                            expense.WorkRecordId = addedRecordsList[i].Id;
                            return expense;
                        });
                        allExpensesToAdd.AddRange(expenses);
                    }
                }

                // Tüm masrafları toplu ekleme
                if (allExpensesToAdd.Any())
                {
                    await _unitOfWork.Repository<IdtWorkRecordExpense>().AddRangeAsync(allExpensesToAdd, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Toplu iş kaydı işlemi başarıyla tamamlandı. İşlenen kayıt sayısı: {Count}, Güncellenen: {UpdatedCount}, Yeni eklenen: {AddedCount}, UserId: {UserId}",
                    resultWorkRecords.Count, recordsToUpdate.Count, recordsToAdd.Count, currentUserId);

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(resultWorkRecords,
                    $"Toplu iş kayıtları başarıyla işlendi. {recordsToUpdate.Count} kayıt güncellendi, {recordsToAdd.Count} yeni kayıt eklendi.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "BatchCreateWorkRecordsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Toplu iş kayıtları işlenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchUpdateWorkRecordByUserAsync(string userId, List<UpdateWorkRecordDTO> updateWorkRecordDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // Gelen DTO'lardan tarihleri al
                var dates = updateWorkRecordDTOs.Select(dto => dto.Date.Date).Distinct().ToList();

                // Kullanıcının bu tarihlerdeki mevcut kayıtlarını getir
                var existingWorkRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                    .GetWhereAsync(wr => dates.Contains(wr.Date.Date) && wr.CreatedBy == userId, cancellationToken);

                var existingRecordsDict = existingWorkRecords.ToDictionary(wr => wr.Date.Date);

                var recordsToUpdate = new List<IdtWorkRecord>();
                var expensesToDelete = new List<string>(); // WorkRecord ID'leri
                var allExpensesToAdd = new List<IdtWorkRecordExpense>();
                var resultWorkRecords = new List<WorkRecordDTO>();

                foreach (var updateDTO in updateWorkRecordDTOs)
                {
                    var dateKey = updateDTO.Date.Date; // DTO'dan gelen tarih kullanılıyor

                    if (existingRecordsDict.TryGetValue(dateKey, out var existingWorkRecord))
                    {
                        if (existingWorkRecord.Status == WorkRecordStatus.Approved)
                        {
                            _logger.LogWarning("Onaylanmış iş kaydı güncelleme girişimi. " +
                                "WorkRecordId: {WorkRecordId}, Status: {Status}",
                                existingWorkRecord.Id, existingWorkRecord.Status);
                            continue; // Bu kaydı atla, diğerlerini işlemeye devam et
                        }

                        // Değişiklik kontrolü - sadece değişen alanları güncelle
                        bool hasChanges = false;

                        if (existingWorkRecord.IsWeekend != updateDTO.IsWeekend ||
                            existingWorkRecord.StartTime != updateDTO.StartTime ||
                            existingWorkRecord.EndTime != updateDTO.EndTime ||
                            existingWorkRecord.Project != updateDTO.Project ||
                            existingWorkRecord.EquipmentId != updateDTO.EquipmentId ||
                            existingWorkRecord.Province != updateDTO.Province ||
                            existingWorkRecord.District != updateDTO.District ||
                            existingWorkRecord.HasBreakfast != updateDTO.HasBreakfast ||
                            existingWorkRecord.HasLunch != updateDTO.HasLunch ||
                            existingWorkRecord.HasDinner != updateDTO.HasDinner ||
                            existingWorkRecord.HasNightMeal != updateDTO.HasNightMeal)
                        {
                            hasChanges = true;
                        }

                        // Eğer değişiklik varsa güncelle
                        if (hasChanges)
                        {
                            existingWorkRecord.IsWeekend = updateDTO.IsWeekend;
                            existingWorkRecord.StartTime = updateDTO.StartTime;
                            existingWorkRecord.EndTime = updateDTO.EndTime;
                            existingWorkRecord.Project = updateDTO.Project;
                            existingWorkRecord.EquipmentId = updateDTO.EquipmentId;
                            existingWorkRecord.Province = updateDTO.Province;
                            existingWorkRecord.District = updateDTO.District;
                            existingWorkRecord.HasBreakfast = updateDTO.HasBreakfast;
                            existingWorkRecord.HasLunch = updateDTO.HasLunch;
                            existingWorkRecord.HasDinner = updateDTO.HasDinner;
                            existingWorkRecord.HasNightMeal = updateDTO.HasNightMeal;
                            recordsToUpdate.Add(existingWorkRecord);
                        }

                        // Expenses kontrolü ve güncellenmesi
                        if (updateDTO.Expenses != null)
                        {
                            // Mevcut expenses'leri sil ve yenilerini ekle
                            expensesToDelete.Add(existingWorkRecord.Id);

                            if (updateDTO.Expenses.Any())
                            {
                                var expenses = updateDTO.Expenses.Select(exp =>
                                {
                                    var expense = exp.Adapt<IdtWorkRecordExpense>();
                                    expense.WorkRecordId = existingWorkRecord.Id;
                                    expense.CreatedBy = _currentUserService.GetCurrentUserId();
                                    expense.CreatedDate = DateTime.UtcNow;
                                    return expense;
                                });
                                allExpensesToAdd.AddRange(expenses);
                            }
                        }

                        // Güncel expenses'leri getir ve DTO'ya ekle
                        var currentExpenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                            .GetWhereAsync(exp => exp.WorkRecordId == existingWorkRecord.Id, cancellationToken);

                        var workRecordDTO = existingWorkRecord.Adapt<WorkRecordDTO>();
                        workRecordDTO.Expenses = currentExpenses.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();

                        resultWorkRecords.Add(workRecordDTO);
                    }
                    else
                    {
                        // Kayıt bulunamadı, log'la ama işlemi durdurma
                        _logger.LogWarning("Güncellenmek istenen iş kaydı bulunamadı. UserId: {UserId}, Date: {Date}",
                            userId, updateDTO.Date);
                    }
                }

                // Toplu güncelleme işlemleri
                if (recordsToUpdate.Any())
                {
                    await _unitOfWork.Repository<IdtWorkRecord>().UpdateRangeAsync(recordsToUpdate, cancellationToken);
                }

                // Mevcut expenses'leri toplu silme
                if (expensesToDelete.Any())
                {
                    foreach (var workRecordId in expensesToDelete)
                    {
                        await _unitOfWork.Repository<IdtWorkRecordExpense>()
                            .DeleteRangeAsync(exp => exp.WorkRecordId == workRecordId, cancellationToken);
                    }
                }

                // Yeni expenses'leri toplu ekleme
                if (allExpensesToAdd.Any())
                {
                    await _unitOfWork.Repository<IdtWorkRecordExpense>().AddRangeAsync(allExpensesToAdd, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Toplu iş kaydı güncelleme işlemi başarıyla tamamlandı. UserId: {UserId}, Güncellenen kayıt sayısı: {Count}, GüncelleyenUserId: {UpdaterUserId}",
                    userId, recordsToUpdate.Count, _currentUserService.GetCurrentUserId());

                // Güncellenmiş kayıtları yeniden getir (expenses ile birlikte)
                var updatedRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                    .GetWhereAsync(wr => dates.Contains(wr.Date.Date) && wr.CreatedBy == userId, cancellationToken);

                var finalResult = new List<WorkRecordDTO>();
                foreach (var record in updatedRecords)
                {
                    var workRecordDTO = record.Adapt<WorkRecordDTO>();
                    var expenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                        .GetWhereAsync(exp => exp.WorkRecordId == record.Id, cancellationToken);
                    workRecordDTO.Expenses = expenses.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();
                    finalResult.Add(workRecordDTO);
                }

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(finalResult,
                    $"Toplu iş kaydı güncelleme işlemi başarıyla tamamlandı. {recordsToUpdate.Count} kayıt güncellendi.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "BatchUpdateWorkRecordByUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Toplu iş kaydı güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> BatchApproveWorkRecordsByUserAndMonthAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                    .GetWhereAsync(wr => wr.CreatedBy == userId &&
                                   wr.Date.Year == date.Date.Year &&
                                   wr.Date.Month == date.Date.Month, cancellationToken);

                var workRecordsList = workRecords.ToList();

                if (!workRecordsList.Any())
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<bool>.Error("Belirtilen tarih aralığında iş kaydı bulunamadı");
                }

                // Tüm kayıtları onaylı duruma getir
                foreach (var workRecord in workRecordsList)
                {
                    workRecord.Status = WorkRecordStatus.Approved;
                }

                await _unitOfWork.Repository<IdtWorkRecord>().UpdateRangeAsync(workRecordsList);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Toplu iş kaydı onaylandı. UserId: {UserId}, Year: {Year}, Month: {Month}, Count: {Count}, OnaylayanUserId: {ApproverUserId}",
                    userId, date.Date.Year, date.Date.Month, workRecordsList.Count, _currentUserService.GetCurrentUserId());

                return ApiResponse<bool>.Success(true, $"{workRecordsList.Count} adet iş kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "BatchApproveWorkRecordsByUserAndMonthAsync işleminde hata oluştu. UserId: {UserId}, Year: {Year}, Month: {Month}", userId, date.Date.Year, date.Date.Month);
                return ApiResponse<bool>.Error("Toplu iş kaydı onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> BatchRejectWorkRecordsByUserAndMonthAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                      .GetWhereAsync(wr => wr.CreatedBy == userId &&
                                     wr.Date.Year == date.Date.Year &&
                                     wr.Date.Month == date.Date.Month, cancellationToken);

                var workRecordsList = workRecords.ToList();

                if (!workRecordsList.Any())
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<bool>.Error("Belirtilen tarih aralığında beklemede olan iş kaydı bulunamadı");
                }

                // Tüm kayıtları reddedildi duruma getir
                foreach (var workRecord in workRecordsList)
                {
                    workRecord.Status = WorkRecordStatus.Rejected;
                }
                await _unitOfWork.Repository<IdtWorkRecord>().UpdateRangeAsync(workRecordsList);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Toplu iş kaydı reddedildi. UserId: {UserId}, Year: {Year}, Month: {Month}, Count: {Count}, ReddedenUserId: {RejecterUserId}",
                    userId, date.Date.Year, date.Date.Month, workRecordsList.Count, _currentUserService.GetCurrentUserId());

                return ApiResponse<bool>.Success(true, $"{workRecordsList.Count} adet iş kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "BatchRejectWorkRecordsByUserAndMonthAsync işleminde hata oluştu. UserId: {UserId}, Year: {Year}, Month: {Month}", userId, date.Date.Year, date.Date.Month);
                return ApiResponse<bool>.Error("Toplu iş kaydı reddedilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<int>> GetWorkRecordCountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _unitOfWork.Repository<IdtWorkRecord>().CountAsync(cancellationToken);
                return ApiResponse<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordCountAsync işleminde hata oluştu");
                return ApiResponse<int>.Error("İş kaydı sayısı hesaplanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<int>> GetWorkRecordCountByStatusAsync(WorkRecordStatus status, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _unitOfWork.Repository<IdtWorkRecord>()
                    .CountAsync(wr => wr.Status == status, cancellationToken);

                return ApiResponse<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordCountByStatusAsync işleminde hata oluştu. Status: {Status}", status);
                return ApiResponse<int>.Error("Duruma göre iş kaydı sayısı hesaplanırken hata oluştu");
            }
        }
    }
}