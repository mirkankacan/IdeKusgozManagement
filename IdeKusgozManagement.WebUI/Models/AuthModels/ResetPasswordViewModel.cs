using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.AuthModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "TC Kimlik Numarası gereklidir")]
        public string TCNo { get; set; }

        [Required(ErrorMessage = "Doğrulama kodu gereklidir")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Doğrulama kodu 6 haneli olmalıdır")]
        public string VerificationCode { get; set; }

        [Required(ErrorMessage = "Yeni şifre gereklidir")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Şifre onayı gereklidir")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; }

    }
}