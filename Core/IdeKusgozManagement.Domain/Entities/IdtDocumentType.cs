using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtDocumentType : BaseEntity
    {
        public string Name { get; set; }
        public int? RenewalPeriodInMonths { get; set; }
        public virtual ICollection<IdtDepartmentDocumentType> RequiredByDepartments { get; set; } = new List<IdtDepartmentDocumentType>();
        public virtual ICollection<IdtFile> Files { get; set; } = new List<IdtFile>();
    }
}