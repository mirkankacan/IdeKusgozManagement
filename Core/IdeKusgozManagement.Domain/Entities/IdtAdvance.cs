using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtAdvance : BaseEntity
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public AdvanceStatus Status { get; set; }

        public DateTime? UnitManagerProcessedDate { get; set; }
        public string? ProcessedByUnitManagerId { get; set; }
        public string? UnitManagerRejectReason { get; set; }
        public DateTime? ChiefProcessedDate { get; set; }
        public string? ProcessedByChiefId { get; set; }
        public string? ChiefRejectReason { get; set; }

        public virtual ApplicationUser CreatedByUser { get; set; }
        public virtual ApplicationUser? UpdatedByUser { get; set; }
        public virtual ApplicationUser? ChiefUser { get; set; }
        public virtual ApplicationUser? UnitManagerUser { get; set; }
    }
}