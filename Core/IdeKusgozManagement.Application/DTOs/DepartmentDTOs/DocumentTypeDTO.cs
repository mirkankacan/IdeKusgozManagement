namespace IdeKusgozManagement.Application.DTOs.DepartmentDTOs
{
    public class DocumentTypeDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? RenewalPeriodInMonths { get; set; }
        public bool IsActive { get; set; }
    }
}