using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IUserApiService
    {
        Task<ApiResponse<IEnumerable<UserViewModel>>> GetAllUsersAsync();

        Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string id);

        Task<ApiResponse<UserViewModel>> CreateUserAsync(CreateUserViewModel model);

        Task<ApiResponse<UserViewModel>> UpdateUserAsync(string id, UpdateUserViewModel model);

        Task<ApiResponse<bool>> DeleteUserAsync(string id);

        Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleViewModel model);

        Task<ApiResponse<bool>> ActivateUserAsync(string id);

        Task<ApiResponse<bool>> DeactivateUserAsync(string id);

        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordViewModel model);
    }
}