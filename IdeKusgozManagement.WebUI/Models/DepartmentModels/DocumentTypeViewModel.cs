namespace IdeKusgozManagement.WebUI.Models.DepartmentModels
{
    public class DocumentTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? RenewalPeriodInMonths { get; set; }
        public bool IsActive { get; set; }
    }
}