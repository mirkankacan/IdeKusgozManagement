using Microsoft.AspNetCore.Http;

namespace IdeKusgozManagement.Application.DTOs.CompanyPaymentDTOs
{
    public class CreateCompanyPaymentDTO
    {
        public string? EquipmentId { get; set; }
        public decimal Amount { get; set; }
        public string ExpenseId { get; set; }
        public string ProjectId { get; set; }
        public string? PersonnelNote { get; set; }
        public List<IFormFile> Files { get; set; }
        public string? SelectedApproverId { get; set; }
    }
}