namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        string GetUserId();

        string GetUserFullName();

        string GetUserTCNo();

        string GetUserRole();

        Task<string[]?> GetUserSuperiorsAsync(CancellationToken cancellationToken = default);

        Task<string[]?> GetUserSubordinatesAsync(CancellationToken cancellationToken = default);
    }
}