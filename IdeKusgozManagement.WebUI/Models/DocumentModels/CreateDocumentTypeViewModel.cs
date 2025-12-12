using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.DocumentModels
{
    public class CreateDocumentTypeViewModel
    {
        public string Name { get; set; }

        public int? RenewalPeriodInMonths { get; set; }
    }
}

