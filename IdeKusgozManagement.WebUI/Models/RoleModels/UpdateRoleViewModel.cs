using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.RoleModels
{
    public class UpdateRoleViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}