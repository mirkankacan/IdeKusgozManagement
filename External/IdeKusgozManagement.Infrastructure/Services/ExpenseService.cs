using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces;
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

        public async Task<ApiResponse<IEnumerable<ExpenseListDTO>>> GetAllExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var expenses = await _unitOfWork.Repository<IdtExpense>().GetAllAsync(cancellationToken, e => e.CreatedBy);

                var expenseDTOs = expenses
                    .Adapt<IEnumerable<ExpenseListDTO>>()
                    .OrderBy(e => e.Name);

                return ApiResponse<IEnumerable<ExpenseListDTO>>.Success(expenseDTOs, "Masraf türü listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Masraf türü listesi getirilirken hata oluştu");
                return ApiResponse<IEnumerable<ExpenseListDTO>>.Error("Masraf türü listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<ExpenseDetailDTO>> GetExpenseByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtExpense>()
                    .GetByIdAsync(id, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<ExpenseDetailDTO>.Error("Masraf türü bulunamadı");
                }

                var expenseDTO = expense.Adapt<ExpenseDetailDTO>();

                return ApiResponse<ExpenseDetailDTO>.Success(expenseDTO, "Masraf türü başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Masraf türü getirilirken hata oluştu. Id: {Id}", id);
                return ApiResponse<ExpenseDetailDTO>.Error("Masraf türü getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if expense with same name already exists
                var existingExpense = await _unitOfWork.Repository<IdtExpense>()
                    .FirstOrDefaultAsync(e => e.Name.ToLower() == createExpenseDTO.Name.ToLower(), cancellationToken);

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
                _logger.LogError(ex, "Masraf türü oluşturulurken hata oluştu. Name: {Name}", createExpenseDTO.Name);
                return ApiResponse<string>.Error("Masraf türü oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> UpdateExpenseAsync(string id, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtExpense>()
                    .GetByIdAsync(id, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                // Check if another expense with same name already exists
                var existingExpense = await _unitOfWork.Repository<IdtExpense>()
                    .FirstOrDefaultAsync(e => e.Name.ToLower() == updateExpenseDTO.Name.ToLower() && e.Id != id, cancellationToken);

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
                _logger.LogError(ex, "Masraf türü güncellenirken hata oluştu. Id: {Id}", id);
                return ApiResponse<bool>.Error("Masraf türü güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteExpenseAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtExpense>()
                    .GetByIdAsync(id, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf türü bulunamadı");
                }

                // Check if expense is used in any work record expenses
                var isExpenseUsed = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                    .AnyAsync(wre => wre.Expense == expense.Name, cancellationToken);

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
                _logger.LogError(ex, "Masraf türü silinirken hata oluştu. Id: {Id}", id);
                return ApiResponse<bool>.Error("Masraf türü silinirken hata oluştu");
            }
        }
    }
}