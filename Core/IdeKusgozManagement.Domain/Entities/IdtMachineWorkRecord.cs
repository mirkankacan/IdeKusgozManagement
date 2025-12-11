using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtMachineWorkRecord : BaseEntity
    {
        public DateTime Date { get; set; }
        public string DailyStatus { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? ProjectId { get; set; }
        public string? EquipmentId { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public bool HasInternalTransport { get; set; }
        public string? Description { get; set; }
        public WorkRecordStatus Status { get; set; }
        public string? RejectReason { get; set; }

        public virtual ApplicationUser CreatedByUser { get; set; }

        public virtual ApplicationUser? UpdatedByUser { get; set; }

        public virtual IdtProject? Project { get; set; }

        public virtual IdtEquipment? Equipment { get; set; }
    }
}