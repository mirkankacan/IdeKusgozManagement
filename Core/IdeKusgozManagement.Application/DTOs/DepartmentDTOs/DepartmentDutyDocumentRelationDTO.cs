namespace IdeKusgozManagement.Application.DTOs.DepartmentDTOs
{
    public class DepartmentDutyDocumentRelationDTO
    {
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentDutyId { get; set; }
        public string DocumentTypeId { get; set; }
        public string? CompanyId { get; set; }
    }
}