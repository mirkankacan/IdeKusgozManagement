namespace IdeKusgozManagement.Application.DTOs.UserDTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string TCNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Name} {Surname}";
        public string FullNameWithExp => IsExpatriate ? $"{Name} {Surname} (Gurbetçi)" : FullName;
        public bool IsActive { get; set; }
        public bool IsExpatriate { get; set; }
        public string RoleName { get; set; }
        public List<string> SuperiorIds { get; set; }
    }
}