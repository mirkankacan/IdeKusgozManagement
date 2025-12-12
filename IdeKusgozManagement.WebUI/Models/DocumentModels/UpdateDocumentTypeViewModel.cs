using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.DocumentModels
{
    public class UpdateDocumentTypeViewModel
    {
        [Required]
        public string Name { get; set; }

        public int? RenewalPeriodInMonths { get; set; }
    }
}

