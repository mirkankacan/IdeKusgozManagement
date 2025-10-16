using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.NotificationDTOs
{
    public class CreateNotificationDTO
    {
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public List<string>? TargetUsers { get; set; }
        public List<string>? TargetRoles { get; set; }
        public string? RedirectUrl { get; set; }
    }
}