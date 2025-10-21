using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs
{
    public class UpdateTrafficTicketDTO
    {
        public string ProjectId { get; set; }
        public string EquipmentId { get; set; }
        public TrafficTicketType Type { get; set; }
        public DateTime TicketDate { get; set; }
        public decimal Amount { get; set; }
        public string? TargetUserId { get; set; }
        public UploadFileDTO? File { get; set; }
    }
}