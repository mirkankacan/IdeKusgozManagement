using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.AuthModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "TC Kimlik Numarası gereklidir")]
        public string TCNo { get; set; }


    }
}