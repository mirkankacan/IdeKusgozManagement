using IdeKusgozManagement.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtWorkRecordExpense : BaseEntity
    {
        public string WorkRecordId { get; set; }
        public string ExpenseId { get; set; }
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string? FileId { get; set; }

        [ForeignKey(nameof(FileId))]
        public virtual IdtFile? File { get; set; }

        [ForeignKey(nameof(WorkRecordId))]
        public virtual IdtWorkRecord WorkRecord { get; set; }

        [ForeignKey(nameof(ExpenseId))]
        public virtual IdtExpense Expense { get; set; }
    }
}