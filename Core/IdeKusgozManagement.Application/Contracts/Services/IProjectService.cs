using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ProjectDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IProjectService
    {
        Task<ServiceResponse<IEnumerable<ProjectDTO>>> GetProjectsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<ProjectDTO>> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<string>> CreateProjectAsync(CreateProjectDTO createProjectDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> UpdateProjectAsync(string projectId, UpdateProjectDTO updateProjectDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<ProjectDTO>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> EnableProjectAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DisableProjectAsync(string projectId, CancellationToken cancellationToken = default);
    }
}