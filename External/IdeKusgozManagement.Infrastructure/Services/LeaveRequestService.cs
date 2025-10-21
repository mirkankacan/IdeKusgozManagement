using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.HolidayDTOs;
using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class LeaveRequestService(IUnitOfWork unitOfWork, IMemoryCache memoryCache, IOptions<HolidayApiOptionsDTO> holidayApiOptions, ILogger<LeaveRequestService> logger, IFileService fileService, INotificationService notificationService, IIdentityService identityService) : ILeaveRequestService
    {
        public async Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequests = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .WhereAsNoTracking(null)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate).ToListAsync(cancellationToken);

                // Mapping config dosyasında CreatedByUser ve UpdatedByUser için gerekli dönüşümler yapıldı
                var mappedLeaveRequests = leaveRequests.Adapt<IEnumerable<LeaveRequestDTO>>();

                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Success(mappedLeaveRequests, "İzin talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Error("İzin talepleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<LeaveRequestDTO>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .WhereAsNoTracking(x => x.Id == leaveRequestId)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefaultAsync(cancellationToken);

                if (leaveRequest == null)
                {
                    return ApiResponse<LeaveRequestDTO>.Error("İzin talebi bulunamadı");
                }

                var leaveRequestDTO = leaveRequest.Adapt<LeaveRequestDTO>();

                return ApiResponse<LeaveRequestDTO>.Success(leaveRequestDTO, "İzin talebi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestByIdAsync işleminde hata oluştu");
                return ApiResponse<LeaveRequestDTO>.Error("İzin talebi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<LeaveRequestDTO>> CreateLeaveRequestAsync(CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default)
        {
            try
            {

                // Tarih kontrolü
                if (createLeaveRequestDTO.StartDate >= createLeaveRequestDTO.EndDate)
                {
                    return ApiResponse<LeaveRequestDTO>.Error("Başlangıç tarihi bitiş tarihinden önce olmalıdır");
                }

                // Geçmiş tarih kontrolü
                if (createLeaveRequestDTO.StartDate < DateTime.Today)
                {
                    return ApiResponse<LeaveRequestDTO>.Error("Geçmiş tarihli izin talebi oluşturulamaz");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var leaveRequest = createLeaveRequestDTO.Adapt<IdtLeaveRequest>();
                leaveRequest.Status = LeaveRequestStatus.Pending; // Yeni talepler beklemede başlar

                var workingDays = await CalculateWorkingDaysAsync(
                          createLeaveRequestDTO.StartDate.Date,
                          createLeaveRequestDTO.EndDate.Date);

                leaveRequest.Duration = $"{workingDays} gün";

                if (createLeaveRequestDTO.File != null && createLeaveRequestDTO.File.FormFile.Length > 0)
                {
                    createLeaveRequestDTO.File.TargetUserId = identityService.GetUserId();
                    createLeaveRequestDTO.File.FileType = FileType.LeaveRequest;
                    var fileResult = await fileService.UploadFileAsync(createLeaveRequestDTO.File, cancellationToken);

                    if (!fileResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);

                        return ApiResponse<LeaveRequestDTO>.Error(fileResult.Message);
                    }

                    leaveRequest.FileId = fileResult.Data.Id;
                }
                await unitOfWork.GetRepository<IdtLeaveRequest>().AddAsync(leaveRequest, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var createdLeaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .Where(x => x.Id == leaveRequest.Id)
                    .AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .FirstOrDefaultAsync(cancellationToken);

                var leaveRequestDTO = createdLeaveRequest.Adapt<LeaveRequestDTO>();

                CreateNotificationDTO createNotification = new()
                {
                    Message = $"{leaveRequestDTO.CreatedByFullName} tarafından, {leaveRequestDTO.CreatedDate.ToString("dd.MM.yyyy HH:mm")} tarihinde yeni bir izin talebi oluşturuldu.",
                    Type = NotificationType.LeaveRequest,
                    RedirectUrl = "/izin",
                    TargetRoles = await identityService.GetUserSuperiorsAsync()
                };
                await notificationService.SendNotificationToSuperiorsAsync(createNotification, cancellationToken);
                return ApiResponse<LeaveRequestDTO>.Success(leaveRequestDTO, "İzin talebi başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);

                logger.LogError(ex, "CreateLeaveRequestAsync işleminde hata oluştu");
                return ApiResponse<LeaveRequestDTO>.Error("İzin talebi oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>().GetByIdAsync(id, cancellationToken);

                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.Error("İzin talebi bulunamadı");
                }

                // Sadece beklemede olan talepler silinebilir
                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ApiResponse<bool>.Error("Sadece beklemede olan izin talepleri silinebilir");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (!string.IsNullOrEmpty(leaveRequest.FileId))
                    await fileService.DeleteFileAsync(leaveRequest.FileId, cancellationToken);

                unitOfWork.GetRepository<IdtLeaveRequest>().Remove(leaveRequest);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ApiResponse<bool>.Success(true, "İzin talebi başarıyla silindi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "DeleteLeaveRequestAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("İzin talebi silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>().GetByIdAsync(leaveRequestId, cancellationToken);



                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.Error("İzin talebi bulunamadı");
                }

                // Sadece beklemede olan talepler onaylanabilir
                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ApiResponse<bool>.Error("Sadece beklemede olan izin talepleri onaylanabilir");
                }

                leaveRequest.Status = LeaveRequestStatus.Approved;

                unitOfWork.GetRepository<IdtLeaveRequest>().Update(leaveRequest);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var approvedLeaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                 .Where(x => x.Id == leaveRequestId)
                 .Include(x => x.CreatedByUser)
                 .Include(x => x.UpdatedByUser)
                 .Include(x => x.File)
                 .FirstOrDefaultAsync(cancellationToken);
                var mappedLeaveRequest = approvedLeaveRequest.Adapt<LeaveRequestDTO>();
                CreateNotificationDTO createNotification = new()
                {
                    Message = $"{mappedLeaveRequest.UpdatedByFullName} tarafından, {mappedLeaveRequest.UpdatedDate?.ToString("dd.MM.yyyy HH:mm")} tarihinde bir izin talebiniz onaylandı.",
                    Type = NotificationType.LeaveRequest,
                    RedirectUrl = "/izin/olustur",
                    TargetUsers = new List<string> { mappedLeaveRequest.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(createNotification, cancellationToken);
                return ApiResponse<bool>.Success(true, $"İzin talebi başarıyla onaylandı. LeaveRequestId: {leaveRequestId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ApproveLeaveRequestAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("İzin talebi onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>().GetByIdAsync(leaveRequestId, cancellationToken);
                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.Error("İzin talebi bulunamadı");
                }

                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ApiResponse<bool>.Error("Sadece beklemede olan izin talepleri reddedilebilir");
                }

                leaveRequest.Status = LeaveRequestStatus.Rejected;
                leaveRequest.RejectReason = !string.IsNullOrEmpty(rejectReason) ? rejectReason : null;

                unitOfWork.GetRepository<IdtLeaveRequest>().Update(leaveRequest);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var rejectedLeaveRequest = await unitOfWork.GetRepository<IdtLeaveRequest>()
                  .WhereAsNoTracking(x => x.Id == leaveRequestId)
                  .Include(x => x.CreatedByUser)
                  .Include(x => x.UpdatedByUser)
                  .Include(x => x.File)
                  .FirstOrDefaultAsync(cancellationToken);
                var mappedLeaveRequest = rejectedLeaveRequest.Adapt<LeaveRequestDTO>();
                CreateNotificationDTO createNotification = new()
                {
                    Message = $"{mappedLeaveRequest.UpdatedByFullName} tarafından, {mappedLeaveRequest.UpdatedDate?.ToString("dd.MM.yyyy HH:mm")} tarihinde bir izin talebiniz reddedildi.",
                    Type = NotificationType.LeaveRequest,
                    RedirectUrl = "/izin/olustur",
                    TargetUsers = new List<string> { mappedLeaveRequest.CreatedBy }
                };
                await notificationService.SendNotificationToUsersAsync(createNotification, cancellationToken);
                return ApiResponse<bool>.Success(true, $"İzin talebi başarıyla reddedildi. LeaveRequestId: {leaveRequestId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RejectLeaveRequestAsync işleminde hata oluştu. LeaveRequestId: {LeaveRequestId}", leaveRequestId);
                return ApiResponse<bool>.Error("İzin talebi reddedilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequests = await unitOfWork.GetRepository<IdtLeaveRequest>()
                    .WhereAsNoTracking(x => x.CreatedBy == userId)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                var leaveRequestDTOs = leaveRequests.Adapt<IEnumerable<LeaveRequestDTO>>();

                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Success(leaveRequestDTOs, $"Kullanıcının izin talepleri başarıyla getirildi. UserId: {userId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestsByUserIdAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Error($"Kullanıcının izin talepleri getirilirken hata oluştu. UserId: {userId}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByStatusAsync(LeaveRequestStatus status, string? userId, CancellationToken cancellationToken = default)
        {
            try
            {

                var baseQuery = unitOfWork.GetRepository<IdtLeaveRequest>().WhereAsNoTracking(x => x.Status == status);

                if (!string.IsNullOrEmpty(userId))
                {
                    baseQuery = baseQuery.Where(x => x.CreatedBy == userId);
                }

                var leaveRequests = await baseQuery
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.File)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                var leaveRequestDTOs = leaveRequests.Adapt<IEnumerable<LeaveRequestDTO>>();

                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Success(leaveRequestDTOs, "Duruma göre izin talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetLeaveRequestByStatus işleminde hata oluştu");
                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Error("Duruma göre izin talepleri getirilirken hata oluştu");
            }
        }

        private async Task<List<HolidayDTO>> GetHolidaysByYearAsync(int year)
        {
            try
            {
                if (memoryCache.TryGetValue($"holidays_{year}", out List<HolidayDTO> cachedHolidays))
                {
                    return cachedHolidays;
                }

                using var httpClient = new HttpClient();
                var requestUrl = $"{holidayApiOptions.Value.BaseUrl}/holidays?api_key={holidayApiOptions.Value.ApiKey}&country={holidayApiOptions.Value.Country}&year={year}";

                var response = await httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var holidayResponse = JsonSerializer.Deserialize<HolidayApiResponseDTO>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var holidays = holidayResponse?.Response?.Holidays ?? new List<HolidayDTO>();

                    // Cache'e koy (süre yıl sonuna kadar)
                    var yearEnd = new DateTime(year, 12, 31, 23, 59, 59);
                    memoryCache.Set($"holidays_{year}", holidays, yearEnd);

                    return holidays;
                }

                logger.LogWarning($"Holiday API çağrısı başarısız. Status: {response.StatusCode}, Year: {year}");
                return new List<HolidayDTO>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetHolidaysByYearAsync işleminde hata oluştu");
                return new List<HolidayDTO>();
            }
        }

        private async Task<double> CalculateWorkingDaysAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Bayram listesini al
                var holidays = await GetHolidaysByYearAsync(startDate.Year);

                // Eğer tarihler farklı yıllara yayılıyorsa, her iki yılın bayramlarını al
                if (startDate.Year != endDate.Year)
                {
                    var nextYearHolidays = await GetHolidaysByYearAsync(endDate.Year);
                    holidays.AddRange(nextYearHolidays);
                }

                // Milli ve dini bayramları ayır
                var nationalHolidays = holidays
                    .Where(h => h.PrimaryType.Contains("National holiday")
                             || h.PrimaryType.Contains("Holiday for Public Servants"))
                    .Select(h => new DateTime(h.Date.Datetime.Year, h.Date.Datetime.Month, h.Date.Datetime.Day)).ToHashSet();

                var halfDayHolidays = holidays
                    .Where(h => h.PrimaryType.Contains("Half Day"))
                    .Select(h => new DateTime(h.Date.Datetime.Year, h.Date.Datetime.Month, h.Date.Datetime.Day)).ToHashSet();

                double workingDays = 0;
                var currentDate = startDate;

                while (currentDate <= endDate.Date)
                {
                    bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday ||
                                     currentDate.DayOfWeek == DayOfWeek.Sunday;

                    if (!isWeekend)
                    {
                        if (nationalHolidays.Contains(currentDate))
                        {
                            // Tam tatil → 0 gün ekle
                        }
                        else if (halfDayHolidays.Contains(currentDate))
                        {
                            workingDays += 0.5; // Yarım gün
                        }
                        else
                        {
                            workingDays += 1; // Normal iş günü
                        }
                    }

                    currentDate = currentDate.AddDays(1);
                }

                return workingDays;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CalculateWorkingDaysAsync işleminde hata oluştu");
                // Hata durumunda basit hesaplama yap
                return (endDate - startDate).Days + 1;
            }
        }
    }
}