using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.AdvanceModels
{
    public class ApproveAdvanceViewModel
    {
        [Required]
        public List<AdvancePartViewModel> Parts { get; set; } = new();
    }
}

