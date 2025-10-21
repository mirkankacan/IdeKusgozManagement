using IdeKusgozManagement.WebUI.Models.FileModels;
using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.TrafficTicketModels
{
    public class CreateTrafficTicketViewModel
    {
        [Required]
        public string ProjectId { get; set; }

        [Required]
        public string EquipmentId { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime TicketDate { get; set; }
        [Required]
        public UploadFileViewModel? File { get; set; }
        public string? TargetUserId { get; set; }

    }
}