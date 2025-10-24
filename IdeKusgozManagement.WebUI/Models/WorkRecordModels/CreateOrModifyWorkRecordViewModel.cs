using IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels;
using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordModels
{
    public class CreateOrModifyWorkRecordViewModel
    {
        [Required]
        public DateTime Date { get; set; }

        public string DailyStatus { get; set; }

        public TimeSpan? StartTime { get; set; } = null;

        public TimeSpan? EndTime { get; set; } = null;

        public TimeSpan? AdditionalStartTime { get; set; } = null;

        public TimeSpan? AdditionalEndTime { get; set; } = null;

        public string? ProjectId { get; set; } = null;

        public string? EquipmentId { get; set; } = null;

        public string? Province { get; set; } = null;

        public string? District { get; set; } = null;

        public bool HasBreakfast { get; set; } = false;

        public bool HasLunch { get; set; } = false;

        public bool HasDinner { get; set; } = false;

        public bool HasNightMeal { get; set; } = false;
        public string? TravelExpenseAmount { get; set; } = null;

        public List<CreateOrModifyWorkRecordExpenseViewModel>? WorkRecordExpenses { get; set; } = new();
    }
}