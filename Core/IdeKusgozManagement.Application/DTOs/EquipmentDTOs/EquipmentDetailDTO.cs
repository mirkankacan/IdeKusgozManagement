namespace IdeKusgozManagement.Application.DTOs.EquipmentDTOs
{
    public class EquipmentDetailDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
