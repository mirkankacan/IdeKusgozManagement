using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using IdeKusgozManagement.Infrastructure.Helpers;
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
                    .FindAsync(wr => wr.Date.Year == date.Date.Year && wr.Date.Month == date.Date.Month && wr.CreatedBy == userId, cancellationToken);

                var workRecordDTOs = workRecords.Select(wr => wr.Adapt<WorkRecordDTO>()).ToList();

                return ApiResponse<IEnumerable<WorkRecordDTO>>.Success(workRecordDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordsByDateAndUserAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}", date, userId);
                return ApiResponse<IEnumerable<WorkRecordDTO>>.Error("Tarih ve kullanıcıya göre iş kayıtları getirilirken hata oluştu");
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

                _logger.LogInformation("Toplu iş kaydı başarıyla oluşturuldu. Kayıt sayısı: {Count}, EkleyenUserId: {UserId}", createdWorkRecords.Count(), CurrentUserHelper.GetCurrentUserId());

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

                // Sadece Pending durumundaki kayıtlar güncellenebilir
                if (workRecord.Status != WorkRecordStatus.Pending)
                {
                    return ApiResponse<WorkRecordDTO>.Error("Sadece beklemede olan iş kayıtları güncellenebilir");
                }

                updateWorkRecordDTO.Adapt(workRecord);

                var updatedWorkRecord = await _unitOfWork.Repository<IdtWorkRecord>()
                    .UpdateAsync(workRecord, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var workRecordDTO = updatedWorkRecord.Adapt<WorkRecordDTO>();

                _logger.LogInformation("İş kaydı başarıyla güncellendi. WorkRecordId: {WorkRecordId}, GüncelleyenUserId", id, CurrentUserHelper.GetCurrentUserId());

                return ApiResponse<WorkRecordDTO>.Success(workRecordDTO, "İş kaydı başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return ApiResponse<WorkRecordDTO>.Error("İş kaydı güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> BatchApproveWorkRecordsByUserAndMonthAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                    .FindAsync(wr => wr.CreatedBy == userId &&
                                   wr.Date.Year >= date.Date.Year &&
                                   wr.Date.Month <= date.Date.Month, cancellationToken);

                var workRecordsList = workRecords.ToList();

                if (!workRecordsList.Any())
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<bool>.Error("Belirtilen tarih aralığında iş kaydı bulunamadı");
                }

                // Tüm kayıtları onaylı duruma getir
                foreach (var workRecord in workRecordsList)
                {
                    workRecord.Status = WorkRecordStatus.Approved;
                    await _unitOfWork.Repository<IdtWorkRecord>().UpdateAsync(workRecord, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Toplu iş kaydı onaylandı. UserId: {UserId}, Year: {Year}, Month: {Month}, Count: {Count}, OnaylayanUserId: {ApproverUserId}",
                    userId, date.Date.Year, date.Date.Month, workRecordsList.Count, CurrentUserHelper.GetCurrentUserId());

                return ApiResponse<bool>.Success(true, $"{workRecordsList.Count} adet iş kaydı başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "BatchApproveWorkRecordsByUserAndMonthAsync işleminde hata oluştu. UserId: {UserId}, Year: {Year}, Month: {Month}", userId, date.Date.Year, date.Date.Month);
                return ApiResponse<bool>.Error("Toplu iş kaydı onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> BatchRejectWorkRecordsByUserAndMonthAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var workRecords = await _unitOfWork.Repository<IdtWorkRecord>()
                      .FindAsync(wr => wr.CreatedBy == userId &&
                                     wr.Date.Year >= date.Date.Year &&
                                     wr.Date.Month <= date.Date.Month, cancellationToken);

                var workRecordsList = workRecords.ToList();

                if (!workRecordsList.Any())
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<bool>.Error("Belirtilen tarih aralığında beklemede olan iş kaydı bulunamadı");
                }

                // Tüm kayıtları reddedildi duruma getir
                foreach (var workRecord in workRecordsList)
                {
                    workRecord.Status = WorkRecordStatus.Rejected;
                    await _unitOfWork.Repository<IdtWorkRecord>().UpdateAsync(workRecord, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Toplu iş kaydı reddedildi. UserId: {UserId}, Year: {Year}, Month: {Month}, Count: {Count}, ReddedenUserId: {RejecterUserId}",
                    userId, date.Date.Year, date.Date.Month, workRecordsList.Count, CurrentUserHelper.GetCurrentUserId());

                return ApiResponse<bool>.Success(true, $"{workRecordsList.Count} adet iş kaydı başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "BatchRejectWorkRecordsByUserAndMonthAsync işleminde hata oluştu. UserId: {UserId}, Year: {Year}, Month: {Month}", userId, date.Date.Year, date.Date.Month);
                return ApiResponse<bool>.Error("Toplu iş kaydı reddedilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<int>> GetWorkRecordCountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _unitOfWork.Repository<IdtWorkRecord>().CountAsync(cancellationToken);
                return ApiResponse<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordCountAsync işleminde hata oluştu");
                return ApiResponse<int>.Error("İş kaydı sayısı hesaplanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<int>> GetWorkRecordCountByStatusAsync(WorkRecordStatus status, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _unitOfWork.Repository<IdtWorkRecord>()
                    .CountAsync(wr => wr.Status == status, cancellationToken);

                return ApiResponse<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordCountByStatusAsync işleminde hata oluştu. Status: {Status}", status);
                return ApiResponse<int>.Error("Duruma göre iş kaydı sayısı hesaplanırken hata oluştu");
            }
        }
    }
}