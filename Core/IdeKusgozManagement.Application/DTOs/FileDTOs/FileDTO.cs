namespace IdeKusgozManagement.Application.DTOs.FileDTOs
{
    public class FileDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string? TargetUserId { get; set; }
        public string? TargetEquipmentId { get; set; }
        public string? TargetProjectId { get; set; }
        public string? TargetUserName { get; set; }
        public string? TargetEquipmentName { get; set; }
        public string? TargetProjectName { get; set; }
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}