using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;

namespace IdeKusgozManagement.Application.DTOs.WorkRecordDTOs
{
    public class UpdateWorkRecordDTO
    {
        public DateTime Date { get; set; }
        public string? ExcuseReason { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? AdditionalStartTime { get; set; }
        public TimeSpan? AdditionalEndTime { get; set; }
        public string ProjectId { get; set; }
        public string EquipmentId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDinner { get; set; }
        public bool HasNightMeal { get; set; }
        public bool HasTravel { get; set; }
        public List<UpdateWorkRecordExpenseDTO>? WorkRecordExpenses { get; set; }
    }
}