namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        string GetUserId();

        string GetUserFullName();

        string GetUserTCNo();

        string GetUserRole();

        Task<List<string>?> GetUserSuperiorsAsync(CancellationToken cancellationToken = default);

        Task<List<string>?> GetUserSubordinatesAsync(CancellationToken cancellationToken = default);
    }
}