using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtWorkRecordExpense : BaseEntity
    {
        public string WorkRecordId { get; set; }
        public string Expense { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string? ReceiptImageUrl { get; set; }
    }
}