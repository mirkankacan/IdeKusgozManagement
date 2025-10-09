using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.WorkRecordDTOs
{
    public class WorkRecordDTO
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string? ExcuseReason { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDinner { get; set; }
        public bool HasNightMeal { get; set; }
        public bool HasTravel { get; set; }
        public WorkRecordStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedByFullName { get; set; }
        public string? UpdatedByFullName { get; set; }

        public string StatusText => Status switch
        {
            WorkRecordStatus.Pending => "Beklemede",
            WorkRecordStatus.Approved => "Onaylandý",
            WorkRecordStatus.Rejected => "Reddedildi",
            _ => "Bilinmiyor"
        };

        public List<WorkRecordExpenseDTO>? WorkRecordExpenses { get; set; }
    }
}