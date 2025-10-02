using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.NotificationDTOs
{
    public class CreateNotificationDTO
    {
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string[]? TargetUsers { get; set; }
        public string[]? TargetRoles { get; set; }
        public string? RedirectUrl { get; set; }
    }
}