using System.ComponentModel.DataAnnotations.Schema;
using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtNotification : BaseEntity
    {
        public string Message { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual ApplicationUser CreatedByUser { get; set; }

        public virtual ICollection<IdtNotificationRead> NotificationReads { get; set; } = new List<IdtNotificationRead>();
    }
}