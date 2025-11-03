using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.FileModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class FileApiService : IFileApiService
    {
        private readonly HttpClient _httpClient;

        public FileApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<bool>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/files/{fileId}", cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
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

        public async Task<ApiResponse<FileViewModel>> UploadFileAsync(UploadFileViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                using var formData = new MultipartFormDataContent();

                var fileContent = new StreamContent(model.FormFile.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.FormFile.ContentType);
                formData.Add(fileContent, $"FormFile", model.FormFile.FileName);

                var response = await _httpClient.PostAsync($"api/files/upload", formData, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<FileViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<FileViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<FileViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<FileViewModel> { IsSuccess = false, Message = "Dosya yüklenemedi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<FileViewModel> { IsSuccess = false, Message = $"Bir hata oluştu: {ex.Message}" };
            }
        }
    }
}