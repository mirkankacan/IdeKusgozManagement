namespace IdeKusgozManagement.WebUI.Models.ProjectModels
{
    public class UpdateProjectViewModel
    {
        public string Name { get; set; }
        public List<string>? TargetUserIds { get; set; }
        public List<string>? TargetEquipmentIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}