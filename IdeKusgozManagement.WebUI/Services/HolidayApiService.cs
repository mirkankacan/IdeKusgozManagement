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

        public async Task<ApiResponse<List<HolidayViewModel>>> GetHolidaysByYear(int year, CancellationToken cancellationToken = default)
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