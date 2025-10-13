using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.FileModels
{
    public class UploadFileViewModel
    {
        [Required(ErrorMessage = "Dosya yüklenmesi gerekmektedir")]
        public IFormFile FormFile { get; set; }
    }
}