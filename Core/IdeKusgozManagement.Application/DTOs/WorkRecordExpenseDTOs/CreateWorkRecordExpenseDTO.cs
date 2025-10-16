using System.Text.Json.Serialization;
using IdeKusgozManagement.Application.DTOs.FileDTOs;

namespace IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs
{
    public class CreateWorkRecordExpenseDTO
    {
        [JsonIgnore]
        public string? WorkRecordId { get; set; }
        public string ExpenseId { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public UploadFileDTO File { get; set; } = null!;
    }
}