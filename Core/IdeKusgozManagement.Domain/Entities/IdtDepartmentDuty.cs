using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtDepartmentDuty : BaseEntity
    {
        public string Name { get; set; }
        public string DepartmentId { get; set; }
        public DutyScope Scope { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<IdtDepartmentDocumentRequirment> RequiredDepartmentDocuments { get; set; } = new List<IdtDepartmentDocumentRequirment>();
        public virtual ICollection<IdtFile> Files { get; set; } = new List<IdtFile>();
    }
}