using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UserApiService> _logger;
        private const string BaseEndpoint = "api/users";

        public UserApiService(
            IApiService apiService,
            ILogger<UserApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<UserViewModel>>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<UserViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<UserViewModel>>> GetActiveSuperiorsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<UserViewModel>>($"{BaseEndpoint}/active-superiors", cancellationToken);
        }

        public async Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<UserViewModel>($"{BaseEndpoint}/{userId}", cancellationToken);
        }

        public async Task<ApiResponse<UserViewModel>> CreateUserAsync(CreateUserViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<UserViewModel>(BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<UserViewModel>> UpdateUserAsync(string userId, UpdateUserViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<UserViewModel>($"{BaseEndpoint}/{userId}", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{userId}", cancellationToken);
        }

        public async Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<bool>($"{BaseEndpoint}/assign-role", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> EnableUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{userId}/enable", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DisableUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{userId}/disable", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<bool>($"{BaseEndpoint}/{userId}/change-password", model, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<UserViewModel>>> GetSubordinatesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<UserViewModel>>($"{BaseEndpoint}/subordiantes?userId={userId}", cancellationToken);
        }

        public async Task<ApiResponse<UserViewModel>> GetMyUserAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<UserViewModel>($"{BaseEndpoint}/my-user", cancellationToken);
        }

        public async Task<ApiResponse<AnnualLeaveBalanceViewModel>> GetAnnualLeaveDaysByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<AnnualLeaveBalanceViewModel>($"{BaseEndpoint}/{userId}/annual-leave", cancellationToken);
        }

        public async Task<ApiResponse<AnnualLeaveBalanceViewModel>> GetMyAnnualLeaveDaysAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<AnnualLeaveBalanceViewModel>($"{BaseEndpoint}/my-annual-leave", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<UserViewModel>>> GetUsersByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<UserViewModel>>($"{BaseEndpoint}/by-department/{departmentId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<UserViewModel>>> GetUsersByDepartmentDutyAsync(string departmentDutyId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<UserViewModel>>($"{BaseEndpoint}/by-department-duty/{departmentDutyId}", cancellationToken);
        }
    }
}
