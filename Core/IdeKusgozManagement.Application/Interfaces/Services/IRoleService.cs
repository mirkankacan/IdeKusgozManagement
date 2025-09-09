using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.RoleDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ApiResponse<IEnumerable<RoleDTO>>> GetRolesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<RoleDTO>>> GetActiveRolesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleDTO>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleDTO>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleDTO>> CreateRoleAsync(CreateRoleDTO createRoleDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<RoleDTO>> UpdateRoleAsync(string roleId, UpdateRoleDTO updateRoleDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> IsUserInRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    }
}