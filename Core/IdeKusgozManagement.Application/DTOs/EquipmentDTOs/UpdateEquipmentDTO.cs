using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.Application.DTOs.EquipmentDTOs
{
    public class UpdateEquipmentDTO
    {
        [Required(ErrorMessage = "Ekipman adı zorunludur")]
        [StringLength(100, ErrorMessage = "Ekipman adı en fazla 100 karakter olabilir")]
        public string Name { get; set; }
    }
}
