using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Services
{
    public class FileApiService : IFileApiService
    {
        private readonly HttpClient _httpClient;

        public FileApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<FileStreamResult>> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/files/download/{fileId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<FileStreamResult>
                    {
                        IsSuccess = false,
                        Message = $"Dosya bulunamadı. Status: {response.StatusCode}"
                    };
                }

                // Başarılı durumda dosya stream'i gelir
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "downloaded_file";

                var fileResult = new FileStreamResult(stream, contentType)
                {
                    FileDownloadName = fileName
                };

                return new ApiResponse<FileStreamResult>
                {
                    IsSuccess = true,
                    Data = fileResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<FileStreamResult>
                {
                    IsSuccess = false,
                    Message = $"Bir hata oluştu: {ex.Message}"
                };
            }
        }
    }

}