namespace IdeKusgozManagement.Application.DTOs.AdvanceDTOs
{
    public class CreateAdvanceDTO
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }
    }
}