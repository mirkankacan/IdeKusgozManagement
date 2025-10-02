using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class ExpenseService(IUnitOfWork unitOfWork, ILogger<ExpenseService> logger) : IExpenseService
    {
        public async Task<ApiResponse<IEnumerable<ExpenseDTO>>> GetExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IOrderedEnumerable<IdtExpense>? expenses = (await unitOfWork.GetRepository<IdtExpense>().GetAllAsync(cancellationToken)).OrderByDescending(e => e.CreatedDate);

                var expenseDTOs = expenses.Adapt<IEnumerable<ExpenseDTO>>();

                return ApiResponse<IEnumerable<ExpenseDTO>>.Success(expenseDTOs, "Masraf türü listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetExpensesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<ExpenseDTO>>.Error("Masraf türü listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<ExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetFirstOrDefaultAsync(x => x.Id == expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<ExpenseDTO>.Error("Masraf türü bulunamadı");
                }

                var expenseDTO = expense.Adapt<ExpenseDTO>();

                return ApiResponse<ExpenseDTO>.Success(expenseDTO, "Masraf türü başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetExpenseByIdAsync işleminde hata oluştu");
                return ApiResponse<ExpenseDTO>.Error("Masraf türü getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingExpense = await unitOfWork.GetRepository<IdtExpense>().GetFirstOrDefaultAsync(e => e.Name.ToLower() == createExpenseDTO.Name.ToLower(), cancellationToken);

                if (existingExpense != null)
                {
                    return ApiResponse<string>.Error("Bu isimde bir masraf türü zaten mevcut");
                }

                var expense = createExpenseDTO.Adapt<IdtExpense>();

                await unitOfWork.GetRepository<IdtExpense>().AddAsync(expense, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<string>.Success(expense.Id, "Masraf türü başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateExpenseAsync işleminde hata oluştu");
                return ApiResponse<string>.Error("Masraf türü oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                var existingExpense = await unitOfWork.GetRepository<IdtExpense>().AnyAsync(e => e.Name.ToLower() == updateExpenseDTO.Name.ToLower() && e.Id != expenseId, cancellationToken);

                if (existingExpense)
                {
                    return ApiResponse<bool>.Error("Bu isimde başka bir masraf türü zaten mevcut");
                }

                updateExpenseDTO.Adapt(expense);
                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Masraf türü başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateExpenseAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Masraf türü güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                var isExpenseUsed = await unitOfWork.GetRepository<IdtWorkRecordExpense>().AnyAsync(wre => wre.ExpenseId == expense.Id, cancellationToken);

                if (isExpenseUsed)
                {
                    return ApiResponse<bool>.Error("Bu masraf türü iş kayıtlarında kullanıldığı için silinemez");
                }

                unitOfWork.GetRepository<IdtExpense>().Remove(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Masraf türü başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteExpenseAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Masraf türü silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<ExpenseDTO>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var expenses = await unitOfWork.GetRepository<IdtExpense>().Where(e => e.IsActive == true).OrderBy(e => e.Name).ToListAsync(cancellationToken);

                var expenseDTOs = expenses
                    .Adapt<IEnumerable<ExpenseDTO>>();

                return ApiResponse<IEnumerable<ExpenseDTO>>.Success(expenseDTOs, "Aktif masraf türü listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveExpensesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<ExpenseDTO>>.Error("Aktif masraf türü listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                expense.IsActive = true;

                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Masraf türü başarıyla aktif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableExpenseAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Masraf türü aktif duruma getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                expense.IsActive = false;

                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Masraf türü başarıyla pasif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableExpenseAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Masraf türü pasif duruma getirilirken hata oluştu");
            }
        }
    }
}