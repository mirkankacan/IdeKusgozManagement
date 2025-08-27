using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtEquipment : BaseEntity
    {
        public IdtEquipment()
        {
            IsActive = true;
        }

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<IdtWorkRecord> WorkRecords { get; set; } = new List<IdtWorkRecord>();
    }
}