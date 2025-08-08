using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.Application.DTOs.UserDTOs
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Mevcut şifre gereklidir")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre gereklidir")]
        [MinLength(3, ErrorMessage = "Şifre en az 3 karakter olmalıdır")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Şifre onayı gereklidir")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmNewPassword { get; set; }
    }
}