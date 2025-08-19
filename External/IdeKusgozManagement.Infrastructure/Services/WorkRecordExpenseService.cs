using System.Security.Claims;
using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class WorkRecordExpenseService : IWorkRecordExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<WorkRecordExpenseService> _logger;

        public WorkRecordExpenseService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<WorkRecordExpenseService> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponse<WorkRecordExpenseDTO>> AddExpenseToWorkRecordAsync(string workRecordId, CreateWorkRecordExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // İş kaydının varlığını kontrol et
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(workRecordId, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<WorkRecordExpenseDTO>.Error("İş kaydı bulunamadı");
                }

                // Sadece Pending durumundaki iş kayıtlarına masraf eklenebilir
                if (workRecord.Status != WorkRecordStatus.Pending)
                {
                    return ApiResponse<WorkRecordExpenseDTO>.Error("Sadece beklemede olan iş kayıtlarına masraf eklenebilir");
                }

                // Kullanıcı sadece kendi iş kayıtlarına masraf ekleyebilir (Admin hariç)
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                if (currentUserRole != "Admin" && workRecord.CreatedBy != currentUserId)
                {
                    return ApiResponse<WorkRecordExpenseDTO>.Error("Bu iş kaydına masraf ekleme yetkiniz yok");
                }

                var expense = createExpenseDTO.Adapt<IdtWorkRecordExpense>();
                expense.WorkRecordId = workRecordId;

                var createdExpense = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                    .AddAsync(expense, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var expenseDTO = createdExpense.Adapt<WorkRecordExpenseDTO>();

                _logger.LogInformation("İş kaydına masraf başarıyla eklendi. WorkRecordId: {WorkRecordId}, ExpenseId: {ExpenseId}",
                    workRecordId, createdExpense.Id);

                return ApiResponse<WorkRecordExpenseDTO>.Success(expenseDTO, "Masraf başarıyla eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddExpenseToWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", workRecordId);
                return ApiResponse<WorkRecordExpenseDTO>.Error("Masraf eklenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> GetWorkRecordExpensesAsync(string workRecordId, CancellationToken cancellationToken = default)
        {
            try
            {
                // İş kaydının varlığını kontrol et
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(workRecordId, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Error("İş kaydı bulunamadı");
                }

                var expenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                    .FindAsync(exp => exp.WorkRecordId == workRecordId, cancellationToken);

                var expenseDTOs = expenses.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();

                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Success(expenseDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordExpensesAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", workRecordId);
                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Error("İş kaydı masrafları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<WorkRecordExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtWorkRecordExpense>().GetByIdAsync(expenseId, cancellationToken);
                if (expense == null)
                {
                    return ApiResponse<WorkRecordExpenseDTO>.Error("Masraf kaydı bulunamadı");
                }

                var expenseDTO = expense.Adapt<WorkRecordExpenseDTO>();
                return ApiResponse<WorkRecordExpenseDTO>.Success(expenseDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetExpenseByIdAsync işleminde hata oluştu. ExpenseId: {ExpenseId}", expenseId);
                return ApiResponse<WorkRecordExpenseDTO>.Error("Masraf kaydı getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<WorkRecordExpenseDTO>> UpdateWorkRecordExpenseAsync(string expenseId, UpdateWorkRecordExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtWorkRecordExpense>().GetByIdAsync(expenseId, cancellationToken);
                if (expense == null)
                {
                    return ApiResponse<WorkRecordExpenseDTO>.Error("Masraf kaydı bulunamadı");
                }

                // İlgili iş kaydını kontrol et
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(expense.WorkRecordId, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<WorkRecordExpenseDTO>.Error("İlgili iş kaydı bulunamadı");
                }

                // Sadece Pending durumundaki iş kayıtlarının masrafları güncellenebilir
                if (workRecord.Status != WorkRecordStatus.Pending)
                {
                    return ApiResponse<WorkRecordExpenseDTO>.Error("Sadece beklemede olan iş kayıtlarının masrafları güncellenebilir");
                }

                // Kullanıcı sadece kendi masraflarını güncelleyebilir (Admin hariç)
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                if (currentUserRole != "Admin" && expense.CreatedBy != currentUserId)
                {
                    return ApiResponse<WorkRecordExpenseDTO>.Error("Bu masraf kaydını güncelleme yetkiniz yok");
                }

                updateExpenseDTO.Adapt(expense);

                var updatedExpense = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                    .UpdateAsync(expense, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var expenseDTO = updatedExpense.Adapt<WorkRecordExpenseDTO>();

                _logger.LogInformation("Masraf kaydı başarıyla güncellendi. ExpenseId: {ExpenseId}", expenseId);

                return ApiResponse<WorkRecordExpenseDTO>.Success(expenseDTO, "Masraf kaydı başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateWorkRecordExpenseAsync işleminde hata oluştu. ExpenseId: {ExpenseId}", expenseId);
                return ApiResponse<WorkRecordExpenseDTO>.Error("Masraf kaydı güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteWorkRecordExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtWorkRecordExpense>().GetByIdAsync(expenseId, cancellationToken);
                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Masraf kaydı bulunamadı");
                }

                // İlgili iş kaydını kontrol et
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(expense.WorkRecordId, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<bool>.Error("İlgili iş kaydı bulunamadı");
                }

                // Sadece Pending durumundaki iş kayıtlarının masrafları silinebilir
                if (workRecord.Status != WorkRecordStatus.Pending)
                {
                    return ApiResponse<bool>.Error("Sadece beklemede olan iş kayıtlarının masrafları silinebilir");
                }

                // Kullanıcı sadece kendi masraflarını silebilir (Admin hariç)
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                if (currentUserRole != "Admin" && expense.CreatedBy != currentUserId)
                {
                    return ApiResponse<bool>.Error("Bu masraf kaydını silme yetkiniz yok");
                }

                await _unitOfWork.Repository<IdtWorkRecordExpense>().DeleteAsync(expense, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Masraf kaydı başarıyla silindi. ExpenseId: {ExpenseId}", expenseId);

                return ApiResponse<bool>.Success(true, "Masraf kaydı başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteWorkRecordExpenseAsync işleminde hata oluştu. ExpenseId: {ExpenseId}", expenseId);
                return ApiResponse<bool>.Error("Masraf kaydı silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> GetAllExpensesByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var expenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                    .FindAsync(exp => exp.CreatedBy == userId, cancellationToken);

                var expenseDTOs = expenses.Select(exp => exp.Adapt<WorkRecordExpenseDTO>()).ToList();

                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Success(expenseDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllExpensesByUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Error("Kullanıcının masrafları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalExpenseAmountByWorkRecordAsync(string workRecordId, CancellationToken cancellationToken = default)
        {
            try
            {
                // İş kaydının varlığını kontrol et
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(workRecordId, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<decimal>.Error("İş kaydı bulunamadı");
                }

                var expenses = await _unitOfWork.Repository<IdtWorkRecordExpense>()
                    .FindAsync(exp => exp.WorkRecordId == workRecordId, cancellationToken);

                var totalAmount = expenses.Sum(exp => exp.Amount);

                return ApiResponse<decimal>.Success(totalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTotalExpenseAmountByWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", workRecordId);
                return ApiResponse<decimal>.Error("İş kaydının toplam masraf tutarı hesaplanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalExpenseAmountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var allExpenses = await _unitOfWork.Repository<IdtWorkRecordExpense>().GetAllAsync(cancellationToken);
                var totalAmount = allExpenses.Sum(exp => exp.Amount);

                return ApiResponse<decimal>.Success(totalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTotalExpenseAmountAsync işleminde hata oluştu");
                return ApiResponse<decimal>.Error("Toplam masraf tutarı hesaplanırken hata oluştu");
            }
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private string? GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}