using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtLeaveRequest : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public LeaveRequestStatus Status { get; set; }// 0 = Pending, 1 = Approved, 2 = Rejected
        public string? FileId { get; set; }
        public string? Duration { get; set; }
        public string? RejectReason { get; set; }

        [ForeignKey(nameof(FileId))]
        public virtual IdtFile? File { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual ApplicationUser CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual ApplicationUser? UpdatedByUser { get; set; }
    }
}