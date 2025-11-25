namespace IdeKusgozManagement.WebUI.Models.AuthModels
{
    public class TokenViewModel
    {
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
        public string UserId { get; set; }
        public string TCNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsExpatriate { get; set; }
        public string FullName { get; set; }
        public string FullNameWithExp { get; set; }
        public string RoleName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDutyName { get; set; }
    }
}