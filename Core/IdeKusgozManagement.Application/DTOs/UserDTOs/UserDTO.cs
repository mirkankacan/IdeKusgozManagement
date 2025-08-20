namespace IdeKusgozManagement.Application.DTOs.UserDTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Name} {Surname}";
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public List<string> SuperiorIds { get; set; }
    }
}