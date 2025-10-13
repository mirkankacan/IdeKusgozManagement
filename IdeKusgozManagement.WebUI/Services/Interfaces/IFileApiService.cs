using IdeKusgozManagement.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IFileApiService
    {
        Task<ApiResponse<FileStreamResult>> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default);
    }
}