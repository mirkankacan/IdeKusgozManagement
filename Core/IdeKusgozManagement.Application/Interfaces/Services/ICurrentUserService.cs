namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string? GetCurrentUserId();

        string? GetCurrentUserName();

        string? GetCurrentUserRole();
    }
}