using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.MessageModels
{
    public class CreateMessageViewModel
    {
        [Required]
        public string Content { get; set; }

        public string[]? TargetRoles { get; set; } = null;
        public string[]? TargetUsers { get; set; } = null;
    }
}