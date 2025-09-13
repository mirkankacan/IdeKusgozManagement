namespace IdeKusgozManagement.WebUI.Models.NotificationModels
{
    public class NotificationViewModel
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }
        public string? TargetUserId { get; set; }
        public string? TargetRole { get; set; }
        public string? RedirectUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}