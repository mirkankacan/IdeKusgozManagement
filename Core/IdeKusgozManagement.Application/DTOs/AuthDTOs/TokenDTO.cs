namespace IdeKusgozManagement.Application.DTOs.AuthDTOs
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
        public string UserId { get; set; }
        public string TCNo { get; set; }
        public bool IsExpatriate { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Name} {Surname}";
        public string FullNameWithExp => IsExpatriate ? $"{Name} {Surname} (Gurbetçi)" : FullName;
        public string RoleName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDutyName { get; set; }
    }
}