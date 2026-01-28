namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        string GetUserId();
        string? GetUserIdOrNull();

        string GetUserFullName();

        string GetUserTCNo();

        string GetUserRole();

        string GetUserDepartment();

        string GetUserDepartmentDuty();

        Task<List<string>?> GetUserSuperiorsAsync(CancellationToken cancellationToken = default);

        Task<List<string>?> GetUserSubordinatesAsync(CancellationToken cancellationToken = default);
    }
}