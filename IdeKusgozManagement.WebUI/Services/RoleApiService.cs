using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.RoleModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class RoleApiService : IRoleApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<RoleApiService> _logger;
        private const string BaseEndpoint = "api/roles";

        public RoleApiService(
            IApiService apiService,
            ILogger<RoleApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<RoleViewModel>>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<RoleViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<RoleViewModel>>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<RoleViewModel>>($"{BaseEndpoint}/active-roles", cancellationToken);
        }

        public async Task<ApiResponse<RoleViewModel>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<RoleViewModel>($"{BaseEndpoint}/{roleId}", cancellationToken);
        }

        public async Task<ApiResponse<RoleViewModel>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<RoleViewModel>($"{BaseEndpoint}/name/{roleName}", cancellationToken);
        }

        public async Task<ApiResponse<RoleViewModel>> CreateRoleAsync(CreateRoleViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<RoleViewModel>(BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<RoleViewModel>> UpdateRoleAsync(string roleId, UpdateRoleViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<RoleViewModel>($"{BaseEndpoint}/{roleId}", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{roleId}", cancellationToken);
        }

        public async Task<ApiResponse<bool>> EnableRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<bool>($"{BaseEndpoint}/{roleId}/enable", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DisableRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<bool>($"{BaseEndpoint}/{roleId}/disable", null, cancellationToken);
        }
    }
}
