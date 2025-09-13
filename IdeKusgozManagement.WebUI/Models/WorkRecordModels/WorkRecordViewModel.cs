using IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels;

namespace IdeKusgozManagement.WebUI.Models.WorkRecordModels
{
    public class WorkRecordViewModel
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsWeekend { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Project { get; set; }
        public string EquipmentId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDinner { get; set; }
        public bool HasNightMeal { get; set; }
        public int Status { get; set; } // 0 = Pending, 1 = Approved, 2 = Rejected

        public string StatusText { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
        public List<WorkRecordExpenseViewModel>? Expenses { get; set; }
    }
}