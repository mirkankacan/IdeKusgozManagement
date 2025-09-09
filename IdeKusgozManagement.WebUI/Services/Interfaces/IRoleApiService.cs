using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.RoleModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IRoleApiService
    {
        Task<ApiResponse<IEnumerable<RoleViewModel>>> GetRolesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<RoleViewModel>>> GetActiveRolesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleViewModel>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleViewModel>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleViewModel>> CreateRoleAsync(CreateRoleViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleViewModel>> UpdateRoleAsync(string roleId, UpdateRoleViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableRoleAsync(string roleId, CancellationToken cancellationToken = default);
    }
}