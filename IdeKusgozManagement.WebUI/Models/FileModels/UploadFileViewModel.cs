using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.FileModels
{
    public class UploadFileViewModel
    {
        [Required]
        public IFormFile FormFile { get; set; }

        public string? DocumentTypeId { get; set; }
        public string? TargetUserId { get; set; }
        public string? TargetProjectId { get; set; }
        public string? TargetEquipmentId { get; set; }
        public string? TargetDepartmentId { get; set; }
        public string? TargetCompanyId { get; set; }
        public string? TargetDepartmentDutyId { get; set; }
        public bool? HasRenewalPeriod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}