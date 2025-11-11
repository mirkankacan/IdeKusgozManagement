using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtDepartment : BaseEntity
    {
        public string Name { get; set; }

        public DepartmentScope Scope { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public virtual ICollection<IdtDepartmentDocumentType> RequiredDocuments { get; set; } = new List<IdtDepartmentDocumentType>();
        public virtual ICollection<IdtFile> Files { get; set; } = new List<IdtFile>();
    }
}