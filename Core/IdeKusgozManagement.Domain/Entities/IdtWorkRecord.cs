using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtWorkRecord : BaseEntity
    {
        public DateTime Date { get; set; }
        public bool IsWeekend { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Project { get; set; }
        public string EquipmentId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDinner { get; set; }
        public bool HasNightMeal { get; set; }

        public WorkRecordStatus Status { get; set; } // 0 = Pending, 1 = Approved, 2 = Rejected

        public virtual IdtEquipment Equipment { get; set; }
        public virtual ICollection<IdtWorkRecordExpense> WorkRecordExpenses { get; set; } = new List<IdtWorkRecordExpense>();
    }
}