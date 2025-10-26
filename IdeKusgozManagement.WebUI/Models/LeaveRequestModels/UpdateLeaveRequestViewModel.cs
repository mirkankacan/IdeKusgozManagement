using IdeKusgozManagement.WebUI.Models.FileModels;
using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.LeaveRequestModels
{
    public class UpdateLeaveRequestViewModel
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public string? Description { get; set; } = null;
        public UploadFileViewModel? File { get; set; } = null;
    }
}