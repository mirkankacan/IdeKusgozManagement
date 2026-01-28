using IdeKusgozManagement.Application.DTOs.FileDTOs;

namespace IdeKusgozManagement.Application.DTOs.CompanyPaymentDTOs
{
    public class UpdateCompanyPaymentDTO
    {
        public string? EquipmentId { get; set; }
        public decimal Amount { get; set; }
        public string ExpenseId { get; set; }
        public string ProjectId { get; set; }
        public string? ChiefNote { get; set; }
        public string? PersonnelNote { get; set; }
        public UploadFileDTO File { get; set; }
        public string? SelectedApproverId { get; set; }
    }
}