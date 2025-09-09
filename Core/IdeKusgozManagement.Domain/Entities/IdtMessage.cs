using System.ComponentModel.DataAnnotations.Schema;
using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtMessage : BaseEntity
    {
        public string Content { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual ApplicationUser CreatedByUser { get; set; }
    }
}