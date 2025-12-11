using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<IEnumerable<UserDTO>>> GetUsersAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<UserDTO>>> GetUsersByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<UserDTO>>> GetUsersByDepartmentDutyAsync(string departmentDutyId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<UserDTO>>> GetSubordinatesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<UserDTO>> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<UserDTO>>> GetActiveSuperiorsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<UserDTO>> UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteUserAsync(string userId);

        Task<ServiceResponse<bool>> AssignRoleToUserAsync(AssignRoleDTO assignRoleDTO);

        Task<ServiceResponse<bool>> EnableUserAsync(string userId);

        Task<ServiceResponse<bool>> DisableUserAsync(string userId);

        Task<ServiceResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO);

        Task<ServiceResponse<AnnualLeaveBalanceDTO>> GetAnnualLeaveDaysByUserAsync(string userId, CancellationToken cancellationToken = default);
    }
}