using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.UserModels
{
    public class CreateUserViewModel
    {
        [Required]
        public string TCNo { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string RoleName { get; set; }

        [Required]
        public bool IsExpatriate { get; set; }
        public string? Email { get; set; } = null;
        public List<string>? SuperiorIds { get; set; } = new();
    }
}