namespace IdeKusgozManagement.Application.DTOs.ExpenseDTOs
{
    public class ExpenseListDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}