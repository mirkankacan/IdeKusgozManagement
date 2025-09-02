using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs
{
    public class LeaveRequestDTO
    {
        public string Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public LeaveRequestStatus Status { get; set; } // 0 = Pending, 1 = Approved, 2 = Rejected

        public string StatusText => Status switch
        {
            LeaveRequestStatus.Pending => "Beklemede",
            LeaveRequestStatus.Approved => "Onaylandı",
            LeaveRequestStatus.Rejected => "Reddedildi",
            _ => "Bilinmiyor"
        };

        public string? DocumentUrl { get; set; }
    }
}