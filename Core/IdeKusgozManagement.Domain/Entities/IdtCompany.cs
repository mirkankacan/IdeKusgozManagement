using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtCompany : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<IdtFile> Files { get; set; } = new List<IdtFile>();
        public virtual ICollection<IdtDepartmentDocumentRequirment> RequiredDepartmentDocuments { get; set; } = new List<IdtDepartmentDocumentRequirment>();
    }
}