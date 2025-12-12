namespace IdeKusgozManagement.Application.DTOs.DocumentDTOs
{
    public class DepartmentDocumentRequirmentDTO
    {
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDutyId { get; set; }
        public string DepartmentDutyName { get; set; }
        public string DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }
}