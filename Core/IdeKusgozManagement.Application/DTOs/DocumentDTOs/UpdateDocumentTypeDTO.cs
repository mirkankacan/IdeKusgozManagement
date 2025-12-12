using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.Application.DTOs.DocumentDTOs
{
    public class UpdateDocumentTypeDTO
    {
        public string Name { get; set; }

        public int? RenewalPeriodInMonths { get; set; }
    }
}

