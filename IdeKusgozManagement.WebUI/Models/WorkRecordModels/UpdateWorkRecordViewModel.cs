using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordModels
{
    public class UpdateWorkRecordViewModel
    {
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

        public bool HasBreakfast { get; set; } = false;

        public bool HasLunch { get; set; } = false;

        public bool HasDinner { get; set; } = false;

        public bool HasNighMeal { get; set; } = false;
        public bool HasTravel { get; set; } = false;
    }
}