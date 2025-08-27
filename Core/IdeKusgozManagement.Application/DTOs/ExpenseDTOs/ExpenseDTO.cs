namespace IdeKusgozManagement.Application.DTOs.ExpenseDTOs
{
    public class ExpenseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}