namespace IdeKusgozManagement.Application.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public string TCNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public string? RoleName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public bool IsExpatriate { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        public List<string> SuperiorIds { get; set; } = new();
    }
}