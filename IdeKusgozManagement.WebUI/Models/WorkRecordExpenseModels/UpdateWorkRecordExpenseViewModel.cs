using IdeKusgozManagement.WebUI.Models.FileModels;
using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels
{
    public class UpdateWorkRecordExpenseViewModel
    {
        [Required]
        public string ExpenseId { get; set; }

        public string? Description { get; set; } = null;

        [Required]
        public decimal Amount { get; set; }

        public UploadFileViewModel? File { get; set; }
    }
}