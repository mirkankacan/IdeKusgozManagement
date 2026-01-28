using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ProjectDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IProjectService
    {
        Task<ServiceResult<IEnumerable<ProjectDTO>>> GetProjectsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<ProjectDTO>> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateProjectAsync(CreateProjectDTO createProjectDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateProjectAsync(string projectId, UpdateProjectDTO updateProjectDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<ProjectDTO>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> EnableProjectAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DisableProjectAsync(string projectId, CancellationToken cancellationToken = default);
    }
}