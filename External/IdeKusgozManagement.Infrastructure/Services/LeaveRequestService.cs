using System.Text.Json;
using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.HolidayDTOs;
using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using IdeKusgozManagement.Application.Interfaces.Repositories;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LeaveRequestService> _logger;
        private readonly HolidayApiOptionsDTO _holidayApiOptions;
        private readonly IMemoryCache _memoryCache;

        public LeaveRequestService(IUnitOfWork unitOfWork, ILogger<LeaveRequestService> logger, IOptions<HolidayApiOptionsDTO> holidayApiOptions, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _holidayApiOptions = holidayApiOptions.Value;
            _memoryCache = memoryCache;
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequests = await _unitOfWork.Repository<IdtLeaveRequest>().GetAllNoTrackingAsync(cancellationToken, lr => lr.CreatedByUser, lr => lr.UpdatedByUser);

                var leaveRequestDTOs = leaveRequests
                    .Adapt<IEnumerable<LeaveRequestDTO>>()
                    .OrderByDescending(lr => lr.CreatedDate)
                    .ToList();

                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Success(leaveRequestDTOs, "İzin talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLeaveRequestsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Error("İzin talepleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestByStatusAsync(LeaveRequestStatus status, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequests = await _unitOfWork.Repository<IdtLeaveRequest>()
                    .GetWhereNoTrackingAsync(x => x.Status == status, cancellationToken, lr => lr.CreatedByUser, lr => lr.UpdatedByUser);

                var leaveRequestDTOs = leaveRequests
                    .Adapt<IEnumerable<LeaveRequestDTO>>()
                    .OrderByDescending(lr => lr.CreatedDate)
                    .ToList();

                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Success(leaveRequestDTOs, "Duruma göre izin talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLeaveRequestByStatus işleminde hata oluştu");
                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Error("Duruma göre izin talepleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<LeaveRequestDTO>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await _unitOfWork.Repository<IdtLeaveRequest>()
                    .GetByIdNoTrackingAsync(leaveRequestId, cancellationToken);

                if (leaveRequest == null)
                {
                    return ApiResponse<LeaveRequestDTO>.Error("İzin talebi bulunamadı");
                }

                var leaveRequestDTO = leaveRequest.Adapt<LeaveRequestDTO>();

                return ApiResponse<LeaveRequestDTO>.Success(leaveRequestDTO, "İzin talebi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLeaveRequestByIdAsync işleminde hata oluştu");
                return ApiResponse<LeaveRequestDTO>.Error("İzin talebi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> CreateLeaveRequestAsync(CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Tarih kontrolü
                if (createLeaveRequestDTO.StartDate >= createLeaveRequestDTO.EndDate)
                {
                    return ApiResponse<string>.Error("Başlangıç tarihi bitiş tarihinden önce olmalıdır");
                }

                // Geçmiş tarih kontrolü
                if (createLeaveRequestDTO.StartDate < DateTime.Today)
                {
                    return ApiResponse<string>.Error("Geçmiş tarihli izin talebi oluşturulamaz");
                }

                var leaveRequest = createLeaveRequestDTO.Adapt<IdtLeaveRequest>();
                leaveRequest.Status = LeaveRequestStatus.Pending; // Yeni talepler beklemede başlar

                var workingDays = await CalculateWorkingDaysAsync(
                          createLeaveRequestDTO.StartDate.Date,
                          createLeaveRequestDTO.EndDate.Date);

                leaveRequest.Duration = $"{workingDays} gün";
                await _unitOfWork.Repository<IdtLeaveRequest>().AddAsync(leaveRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<string>.Success(leaveRequest.Id, "İzin talebi başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateLeaveRequestAsync işleminde hata oluştu");
                return ApiResponse<string>.Error("İzin talebi oluşturulurken hata oluştu");
            }
        }

        private async Task<List<HolidayDTO>> GetHolidaysForYearAsync(int year)
        {
            try
            {
                if (_memoryCache.TryGetValue($"holidays_{year}", out List<HolidayDTO> cachedHolidays))
                {
                    return cachedHolidays;
                }

                using var httpClient = new HttpClient();
                var requestUrl = $"{_holidayApiOptions.BaseUrl}/holidays?api_key={_holidayApiOptions.ApiKey}&country={_holidayApiOptions.Country}&year={year}";

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
                    _memoryCache.Set($"holidays_{year}", holidays, yearEnd);

                    return holidays;
                }

                _logger.LogWarning($"Holiday API çağrısı başarısız. Status: {response.StatusCode}, Year: {year}");
                return new List<HolidayDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetHolidaysForYearAsync işleminde hata oluştu");
                return new List<HolidayDTO>();
            }
        }

        private async Task<double> CalculateWorkingDaysAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Bayram listesini al
                var holidays = await GetHolidaysForYearAsync(startDate.Year);

                // Eğer tarihler farklı yıllara yayılıyorsa, her iki yılın bayramlarını al
                if (startDate.Year != endDate.Year)
                {
                    var nextYearHolidays = await GetHolidaysForYearAsync(endDate.Year);
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
                _logger.LogError(ex, "CalculateWorkingDaysAsync işleminde hata oluştu");
                // Hata durumunda basit hesaplama yap
                return (endDate - startDate).Days + 1;
            }
        }

        public async Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await _unitOfWork.Repository<IdtLeaveRequest>()
                    .GetByIdNoTrackingAsync(id, cancellationToken);

                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.Error("İzin talebi bulunamadı");
                }

                // Sadece beklemede olan talepler silinebilir
                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ApiResponse<bool>.Error("Sadece beklemede olan izin talepleri silinebilir");
                }

                await _unitOfWork.Repository<IdtLeaveRequest>().DeleteAsync(leaveRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "İzin talebi başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteLeaveRequestAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("İzin talebi silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await _unitOfWork.Repository<IdtLeaveRequest>()
                    .GetByIdAsync(leaveRequestId, cancellationToken);

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

                await _unitOfWork.Repository<IdtLeaveRequest>().UpdateAsync(leaveRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "İzin talebi başarıyla onaylandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApproveLeaveRequestAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("İzin talebi onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequest = await _unitOfWork.Repository<IdtLeaveRequest>()
                    .GetByIdAsync(leaveRequestId, cancellationToken);

                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.Error("İzin talebi bulunamadı");
                }

                // Sadece beklemede olan talepler reddedilebilir
                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ApiResponse<bool>.Error("Sadece beklemede olan izin talepleri reddedilebilir");
                }

                leaveRequest.Status = LeaveRequestStatus.Rejected;
                leaveRequest.RejectReason = rejectReason ?? null;
                await _unitOfWork.Repository<IdtLeaveRequest>().UpdateAsync(leaveRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "İzin talebi başarıyla reddedildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RejectLeaveRequestAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("İzin talebi reddedilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaveRequests = await _unitOfWork.Repository<IdtLeaveRequest>()
                    .GetWhereNoTrackingAsync(x => x.CreatedBy == userId, cancellationToken, lr => lr.CreatedByUser, lr => lr.UpdatedByUser);

                var leaveRequestDTOs = leaveRequests
                    .Adapt<IEnumerable<LeaveRequestDTO>>()
                    .OrderByDescending(lr => lr.CreatedDate)
                    .ToList();

                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Success(leaveRequestDTOs, "Kullanıcının izin talepleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLeaveRequestsByUserIdAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<LeaveRequestDTO>>.Error("Kullanıcının izin talepleri getirilirken hata oluştu");
            }
        }
    }
}