using IdeKusgozManagement.Application.DTOs.FileDTOs;
using System.Text.Json.Serialization;

namespace IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs
{
    public class CreateOrModifyWorkRecordExpenseDTO
    {
        // Güncelleme için gerekli
        // UI'den gelmesi gereken alan
        public string? Id { get; set; }

        [JsonIgnore] // Servis içinde set edilecek
        public string? WorkRecordId { get; set; }

        // UI'den gelmesi gereken alan
        public string ExpenseId { get; set; }

        // UI'den gelmesi gereken alan
        public string? Description { get; set; }

        // UI'den gelmesi gereken alan
        public decimal Amount { get; set; }

        // UI'den gelmesi gereken alan
        public UploadFileDTO? File { get; set; }
    }
}