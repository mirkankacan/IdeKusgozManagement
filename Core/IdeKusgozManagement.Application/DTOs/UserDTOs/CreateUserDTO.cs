namespace IdeKusgozManagement.Application.DTOs.UserDTOs
{
    public class CreateUserDTO
    {
        public string TCNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }

        public List<string> SuperiorIds { get; set; } = new();
    }
}