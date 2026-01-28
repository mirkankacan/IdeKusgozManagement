using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.ProjectModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class ProjectApiService : IProjectApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ProjectApiService> _logger;
        private const string BaseEndpoint = "api/projects";

        public ProjectApiService(
            IApiService apiService,
            ILogger<ProjectApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<ProjectViewModel>>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<ProjectViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<ProjectViewModel>> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<ProjectViewModel>($"{BaseEndpoint}/{projectId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateProjectAsync(CreateProjectViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<string>(BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateProjectAsync(string projectId, UpdateProjectViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{projectId}", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{projectId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<ProjectViewModel>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<ProjectViewModel>>($"{BaseEndpoint}/active-projects", cancellationToken);
        }

        public async Task<ApiResponse<bool>> EnableProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{projectId}/enable", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DisableProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{projectId}/disable", null, cancellationToken);
        }
    }
}
