namespace IdeKusgozManagement.Application.DTOs.DeviceDTOs
{
    public class RegisterDeviceDTO
    {
        public string DeviceToken { get; set; }
        public string Platform { get; set; }  // "Android" veya "iOS"
        public string UserId { get; set; }
        public string UserRole { get; set; }
    }
}