using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtUserHierarchy : BaseEntity
    {
        public string SubordinateId { get; set; } // Alt personel (bağlı olan)

        public string SuperiorId { get; set; } // Üst personel (bağlı olduğu)
    }
}