

namespace IdeKusgozManagement.Application.DTOs.DocumentDTOs
{
    public class CreateDepartmentDocumentRequirmentDTO
    {
        public string DepartmentId { get; set; }

        public string DepartmentDutyId { get; set; }

        public string DocumentTypeId { get; set; }

        public string? CompanyId { get; set; }
    }
}

