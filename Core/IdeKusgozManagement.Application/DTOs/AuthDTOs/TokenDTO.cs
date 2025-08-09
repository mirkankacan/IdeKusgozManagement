namespace IdeKusgozManagement.Application.DTOs.AuthDTOs
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Name} {Surname}";
        public string RoleName { get; set; }
    }
}