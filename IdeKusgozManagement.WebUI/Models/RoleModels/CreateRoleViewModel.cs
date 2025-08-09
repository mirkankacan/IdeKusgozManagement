using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.RoleModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}