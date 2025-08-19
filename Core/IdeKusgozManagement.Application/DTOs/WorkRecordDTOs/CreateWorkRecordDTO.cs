namespace IdeKusgozManagement.Application.DTOs.WorkRecordDTOs
{
    public class CreateWorkRecordDTO
    {
        public DateTime Date { get; set; }
        public bool IsWeekend { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Project { get; set; }
        public string Equipment { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasLunch { get; set; }
        public bool HasDinner { get; set; }
        public bool HasNightMeal { get; set; }
        public List<CreateWorkRecordExpenseDTO>? Expenses { get; set; }
    }
}