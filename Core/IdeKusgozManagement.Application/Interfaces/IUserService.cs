using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<UserDTO>>> GetAllUsersAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserDTO>>> GetAssignedUsersByIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserDTO>> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserDTO>>> GetActiveSuperiorUsersAsync();

        Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserDTO>> UpdateUserAsync(string id, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteUserAsync(string id);

        Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleDTO assignRoleDTO);

        Task<ApiResponse<bool>> ActivateUserAsync(string userId);

        Task<ApiResponse<bool>> DeactivateUserAsync(string userId);

        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO);
    }
}