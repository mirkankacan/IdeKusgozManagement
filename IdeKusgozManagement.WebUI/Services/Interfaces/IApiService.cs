

using IdeKusgozManagement.WebUI.Models;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IApiService
    {
        Task<ApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);

        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);

        Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);

        Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);

        Task<ApiResponse> PostAsync(string endpoint, object data, CancellationToken cancellationToken = default);

        Task<ApiResponse> PutAsync(string endpoint, object data, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);

        Task<ApiResponse<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent formData, CancellationToken cancellationToken = default);

        Task<ApiResponse<T>> PutMultipartAsync<T>(string endpoint, MultipartFormDataContent formData, CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}