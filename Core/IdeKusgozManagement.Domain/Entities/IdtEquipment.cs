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
    }
}