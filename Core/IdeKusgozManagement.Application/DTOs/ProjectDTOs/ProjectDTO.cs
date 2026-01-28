namespace IdeKusgozManagement.Application.DTOs.ProjectDTOs
{
    public class ProjectDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public List<string>? TargetUserIds { get; set; }
        public List<string>? TargetEquipmentIds { get; set; }
        public List<string>? TargetUsers { get; set; }
        public List<string>? TargetEquipments { get; set; }
        public string ProjectColor { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}