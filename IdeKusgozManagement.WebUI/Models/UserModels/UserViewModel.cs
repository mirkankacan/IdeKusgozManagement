namespace IdeKusgozManagement.WebUI.Models.UserModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string TCNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
        public string FullNameWithExp { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpatriate { get; set; }
        public string RoleName { get; set; }
        public string? Email { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public List<string> SuperiorIds { get; set; }
    }
}