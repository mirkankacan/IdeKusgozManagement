using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtDevice : BaseEntity
    {
        /// <summary>
        /// Kullanıcı ID'si (Foreign Key)
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// FCM (Android) veya APNs (iOS) token'ı
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        /// Platform: "Android" veya "iOS"
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// OneSignal'dan dönen Player ID
        /// </summary>
        public string? OneSignalPlayerId { get; set; }

        /// <summary>
        /// Cihaz aktif mi? (Logout olunca false yapılabilir)
        /// </summary>
        public bool IsActive { get; set; }
    }
}