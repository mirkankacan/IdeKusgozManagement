using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs
{
    public class TrafficTicketDTO
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public TrafficTicketType Type { get; set; }
        public decimal Amount { get; set; }
        public string? TargetUserId { get; set; }
        public string? TargetUserFullName { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}