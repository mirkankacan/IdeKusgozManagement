using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<UserDTO>>> GetUsersAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserDTO>>> GetSubordinatesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserDTO>> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserDTO>>> GetActiveSuperiorsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserDTO>> UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteUserAsync(string userId);

        Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleDTO assignRoleDTO);

        Task<ApiResponse<bool>> EnableUserAsync(string userId);

        Task<ApiResponse<bool>> DisableUserAsync(string userId);

        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO);
    }
}