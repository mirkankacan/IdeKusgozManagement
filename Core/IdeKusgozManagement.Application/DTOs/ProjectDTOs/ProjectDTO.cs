namespace IdeKusgozManagement.Application.DTOs.ProjectDTOs
{
    public class ProjectDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}