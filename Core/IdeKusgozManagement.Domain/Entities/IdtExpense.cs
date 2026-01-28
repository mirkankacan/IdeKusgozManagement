using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtExpense : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public virtual ICollection<IdtWorkRecordExpense> WorkRecordExpenses { get; set; } = new List<IdtWorkRecordExpense>();
        public virtual ICollection<IdtCompanyPayment> CompanyPayments { get; set; } = new List<IdtCompanyPayment>();
    }
}