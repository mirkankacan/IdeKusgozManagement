namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IOneSignalService
    {
        Task SendNotificationAsync(string message, string heading, List<string>? userIds = null,
            List<string>? roles = null, Dictionary<string, object>? additionalData = null);
    }
}