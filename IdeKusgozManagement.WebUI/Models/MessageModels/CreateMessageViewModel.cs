using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.MessageModels
{
    public class CreateMessageViewModel
    {
        [Required]
        public string Content { get; set; }
    }
}