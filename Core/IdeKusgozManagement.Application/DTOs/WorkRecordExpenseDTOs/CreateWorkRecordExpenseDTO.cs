namespace IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs
{
    public class CreateWorkRecordExpenseDTO
    {
        public string Expense { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? ReceiptImageUrl { get; set; }
    }
}