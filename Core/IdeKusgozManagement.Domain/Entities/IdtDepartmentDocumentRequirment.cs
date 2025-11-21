using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtDepartmentDocumentRequirment : BaseEntity
    {
        public string? CompanyId { get; set; }
        public virtual IdtCompany? Company { get; set; }
        public string DepartmentId { get; set; }
        public virtual IdtDepartment Department { get; set; }
        public string DepartmentDutyId { get; set; }
        public virtual IdtDepartmentDuty DepartmentDuty { get; set; }
        public string DocumentTypeId { get; set; }
        public virtual IdtDocumentType DocumentType { get; set; }
    }
}