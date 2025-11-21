using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DocumentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class DocumentApiService : IDocumentApiService
    {
        private readonly HttpClient _httpClient;

        public DocumentApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/documents/types", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DocumentTypeViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<DocumentTypeViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DocumentTypeViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<DocumentTypeViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<DocumentTypeViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesByDutyAsync(string departmentDutyId, string? companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/documents/{departmentDutyId}/types?companyId={companyId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DocumentTypeViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<DocumentTypeViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<DocumentTypeViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<DocumentTypeViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<DocumentTypeViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<RequiredDocumentViewModel>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/documents/check?departmentId={departmentId}&departmentDutyId={departmentDutyId}&companyId={companyId}&targetId={targetId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<RequiredDocumentViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<RequiredDocumentViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<RequiredDocumentViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<RequiredDocumentViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<RequiredDocumentViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}