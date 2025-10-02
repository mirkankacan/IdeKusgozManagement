using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels
{
    public class CreateWorkRecordExpenseViewModel
    {
        [Required]
        public string ExpenseId { get; set; }

        public string? Description { get; set; } = null;

        [Required]
        public decimal Amount { get; set; }
    }
}