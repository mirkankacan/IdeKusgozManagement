using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class WorkRecordExpenseService(IUnitOfWork unitOfWork, IFileService fileService, ILogger<WorkRecordExpenseService> logger, IIdentityService identityService) : IWorkRecordExpenseService
    {
        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchCreateOrModifyWorkRecordExpensesAsync(IEnumerable<CreateModifyWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();
                var workRecordIds = expenseDTOs.Select(x => x.WorkRecordId).Distinct().ToList();

                var existingExpenses = await unitOfWork.GetRepository<IdtWorkRecordExpense>()
                 .Where(x => workRecordIds.Contains(x.WorkRecordId) && x.CreatedBy == userId)
                 .Include(x => x.File)
                 .ToListAsync(cancellationToken);

                var expensesToCreate = new List<IdtWorkRecordExpense>();
                var expensesToModify = new List<IdtWorkRecordExpense>();
                var expensesToRemove = new List<IdtWorkRecordExpense>();
                foreach (var dto in expenseDTOs)
                {
                    var existingRecord = existingExpenses.FirstOrDefault(x => x.WorkRecordId == dto.WorkRecordId);

                    if (existingRecord != null)
                    {
                        // ========== GÜNCELLEME ==========
                        if (string.IsNullOrEmpty(dto.ExpenseId) && dto.Amount <= 0)
                        {
                            var removeDTO = dto.Adapt<IdtWorkRecordExpense>();
                            expensesToRemove.Add(removeDTO);
                        }
                        existingRecord.ExpenseId = dto.ExpenseId;
                        existingRecord.Description = dto.Description ?? null;
                        existingRecord.Amount = dto.Amount;

                        string fileId = existingRecord.FileId; // Mevcut dosyayı koru

                        // Yeni dosya varsa
                        if (dto.File != null && dto.File.FormFile != null && dto.File.FormFile.Length > 0)
                        {
                            // Eski dosyayı sil
                            if (!string.IsNullOrEmpty(existingRecord.FileId))
                            {
                                await fileService.DeleteFileAsync(existingRecord.FileId, cancellationToken);
                            }

                            // Yeni dosyayı yükle
                            dto.File.TargetUserId = userId;
                            var fileResult = await fileService.UploadFileAsync(dto.File, cancellationToken);
                            if (!fileResult.IsSuccess)
                            {
                                throw new Exception($"Dosya yüklenirken hata oluştu: {fileResult.Message}");
                            }
                            fileId = fileResult.Data.Id;
                        }

                        existingRecord.FileId = fileId;
                        expensesToModify.Add(existingRecord);
                    }
                    else
                    {
                        // ========== YENİ KAYIT ==========
                        string fileId = null;

                        if (dto.File != null && dto.File.FormFile != null && dto.File.FormFile.Length > 0)
                        {
                            dto.File.TargetUserId = userId;
                            var fileResult = await fileService.UploadFileAsync(dto.File, cancellationToken);
                            if (!fileResult.IsSuccess)
                            {
                                throw new Exception($"Dosya yüklenirken hata oluştu: {fileResult.Message}");
                            }
                            fileId = fileResult.Data.Id;
                        }

                        var newRecord = new IdtWorkRecordExpense
                        {
                            WorkRecordId = dto.WorkRecordId,
                            ExpenseId = dto.ExpenseId,
                            Description = dto.Description ?? null,
                            Amount = dto.Amount,
                            FileId = fileId
                        };
                        expensesToCreate.Add(newRecord);
                    }
                }

                // Veritabanı işlemleri

                if (expensesToRemove.Any())
                {
                    unitOfWork.GetRepository<IdtWorkRecordExpense>().RemoveRange(expensesToRemove);
                }
                if (expensesToModify.Any())
                {
                    unitOfWork.GetRepository<IdtWorkRecordExpense>().UpdateRange(expensesToModify);
                }
                if (expensesToCreate.Any())
                {
                    await unitOfWork.GetRepository<IdtWorkRecordExpense>().AddRangeAsync(expensesToCreate, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                // Tüm işlem yapılan kayıtları döndür
                var allProcessedExpenses = new List<IdtWorkRecordExpense>();
                allProcessedExpenses.AddRange(expensesToModify);
                allProcessedExpenses.AddRange(expensesToCreate);

                var mappedExpenses = allProcessedExpenses.Adapt<IEnumerable<WorkRecordExpenseDTO>>();

                logger.LogInformation("Masraf kayıtları işlendi. ModifiedCount: {ModifiedCount}, AddedCount: {AddedCount}, UserId: {UserId}",
                    expensesToModify.Count, expensesToCreate.Count, userId);

                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Success(mappedExpenses,
                    $"Masraf kayıtları işlendi. {expensesToModify.Count} güncellendi, {expensesToCreate.Count} eklendi.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "BatchCreateOrModifyWorkRecordExpensesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ApiResponse<bool>> BatchDeleteWorkRecordExpensesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                List<IdtWorkRecordExpense> expensesToDelete = new();

                foreach (var id in ids)
                {
                    var expense = await unitOfWork.GetRepository<IdtWorkRecordExpense>().GetByIdAsync(id, cancellationToken);
                    if (expense == null)
                    {
                        logger.LogWarning("Silinecek masraf kaydı bulunamadı. ExpenseId: {Id}", id);
                        continue;
                    }

                    // İlgili dosyayı sil
                    if (!string.IsNullOrEmpty(expense.FileId))
                    {
                        await fileService.DeleteFileAsync(expense.FileId, cancellationToken);
                    }

                    expensesToDelete.Add(expense);
                }

                unitOfWork.GetRepository<IdtWorkRecordExpense>().RemoveRange(expensesToDelete);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Masraf kayıtları silindi. DeletedCount: {DeletedCount}", expensesToDelete.Count);

                return ApiResponse<bool>.Success(true, $"{expensesToDelete.Count} adet harcama kaydı başarıyla silindi.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Masraf kayıtları silinirken hata oluştu");
                throw;
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchUpdateWorkRecordExpensesAsync(IEnumerable<UpdateWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                var expensesToUpdate = new List<IdtWorkRecordExpense>();
                var userId = identityService.GetUserId();

                foreach (var dto in expenseDTOs)
                {
                    // Mevcut expense'i bul
                    var existingExpense = await unitOfWork.GetRepository<IdtWorkRecordExpense>()
                        .GetByIdAsync(dto.Id, cancellationToken);

                    if (existingExpense == null)
                    {
                        logger.LogWarning("Güncellenecek masraf kaydı bulunamadı. ExpenseId: {Id}", dto.Id);
                        continue;
                    }

                    string fileId = existingExpense.FileId; // Mevcut fileId'yi koru

                    // Yeni dosya yüklendiyse
                    if (dto.File != null && dto.File.FormFile != null && dto.File.FormFile.Length > 0)
                    {
                        // Eski dosyayı sil
                        if (!string.IsNullOrEmpty(existingExpense.FileId))
                        {
                            await fileService.DeleteFileAsync(existingExpense.FileId, cancellationToken);
                        }

                        // Yeni dosyayı yükle
                        dto.File.TargetUserId = userId;
                        var fileResult = await fileService.UploadFileAsync(dto.File, cancellationToken);
                        if (fileResult.IsSuccess)
                        {
                            fileId = fileResult.Data.Id;
                        }
                        else
                        {
                            throw new Exception($"Dosya yüklenirken hata oluştu: {fileResult.Message}");
                        }
                    }

                    // Expense'i güncelle
                    existingExpense.ExpenseId = dto.ExpenseId;
                    existingExpense.Description = dto.Description;
                    existingExpense.Amount = dto.Amount;
                    existingExpense.FileId = fileId;

                    expensesToUpdate.Add(existingExpense);
                }

                unitOfWork.GetRepository<IdtWorkRecordExpense>().UpdateRange(expensesToUpdate);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var mappedExpenses = expensesToUpdate.Adapt<IEnumerable<WorkRecordExpenseDTO>>();

                logger.LogInformation("Masraf kayıtları güncellendi. Count: {Count}", expensesToUpdate.Count);

                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Success(mappedExpenses, $"Masraf kayıtları başarıyla güncellendi. Count:{expensesToUpdate.Count}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Masraf kayıtları güncellenirken hata oluştu");
                throw;
            }
        }
    }
}