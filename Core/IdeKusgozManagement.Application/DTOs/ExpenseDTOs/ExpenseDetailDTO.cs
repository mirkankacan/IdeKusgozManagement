namespace IdeKusgozManagement.Application.DTOs.ExpenseDTOs
{
    public class ExpenseDetailDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
