using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.CompanyModels
{
    public class UpdateCompanyViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}

