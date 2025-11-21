using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.CompanyModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class CompanyApiService : ICompanyApiService
    {
        private readonly HttpClient _httpClient;

        public CompanyApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<CompanyViewModel>>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/companies", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<CompanyViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<CompanyViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<CompanyViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<CompanyViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CompanyViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}