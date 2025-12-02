using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.HolidayDTOs;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class HolidayService(IMemoryCache memoryCache, IOptions<HolidayApiOptionsDTO> holidayApiOptions, ILogger<HolidayService> logger) : IHolidayService
    {
        public async Task<ServiceResponse<List<HolidayDTO>>> GetHolidaysByYearAsync(int year, CancellationToken cancellationToken = default)
        {
            try
            {
                if (memoryCache.TryGetValue($"holidays_{year}", out List<HolidayDTO> cachedHolidays))
                {
                    return ServiceResponse<List<HolidayDTO>>.Success(cachedHolidays, "Tatiller listesi cacheden başarıyla getirildi");
                }

                using var httpClient = new HttpClient();
                var requestUrl = $"{holidayApiOptions.Value.BaseUrl}/holidays?api_key={holidayApiOptions.Value.ApiKey}&country={holidayApiOptions.Value.Country}&year={year}";
                var response = await httpClient.GetAsync(requestUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning($"Holiday API çağrısı başarısız. Status: {response.StatusCode}, Year: {year}");
                    return ServiceResponse<List<HolidayDTO>>.Error("Tatiller listesi getirilirken hata oluştu");
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var holidayResponse = JsonSerializer.Deserialize<HolidayServiceResponseDTO>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var allHolidays = holidayResponse?.Response?.Holidays ?? new List<HolidayDTO>();

                // Sadece resmi tatilleri filtrele (National holiday ve Half Day)
                var officialHolidays = allHolidays.Where(h =>
                    h.PrimaryType == "National holiday" ||
                    h.PrimaryType == "Half Day").ToList();

                // Cache'e koy (süre yıl sonuna kadar)
                var yearEnd = new DateTime(year, 12, 31, 23, 59, 59);
                memoryCache.Set($"holidays_{year}", officialHolidays, yearEnd);

                return ServiceResponse<List<HolidayDTO>>.Success(officialHolidays, "Tatiller listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetHolidaysByYearAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<double>> CalculateWorkingDaysAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            try
            {
                // Bayram listesini al
                var resp = await GetHolidaysByYearAsync(startDate.Year, cancellationToken);
                if (!resp.IsSuccess)
                {
                    return ServiceResponse<double>.Error("Tatil verileri alınırken hata oluştu");
                }

                var holidays = resp.Data ?? new List<HolidayDTO>();

                // Eğer tarihler farklı yıllara yayılıyorsa, her iki yılın bayramlarını al
                if (startDate.Year != endDate.Year)
                {
                    var nextYearResp = await GetHolidaysByYearAsync(endDate.Year, cancellationToken);
                    if (nextYearResp.IsSuccess && nextYearResp.Data != null)
                    {
                        holidays.AddRange(nextYearResp.Data);
                    }
                }

                // Sadece özel sektör için geçerli tatiller
                var nationalHolidays = holidays
                    .Where(h => h.PrimaryType == "National holiday")
                    .Select(h => new DateTime(h.Date.DateTime.Year, h.Date.DateTime.Month, h.Date.DateTime.Day))
                    .ToHashSet();

                var halfDayHolidays = holidays
                    .Where(h => h.PrimaryType == "Half Day")
                    .Select(h => new DateTime(h.Date.DateTime.Year, h.Date.DateTime.Month, h.Date.DateTime.Day))
                    .ToHashSet();

                double workingDays = 0;
                var currentDate = startDate.Date;

                while (currentDate <= endDate.Date)
                {
                    bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Sunday;

                    if (!isWeekend)
                    {
                        if (nationalHolidays.Contains(currentDate))
                        {
                            // Tam tatil → 0 gün ekle (Ramazan Bayramı, Kurban Bayramı, Cumhuriyet Bayramı vs.)
                        }
                        else if (halfDayHolidays.Contains(currentDate))
                        {
                            workingDays += 0.5; // Yarım gün (Arefe günleri)
                        }
                        else
                        {
                            workingDays += 1; // Normal iş günü
                        }
                    }

                    currentDate = currentDate.AddDays(1);
                }

                return ServiceResponse<double>.Success(workingDays, "Net çalışma günü başarıyla hesaplandı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CalculateWorkingDaysAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}