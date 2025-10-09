using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtNotification : BaseEntity
    {
        public string Message { get; set; }

        /// <summary>
        /// Bildirimin türü
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Hedef kullanıcı ID'si (null ise tüm kullanıcılara gider)
        /// </summary>
        public string? TargetUsers { get; set; }

        /// <summary>
        /// Hedef rol adı (null ise rol bazlı gönderim yapılmaz)
        /// </summary>
        public string? TargetRoles { get; set; }

        /// <summary>
        /// Bildirime tıklandığında yönlendirilecek URL
        /// </summary>
        public string? RedirectUrl { get; set; }

        public virtual ApplicationUser CreatedByUser { get; set; }

        public virtual ICollection<IdtNotificationRead> NotificationReads { get; set; } = new List<IdtNotificationRead>();
    }
}