namespace IdeKusgozManagement.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetCurrentUserId();

        string? GetCurrentUserName();

        string? GetCurrentUserRole();

        string? GetUserIpAddress();
    }
}