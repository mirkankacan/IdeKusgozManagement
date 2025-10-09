using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtMessage : BaseEntity
    {
        public string Content { get; set; }
        public string? TargetUsers { get; set; }
        public string? TargetRoles { get; set; }

        public virtual ApplicationUser CreatedByUser { get; set; }
    }
}