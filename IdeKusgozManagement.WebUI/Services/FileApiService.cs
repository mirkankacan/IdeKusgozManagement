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

        public async Task<ApiResponse<List<FileViewModel>>> GetFilesByParamsAsync(string? userId, string? documentType, string? departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/files/by-params?userId={userId}&documentType={documentType}&departmentId={departmentId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<FileViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<List<FileViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<List<FileViewModel>>>(content);
                return errorResponse ?? new ApiResponse<List<FileViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FileViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<List<FileViewModel>>> UploadFileAsync(List<UploadFileViewModel> files, CancellationToken cancellationToken = default)
        {
            try
            {
                using var formData = new MultipartFormDataContent();

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    // Dosya içeriği ekle
                    var fileContent = new StreamContent(file.FormFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.FormFile.ContentType);
                    formData.Add(fileContent, $"files[{i}].FormFile", file.FormFile.FileName);

                    if (!string.IsNullOrEmpty(file.DocumentTypeId))
                        formData.Add(new StringContent(file.DocumentTypeId.ToString()), $"files[{i}].DocumentTypeId");

                    if (!string.IsNullOrEmpty(file.TargetUserId))
                        formData.Add(new StringContent(file.TargetUserId), $"files[{i}].TargetUserId");

                    if (!string.IsNullOrEmpty(file.TargetProjectId))
                        formData.Add(new StringContent(file.TargetProjectId.ToString()), $"files[{i}].TargetProjectId");

                    if (!string.IsNullOrEmpty(file.TargetEquipmentId))
                        formData.Add(new StringContent(file.TargetEquipmentId.ToString()), $"files[{i}].TargetEquipmentId");

                    if (!string.IsNullOrEmpty(file.TargetDepartmentId))
                        formData.Add(new StringContent(file.TargetDepartmentId.ToString()), $"files[{i}].TargetDepartmentId");

                    if (!string.IsNullOrEmpty(file.TargetCompanyId))
                        formData.Add(new StringContent(file.TargetCompanyId.ToString()), $"files[{i}].TargetCompanyId");

                    if (!string.IsNullOrEmpty(file.TargetDepartmentDutyId))
                        formData.Add(new StringContent(file.TargetDepartmentDutyId.ToString()), $"files[{i}].TargetDepartmentDutyId");

                    if (file.StartDate.HasValue)
                        formData.Add(new StringContent(file.StartDate.Value.ToString("yyyy-MM-dd")), $"files[{i}].StartDate");

                    if (file.EndDate.HasValue)
                        formData.Add(new StringContent(file.EndDate.Value.ToString("yyyy-MM-dd")), $"files[{i}].EndDate");

                    if (file.HasRenewalPeriod.HasValue)
                        formData.Add(new StringContent(file.HasRenewalPeriod.Value.ToString()), $"files[{i}].HasRenewalPeriod");
                }

                var response = await _httpClient.PostAsync("api/files/upload", formData, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<FileViewModel>>>(responseContent);
                    return apiResponse ?? new ApiResponse<List<FileViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<List<FileViewModel>>>(responseContent);
                return errorResponse ?? new ApiResponse<List<FileViewModel>> { IsSuccess = false, Message = "Dosya yüklenemedi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FileViewModel>> { IsSuccess = false, Message = $"Bir hata oluştu: {ex.Message}" };
            }
        }
    }
}