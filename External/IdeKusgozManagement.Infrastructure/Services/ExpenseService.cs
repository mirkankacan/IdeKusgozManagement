using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.Repositories;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExpenseService> _logger;

        public ExpenseService(IUnitOfWork unitOfWork, ILogger<ExpenseService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<ExpenseDTO>>> GetExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var expenses = await _unitOfWork.Repository<IdtExpense>().GetAllNoTrackingAsync(cancellationToken);

                var expenseDTOs = expenses
                    .Adapt<IEnumerable<ExpenseDTO>>()
                    .OrderByDescending(e => e.CreatedDate);

                return ApiResponse<IEnumerable<ExpenseDTO>>.Success(expenseDTOs, "Masraf türü listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetExpensesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<ExpenseDTO>>.Error("Masraf türü listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<ExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtExpense>()
                    .GetByIdNoTrackingAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<ExpenseDTO>.Error("Masraf türü bulunamadı");
                }

                var expenseDTO = expense.Adapt<ExpenseDTO>();

                return ApiResponse<ExpenseDTO>.Success(expenseDTO, "Masraf türü başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetExpenseByIdAsync işleminde hata oluştu");
                return ApiResponse<ExpenseDTO>.Error("Masraf türü getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if expense with same name already exists
                var existingExpense = await _unitOfWork.Repository<IdtExpense>()
                    .FirstOrDefaultNoTrackingAsync(e => e.Name.ToLower() == createExpenseDTO.Name.ToLower(), cancellationToken);

                if (existingExpense != null)
                {
                    return ApiResponse<string>.Error("Bu isimde bir masraf türü zaten mevcut");
                }

                var expense = createExpenseDTO.Adapt<IdtExpense>();
                expense.Name = expense.Name.Trim();

                await _unitOfWork.Repository<IdtExpense>().AddAsync(expense, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<string>.Success(expense.Id, "Masraf türü başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateExpenseAsync işleminde hata oluştu");
                return ApiResponse<string>.Error("Masraf türü oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtExpense>()
                    .GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                // Check if another expense with same name already exists
                var existingExpense = await _unitOfWork.Repository<IdtExpense>()
                    .FirstOrDefaultNoTrackingAsync(e => e.Name.ToLower() == updateExpenseDTO.Name.ToLower() && e.Id != expenseId, cancellationToken);

                if (existingExpense != null)
                {
                    return ApiResponse<bool>.Error("Bu isimde başka bir masraf türü zaten mevcut");
                }

                updateExpenseDTO.Adapt(expense);
                expense.Name = expense.Name.Trim();

                await _unitOfWork.Repository<IdtExpense>().UpdateAsync(expense, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Masraf türü başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateExpenseAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Masraf türü güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtExpense>()
                    .GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                // Check if expense is used in any work record expenses
                var isExpenseUsed = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                    .AnyNoTrackingAsync(wre => wre.ExpenseId == expense.Id, cancellationToken);

                if (isExpenseUsed)
                {
                    return ApiResponse<bool>.Error("Bu masraf türü iş kayıtlarında kullanıldığı için silinemez");
                }

                await _unitOfWork.Repository<IdtExpense>().DeleteAsync(expense, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Masraf türü başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteExpenseAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Masraf türü silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<ExpenseDTO>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var expenses = await _unitOfWork.Repository<IdtExpense>().GetWhereNoTrackingAsync(x => x.IsActive == true, cancellationToken);

                var expenseDTOs = expenses
                    .Adapt<IEnumerable<ExpenseDTO>>()
                    .OrderBy(e => e.Name);

                return ApiResponse<IEnumerable<ExpenseDTO>>.Success(expenseDTOs, "Aktif masraf türü listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveExpensesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<ExpenseDTO>>.Error("Aktif masraf türü listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtExpense>()
                    .GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                expense.IsActive = true;

                await _unitOfWork.Repository<IdtExpense>().UpdateAsync(expense, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Masraf türü başarıyla aktif duruma getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnableExpenseAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Masraf türü aktif duruma getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtExpense>()
                    .GetByIdAsync(expenseId, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                expense.IsActive = false;

                await _unitOfWork.Repository<IdtExpense>().UpdateAsync(expense, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Masraf türü başarıyla pasif duruma getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DisableExpenseAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Masraf türü pasif duruma getirilirken hata oluştu");
            }
        }
    }
}