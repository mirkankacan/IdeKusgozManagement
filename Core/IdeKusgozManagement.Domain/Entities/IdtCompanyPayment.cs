using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtCompanyPayment : BaseEntity
    {
        public string? EquipmentId { get; set; }
        public decimal Amount { get; set; }
        public string ExpenseId { get; set; }
        public string ProjectId { get; set; }
        public string? PersonnelNote { get; set; }
        public string? ChiefNote { get; set; }
        public string FileIds { get; set; }
        public string? SelectedApproverId { get; set; }
        public CompanyPaymentStatus Status { get; set; }
        public virtual IdtEquipment? Equipment { get; set; }
        public virtual IdtExpense Expense { get; set; }
        public virtual IdtProject Project { get; set; }
        public virtual ApplicationUser? Approver { get; set; }
        public virtual ApplicationUser CreatedByUser { get; set; }

    }
}