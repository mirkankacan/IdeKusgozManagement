using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.RoleModels;
using IdeKusgozManagement.WebUI.Models.UserModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IRoleApiService
    {
        Task<ApiResponse<IEnumerable<RoleViewModel>>> GetAllRolesAsync();

        Task<ApiResponse<RoleViewModel>> GetRoleByIdAsync(string id);

        Task<ApiResponse<RoleViewModel>> GetRoleByNameAsync(string name);

        Task<ApiResponse<RoleViewModel>> CreateRoleAsync(CreateRoleViewModel model);

        Task<ApiResponse<RoleViewModel>> UpdateRoleAsync(string id, UpdateRoleViewModel model);

        Task<ApiResponse<bool>> DeleteRoleAsync(string id);

        Task<ApiResponse<bool>> ActivateRoleAsync(string id);

        Task<ApiResponse<bool>> DeactivateRoleAsync(string id);

        Task<ApiResponse<IEnumerable<UserViewModel>>> GetUsersInRoleAsync(string roleName);
    }
}