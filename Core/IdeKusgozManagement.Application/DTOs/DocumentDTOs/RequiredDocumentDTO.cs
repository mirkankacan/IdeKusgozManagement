using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.DocumentDTOs
{
    public class RequiredDocumentDTO
    {
        public string DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public int? RenewalPeriodInMonths { get; set; }
        public DutyScope Scope { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? TargetId { get; set; }
        public string? TargetFullName { get; set; }
        public bool IsUploaded { get; set; }
        public bool IsExpired { get; set; }
        public bool IsNeverUploaded { get; set; }
        public string? CurrentFileId { get; set; }
        public DateTime? CurrentFileStartDate { get; set; }
        public DateTime? CurrentFileEndDate { get; set; }
        public DateTime? CurrentFileCreatedDate { get; set; }
    }
}