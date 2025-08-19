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
    public class WorkRecordService : IWorkRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<WorkRecordService> _logger;

        public WorkRecordService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<WorkRecordService> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetAllWorkRecordsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecords = await _unitOfWork.Repository<IdtWorkRecord>().GetAllAsync(cancellationToken);
                var workRecordDTOs = workRecords.Select(wr => wr.Adapt<WorkRecordDTO>()).ToList();

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
                    .FindAsync(wr => wr.CreatedBy == userId, cancellationToken);

                var workRecordDTOs = workRecords.Select(wr => wr.Adapt<WorkRecordDTO>()).ToList();

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
                    .FindAsync(wr => wr.Date.Date == date.Date && wr.CreatedBy == userId, cancellationToken);

                var workRecordDTOs = workRecords.Select(wr => wr.Adapt<WorkRecordDTO>()).ToList();

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordsByDateAndUserAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}", date, userId);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Tarih ve kullanıcıya göre iş kayıtları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<WorkRecordDTO>> CreateWorkRecordAsync(CreateWorkRecordDTO createWorkRecordDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecord = createWorkRecordDTO.Adapt<IdtWorkRecord>();
                workRecord.Status = WorkRecordStatus.Pending;

                var createdWorkRecord = await _unitOfWork.Repository<IdtWorkRecord>()
                    .AddAsync(workRecord, cancellationToken);

                // Eğer masraflar varsa, onları da ekle
                if (createWorkRecordDTO.Expenses != null && createWorkRecordDTO.Expenses.Any())
                {
                    var expenses = createWorkRecordDTO.Expenses.Select(exp =>
                    {
                        var expense = exp.Adapt<IdtWorkRecordExpense>();
                        expense.WorkRecordId = createdWorkRecord.Id;
                        return expense;
                    }).ToList();

                    await _unitOfWork.Repository<IdtWorkRecordExpense>()
                        .AddRangeAsync(expenses, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var workRecordDTO = createdWorkRecord.Adapt<WorkRecordDTO>();

                _logger.LogInformation("İş kaydı başarıyla oluşturuldu. WorkRecordId: {WorkRecordId}", createdWorkRecord.Id);

                return ApiResponse<WorkRecordDTO>.Success(workRecordDTO, "İş kaydı başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "CreateWorkRecordAsync işleminde hata oluştu");
                return ApiResponse<WorkRecordDTO>.Error("İş kaydı oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchCreateWorkRecordsAsync(List<CreateWorkRecordDTO> createWorkRecordDTOs, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecords = createWorkRecordDTOs.Select(dto =>
                {
                    var workRecord = dto.Adapt<IdtWorkRecord>();
                    workRecord.Status = WorkRecordStatus.Pending;
                    return workRecord;
                }).ToList();

                var createdWorkRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                    .AddRangeAsync(workRecords, cancellationToken);

                // Tüm masrafları topla ve ekle
                var allExpenses = new List<IdtWorkRecordExpense>();
                for (int i = 0; i < createWorkRecordDTOs.Count; i++)
                {
                    if (createWorkRecordDTOs[i].Expenses != null && createWorkRecordDTOs[i].Expenses.Any())
                    {
                        var expenses = createWorkRecordDTOs[i].Expenses.Select(exp =>
                        {
                            var expense = exp.Adapt<IdtWorkRecordExpense>();
                            expense.WorkRecordId = workRecords[i].Id;
                            return expense;
                        });
                        allExpenses.AddRange(expenses);
                    }
                }

                if (allExpenses.Any())
                {
                    await _unitOfWork.Repository<IdtWorkRecordExpense>()
                        .AddRangeAsync(allExpenses, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var workRecordDTOs = createdWorkRecords.Select(wr => wr.Adapt<WorkRecordDTO>()).ToList();

                _logger.LogInformation("Toplu iş kaydı başarıyla oluşturuldu. Kayıt sayısı: {Count}", createdWorkRecords.Count());

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(workRecordDTOs, "Toplu iş kayıtları başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "BatchCreateWorkRecordsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Toplu iş kayıtları oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<WorkRecordDTO>> UpdateWorkRecordAsync(string id, UpdateWorkRecordDTO updateWorkRecordDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(id, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<WorkRecordDTO>.Error("İş kaydı bulunamadı");
                }

                updateWorkRecordDTO.Adapt(workRecord);

                var updatedWorkRecord = await _unitOfWork.Repository<IdtWorkRecord>()
                    .UpdateAsync(workRecord, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var workRecordDTO = updatedWorkRecord.Adapt<WorkRecordDTO>();

                _logger.LogInformation("İş kaydı başarıyla güncellendi. WorkRecordId: {WorkRecordId}", id);

                return ApiResponse<WorkRecordDTO>.Success(workRecordDTO, "İş kaydı başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return ApiResponse<WorkRecordDTO>.Error("İş kaydı güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> ApproveWorkRecordAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(id, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<bool>.Error("İş kaydı bulunamadı");
                }
                workRecord.Status = WorkRecordStatus.Approved;

                await _unitOfWork.Repository<IdtWorkRecord>().UpdateAsync(workRecord, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("İş kaydı başarıyla onaylandı. WorkRecordId: {WorkRecordId}, OnaylayanUserId: {UserId}",
                    id, GetCurrentUserId());

                return ApiResponse<bool>.Success(true, "İş kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApproveWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return ApiResponse<bool>.Error("İş kaydı onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> RejectWorkRecordAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRecord = await _unitOfWork.Repository<IdtWorkRecord>().GetByIdAsync(id, cancellationToken);
                if (workRecord == null)
                {
                    return ApiResponse<bool>.Error("İş kaydı bulunamadı");
                }

                workRecord.Status = WorkRecordStatus.Rejected;

                await _unitOfWork.Repository<IdtWorkRecord>().UpdateAsync(workRecord, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("İş kaydı başarıyla reddedildi. WorkRecordId: {WorkRecordId}, ReddedenUserId: {UserId}",
                    id, GetCurrentUserId());

                return ApiResponse<bool>.Success(true, "İş kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RejectWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return ApiResponse<bool>.Error("İş kaydı reddedilirken hata oluştu");
            }
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}