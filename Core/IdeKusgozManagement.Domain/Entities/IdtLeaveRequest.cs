using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtLeaveRequest : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType Reason { get; set; }
        public string? Description { get; set; }
        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;// 0 = Pending, 1 = Approved, 2 = Rejected
        public string? DocumentUrl { get; set; }
    }
}