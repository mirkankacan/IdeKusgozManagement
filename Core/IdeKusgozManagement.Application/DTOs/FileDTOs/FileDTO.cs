namespace IdeKusgozManagement.Application.DTOs.FileDTOs
{
    public class FileDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string? DocumentTypeId { get; set; }
        public string? TargetUserId { get; set; }
        public string? TargetProjectId { get; set; }
        public string? TargetEquipmentId { get; set; }
        public string? TargetDepartmentId { get; set; }
        public string? TargetCompanyId { get; set; }
        public string? TargetDepartmentDutyId { get; set; }
        public string? DocumentTypeName { get; set; }
        public string? TargetUserFullName { get; set; }
        public string? TargetProjectName { get; set; }
        public string? TargetEquipmentName { get; set; }
        public string? TargetDepartmentName { get; set; }
        public string? TargetCompanyName { get; set; }
        public string? TargetDepartmentDutyName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}