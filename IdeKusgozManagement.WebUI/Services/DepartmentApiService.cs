using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DepartmentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class DepartmentApiService : IDepartmentApiService
    {
        private readonly HttpClient _httpClient;

        public DepartmentApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>>> GetDepartmentDocumentTypeRelationsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/departments/relations", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>>> GetDepartmentDocumentTypeRelationsByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/departments/relations-by-department/{departmentId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<DepartmentViewModel>>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/departments", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DepartmentViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<DepartmentViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DepartmentViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<DepartmentViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<DepartmentViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}