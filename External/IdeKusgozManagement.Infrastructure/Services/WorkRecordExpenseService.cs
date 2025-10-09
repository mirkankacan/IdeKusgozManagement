using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class WorkRecordExpenseService(IUnitOfWork unitOfWork, IFileService fileService, ILogger<WorkRecordExpenseService> logger, IIdentityService identityService) : IWorkRecordExpenseService
    {
        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchCreateWorkRecordExpensesAsync(IEnumerable<CreateWorkRecordExpenseDTO> createWorkRecordExpenseDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                var expensesToCreate = new List<IdtWorkRecordExpense>();
                var userId = identityService.GetUserId();

                foreach (var dto in createWorkRecordExpenseDTOs)
                {
                    string fileId = null;
                    if (dto.File != null && dto.File.FormFile != null && dto.File.FormFile.Length > 0)
                    {
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

                    var expense = new IdtWorkRecordExpense
                    {
                        WorkRecordId = dto.WorkRecordId!,
                        ExpenseId = dto.ExpenseId,
                        Description = dto.Description,
                        Amount = dto.Amount,
                        FileId = fileId,
                    };

                    expensesToCreate.Add(expense);
                }
                await unitOfWork.GetRepository<IdtWorkRecordExpense>().AddRangeAsync(expensesToCreate, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var mappedExpenses = expensesToCreate.Adapt<IEnumerable<WorkRecordExpenseDTO>>();

                logger.LogInformation("Masraf kayıtları işlendi. Count: {Count}", expensesToCreate.Count);

                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Success(mappedExpenses, $"Masraf kayıtları başarıyla işlendi. Count:{expensesToCreate.Count}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Masraf kayıtları oluşturulurken hata oluştu");
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

        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchUpdateWorkRecordExpensesAsync(IEnumerable<UpdateWorkRecordExpenseDTO> updateWorkRecordExpenseDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                var expensesToCreate = new List<IdtWorkRecordExpense>();
                var userId = identityService.GetUserId();

                foreach (var dto in updateWorkRecordExpenseDTOs)
                {
                    string fileId = null;
                    if (dto.File != null && dto.File.FormFile != null && dto.File.FormFile.Length > 0)
                    {
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

                    var expense = new IdtWorkRecordExpense
                    {
                        WorkRecordId = dto.WorkRecordId!,
                        ExpenseId = dto.ExpenseId,
                        Description = dto.Description,
                        Amount = dto.Amount,
                        FileId = fileId,
                    };

                    expensesToCreate.Add(expense);
                }
                await unitOfWork.GetRepository<IdtWorkRecordExpense>().AddRangeAsync(expensesToCreate, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var mappedExpenses = expensesToCreate.Adapt<IEnumerable<WorkRecordExpenseDTO>>();

                logger.LogInformation("Masraf kayıtları işlendi. Count: {Count}", expensesToCreate.Count);

                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Success(mappedExpenses, $"Masraf kayıtları başarıyla işlendi. Count:{expensesToCreate.Count}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Masraf kayıtları oluşturulurken hata oluştu");
                throw;
            }
        }
    }
}