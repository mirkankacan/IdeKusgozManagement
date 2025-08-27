namespace IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs
{
    public class UpdateWorkRecordExpenseDTO
    {
        public string ExpenseId { get; set; }

        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? ReceiptImageUrl { get; set; }
    }
}