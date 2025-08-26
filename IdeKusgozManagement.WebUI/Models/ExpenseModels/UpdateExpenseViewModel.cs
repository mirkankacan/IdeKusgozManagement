using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.ExpenseModels
{
    public class UpdateExpenseViewModel
    {

        [Required(ErrorMessage = "Masraf türü adı zorunludur")]
        [StringLength(100, ErrorMessage = "Masraf türü adı en fazla 100 karakter olabilir")]
        [Display(Name = "Masraf Türü Adı")]
        public string Name { get; set; }
    }
}
