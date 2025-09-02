using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs
{
    public class UpdateLeaveRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType Reason { get; set; }
        public string? Description { get; set; }
        public string? DocumentUrl { get; set; }
    }
}