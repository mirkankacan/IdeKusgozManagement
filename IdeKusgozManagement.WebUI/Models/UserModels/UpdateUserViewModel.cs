using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.UserModels
{
    public class UpdateUserViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }
        public string? Password { get; set;}

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}