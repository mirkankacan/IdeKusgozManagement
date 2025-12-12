using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.Application.DTOs.DocumentDTOs
{
    public class CreateDocumentTypeDTO
    {
        [Required]
        public string Name { get; set; }

        public int? RenewalPeriodInMonths { get; set; }
    }
}

