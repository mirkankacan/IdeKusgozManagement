using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtNotificationRead : BaseEntity
    {
        public string NotificationId { get; set; }
        public bool IsRead { get; set; }

        public virtual IdtNotification Notification { get; set; }

        public virtual ApplicationUser CreatedByUser { get; set; }
    }
}