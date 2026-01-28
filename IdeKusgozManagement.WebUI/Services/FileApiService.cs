using System.Net.Http;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.FileModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Services
{
    public class FileApiService : IFileApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<FileApiService> _logger;
        private const string BaseEndpoint = "api/files";

        public FileApiService(
            IApiService apiService,
            ILogger<FileApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{fileId}", cancellationToken);
        }

        public async Task<ApiResponse<FileStreamResult>> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseEndpoint}/download/{fileId}");
                var response = await _apiService.SendRequestAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<FileStreamResult>.Failure($"Dosya bulunamadı. Status: {response.StatusCode}", response.StatusCode);
                }

                // Başarılı durumda dosya stream'i gelir
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "downloaded_file";

                var fileResult = new FileStreamResult(stream, contentType)
                {
                    FileDownloadName = fileName
                };

                return ApiResponse<FileStreamResult>.Success(fileResult, response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<FileStreamResult>.Failure($"Bir hata oluştu: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<List<FileViewModel>>> GetFilesByParamsAsync(string? userId, string? documentType, string? departmentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<List<FileViewModel>>($"{BaseEndpoint}/by-params?userId={userId}&documentType={documentType}&departmentId={departmentId}", cancellationToken);
        }

        public async Task<ApiResponse<List<FileViewModel>>> UploadFileAsync(List<UploadFileViewModel> files, CancellationToken cancellationToken = default)
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

                if (file.TargetCompanyIds != null && file.TargetCompanyIds.Any())
                {
                    for (int j = 0; j < file.TargetCompanyIds.Count; j++)
                    {
                        formData.Add(new StringContent(file.TargetCompanyIds[j].ToString()),
                            $"files[{i}].TargetCompanyIds[{j}]");
                    }
                }

                if (!string.IsNullOrEmpty(file.TargetDepartmentDutyId))
                    formData.Add(new StringContent(file.TargetDepartmentDutyId.ToString()), $"files[{i}].TargetDepartmentDutyId");

                if (file.StartDate.HasValue)
                    formData.Add(new StringContent(file.StartDate.Value.ToString("yyyy-MM-dd")), $"files[{i}].StartDate");

                if (file.EndDate.HasValue)
                    formData.Add(new StringContent(file.EndDate.Value.ToString("yyyy-MM-dd")), $"files[{i}].EndDate");

                if (file.HasRenewalPeriod.HasValue)
                    formData.Add(new StringContent(file.HasRenewalPeriod.Value.ToString()), $"files[{i}].HasRenewalPeriod");
            }

            return await _apiService.PostMultipartAsync<List<FileViewModel>>($"{BaseEndpoint}/upload", formData, cancellationToken);
        }
    }
}
