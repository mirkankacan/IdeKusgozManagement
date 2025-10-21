namespace IdeKusgozManagement.WebUI.Models.TrafficTicketModels
{
    public class TrafficTicketViewModel
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime TicketDate { get; set; }
        public string? TargetUserId { get; set; }
        public string? TargetUserFullName { get; set; }
        public string? FileId { get; set; }
        public string? FilePath { get; set; }
        public string? OriginalFileName { get; set; }
    }
}