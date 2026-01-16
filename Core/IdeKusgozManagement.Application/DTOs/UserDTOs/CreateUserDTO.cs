namespace IdeKusgozManagement.Application.DTOs.UserDTOs
{
    public class CreateUserDTO
    {
        public string TCNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentDutyId { get; set; }
        public string? Email { get; set; }
        public bool IsExpatriate { get; set; }
        public DateTime? HireDate { get; set; }
        public List<string> SuperiorIds { get; set; } = new();
        public decimal? SalaryAdvanceBalance { get; set; } = null;
        public decimal? JobAdvanceBalance { get; set; } = null;
    }
}