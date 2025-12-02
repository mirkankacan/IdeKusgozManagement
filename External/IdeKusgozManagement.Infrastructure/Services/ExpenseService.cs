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
        public async Task<ServiceResponse<IEnumerable<ExpenseDTO>>> GetExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<IdtExpense>? expenses = (await unitOfWork.GetRepository<IdtExpense>().GetAllAsync(cancellationToken)).OrderByDescending(e => e.CreatedDate);

                var expenseDTOs = expenses.Adapt<IEnumerable<ExpenseDTO>>();

                return ServiceResponse<IEnumerable<ExpenseDTO>>.Success(expenseDTOs, "Masraf türü listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetExpensesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<ExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetFirstOrDefaultAsync(x => x.Id == expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResponse<ExpenseDTO>.Error("Masraf türü bulunamadı");
                }

                var expenseDTO = expense.Adapt<ExpenseDTO>();

                return ServiceResponse<ExpenseDTO>.Success(expenseDTO, "Masraf türü başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetExpenseByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingExpense = await unitOfWork.GetRepository<IdtExpense>().AnyAsync(e => e.Name.ToLower() == createExpenseDTO.Name.ToLower(), cancellationToken);

                if (existingExpense)
                {
                    return ServiceResponse<string>.Error("Bu isimde bir masraf türü zaten mevcut");
                }

                var expense = createExpenseDTO.Adapt<IdtExpense>();
                expense.IsActive = true;
                await unitOfWork.GetRepository<IdtExpense>().AddAsync(expense, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<string>.Success(expense.Id, "Masraf türü başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResponse<bool>.Error("Masraf türü bulunamadı");
                }

                var existingExpense = await unitOfWork.GetRepository<IdtExpense>().AnyAsync(e => e.Name.ToLower() == updateExpenseDTO.Name.ToLower() && e.Id != expenseId, cancellationToken);

                if (existingExpense)
                {
                    return ServiceResponse<bool>.Error("Bu isimde başka bir masraf türü zaten mevcut");
                }

                updateExpenseDTO.Adapt(expense);
                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Masraf türü başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResponse<bool>.Error("Masraf türü bulunamadı");
                }

                var isExpenseUsed = await unitOfWork.GetRepository<IdtWorkRecordExpense>().AnyAsync(wre => wre.ExpenseId == expense.Id, cancellationToken);

                if (isExpenseUsed)
                {
                    return ServiceResponse<bool>.Error("Bu masraf türü iş kayıtlarında kullanıldığı için silinemez");
                }

                unitOfWork.GetRepository<IdtExpense>().Remove(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Masraf türü başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<ExpenseDTO>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var expenses = await unitOfWork.GetRepository<IdtExpense>().Where(e => e.IsActive == true).OrderBy(e => e.Name).ToListAsync(cancellationToken);

                var expenseDTOs = expenses
                    .Adapt<IEnumerable<ExpenseDTO>>();

                return ServiceResponse<IEnumerable<ExpenseDTO>>.Success(expenseDTOs, "Aktif masraf türü listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveExpensesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResponse<bool>.Error("Masraf türü bulunamadı");
                }

                expense.IsActive = true;

                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Masraf türü başarıyla aktif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResponse<bool>.Error("Masraf türü bulunamadı");
                }

                expense.IsActive = false;

                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Masraf türü başarıyla pasif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableExpenseAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}