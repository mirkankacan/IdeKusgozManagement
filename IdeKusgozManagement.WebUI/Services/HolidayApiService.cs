using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.HolidayModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class HolidayApiService : IHolidayApiService
    {
        private readonly HttpClient _httpClient;

        public HolidayApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<double>> CalculateWorkingDaysAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/holidays/start/{startDate:yyyy-MM-dd}/end/{endDate:yyyy-MM-dd}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<double>>(content);
                    return apiResponse ?? new ApiResponse<double> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<double>>(content);
                return errorResponse ?? new ApiResponse<double> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<double> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<List<HolidayViewModel>>> GetHolidaysByYearAsync(int year, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/holidays/{year}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<HolidayViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<List<HolidayViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<List<HolidayViewModel>>>(content);
                return errorResponse ?? new ApiResponse<List<HolidayViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<HolidayViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}