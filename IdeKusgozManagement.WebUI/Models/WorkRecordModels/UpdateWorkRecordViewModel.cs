using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordModels
{
    public class UpdateWorkRecordViewModel
    {
        public bool IsWeekend { get; set; } = false;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public string Project { get; set; }

        [Required]
        public string Equipment { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string District { get; set; }

        public bool HasBreakfast { get; set; } = false;

        public bool HasLunch { get; set; } = false;

        public bool HasDinner { get; set; } = false;

        public bool HasNighMeal { get; set; } = false;
    }
}