using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtDepartment : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<IdtFile> Files { get; set; } = new List<IdtFile>();
        public virtual ICollection<IdtDepartmentDocumentRequirment> RequiredDepartmentDocuments { get; set; } = new List<IdtDepartmentDocumentRequirment>();
    }
}