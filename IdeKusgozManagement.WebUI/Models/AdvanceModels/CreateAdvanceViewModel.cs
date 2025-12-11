using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.AdvanceModels
{
    public class CreateAdvanceViewModel
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Reason { get; set; }

        public string? Description { get; set; } = null;

        public string? UserId { get; set; } = null;
    }
}