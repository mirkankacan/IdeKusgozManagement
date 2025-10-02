using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.FileModels
{
    public class UploadFileViewModel
    {
        [Required(ErrorMessage = "Dosya yüklenmesi gerekmektedir")]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "Dosya tipi seçilmelidir")]
        public int FileType { get; set; }

        public string? TargetUserId { get; set; } = null;
    }
}