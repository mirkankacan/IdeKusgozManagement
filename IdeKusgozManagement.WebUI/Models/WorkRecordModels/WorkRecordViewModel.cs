using IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordModels
{
    public class WorkRecordViewModel
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string DailyStatus { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? AdditionalStartTime { get; set; }
        public TimeSpan? AdditionalEndTime { get; set; }
        public string? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
        public string? Province { get; set; }
        public string District { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDinner { get; set; }
        public bool HasNightMeal { get; set; }
        public decimal? TravelExpenseAmount { get; set; }
        public int Status { get; set; }
        public string? RejectReason { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedByFullName { get; set; }
        public string? UpdatedByFullName { get; set; }

        public string StatusText { get; set; }

        public List<WorkRecordExpenseViewModel>? WorkRecordExpenses { get; set; }
    }
}