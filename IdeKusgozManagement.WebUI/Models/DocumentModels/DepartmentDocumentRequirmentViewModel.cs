namespace IdeKusgozManagement.WebUI.Models.DocumentModels
{
    public class DepartmentDocumentRequirmentViewModel
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
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

