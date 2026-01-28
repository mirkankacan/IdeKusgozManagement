using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.CompanyPaymentDTOs
{
    public class CompanyPaymentDTO
    {
        public string Id { get; set; }
        public string? EquipmentId { get; set; }
        public string Equipment { get; set; }
        public decimal Amount { get; set; }
        public string ExpenseId { get; set; }
        public string Expense { get; set; }
        public string ProjectId { get; set; }
        public string Project { get; set; }
        public string? PersonnelNote { get; set; }
        public string? ChiefNote { get; set; }
        public List<string> FileIds { get; set; }
        public string? SelectedApproverId { get; set; }
        public string? SelectedApproverFullName { get; set; }
        public string CreatedByFullName { get; set; }
        public string CreatedBy { get; set; }
        public CompanyPaymentStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string StatusText => Status switch
        {
            CompanyPaymentStatus.FinanceApproved => "Finans Onaylandı",
            CompanyPaymentStatus.Pending => "Beklemede",
            CompanyPaymentStatus.ChiefApproved => "Şef Onayladı",
            CompanyPaymentStatus.FinanceRejected => "Finans Reddetti",
            CompanyPaymentStatus.ChiefRejected => "Şef Reddetti",
            _ => "Bilinmiyor"
        };
    }
}