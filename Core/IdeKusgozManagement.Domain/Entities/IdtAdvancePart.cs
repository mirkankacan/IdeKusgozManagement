using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtAdvancePart : BaseEntity
    {
        public string AdvanceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string? ApprovedById { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? CompletedById { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Navigation properties
        public virtual IdtAdvance Advance { get; set; }

        public virtual ApplicationUser? CompletedByUser { get; set; }
        public virtual ApplicationUser? ApprovedByUser { get; set; }
    }
}