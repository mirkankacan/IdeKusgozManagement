using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.RoleDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ServiceResult<IEnumerable<RoleDTO>>> GetRolesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<RoleDTO>>> GetActiveRolesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<RoleDTO>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ServiceResult<RoleDTO>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

        Task<ServiceResult<RoleDTO>> CreateRoleAsync(CreateRoleDTO createRoleDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<RoleDTO>> UpdateRoleAsync(string roleId, UpdateRoleDTO updateRoleDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> EnableRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DisableRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> IsUserInRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    }
}