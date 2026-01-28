namespace IdeKusgozManagement.Application.DTOs.AdvanceDTOs
{
    public class AdvanceStatisticDTO
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }

        public decimal PendingPercentage { get; set; }
        public decimal ApprovedPercentage { get; set; }
        public decimal RejectedPercentage { get; set; }
        public decimal CompletedPercentage { get; set; }
    }
}
