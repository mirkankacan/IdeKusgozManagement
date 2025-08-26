using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.EquipmentModels
{
    public class CreateEquipmentViewModel
    {
        [Required(ErrorMessage = "Ekipman adı zorunludur")]
        [StringLength(100, ErrorMessage = "Ekipman adı en fazla 100 karakter olabilir")]
        [Display(Name = "Ekipman Adı")]
        public string Name { get; set; }
    }
}
