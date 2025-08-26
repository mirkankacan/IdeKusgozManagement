using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.Application.DTOs.ExpenseDTOs
{
    public class CreateExpenseDTO
    {
        [Required(ErrorMessage = "Masraf türü adı zorunludur")]
        [StringLength(100, ErrorMessage = "Masraf türü adı en fazla 100 karakter olabilir")]
        public string Name { get; set; }
    }
}
