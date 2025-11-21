using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtEquipment : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<IdtWorkRecord> WorkRecords { get; set; } = new List<IdtWorkRecord>();
        public virtual ICollection<IdtTrafficTicket> TrafficTickets { get; set; } = new List<IdtTrafficTicket>();
        public virtual ICollection<IdtFile> Files { get; set; } = new List<IdtFile>();
    }
}