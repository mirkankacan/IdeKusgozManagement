using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.DocumentModels
{
    public class CreateDepartmentDocumentRequirmentViewModel
    {
        [Required]
        public string DepartmentId { get; set; }

        [Required]
        public string DepartmentDutyId { get; set; }

        [Required]
        public string DocumentTypeId { get; set; }

        public string? CompanyId { get; set; }
    }
}

