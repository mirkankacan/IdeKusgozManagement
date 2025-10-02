using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs
{
    public class LeaveRequestDTO
    {
        public string Id { get; set; }

        public LeaveRequestStatus Status { get; set; } // 0 = Pending, 1 = Approved, 2 = Rejected

        public string StatusText => Status switch
        {
            LeaveRequestStatus.Pending => "Beklemede",
            LeaveRequestStatus.Approved => "Onaylandı",
            LeaveRequestStatus.Rejected => "Reddedildi",
            _ => "Bilinmiyor"
        };

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public string? FileId { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedByFullName { get; set; }
        public string? UpdatedByFullName { get; set; }
        public string Duration { get; set; }
        public string? RejectReason { get; set; }
    }
}