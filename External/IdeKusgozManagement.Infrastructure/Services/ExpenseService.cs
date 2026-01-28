using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class ExpenseService(IUnitOfWork unitOfWork, ILogger<ExpenseService> logger) : IExpenseService
    {
        public async Task<ServiceResult<IEnumerable<ExpenseDTO>>> GetExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<IdtExpense>? expenses = (await unitOfWork.GetRepository<IdtExpense>().GetAllAsync(cancellationToken)).OrderByDescending(e => e.CreatedDate);

                var expenseDTOs = expenses.Adapt<IEnumerable<ExpenseDTO>>();

                return ServiceResult<IEnumerable<ExpenseDTO>>.SuccessAsOk(expenseDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetExpensesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<ExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetFirstOrDefaultAsync(x => x.Id == expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResult<ExpenseDTO>.Error("Masraf Türü Bulunamadı", "Belirtilen ID'ye sahip masraf türü bulunamadı.", HttpStatusCode.NotFound);
                }

                var expenseDTO = expense.Adapt<ExpenseDTO>();

                return ServiceResult<ExpenseDTO>.SuccessAsOk(expenseDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetExpenseByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingExpense = await unitOfWork.GetRepository<IdtExpense>().AnyAsync(e => e.Name.ToLower() == createExpenseDTO.Name.ToLower(), cancellationToken);

                if (existingExpense)
                {
                    return ServiceResult<string>.Error("Masraf Türü Zaten Mevcut", "Bu isimde bir masraf türü zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                var expense = createExpenseDTO.Adapt<IdtExpense>();
                expense.IsActive = true;
                await unitOfWork.GetRepository<IdtExpense>().AddAsync(expense, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<string>.SuccessAsCreated(expense.Id, $"/api/expenses/{expense.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResult<bool>.Error("Masraf Türü Bulunamadı", "Belirtilen ID'ye sahip masraf türü bulunamadı.", HttpStatusCode.NotFound);
                }

                var existingExpense = await unitOfWork.GetRepository<IdtExpense>().AnyAsync(e => e.Name.ToLower() == updateExpenseDTO.Name.ToLower() && e.Id != expenseId, cancellationToken);

                if (existingExpense)
                {
                    return ServiceResult<bool>.Error("Masraf Türü Zaten Mevcut", "Bu isimde başka bir masraf türü zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                updateExpenseDTO.Adapt(expense);
                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResult<bool>.Error("Masraf Türü Bulunamadı", "Belirtilen ID'ye sahip masraf türü bulunamadı.", HttpStatusCode.NotFound);
                }

                var isExpenseUsed = await unitOfWork.GetRepository<IdtWorkRecordExpense>().AnyAsync(wre => wre.ExpenseId == expense.Id, cancellationToken);

                if (isExpenseUsed)
                {
                    return ServiceResult<bool>.Error("Silme İşlemi Başarısız", "Bu masraf türü iş kayıtlarında kullanıldığı için silinemez.", HttpStatusCode.BadRequest);
                }

                unitOfWork.GetRepository<IdtExpense>().Remove(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<ExpenseDTO>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var expenses = await unitOfWork.GetRepository<IdtExpense>().Where(e => e.IsActive == true).OrderBy(e => e.Name).ToListAsync(cancellationToken);

                var expenseDTOs = expenses
                    .Adapt<IEnumerable<ExpenseDTO>>();

                return ServiceResult<IEnumerable<ExpenseDTO>>.SuccessAsOk(expenseDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveExpensesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResult<bool>.Error("Masraf Türü Bulunamadı", "Belirtilen ID'ye sahip masraf türü bulunamadı.", HttpStatusCode.NotFound);
                }

                expense.IsActive = true;

                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await unitOfWork.GetRepository<IdtExpense>().GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ServiceResult<bool>.Error("Masraf Türü Bulunamadı", "Belirtilen ID'ye sahip masraf türü bulunamadı.", HttpStatusCode.NotFound);
                }

                expense.IsActive = false;

                unitOfWork.GetRepository<IdtExpense>().Update(expense);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableExpenseAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<ExpenseDTO>>> GetExpensesByTypeAsync(ExpenseType expenseType, CancellationToken cancellationToken = default)
        {
            try
            {
                var expenses = await unitOfWork.GetRepository<IdtExpense>()
                    .Where(e => e.ExpenseType == expenseType)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToListAsync(cancellationToken);

                var expenseDTOs = expenses.Adapt<IEnumerable<ExpenseDTO>>();

                return ServiceResult<IEnumerable<ExpenseDTO>>.SuccessAsOk(expenseDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetExpensesByTypeAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}