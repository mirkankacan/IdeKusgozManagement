using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IUserApiService
    {
        Task<ApiResponse<IEnumerable<UserViewModel>>> GetAllUsersAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserViewModel>>> GetAssignedUsersByIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserViewModel>>> GetActiveSuperiorUsersAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserViewModel>> GetMyUserAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<UserViewModel>> CreateUserAsync(CreateUserViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserViewModel>> UpdateUserAsync(string userId, UpdateUserViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ActivateUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordViewModel model, CancellationToken cancellationToken = default);
    }
}