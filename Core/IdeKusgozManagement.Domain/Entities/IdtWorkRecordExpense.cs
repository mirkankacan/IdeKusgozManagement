using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtWorkRecordExpense : BaseEntity
    {
        public string WorkRecordId { get; set; }
        public string ExpenseId { get; set; }
        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string FileId { get; set; }

        public virtual IdtFile File { get; set; }

        public virtual IdtWorkRecord WorkRecord { get; set; }

        public virtual IdtExpense Expense { get; set; }
    }
}