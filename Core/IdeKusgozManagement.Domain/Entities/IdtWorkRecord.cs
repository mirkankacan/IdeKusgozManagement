using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtWorkRecord : BaseEntity
    {
        public DateTime Date { get; set; }
        public string DailyStatus { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? AdditionalStartTime { get; set; }
        public TimeSpan? AdditionalEndTime { get; set; }
        public string? ProjectId { get; set; }
        public string? EquipmentId { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDinner { get; set; }
        public bool HasNightMeal { get; set; }
        public string? TravelExpenseAmount { get; set; }
        public string? RejectReason { get; set; }

        public WorkRecordStatus Status { get; set; } // 0 = Pending, 1 = Approved, 2 = Rejected

        public virtual ApplicationUser CreatedByUser { get; set; }

        public virtual ApplicationUser? UpdatedByUser { get; set; }

        public virtual IdtProject? Project { get; set; }

        public virtual IdtEquipment? Equipment { get; set; }

        public virtual ICollection<IdtWorkRecordExpense> WorkRecordExpenses { get; set; } = new List<IdtWorkRecordExpense>();
    }
}