using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.FileDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IFileService
    {
        Task<ServiceResult<List<FileDTO>>> UploadFileAsync(List<UploadFileDTO> files, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);

        Task<ServiceResult<FileDTO>> GetFileByIdAsync(string fileId, CancellationToken cancellationToken = default);

        Task<ServiceResult<List<FileDTO>>> GetFilesByParamsAsync(string? userId, string? documentType, string? departmentId, CancellationToken cancellationToken = default);

        Task<ServiceResult<(FileStream fileStream, string contentType, string originalName)>> GetFileStreamByIdAsync(string fileId, CancellationToken cancellationToken = default);
    }
}