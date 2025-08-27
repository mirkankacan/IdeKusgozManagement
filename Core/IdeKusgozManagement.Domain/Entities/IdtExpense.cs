using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtExpense : BaseEntity
    {
        public IdtExpense()
        {
            IsActive = true;
        }

        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}