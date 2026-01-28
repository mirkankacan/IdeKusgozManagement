using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.ExpenseDTOs
{
    public class UpdateExpenseDTO
    {
        public string Name { get; set; }
        public ExpenseType ExpenseType { get; set; }
    }
}