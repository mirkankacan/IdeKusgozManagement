using IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels;
using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordModels
{
    public class CreateWorkRecordViewModel
    {
        [Required]
        public DateTime Date { get; set; }

        public string? ExcuseReason { get; set; } = null;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public string Project { get; set; }

        [Required]
        public string EquipmentId { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string District { get; set; }

        public string? Expense { get; set; } = null;

        public bool HasBreakfast { get; set; } = false;

        public bool HasLunch { get; set; } = false;

        public bool HasDinner { get; set; } = false;

        public bool HasNightMeal { get; set; } = false;
        public bool HasTravel { get; set; } = false;
        public List<CreateWorkRecordExpenseViewModel>? Expenses { get; set; } = new();
    }
}