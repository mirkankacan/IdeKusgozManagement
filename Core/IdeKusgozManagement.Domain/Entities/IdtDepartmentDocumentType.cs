using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtDepartmentDocumentType : BaseEntity
    {
        public string DepartmentId { get; set; }
        public virtual IdtDepartment Department { get; set; }
        public string DocumentTypeId { get; set; }
        public virtual IdtDocumentType DocumentType { get; set; }
    }
}