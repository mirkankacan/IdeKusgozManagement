using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtParameter : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}