using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
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
    public class WorkRecordExpenseService(IUnitOfWork unitOfWork, IFileService fileService, ILogger<WorkRecordExpenseService> logger, IIdentityService identityService) : IWorkRecordExpenseService
    {
        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchCreateOrModifyWorkRecordExpensesAsync(IEnumerable<CreateOrModifyWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = identityService.GetUserId();

                var workRecordIds = expenseDTOs.Select(e => e.WorkRecordId).Distinct().ToList();
                var existingExpenses = await unitOfWork.GetRepository<IdtWorkRecordExpense>()
                 .Where(x => workRecordIds.Contains(x.WorkRecordId) && x.CreatedBy == userId)
                 .ToListAsync(cancellationToken);

                var expensesToCreate = new List<IdtWorkRecordExpense>();
                var expensesToUpdate = new List<IdtWorkRecordExpense>();
                var expensesToDelete = new List<IdtWorkRecordExpense>();
                var filesToDelete = new List<string>();

                // UI'dan gelen expense ID'leri (sadece ID'si olan kayıtlar)
                var incomingExpenseIds = expenseDTOs
                    .Where(e => !string.IsNullOrEmpty(e.Id))
                    .Select(e => e.Id)
                    .ToHashSet();

                // Silinmesi gereken expense'leri bul
                // Veritabanında var ama UI'dan gelen listede yok
                expensesToDelete = existingExpenses
                    .Where(existing => !incomingExpenseIds.Contains(existing.Id))
                    .ToList();

                // Silinecek expense'lerin dosyalarını topla
                filesToDelete.AddRange(expensesToDelete
                    .Where(e => !string.IsNullOrEmpty(e.FileId))
                    .Select(e => e.FileId!)
                    .ToList());

                foreach (var element in expenseDTOs)
                {
                    IdtWorkRecordExpense? existingExpense = null;

                    if (!string.IsNullOrEmpty(element.Id))
                    {
                        existingExpense = existingExpenses.FirstOrDefault(x => x.WorkRecordId == element.WorkRecordId && x.Id == element.Id && x.CreatedBy == userId);
                    }

                    // ========== VAROLAN GÜNCELLEME ==========
                    if (existingExpense is not null)
                    {
                        existingExpense.ExpenseId = element.ExpenseId;
                        existingExpense.Amount = element.Amount;
                        existingExpense.Description = element.Description ?? null;

                        if (element.File != null && element.File.FormFile != null)
                        {
                            string? oldFileId = existingExpense.FileId ?? null;

                            // Yeni dosyayı yükle
                            element.File.TargetUserId = userId;
                            element.File.FileType = FileType.Expense;
                            var fileUploadServiceResult = await fileService.UploadFileAsync(element.File, cancellationToken);

                            if (!fileUploadServiceResult.IsSuccess)
                                throw new InvalidOperationException(message: $"Dosya yüklenirken hata oluştu: {fileUploadServiceResult.Message}");

                            existingExpense.FileId = fileUploadServiceResult.Data.Id;

                            // Eski dosyayı sil (yeni dosya başarıyla yüklendikten sonra)
                            if (!string.IsNullOrEmpty(oldFileId))
                            {
                                filesToDelete.Add(oldFileId);
                            }
                        }
                        expensesToUpdate.Add(existingExpense);
                    }
                    // ========== YENİ KAYIT ==========
                    else
                    {
                        string? fileId = null;

                        if (element.File != null && element.File.FormFile != null)
                        {
                            element.File.TargetUserId = userId;
                            element.File.FileType = FileType.Expense;
                            var fileUploadResult = await fileService.UploadFileAsync(element.File, cancellationToken);

                            if (!fileUploadResult.IsSuccess)
                                throw new InvalidOperationException($"Dosya yüklenirken hata oluştu: {fileUploadResult.Message}");

                            fileId = fileUploadResult.Data.Id;
                        }

                        var newExpense = new IdtWorkRecordExpense
                        {
                            WorkRecordId = element.WorkRecordId!,
                            ExpenseId = element.ExpenseId,
                            Description = element.Description ?? null,
                            Amount = element.Amount,
                            FileId = fileId
                        };
                        expensesToCreate.Add(newExpense);
                    }
                }

                // Database işlemleri
                if (expensesToDelete.Any())
                {
                    unitOfWork.GetRepository<IdtWorkRecordExpense>().RemoveRange(expensesToDelete);
                }

                if (expensesToUpdate.Any())
                {
                    unitOfWork.GetRepository<IdtWorkRecordExpense>().UpdateRange(expensesToUpdate);
                }

                if (expensesToCreate.Any())
                {
                    await unitOfWork.GetRepository<IdtWorkRecordExpense>().AddRangeAsync(expensesToCreate, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                // Dosya silme işlemleri (database işlemleri başarılı olduktan sonra)
                foreach (var fileId in filesToDelete.Distinct())
                {
                    var fileDeleteResult = await fileService.DeleteFileAsync(fileId, cancellationToken);
                    if (!fileDeleteResult.IsSuccess)
                    {
                        logger.LogWarning("Dosya silinirken hata oluştu. FileId: {FileId}, Error: {Error}",
                            fileId, fileDeleteResult.Message);
                    }
                }

                var allExpenses = expensesToCreate.Concat(expensesToUpdate);
                var mappedExpenses = allExpenses.Adapt<IEnumerable<WorkRecordExpenseDTO>>();

                var updatedCount = expensesToUpdate.Count;
                var createdCount = expensesToCreate.Count;
                var deletedCount = expensesToDelete.Count;

                logger.LogInformation("Puantaj masraf kayıtları işlendi. CreatedCount: {CreatedCount}, UpdatedCount: {UpdatedCount}, DeletedCount: {DeletedCount}, UserId: {UserId}",
                    createdCount, updatedCount, deletedCount, userId);

                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Success(mappedExpenses,
                    $"Puantaj masraf kayıtları işlendi. {createdCount} eklendi, {updatedCount} güncellendi, {deletedCount} silindi.");
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