namespace IdeKusgozManagement.Application.DTOs.NotificationDTOs
{
    public class NotificationDTO
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}