namespace IdeKusgozManagement.WebUI.Models.ParameterModels
{
    public class ParameterViewModel
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}