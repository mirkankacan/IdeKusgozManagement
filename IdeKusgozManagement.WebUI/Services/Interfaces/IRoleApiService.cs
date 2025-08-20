using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.RoleModels;
using IdeKusgozManagement.WebUI.Models.UserModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IRoleApiService
    {
        Task<ApiResponse<IEnumerable<RoleViewModel>>> GetAllRolesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<RoleViewModel>>> GetActiveRolesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleViewModel>> GetRoleByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleViewModel>> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleViewModel>> CreateRoleAsync(CreateRoleViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleViewModel>> UpdateRoleAsync(string id, UpdateRoleViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteRoleAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ActivateRoleAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeactivateRoleAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserViewModel>>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default);
    }
}