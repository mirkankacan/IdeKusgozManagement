using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtDocumentType : BaseEntity
    {
        public string Name { get; set; }
        public int? RenewalPeriodInMonths { get; set; }
        public virtual ICollection<IdtDepartmentDocumentRequirment> RequiredDepartmentDocuments { get; set; } = new List<IdtDepartmentDocumentRequirment>();
        public virtual ICollection<IdtFile> Files { get; set; } = new List<IdtFile>();
    }
}