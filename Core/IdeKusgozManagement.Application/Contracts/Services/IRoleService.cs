using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.RoleDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ServiceResponse<IEnumerable<RoleDTO>>> GetRolesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<RoleDTO>>> GetActiveRolesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<RoleDTO>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<RoleDTO>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

        Task<ServiceResponse<RoleDTO>> CreateRoleAsync(CreateRoleDTO createRoleDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<RoleDTO>> UpdateRoleAsync(string roleId, UpdateRoleDTO updateRoleDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> EnableRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DisableRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> IsUserInRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    }
}