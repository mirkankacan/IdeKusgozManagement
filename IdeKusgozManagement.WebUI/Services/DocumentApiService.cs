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
                var response = await _httpClient.GetAsync("api/documents/document-types", cancellationToken);
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

        public async Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpResponseMessage? response = await _httpClient.GetAsync($"api/documents/document-types-by-department/{departmentId}", cancellationToken);
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

        public async Task<ApiResponse<List<UserRequiredDocumentViewModel>>> GetRequiredDocumentsAsync(string departmentId, string? targetId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/documents/check/by-params?departmentId={departmentId}&targetId={targetId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<UserRequiredDocumentViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<List<UserRequiredDocumentViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<List<UserRequiredDocumentViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UserRequiredDocumentViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}