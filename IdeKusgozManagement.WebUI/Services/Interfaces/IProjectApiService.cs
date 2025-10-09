using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.ProjectModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IProjectApiService
    {
        Task<ApiResponse<IEnumerable<ProjectViewModel>>> GetProjectsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<ProjectViewModel>> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateProjectAsync(CreateProjectViewModel createProjectDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateProjectAsync(string projectId, UpdateProjectViewModel updateProjectDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<ProjectViewModel>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableProjectAsync(string projectId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableProjectAsync(string projectId, CancellationToken cancellationToken = default);
    }
}