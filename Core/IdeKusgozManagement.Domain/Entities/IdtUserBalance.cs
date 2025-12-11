using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtUserBalance : BaseEntity
    {
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public BalanceType Type { get; set; }
        public ApplicationUser User { get; set; }
    }
}