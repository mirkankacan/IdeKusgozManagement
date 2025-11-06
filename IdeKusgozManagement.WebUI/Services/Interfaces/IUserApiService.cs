using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IUserApiService
    {
        Task<ApiResponse<IEnumerable<UserViewModel>>> GetUsersAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserViewModel>>> GetUsersByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserViewModel>>> GetSubordinatesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<UserViewModel>>> GetActiveSuperiorsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<UserViewModel>> CreateUserAsync(CreateUserViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserViewModel>> UpdateUserAsync(string userId, UpdateUserViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserViewModel>> GetMyUserAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<AnnualLeaveBalanceViewModel>> GetAnnualLeaveDaysByUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<AnnualLeaveBalanceViewModel>> GetMyAnnualLeaveDaysAsync(CancellationToken cancellationToken = default);
    }
}