using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtAnnualLeaveEntitlement : BaseEntity
    {
        public string UserId { get; set; }
        public decimal Entitlement { get; set; }
        public int Year { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}