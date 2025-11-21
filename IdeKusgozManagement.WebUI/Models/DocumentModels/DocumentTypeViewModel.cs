namespace IdeKusgozManagement.WebUI.Models.DocumentModels
{
    public class DocumentTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? RenewalPeriodInMonths { get; set; }
        public int Scope { get; set; }
    }
}