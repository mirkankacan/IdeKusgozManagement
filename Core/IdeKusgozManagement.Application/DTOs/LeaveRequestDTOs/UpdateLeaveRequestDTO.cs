using IdeKusgozManagement.Application.DTOs.FileDTOs;

namespace IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs
{
    public class UpdateLeaveRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }

        public UploadFileDTO? File { get; set; }
    }
}