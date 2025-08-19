namespace IdeKusgozManagement.Application.DTOs.WorkRecordDTOs
{
    public class WorkRecordDTO
    {
        public string Id { get; set; }
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
        public int Status { get; set; }
        public string StatusText => GetStatusText();
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public List<WorkRecordExpenseDTO>? Expenses { get; set; }

        private string GetStatusText()
        {
            return Status switch
            {
                0 => "Beklemede",
                1 => "Onaylandı",
                2 => "Reddedildi",
                _ => "Bilinmiyor"
            };
        }
    }
}