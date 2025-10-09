using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtExpense : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<IdtWorkRecordExpense> WorkRecordExpenses { get; set; } = new List<IdtWorkRecordExpense>();
    }
}