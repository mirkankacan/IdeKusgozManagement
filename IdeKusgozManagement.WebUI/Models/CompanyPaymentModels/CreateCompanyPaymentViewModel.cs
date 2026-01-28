using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace IdeKusgozManagement.WebUI.Models.CompanyPaymentModels
{
    public class CreateCompanyPaymentViewModel
    {
        public string? EquipmentId { get; set; }

        [Required(ErrorMessage = "Tutar zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır")]
        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Masraf türü zorunludur")]
        [Display(Name = "Masraf Türü")]
        public string ExpenseId { get; set; }

        [Required(ErrorMessage = "Proje zorunludur")]
        [Display(Name = "Proje")]
        public string ProjectId { get; set; }

        [StringLength(500, ErrorMessage = "Personel notu en fazla 500 karakter olabilir")]
        [Display(Name = "Personel Notu")]
        public string? PersonnelNote { get; set; }

        [Display(Name = "Dosyalar")]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        [Display(Name = "Onaylayıcı")]
        public string? SelectedApproverId { get; set; }
    }
}
