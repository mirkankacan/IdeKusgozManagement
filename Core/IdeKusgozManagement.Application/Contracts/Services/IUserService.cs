using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ServiceResult<IEnumerable<UserDTO>>> GetUsersAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<UserDTO>>> GetUsersByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<UserDTO>>> GetUsersByDepartmentDutyAsync(string departmentDutyId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<UserDTO>>> GetSubordinatesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ServiceResult<UserDTO>> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<UserDTO>>> GetActiveSuperiorsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<UserDTO>> UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken);

        Task<ServiceResult<bool>> AssignRoleToUserAsync(AssignRoleDTO assignRoleDTO);

        Task<ServiceResult<bool>> EnableUserAsync(string userId);

        Task<ServiceResult<bool>> DisableUserAsync(string userId);

        Task<ServiceResult<bool>> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO);

        Task<ServiceResult<AnnualLeaveBalanceDTO>> GetAnnualLeaveDaysByUserAsync(string userId, CancellationToken cancellationToken = default);
    }
}