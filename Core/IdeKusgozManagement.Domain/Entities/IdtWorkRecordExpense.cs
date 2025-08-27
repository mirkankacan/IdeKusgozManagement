using System.ComponentModel.DataAnnotations.Schema;
using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtWorkRecordExpense : BaseEntity
    {
        public string WorkRecordId { get; set; }
        public string ExpenseId { get; set; }
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string? ReceiptImageUrl { get; set; }
    }
}