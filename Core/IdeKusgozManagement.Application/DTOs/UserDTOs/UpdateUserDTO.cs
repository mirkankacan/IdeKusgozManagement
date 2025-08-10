namespace IdeKusgozManagement.Application.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public string? Password { get; set; }
    }
}