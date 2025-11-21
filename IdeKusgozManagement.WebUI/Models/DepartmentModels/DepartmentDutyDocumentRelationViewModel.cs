namespace IdeKusgozManagement.WebUI.Models.DepartmentModels
{
    public class DepartmentDutyDocumentRelationViewModel
    {
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentDutyId { get; set; }
        public string DocumentTypeId { get; set; }
        public string? CompanyId { get; set; }
    }
}