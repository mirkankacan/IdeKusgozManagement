using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public string Id { get; private set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}