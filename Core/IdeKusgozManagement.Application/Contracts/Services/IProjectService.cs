using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ProjectDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IProjectService
    {
        Task<ApiResponse<IEnumerable<ProjectDTO>>> GetProjectsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<ProjectDTO>> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateProjectAsync(CreateProjectDTO createProjectDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateProjectAsync(string projectId, UpdateProjectDTO updateProjectDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<ProjectDTO>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableProjectAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableProjectAsync(string projectId, CancellationToken cancellationToken = default);
    }
}