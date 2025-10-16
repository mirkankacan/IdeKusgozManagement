using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.MessageModels
{
    public class CreateMessageViewModel
    {
        [Required]
        public string Content { get; set; }

        public List<string>? TargetRoles { get; set; } = null;
        public List<string>? TargetUsers { get; set; } = null;
    }
}