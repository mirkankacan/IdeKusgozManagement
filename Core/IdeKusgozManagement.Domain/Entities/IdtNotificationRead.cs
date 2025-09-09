using System.ComponentModel.DataAnnotations.Schema;
using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtNotificationRead : BaseEntity
    {
        public IdtNotificationRead()
        {
            IsRead = true;
        }

        public string NotificationId { get; set; }
        public bool IsRead { get; private set; }

        [ForeignKey(nameof(NotificationId))]
        public virtual IdtNotification Notification { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual ApplicationUser CreatedByUser { get; set; }
    }
}