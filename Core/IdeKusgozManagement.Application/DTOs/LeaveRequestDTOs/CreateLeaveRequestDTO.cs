using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs
{
    public class CreateLeaveRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType Reason { get; set; }
        public string? Description { get; set; }
        public LeaveRequestStatus Status { get; set; }// 0 = Pending, 1 = Approved, 2 = Rejected
        public string? DocumentUrl { get; set; }
    }
}