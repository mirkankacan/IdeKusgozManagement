using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.FileModels;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IFileApiService
    {
        Task<ApiResponse<FileStreamResult>> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);

        Task<ApiResponse<List<FileViewModel>>> UploadFileAsync(List<UploadFileViewModel> files, CancellationToken cancellationToken = default);

        Task<ApiResponse<List<FileViewModel>>> GetFilesByParamsAsync(string? userId, string? documentType, string? departmentId, CancellationToken cancellationToken = default);
    }
}