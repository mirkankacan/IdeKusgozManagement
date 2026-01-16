namespace IdeKusgozManagement.Application.DTOs.ProjectDTOs
{
    public class UpdateProjectDTO
    {
        public string Name { get; set; }
        public List<string>? TargetUserIds { get; set; }
        public List<string>? TargetEquipmentIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}