using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.AuthModels
{
    public class LoginViewModel
    {
        [Required]
        public string TCNo { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; } = false;
    }
}