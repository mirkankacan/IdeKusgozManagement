using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.FileDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IFileService
    {
        Task<ServiceResponse<List<FileDTO>>> UploadFileAsync(List<UploadFileDTO> files, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<FileDTO>> GetFileByIdAsync(string fileId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<List<FileDTO>>> GetFilesByParamsAsync(string? userId, string? documentType, string? departmentId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<(FileStream fileStream, string contentType, string originalName)>> GetFileStreamByIdAsync(string fileId, CancellationToken cancellationToken = default);
    }
}