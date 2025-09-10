using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.NotificationModels
{
    public class CreateNotificationViewModel
    {
        [Required(ErrorMessage = "Mesaj alanı zorunludur")]
        [StringLength(500, ErrorMessage = "Mesaj en fazla 500 karakter olabilir")]
        public string Message { get; set; }
    }
}
