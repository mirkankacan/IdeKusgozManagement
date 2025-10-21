using IdeKusgozManagement.Application.DTOs.FileDTOs;
using System.Text.Json.Serialization;

namespace IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs
{
    public class CreateModifyWorkRecordExpenseDTO
    {
        public string? Id { get; set; }
        [JsonIgnore]
        public string? WorkRecordId { get; set; }
        public string ExpenseId { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public UploadFileDTO? File { get; set; }
    }
}