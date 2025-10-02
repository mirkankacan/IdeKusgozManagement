using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.NotificationDTOs
{
    public class NotificationDTO
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string[]? TargetUsers { get; set; }
        public string[]? TargetRoles { get; set; }
        public string? RedirectUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
    }
}