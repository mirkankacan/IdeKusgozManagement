using IdeKusgozManagement.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtMessage : BaseEntity
    {
        public string Content { get; set; }
        public string? TargetUsers { get; set; }
        public string? TargetRoles { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual ApplicationUser CreatedByUser { get; set; }
    }
}