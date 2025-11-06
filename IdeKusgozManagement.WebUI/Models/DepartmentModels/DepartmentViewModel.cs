namespace IdeKusgozManagement.WebUI.Models.DepartmentModels
{
    public class DepartmentViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? RenewalPeriodInMonths { get; set; }
        public bool IsActive { get; set; }
    }
}