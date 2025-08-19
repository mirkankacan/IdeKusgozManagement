namespace IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs
{
    public class CreateWorkRecordExpenseDTO
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string? ReceiptImageUrl { get; set; }
    }
}