namespace IdeKusgozManagement.Application.DTOs.WorkRecordDTOs
{
    public class UpdateWorkRecordExpenseDTO
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string? ReceiptImageUrl { get; set; }
    }
}