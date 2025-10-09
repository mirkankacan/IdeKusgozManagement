using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtProject : BaseEntity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<IdtWorkRecord> WorkRecords { get; set; } = new List<IdtWorkRecord>();
    }
}