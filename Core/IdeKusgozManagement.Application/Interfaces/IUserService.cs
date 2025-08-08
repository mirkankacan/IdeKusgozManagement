using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<UserDTO>>> GetAllUsersAsync();

        Task<ApiResponse<UserDTO>> GetUserByIdAsync(string id);

        Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO);

        Task<ApiResponse<UserDTO>> UpdateUserAsync(string id, UpdateUserDTO updateUserDTO);

        Task<ApiResponse<bool>> DeleteUserAsync(string id);

        Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleDTO assignRoleDTO);

        Task<ApiResponse<bool>> ActivateUserAsync(string userId);

        Task<ApiResponse<bool>> DeactivateUserAsync(string userId);

        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO);
    }
}