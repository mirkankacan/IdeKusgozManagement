namespace IdeKusgozManagement.WebUI.Models.LeaveRequestModels
{
    public class LeaveRequestViewModel
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public string? DocumentUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string StatusText { get; set; }
        public string Duration { get; set; }
        public string? RejectReason { get; set; }
    }
}