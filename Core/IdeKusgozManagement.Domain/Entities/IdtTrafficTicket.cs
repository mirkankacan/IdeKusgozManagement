using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtTrafficTicket : BaseEntity
    {
        public string ProjectId { get; set; }
        public string EquipmentId { get; set; }
        public TrafficTicketType Type { get; set; }
        public decimal Amount { get; set; }
        public string? TargetUserId { get; set; }
        public string? FileId { get; set; }
        public DateTime TicketDate { get; set; }
        public virtual IdtFile? File { get; set; }
        public virtual IdtProject Project { get; set; }
        public virtual IdtEquipment Equipment { get; set; }
        public virtual ApplicationUser CreatedByUser { get; set; }
        public virtual ApplicationUser? TargetUser { get; set; }
    }
}