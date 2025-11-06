using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.FileDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IFileService
    {
        Task<ApiResponse<List<FileDTO>>> UploadFileAsync(List<UploadFileDTO> files, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);

        Task<ApiResponse<FileDTO>> GetFileByIdAsync(string fileId, CancellationToken cancellationToken = default);

        Task<ApiResponse<List<FileDTO>>> GetFilesByParamsAsync(string? userId, string? documentType, string? departmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<(FileStream fileStream, string contentType, string originalName)>> GetFileStreamByIdAsync(string fileId, CancellationToken cancellationToken = default);
    }
}