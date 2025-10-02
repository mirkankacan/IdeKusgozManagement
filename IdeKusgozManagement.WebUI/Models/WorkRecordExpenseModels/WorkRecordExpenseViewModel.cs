namespace IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels
{
    public class WorkRecordExpenseViewModel
    {
        public string Id { get; set; }
        public string WorkRecordId { get; set; }
        public string ExpenseId { get; set; }
        public string ExpenseName { get; set; }

        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string? FileId { get; set; }
        public string? FilePath { get; set; }
    }
}