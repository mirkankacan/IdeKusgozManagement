using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.LeaveRequestModels
{
    public class CreateLeaveRequestViewModel
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public string? Description { get; set; }
    }
}