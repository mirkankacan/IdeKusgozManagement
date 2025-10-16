using System.ComponentModel.DataAnnotations;
using IdeKusgozManagement.WebUI.Models.FileModels;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels
{
    public class CreateWorkRecordExpenseViewModel
    {
        [Required]
        public string ExpenseId { get; set; }

        public string? Description { get; set; } = null;

        [Required]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Belge yüklenmesi zorunludur.")]
        public UploadFileViewModel File { get; set; } = null!;
    }
}