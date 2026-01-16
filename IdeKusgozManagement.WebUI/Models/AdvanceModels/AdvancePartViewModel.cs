using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.AdvanceModels
{
    public class AdvancePartViewModel
    {
        [Required]
        [Range(1, 31)]
        public int Day { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}

