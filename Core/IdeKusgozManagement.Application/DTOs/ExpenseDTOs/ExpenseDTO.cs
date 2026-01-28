using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.ExpenseDTOs
{
    public class ExpenseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ExpenseType ExpenseType { get; set; }

        public string TypeText => ExpenseType switch
        {
            ExpenseType.InvoiceItem => "Fatura Kalemi",
            ExpenseType.ExpenseItem => "Masraf Kalemi",
            _ => "Bilinmiyor"
        };

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}