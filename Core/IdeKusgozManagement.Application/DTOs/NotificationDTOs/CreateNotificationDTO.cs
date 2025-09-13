using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.NotificationDTOs
{
    public class CreateNotificationDTO
    {
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string? TargetUserId { get; set; }
        public string? TargetRole { get; set; }
        public string? RedirectUrl { get; set; }
    }
}