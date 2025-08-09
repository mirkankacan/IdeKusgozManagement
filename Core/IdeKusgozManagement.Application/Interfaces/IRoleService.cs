using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.RoleDTOs;
using IdeKusgozManagement.Application.DTOs.UserDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IRoleService
    {
        Task<ApiResponse<IEnumerable<RoleDTO>>> GetAllRolesAsync();

        Task<ApiResponse<RoleDTO>> GetRoleByIdAsync(string id);

        Task<ApiResponse<RoleDTO>> GetRoleByNameAsync(string name);

        Task<ApiResponse<RoleDTO>> CreateRoleAsync(CreateRoleDTO createRoleDTO);

        Task<ApiResponse<RoleDTO>> UpdateRoleAsync(string id, UpdateRoleDTO updateRoleDTO);

        Task<ApiResponse<bool>> DeleteRoleAsync(string id);

        Task<ApiResponse<bool>> ActivateRoleAsync(string roleId);

        Task<ApiResponse<bool>> DeactivateRoleAsync(string roleId);

        Task<ApiResponse<IEnumerable<UserDTO>>> GetUsersInRoleAsync(string roleName);

        Task<ApiResponse<bool>> IsUserInRoleAsync(string userId, string roleName);
    }
}