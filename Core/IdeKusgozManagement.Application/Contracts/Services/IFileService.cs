using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.FileDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IFileService
    {
        Task<ApiResponse<FileDTO>> UploadFileAsync(UploadFileDTO uploadFileDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);
    }
}